using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SearchBar.Common
{
    internal sealed class ApplicationSearchResult
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Icon Icon { get; set; }
    }

    internal static class ApplicationSearchService
    {
        private static readonly string[] SupportedExtensions = { ".lnk", ".appref-ms", ".exe" };

        public static List<ApplicationSearchResult> LoadApplications()
        {
            var results = new Dictionary<string, ApplicationSearchResult>(StringComparer.OrdinalIgnoreCase);
            ApplicationLogger.Info("Application indexing started.");
            foreach (string directory in GetStartMenuDirectories())
            {
                LoadDirectory(directory, results);
            }

            ApplicationLogger.Info("Application indexing completed. Indexed application count: " + results.Count);
            return results.Values.OrderBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        public static IEnumerable<ApplicationSearchResult> Search(
            IEnumerable<ApplicationSearchResult> applications,
            string query,
            int maximumCount)
        {
            if (applications == null || string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<ApplicationSearchResult>();
            }

            string keyword = NormalizeName(query);
            return applications
                .Select(item => new ApplicationMatch
                {
                    Application = item,
                    Score = CalculateMatchScore(item.Name, keyword)
                })
                .Where(match => match.Score < int.MaxValue)
                .OrderBy(match => match.Score)
                .ThenBy(match => match.Application.Name.Length)
                .ThenBy(match => match.Application.Name, StringComparer.CurrentCultureIgnoreCase)
                .Take(maximumCount)
                .Select(match => match.Application);
        }

        private static int CalculateMatchScore(string applicationName, string keyword)
        {
            string normalizedName = NormalizeName(applicationName);
            if (normalizedName.Length == 0 || keyword.Length == 0)
            {
                return int.MaxValue;
            }

            if (string.Equals(normalizedName, keyword, StringComparison.Ordinal))
            {
                return 0;
            }

            if (normalizedName.StartsWith(keyword, StringComparison.Ordinal))
            {
                return 100 + normalizedName.Length - keyword.Length;
            }

            int containedIndex = normalizedName.IndexOf(keyword, StringComparison.Ordinal);
            if (containedIndex >= 0)
            {
                return 200 + containedIndex * 5 + normalizedName.Length - keyword.Length;
            }

            int subsequenceGapCount;
            if (TryGetSubsequenceGapCount(normalizedName, keyword, out subsequenceGapCount))
            {
                return 400 + subsequenceGapCount * 4 + normalizedName.Length - keyword.Length;
            }

            if (keyword.Length < 3)
            {
                return int.MaxValue;
            }

            int allowedDistance = Math.Min(3, Math.Max(1, keyword.Length / 3));
            int distance = GetClosestEditDistance(applicationName, normalizedName, keyword, allowedDistance);
            return distance <= allowedDistance
                ? 600 + distance * 20 + Math.Abs(normalizedName.Length - keyword.Length)
                : int.MaxValue;
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return new string(value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }

        private static bool TryGetSubsequenceGapCount(string name, string keyword, out int gapCount)
        {
            gapCount = 0;
            int nameIndex = 0;
            int previousMatchIndex = -1;
            foreach (char character in keyword)
            {
                int matchIndex = name.IndexOf(character, nameIndex);
                if (matchIndex < 0)
                {
                    return false;
                }

                if (previousMatchIndex >= 0)
                {
                    gapCount += matchIndex - previousMatchIndex - 1;
                }
                previousMatchIndex = matchIndex;
                nameIndex = matchIndex + 1;
            }
            return true;
        }

        private static int GetClosestEditDistance(
            string originalName,
            string normalizedName,
            string keyword,
            int maximumDistance)
        {
            int closestDistance = LevenshteinDistance(normalizedName, keyword);
            char[] separators = { ' ', '-', '_', '.', '(', ')', '[', ']' };
            foreach (string part in originalName.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                int distance = LevenshteinDistance(NormalizeName(part), keyword);
                closestDistance = Math.Min(closestDistance, distance);
                if (closestDistance <= maximumDistance)
                {
                    break;
                }
            }
            return closestDistance;
        }

        private static int LevenshteinDistance(string source, string target)
        {
            var previousRow = new int[target.Length + 1];
            var currentRow = new int[target.Length + 1];
            for (int column = 0; column <= target.Length; column++)
            {
                previousRow[column] = column;
            }

            for (int row = 1; row <= source.Length; row++)
            {
                currentRow[0] = row;
                for (int column = 1; column <= target.Length; column++)
                {
                    int substitutionCost = source[row - 1] == target[column - 1] ? 0 : 1;
                    currentRow[column] = Math.Min(
                        Math.Min(currentRow[column - 1] + 1, previousRow[column] + 1),
                        previousRow[column - 1] + substitutionCost);
                }

                int[] temporaryRow = previousRow;
                previousRow = currentRow;
                currentRow = temporaryRow;
            }

            return previousRow[target.Length];
        }

        private sealed class ApplicationMatch
        {
            public ApplicationSearchResult Application { get; set; }
            public int Score { get; set; }
        }

        private static IEnumerable<string> GetStartMenuDirectories()
        {
            yield return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            yield return Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        }

        private static void LoadDirectory(
            string directory,
            IDictionary<string, ApplicationSearchResult> results)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                ApplicationLogger.Warning("Start menu directory is unavailable: " + (directory ?? "<null>"));
                return;
            }

            int countBeforeLoading = results.Count;
            List<string> files = GetFilesSafely(directory);
            ApplicationLogger.Info(
                "Start menu scan completed. Directory: " + directory + "; discovered file count: " + files.Count);

            foreach (string file in files)
            {
                try
                {
                    if (!SupportedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string name = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrWhiteSpace(name) || results.ContainsKey(name))
                    {
                        continue;
                    }

                    Icon icon = null;
                    try
                    {
                        icon = Icon.ExtractAssociatedIcon(file);
                    }
                    catch (Exception)
                    {
                    }

                    results.Add(name, new ApplicationSearchResult { Name = name, Path = file, Icon = icon });
                }
                catch (Exception)
                {
                    // A broken or inaccessible shortcut must not stop indexing other applications.
                }
            }

            ApplicationLogger.Info(
                "Start menu applications loaded. Directory: " + directory
                + "; added application count: " + (results.Count - countBeforeLoading));
        }

        private static List<string> GetFilesSafely(string rootDirectory)
        {
            var files = new List<string>();
            var pendingDirectories = new Stack<string>();
            pendingDirectories.Push(rootDirectory);

            while (pendingDirectories.Count > 0)
            {
                string currentDirectory = pendingDirectories.Pop();
                try
                {
                    files.AddRange(Directory.GetFiles(currentDirectory));
                }
                catch (UnauthorizedAccessException ex)
                {
                    ApplicationLogger.Warning(
                        "Access denied while reading start menu files. Directory: " + currentDirectory
                        + "; error: " + ex.Message);
                }
                catch (IOException ex)
                {
                    ApplicationLogger.Warning(
                        "I/O error while reading start menu files. Directory: " + currentDirectory
                        + "; error: " + ex.Message);
                }

                string[] subdirectories;
                try
                {
                    subdirectories = Directory.GetDirectories(currentDirectory);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Warning(
                        "Failed to read start menu subdirectories. Directory: " + currentDirectory
                        + "; error: " + ex.Message);
                    continue;
                }

                foreach (string subdirectory in subdirectories)
                {
                    try
                    {
                        if ((File.GetAttributes(subdirectory) & FileAttributes.ReparsePoint) == 0)
                        {
                            pendingDirectories.Push(subdirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.Warning(
                            "Failed to inspect start menu subdirectory. Directory: " + subdirectory
                            + "; error: " + ex.Message);
                    }
                }
            }

            return files;
        }
    }
}

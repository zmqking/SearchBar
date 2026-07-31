using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace SearchBar.Common
{
    internal static class ApplicationLogger
    {
        private static readonly object SyncRoot = new object();
        private static readonly string LogDirectoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SearchBar",
            "Logs");

        public static string LogDirectory
        {
            get { return LogDirectoryPath; }
        }

        public static string CurrentLogFile
        {
            get
            {
                return Path.Combine(
                    LogDirectoryPath,
                    "searchbar-" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log");
            }
        }

        public static void Initialize()
        {
            Write("INFO", "Logger initialized. Log file: " + CurrentLogFile);
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warning(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message, Exception exception)
        {
            string details = exception == null
                ? message
                : message + Environment.NewLine + exception;
            Write("ERROR", details);
        }

        private static void Write(string level, string message)
        {
            try
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(LogDirectoryPath);
                    string line = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] [Thread:{2}] {3}{4}",
                        DateTime.Now,
                        level,
                        System.Threading.Thread.CurrentThread.ManagedThreadId,
                        message ?? string.Empty,
                        Environment.NewLine);
                    File.AppendAllText(CurrentLogFile, line, new UTF8Encoding(false));
                }
            }
            catch (Exception)
            {
                // Logging failures must never terminate the application.
            }
        }
    }
}

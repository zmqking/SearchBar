using System;
using Tiktoken;

namespace SearchBar.Common
{
    internal static class TokenEstimator
    {
        private static readonly object SyncRoot = new object();
        private static readonly Encoder Encoder = CreateEncoder();

        public static int Estimate(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }

            try
            {
                lock (SyncRoot)
                {
                    return Encoder == null ? EstimateWithoutEncoder(content) : Encoder.CountTokens(content);
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Warning("Tiktoken estimation failed; using fallback. Error: " + ex.Message);
                return EstimateWithoutEncoder(content);
            }
        }

        private static Encoder CreateEncoder()
        {
            try
            {
                // DeepSeek has a different tokenizer. o200k_base is used only for a close UI estimate.
                return ModelToEncoder.For("gpt-4o");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Warning("Tiktoken encoder initialization failed. Error: " + ex.Message);
                return null;
            }
        }

        private static int EstimateWithoutEncoder(string content)
        {
            int asciiCharacters = 0;
            int nonAsciiCharacters = 0;
            foreach (char character in content)
            {
                if (character <= 127)
                {
                    asciiCharacters++;
                }
                else
                {
                    nonAsciiCharacters++;
                }
            }
            return nonAsciiCharacters + (asciiCharacters + 3) / 4;
        }
    }
}

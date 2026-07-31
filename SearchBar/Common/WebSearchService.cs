using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchBar.Common
{
    internal sealed class WebSearchResult
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Snippet { get; set; }
        public string Provider { get; set; }
    }

    internal sealed class WebSearchService
    {
        private const string GoogleApiUrl = "https://customsearch.googleapis.com/customsearch/v1";
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };

        public List<WebSearchResult> GetProviderChoices(string query)
        {
            string encodedQuery = Uri.EscapeDataString(query);
            return new List<WebSearchResult>
            {
                new WebSearchResult
                {
                    Title = "在百度搜索",
                    Url = "https://www.baidu.com/s?wd=" + encodedQuery,
                    Snippet = "使用百度查看 “" + query + "” 的完整搜索结果",
                    Provider = "百度"
                },
                new WebSearchResult
                {
                    Title = "在 Google 搜索",
                    Url = "https://www.google.com/search?q=" + encodedQuery,
                    Snippet = "使用 Google 查看 “" + query + "” 的完整搜索结果",
                    Provider = "Google"
                }
            };
        }

        public async Task<List<WebSearchResult>> SearchGoogleAsync(string query, int maximumCount)
        {
            string apiKey = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_API_KEY");
            string engineId = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_ENGINE_ID");
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(engineId))
            {
                ApplicationLogger.Info("Google result API is not configured; provider choices only.");
                return new List<WebSearchResult>();
            }

            int resultCount = Math.Max(1, Math.Min(10, maximumCount));
            string requestUrl = GoogleApiUrl
                + "?key=" + Uri.EscapeDataString(apiKey.Trim())
                + "&cx=" + Uri.EscapeDataString(engineId.Trim())
                + "&q=" + Uri.EscapeDataString(query)
                + "&num=" + resultCount
                + "&safe=active";

            ApplicationLogger.Info(
                "Google result API requested. Query length: " + query.Length
                + "; requested count: " + resultCount);
            using (HttpResponseMessage response = await Client.GetAsync(requestUrl).ConfigureAwait(false))
            {
                string responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    ApplicationLogger.Warning(
                        "Google result API failed. HTTP status: " + (int)response.StatusCode);
                    throw new InvalidOperationException(
                        "Google 搜索结果请求失败（HTTP " + (int)response.StatusCode + "）。");
                }

                var results = new List<WebSearchResult>();
                JObject json = JObject.Parse(responseJson);
                JToken items = json["items"];
                if (items != null)
                {
                    foreach (JToken item in items)
                    {
                        string title = (string)item["title"];
                        string url = (string)item["link"];
                        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url))
                        {
                            continue;
                        }

                        results.Add(new WebSearchResult
                        {
                            Title = title,
                            Url = url,
                            Snippet = (string)item["snippet"] ?? string.Empty,
                            Provider = "Google"
                        });
                    }
                }

                ApplicationLogger.Info("Google result API completed. Result count: " + results.Count);
                return results;
            }
        }
    }
}

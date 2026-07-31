using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SearchBar.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBar.Realization
{
    internal sealed class ConversationContextInfo
    {
        public int TurnCount { get; set; }
        public int EstimatedTokenCount { get; set; }
        public int MaximumTokenCount { get; set; }
        public int UsagePercent { get; set; }
        public bool ShouldClear { get; set; }
        public string Recommendation { get; set; }
    }

    internal sealed class DeepSeekTranslationService
    {
        private const string ApiUrl = "https://api.deepseek.com/chat/completions";
        private const string Model = "deepseek-v4-flash";
        private const int MaxQuestionLength = 8000;
        private const int MaxConversationMessages = 12;
        private const int MaxConversationCharacters = 16000;
        private const int MaxConversationTokens = 12000;
        private const int TokenWarningThreshold = 9000;
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private readonly List<ChatMessage> conversationHistory = new List<ChatMessage>();
        private readonly SemaphoreSlim conversationLock = new SemaphoreSlim(1, 1);

        public async Task<string> TranslateAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("翻译内容不能为空。", "content");
            }

            string targetLanguage = content.isChinese() ? "English" : "Simplified Chinese";
            string systemPrompt =
                "You are a professional bilingual translator and dictionary editor. "
                + "Translate accurately and provide useful learning details. "
                + "Write explanations in Simplified Chinese and do not reveal reasoning. "
                + "Return plain text without Markdown tables or code fences. "
                + "Use exactly these sections: "
                + "【核心翻译】, 【词性与释义】, 【用法说明】, 【双语例句】. "
                + "The content under 【核心翻译】 must be written strictly in the requested target language. "
                + "For a word or phrase, include common parts of speech, distinct meanings, collocations, "
                + "usage cautions, and three natural bilingual examples. "
                + "For a sentence, include the primary translation, one alternative translation, key expression notes, "
                + "and two natural bilingual examples with similar usage.";
            ApplicationLogger.Info(
                "DeepSeek translation requested. Input length: " + content.Length
                + "; target language: " + targetLanguage);
            string userPrompt = "Target language: " + targetLanguage
                + ". Preserve the original meaning and tone. Text to translate:\n" + content;
            return await SendRequestAsync(systemPrompt, userPrompt, 0.2, "translation")
                .ConfigureAwait(false);
        }

        public async Task<string> AskAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("DeepSeek 查询内容不能为空。", "question");
            }
            if (question.Length > MaxQuestionLength)
            {
                throw new ArgumentException(
                    "DeepSeek 查询内容过长，最多允许 " + MaxQuestionLength + " 个字符。",
                    "question");
            }

            string systemPrompt =
                "You are a reliable DeepSeek assistant. Answer the user's question directly and accurately. "
                + "Use Simplified Chinese unless the user explicitly asks for another language. "
                + "Do not reveal hidden reasoning and do not invent uncertain facts. "
                + "Return plain text without Markdown tables or code fences. "
                + "Organize the answer with these sections when applicable: "
                + "【直接回答】, 【关键点】, 【补充说明】. "
                + "Omit a section only when it is genuinely unnecessary.";
            await conversationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                TrimConversation(question);
                var messages = new List<ChatMessage>
                {
                    new ChatMessage("system", systemPrompt)
                };
                messages.AddRange(conversationHistory);
                messages.Add(new ChatMessage("user", question));

                ApplicationLogger.Info(
                    "DeepSeek contextual query requested. Input length: " + question.Length
                    + "; history message count: " + conversationHistory.Count);
                string response = await SendMessagesAsync(messages, 0.4, "query")
                    .ConfigureAwait(false);

                conversationHistory.Add(new ChatMessage("user", question));
                conversationHistory.Add(new ChatMessage("assistant", response));
                TrimConversation(null);
                ApplicationLogger.Info(
                    "DeepSeek conversation updated. History message count: " + conversationHistory.Count);
                return response;
            }
            finally
            {
                conversationLock.Release();
            }
        }

        public async Task ClearConversationAsync()
        {
            await conversationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                conversationHistory.Clear();
                ApplicationLogger.Info("DeepSeek conversation context cleared.");
            }
            finally
            {
                conversationLock.Release();
            }
        }

        public async Task<ConversationContextInfo> GetConversationContextInfoAsync()
        {
            await conversationLock.WaitAsync().ConfigureAwait(false);
            try
            {
                return CreateConversationContextInfo();
            }
            finally
            {
                conversationLock.Release();
            }
        }

        private static async Task<string> SendRequestAsync(
            string systemPrompt,
            string userPrompt,
            double temperature,
            string operation)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage("system", systemPrompt),
                new ChatMessage("user", userPrompt)
            };
            return await SendMessagesAsync(messages, temperature, operation).ConfigureAwait(false);
        }

        private static async Task<string> SendMessagesAsync(
            IList<ChatMessage> messages,
            double temperature,
            string operation)
        {
            string apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("未配置 DEEPSEEK_API_KEY 环境变量。");
            }

            string requestJson = JsonConvert.SerializeObject(new
            {
                model = Model,
                messages = messages,
                thinking = new { type = "disabled" },
                temperature = temperature,
                max_tokens = 4096
            });

            using (var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(false))
                {
                    string responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        ApplicationLogger.Warning(
                            "DeepSeek " + operation + " failed. HTTP status: " + (int)response.StatusCode);
                        throw new InvalidOperationException("DeepSeek API 请求失败（HTTP " + (int)response.StatusCode + "）。");
                    }

                    JObject result = JObject.Parse(responseJson);
                    string responseText = (string)result.SelectToken("choices[0].message.content");
                    if (string.IsNullOrWhiteSpace(responseText))
                    {
                        throw new InvalidOperationException("DeepSeek API 未返回有效内容。");
                    }

                    ApplicationLogger.Info(
                        "DeepSeek " + operation + " completed. Output length: " + responseText.Length);
                    return responseText.Trim();
                }
            }
        }

        private void TrimConversation(string incomingContent)
        {
            int totalCharacterCount = incomingContent == null ? 0 : incomingContent.Length;
            int estimatedTokenCount = incomingContent == null ? 0 : TokenEstimator.Estimate(incomingContent);
            foreach (ChatMessage message in conversationHistory)
            {
                totalCharacterCount += message.Content == null ? 0 : message.Content.Length;
                estimatedTokenCount += EstimateMessageTokens(message);
            }

            while (conversationHistory.Count > 0
                && (conversationHistory.Count > MaxConversationMessages
                    || totalCharacterCount > MaxConversationCharacters
                    || estimatedTokenCount > MaxConversationTokens))
            {
                int removeCount = Math.Min(2, conversationHistory.Count);
                for (int index = 0; index < removeCount; index++)
                {
                    string content = conversationHistory[0].Content;
                    totalCharacterCount -= content == null ? 0 : content.Length;
                    estimatedTokenCount -= EstimateMessageTokens(conversationHistory[0]);
                    conversationHistory.RemoveAt(0);
                }
            }
        }

        private ConversationContextInfo CreateConversationContextInfo()
        {
            int estimatedTokenCount = conversationHistory.Count == 0 ? 0 : 2;
            foreach (ChatMessage message in conversationHistory)
            {
                estimatedTokenCount += EstimateMessageTokens(message);
            }

            int turnCount = conversationHistory.Count / 2;
            bool shouldClear = estimatedTokenCount >= TokenWarningThreshold
                || conversationHistory.Count >= MaxConversationMessages - 2;
            string recommendation;
            if (turnCount == 0)
            {
                recommendation = "当前没有历史上下文。";
            }
            else if (shouldClear)
            {
                recommendation = "上下文已接近本地保留上限，建议清除后开始新会话。";
            }
            else
            {
                recommendation = "上下文状态正常，可继续追问。";
            }

            return new ConversationContextInfo
            {
                TurnCount = turnCount,
                EstimatedTokenCount = estimatedTokenCount,
                MaximumTokenCount = MaxConversationTokens,
                UsagePercent = Math.Min(100, estimatedTokenCount * 100 / MaxConversationTokens),
                ShouldClear = shouldClear,
                Recommendation = recommendation
            };
        }

        private static int EstimateMessageTokens(ChatMessage message)
        {
            // Four tokens approximate the role and message framing overhead.
            return 4 + TokenEstimator.Estimate(message == null ? null : message.Content);
        }

        private sealed class ChatMessage
        {
            public ChatMessage(string role, string content)
            {
                Role = role;
                Content = content;
            }

            [JsonProperty("role")]
            public string Role { get; private set; }

            [JsonProperty("content")]
            public string Content { get; private set; }
        }
    }
}

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchBar.Common
{
    internal static class TranslationHtmlRenderer
    {
        private static readonly Regex SectionPattern = new Regex(
            "【(?<title>[^】]+)】",
            RegexOptions.Compiled);

        public static string Render(string title, string content)
        {
            string safeTitle = WebUtility.HtmlEncode(title ?? "翻译结果");
            bool isTranslation = (title ?? string.Empty).IndexOf(
                "翻译",
                StringComparison.OrdinalIgnoreCase) >= 0;
            string safeSubtitle = isTranslation
                ? "DeepSeek · 中英互译、释义与双语例句"
                : "DeepSeek · 智能问答";
            string normalizedContent = (content ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            var body = new StringBuilder();
            MatchCollection sections = SectionPattern.Matches(normalizedContent);
            if (sections.Count == 0)
            {
                AppendSection(body, isTranslation ? "翻译结果" : "回答", normalizedContent);
            }
            else
            {
                string introduction = normalizedContent.Substring(0, sections[0].Index).Trim();
                if (introduction.Length > 0)
                {
                    AppendSection(body, "概览", introduction);
                }

                for (int index = 0; index < sections.Count; index++)
                {
                    Match section = sections[index];
                    int contentStart = section.Index + section.Length;
                    int contentEnd = index + 1 < sections.Count
                        ? sections[index + 1].Index
                        : normalizedContent.Length;
                    AppendSection(
                        body,
                        section.Groups["title"].Value,
                        normalizedContent.Substring(contentStart, contentEnd - contentStart).Trim());
                }
            }

            return "<!DOCTYPE html><html><head><meta charset='utf-8'>"
                + "<style>"
                + "html,body{margin:0;padding:0;background:#f4f6fb;color:#253047;font-family:'Microsoft YaHei','Segoe UI',sans-serif;}"
                + ".page{padding:16px 18px 22px;}"
                + ".header{padding:15px 18px;margin-bottom:13px;background:#4666e5;color:#fff;border-radius:9px;box-shadow:0 3px 10px #c9d0e8;}"
                + ".header .title{font-size:18px;font-weight:bold;}.header .sub{font-size:12px;margin-top:5px;color:#e8ecff;}"
                + ".card{background:#fff;border:1px solid #dfe4f1;border-left:4px solid #607ce9;border-radius:7px;margin:0 0 11px;padding:12px 14px;box-shadow:0 2px 7px #e0e4ef;}"
                + ".card h2{font-size:14px;margin:0 0 8px;color:#3855c8;}"
                + ".line{font-size:13px;line-height:1.75;margin:2px 0;word-break:break-word;}"
                + ".example{background:#f7f9ff;border-radius:5px;padding:7px 9px;margin:6px 0;color:#33415f;}"
                + ".empty{color:#8992a8;font-style:italic;}"
                + "</style></head><body><div class='page'>"
                + "<div class='header'><div class='title'>" + safeTitle + "</div>"
                + "<div class='sub'>" + safeSubtitle + "</div></div>"
                + body
                + "</div></body></html>";
        }

        private static void AppendSection(StringBuilder html, string title, string content)
        {
            html.Append("<div class='card'><h2>")
                .Append(WebUtility.HtmlEncode(title))
                .Append("</h2>");

            if (string.IsNullOrWhiteSpace(content))
            {
                html.Append("<div class='line empty'>暂无内容</div></div>");
                return;
            }

            string[] lines = content.Split(new[] { '\n' }, StringSplitOptions.None);
            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                bool isExample = Regex.IsMatch(line, @"^(\d+[\.、]|例句|Example)", RegexOptions.IgnoreCase);
                html.Append(isExample ? "<div class='line example'>" : "<div class='line'>")
                    .Append(WebUtility.HtmlEncode(line))
                    .Append("</div>");
            }
            html.Append("</div>");
        }
    }
}

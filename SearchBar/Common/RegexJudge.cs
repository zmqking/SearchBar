using System.Text.RegularExpressions;

namespace SearchBar.Common
{
    static class RegexJudge
    {
        public static bool isChinese(this string re)
        {
            return Regex.IsMatch(re, "[\u4e00-\u9fa5]");
        }

        public static bool isWord(this string re)
        {
            return Regex.IsMatch(re, "[a-zA-Z]");
        }
    }
}

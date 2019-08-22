using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchBar.Common
{
    class RegexJudge
    {
        public static bool isChinese(string re)
        {
            return Regex.IsMatch(re, "[\u4e00-\u9fa5]");
        }

        public static bool isWord(string re)
        {
            return Regex.IsMatch(re, "[a-zA-Z]");
        }
    }
}

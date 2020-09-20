using System.Text.RegularExpressions;

namespace Calc
{
    public class CalcExpression
    {
        static float _result = 0;
        public static string Calc(string str)
        {
            int bracket_L = 0, bracket_R = 0;
            string tempStr = string.Empty;
            if (HasOperator(str))
            {
                bracket_L = str.IndexOf("(");
                if (bracket_L > -1)
                {
                    bracket_R = str.IndexOf(")");
                    //取出括号里面的一段
                    tempStr = str.Substring(bracket_L, bracket_R + 1 - bracket_L);
                    CalcResult(tempStr.TrimStart('(').TrimEnd(')'));
                    //替换里面的计算过的一段
                    str = str.Replace(tempStr, _result.ToString());
                }
                else
                {
                    //没有括号的情况
                    tempStr = str;
                    CalcResult(tempStr);
                    //替换里面的计算过的一段
                    str = str.Replace(tempStr, _result.ToString());
                }

                Calc(str);
            }
            return _result.ToString();
        }

        static string strOp = string.Empty;
        /// <summary>
        /// 根据加减乘除计算的结果替换到原来的表达式位置
        /// </summary>
        /// <param name="v"></param>
        private static void CalcResult(string v)
        {
            int add = 0, sub = 0, multiply = 0, divide = 0;
            if (HasOperator(v))
            {
                add = v.IndexOf("+");
                sub = v.IndexOf("-", 1);
                multiply = v.IndexOf("*");
                divide = v.IndexOf("/");
                if (multiply > -1)//乘
                {
                    Extract(v, multiply, "*");
                }
                else if (divide > -1)
                {
                    Extract(v, divide, "/");
                }
                else if (add > -1)
                {
                    Extract(v, add, "+");
                }
                else if (sub > -1)
                {
                    Extract(v, sub, "-");
                }
                v = v.Replace(strOp, _result.ToString());
                CalcResult(v);
            }
        }

        // 3+3 / 6-5*2 计算结果赋值给全局变量
        private static void Extract(string str, int index, string o)
        {
            string start = str.Substring(0, index);
            string end = str.Substring(index + 1);
            var start_strNum = string.Empty;
            var end_strNum = string.Empty;
            if (start.Length > 0 && end.Length > 0)
            {
                var start_num = MatchNumber("end", start, ref start_strNum);
                var end_num = MatchNumber("start", end, ref end_strNum);
                switch (o)
                {
                    case "+":
                        _result = start_num + end_num;
                        strOp = $"{start_strNum}+{end_strNum}";
                        break;
                    case "-":
                        _result = start_num - end_num;
                        strOp = $"{start_strNum}-{end_strNum}";
                        break;
                    case "*":
                        _result = start_num * end_num;
                        strOp = $"{start_strNum}*{end_strNum}";
                        break;
                    case "/":
                        _result = start_num / end_num;
                        strOp = $"{start_strNum}/{end_strNum}";
                        break;
                    default:
                        break;
                }
            }
        }

        // 3+3 / 6-5*2 获取表达式前后数字
        private  static float MatchNumber(string positin, string str, ref string strNum)
        {
            Regex r = new Regex(@"(\-?[1-9]\d*\.?\d*)|(\-?0\.\d*[1-9])");//正则
            Match m = r.Match(str);//匹配
            string num = string.Empty;
            if (positin == "end")
            {
                while (m.Success)
                {
                    num = m.Groups[0].Value;
                    m = m.NextMatch();//匹配下一个
                }
            }
            else
            {
                num = m.Groups[0].Value;
            }
            strNum = num;
            return float.Parse(num);
        }

        private static bool HasOperator(string str)
        {
            bool flag = true;
            if (str.IndexOf("-") == 0 && str.IndexOf("-") == str.LastIndexOf("-"))//说明是一个负号，不是减号
            {
                flag = false;
            }
            else
            {
                flag = str.Contains("+") || str.Contains("-") || str.Contains("*") || str.Contains("/");
            }
            return flag;
        }
    }
}

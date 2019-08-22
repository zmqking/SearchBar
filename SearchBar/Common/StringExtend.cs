using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchBar
{
   static class StringExtend
    {
       /// <summary>
       /// 转义字符串里的特殊字符
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
        public static string escapeStr(this string str)
        {
            string strs = ConfigurationManager.AppSettings["replace"];
            string[] chrs = strs.Split(';');
            foreach (var item in chrs)
            {
                string[] items = item.Split(':');
                str = str.Replace(items[0], items[1]);
            }
            //特殊字符单独存放，跟配置文件有冲突的
            return str.Replace(":", "%3A").Replace(";", "%3B");
        }

       /// <summary>
       /// 转义url
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
        public static string escapeUrlStr(this string str)
        {
            return str;//.Replace("&amp;", "&");
        }

       /// <summary>
       /// 判断字符串是否为空
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
        public static bool isNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 写入history文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void saveSearchContent(this string str)
        {
            FileOperation.saveSearchContent(str);
        }

        /// <summary>
        /// 判断是否是相应的规则字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isMatch(this string str,string strReg)
        {
            Regex reg = new Regex(strReg);            
            return reg.IsMatch(str);
        }
    }
}

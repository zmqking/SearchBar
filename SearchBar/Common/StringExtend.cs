using SearchBar.Common;
using SearchBar.Enums;
using SearchBar.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchBar
{
   public static class StringExtend
    {
        public static StrTypes IsUrlOrIp(this string str)
        {
            string normalizedUrl;
            if (!WebAddressParser.TryNormalize(str, out normalizedUrl))
            {
                return StrTypes.String;
            }

            Uri uri;
            System.Net.IPAddress address;
            return Uri.TryCreate(normalizedUrl, UriKind.Absolute, out uri)
                && System.Net.IPAddress.TryParse(uri.Host, out address)
                ? StrTypes.IP
                : StrTypes.Url;
        }

        public static string GetConfigValue(this string key)
        {
            string res = null;
            try
            {
                IReadConfig irf = CreateReadObj.getReadWay();
                res = irf.getConfigValue(key);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 转义字符串里的特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EscapeStr(this string str)
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
        public static string EscapeUrlStr(this string str)
        {
            return str;//.Replace("&amp;", "&");
        }

       /// <summary>
       /// 判断字符串是否为空
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 写入history文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void SaveSearchContent(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                FileOperation.saveSearchContent(str);
            }
        }

        /// <summary>
        /// 判断是否是相应的规则字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsMatch(this string str,string strReg)
        {
            Regex reg = new Regex(strReg);            
            return reg.IsMatch(str);
        }
    }
}

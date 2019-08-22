using Newtonsoft.Json;
using RestSharp;
using SearchBar.Interface;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SearchBar.Realization
{
    public class CreateTranslateControl : Form, ICreateControl
    {
        /// <summary>
        /// 文本框输入的内容
        /// </summary>
        string txtContent { get; set; }
        public CreateTranslateControl()
        {
        }

        private string sign
        {
            get { return string.Format("{0}{1}{2}{3}", Common.Common.getConfigValue("AppId"), txtContent, Common.Common.getConfigValue("Salt"), Common.Common.getConfigValue("Key")); }
        }
        string getMd5()
        {
            var md5 = new MD5CryptoServiceProvider();
            var result = Encoding.UTF8.GetBytes(sign);
            var output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        public string GetJson()
        {
            var client = new RestClient(Common.Common.getConfigValue("TranslateDomian"));
            var request = new RestRequest(Common.Common.getConfigValue("TranslatePath"), Method.GET);
            //var client = new RestClient("http://api.fanyi.baidu.com");
            //var request = new RestRequest("/api/trans/vip/translate", Method.GET);
            request.AddParameter("q", txtContent);
            request.AddParameter("from", from);
            request.AddParameter("to", to);
            request.AddParameter("appid", Common.Common.getConfigValue("AppId"));
            request.AddParameter("salt", Common.Common.getConfigValue("Salt"));
            request.AddParameter("sign", getMd5());
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        public string GetResult()
        {
            var lst = new List<string>();
            var content = GetJson();
            dynamic json = JsonConvert.DeserializeObject(content);
            if (json != null)
            {
                foreach (var item in json.trans_result)
                {
                    lst.Add(item.dst.ToString());
                }
            }
            return string.Join(";", lst);
        }

        public string from { get; set; }

        public string to { get; set; }


        public Control getResults(string c, SearchBar.Enums.TranslateTypes tt)
        {
            switch (tt)
            {
                case SearchBar.Enums.TranslateTypes.Chinese:
                    from = "zh";
                    to = "en";
                    break;
                case SearchBar.Enums.TranslateTypes.English:
                    from = "en";
                    to = "zh";
                    break;
                default:
                    break;
            }
            this.txtContent = c;
            string s = GetResult();
            TextBox txt = new TextBox();
            txt.BackColor = System.Drawing.SystemColors.Control;
            Clipboard.SetDataObject(s); 
            txt.Text = s;
            return txt;
        }
    }
}

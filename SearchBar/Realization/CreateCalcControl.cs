using Newtonsoft.Json;
using RestSharp;
using SearchBar.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchBar.Realization
{
    public class CreateCalcControl : Form, ICreateControl
    {
    //    /// <summary>
    //    /// 文本框输入的内容
    //    /// </summary>
    //    string txtContent { get; set; }
    //    public CreateCalcControl(string c)
    //    {
    //        this.txtContent = c;
    //    }

    //    private string sign
    //    {
    //        get { return string.Format("{0}{1}{2}{3}", appid, txtContent, salt, key); }
    //    }
    //    string getMd5()
    //    {
    //        var md5 = new MD5CryptoServiceProvider();
    //        var result = Encoding.UTF8.GetBytes(sign);
    //        var output = md5.ComputeHash(result);
    //        return BitConverter.ToString(output).Replace("-", "").ToLower();
    //    }

    //    public string GetJson()
    //    {
    //        var client = new RestClient("http://api.fanyi.baidu.com");
    //        var request = new RestRequest("/api/trans/vip/Calc", Method.GET);
    //        request.AddParameter("q", txtContent);
    //        request.AddParameter("from", from);
    //        request.AddParameter("to", to);
    //        request.AddParameter("appid", appid);
    //        request.AddParameter("salt", salt);
    //        request.AddParameter("sign", getMd5());
    //        IRestResponse response = client.Execute(request);
    //        return response.Content;
    //    }
    //    public string GetResult()
    //    {
    //        var lst = new List<string>();
    //        var content = GetJson();
    //        dynamic json = JsonConvert.DeserializeObject(content);
    //        foreach (var item in json.trans_result)
    //        {
    //            lst.Add(item.dst.ToString());
    //        }
    //        return string.Join(";", lst);
    //    }

    //    public string appid { get { return "20170628000060775"; } }


    //    public string salt { get { return "1435660288"; } }

    //    public string key { get { return "LlbwGO6YLt2xxudZwG__"; } }

    //    public string from { get; set; }

    //    public string to { get; set; }


    //    public Control getResults(string c)
    //    {
    //        string  s =  GetResult();
    //        TextBox txt = new TextBox();
    //        txt.Text = s;
    //        return txt;
    //    }

        public Control getResults(string c, Enums.TranslateTypes tt)
        {
            throw new NotImplementedException();
        }
    }
}

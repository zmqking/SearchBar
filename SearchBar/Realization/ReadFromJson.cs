using SearchBar.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace SearchBar.Common
{
    class ReadFromJson : IReadConfig
    {

        public string getConfigValue(string key)
        {
            string res = string.Empty;
            dynamic resJson = JsonConvert.DeserializeObject(readJson());
            foreach (dynamic property in resJson)
            {
                if (property.Name == key)
                {
                    res = property.Value;
                    break;
                }
            }
            return res;
        }

        private string readJson()
        {
            string res = string.Empty;
            string path = System.Environment.CurrentDirectory.ToString().Contains("bin") ? System.Environment.CurrentDirectory + "\\Config\\config.json" : Directory.GetCurrentDirectory() +"\\Config\\config.json";

            try
            {
                if (File.Exists(path))
                {
                    StreamReader sr = new StreamReader(path);
                    res = sr.ReadToEnd();
                    sr.Close();
                }
                else
                {
                    res = "配置文件不存在！";
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}

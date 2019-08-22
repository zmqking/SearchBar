using SearchBar.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBar.Common
{
    class ReadFromConfig : IReadConfig
    {
        public string getConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

using SearchBar.Interface;
using System.Configuration;

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

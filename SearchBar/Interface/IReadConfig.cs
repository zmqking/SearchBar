using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBar.Interface
{
    interface IReadConfig
    {
        /// <summary>
        /// 根据键获取相应的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string getConfigValue(string key);
    }
}

using SearchBar.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchBar.Interface
{
    interface ICreateControl
    {
        /// <summary>
        /// 根据枚举类型创建不同的控件
        /// </summary>
        /// <param name="clt"></param>
        /// <returns></returns>
        Control getResults(string c, TranslateTypes tt);
    }
}

using SearchBar.Interface;
using SearchBar.Enums;
using SearchBar.Realization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SearchBar.Common
{
    class CreateControlWithEnum
    {
        /// <summary>
        /// 根据枚举获取相应的创建控件格式(搜索，翻译，计算....)
        /// </summary>
        /// <returns></returns>
        public static ICreateControl getShowControl(ControlTypes ct)
        {
            ICreateControl obj = null;
            switch (ct)
            {
                //case ControlTypes.Calc:
                //    //obj = new 
                //    break;
                //case ControlTypes.Search:
                //    break;
                case ControlTypes.fy:
                    obj = new CreateTranslateControl();
                    break;
                default:
                    break;
            }

            return obj;
        }
    }
}

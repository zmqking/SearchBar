using SearchBar.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SearchBar.Common
{
    class CreateReadObj
    {
        /// <summary>
        /// 获取读取何种方式的配置文件(app.config,json,xml....)
        /// </summary>
        /// <returns></returns>
        public static IReadConfig getReadWay()
        {
            string c = ConfigurationManager.AppSettings["readConfigWay"];
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            object obj = assembly.CreateInstance(c);// 创建类的实例，返回为 object 类型，需要强制类型转换
            
            return (IReadConfig)obj;
        }
    }
}

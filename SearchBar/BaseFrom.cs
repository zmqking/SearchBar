using SearchBar.Interface;
using SearchBar.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchBar
{
    public class BaseForm : Form
    {
        protected string getConfigValue(string key)
        {
            string res = string.Empty;
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
    }
}

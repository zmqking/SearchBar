using System.Windows.Forms;

namespace SearchBar
{
    public class BaseForm : Form
    {
        protected string getConfigValue(string key)
        {
            return Common.Common.getConfigValue(key);
        }
    }
}

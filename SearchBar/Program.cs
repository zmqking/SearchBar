using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SearchBar.Common;

namespace SearchBar
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationLogger.Initialize();
            ApplicationLogger.Info("SearchBar process starting. Executable: " + Application.ExecutablePath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (sender, args) =>
            {
                ApplicationLogger.Error("Unhandled UI thread exception.", args.Exception);
                MessageBox.Show("程序发生错误：" + args.Exception.Message, "SearchBar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                ApplicationLogger.Error("Unhandled application exception.", exception);
                MessageBox.Show("程序发生严重错误：" + (exception == null ? "未知错误" : exception.Message),
                    "SearchBar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Application.Run(new SearchBox());
            ApplicationLogger.Info("SearchBar process exited.");
            //Application.Run(new AddSymbols());
        }
    }
}

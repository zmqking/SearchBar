using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace SearchBar.Common
{
    internal static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "SearchBar";

        public static bool IsEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
            {
                return key != null && key.GetValue(ValueName) != null;
            }
        }

        public static void SetEnabled(bool enabled)
        {
            using (RegistryKey key = enabled
                ? Registry.CurrentUser.CreateSubKey(RunKeyPath)
                : Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (key == null)
                {
                    if (!enabled)
                    {
                        return;
                    }
                    throw new InvalidOperationException("无法访问当前用户的启动项配置。");
                }

                if (enabled)
                {
                    key.SetValue(ValueName, "\"" + Application.ExecutablePath + "\"");
                }
                else
                {
                    key.DeleteValue(ValueName, false);
                }
            }
        }
    }
}

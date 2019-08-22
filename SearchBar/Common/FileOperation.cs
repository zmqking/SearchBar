using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBar
{
    static class FileOperation
    {
        static string path = System.Environment.CurrentDirectory + "\\Content\\history.txt";
        static string directory = System.Environment.CurrentDirectory + "\\Content";
        public static void saveSearchContent(string content)
        {
            try
            {
                StreamWriter sw = null;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                if (File.Exists(path))
                {
                    content = "\r\n" + content;
                    sw = File.AppendText(path);
                }
                else
                {
                    sw = File.CreateText(path);
                }
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
                //throw new Exception("文件写入错误！");
            }
        }

        public static string readSearchContent(int index,out int counts)
        {
            string res = string.Empty;
            int c = 0;
            try
            {
                StreamReader sr = null;
                FileStream fs = null;
                if (File.Exists(path))
                {
                    fs = File.OpenRead(path);
                    sr = new StreamReader(fs);
                    string strs = sr.ReadToEnd();
                    if (strs.isNotEmpty())
                    {
                        strs = strs.Replace("\r\n", ",");
                        string[] strLines = strs.Split(',');
                        if (index <= strLines.Length)
                        {
                            res = strLines[strLines.Length - index];
                        }
                        else
                        {
                            res = "max";
                        }
                        c = strLines.Length;
                    }
                    sr.Close();
                    fs.Close();
                }
            }
            catch (Exception)
            {
                //throw new Exception("文件读取错误！");
            }
            counts = c;
            return res;
        }
    }
}

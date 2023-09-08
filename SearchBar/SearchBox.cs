using Calc;
using SearchBar.Common;
using SearchBar.Enums;
using SearchBar.Interface;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SearchBar
{
    public partial class SearchBox : BaseForm
    {
        public SearchBox()
        {
            InitializeComponent();
            //this.ControlBox = false;   // 设置不出现关闭按钮
        }

        private const int WINDOW_BORDER_HEIGHT = 35;
        private const int WINDOW_HEIGHT = 63;
        private const int WM_HOTKEY = 0x312; //窗口消息-热键  
        private const int WM_CREATE = 0x1; //窗口消息-创建  
        private const int WM_DESTROY = 0x2; //窗口消息-销毁  
        private const int Space = 0x3572; //热键ID  

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键   
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            if (this.Visible)
                            {

                                this.Hide();
                            }
                            else
                            {
                                this.TopMost = true;
                                this.txtContent.Text = string.Empty;
                                this.Show();
                                this.Activate();//设置当前窗体为激活状态

                            }
                            break;
                            //case 101:    //按下的是Ctrl+B  
                            //    //此处填写快捷键响应代码  
                            //    this.Text = "按下的是Ctrl+B";
                            //    break;
                            //case 102:    //按下的是Alt+D  
                            //    //此处填写快捷键响应代码  
                            //    this.Text = "按下的是Ctrl+Alt+D";
                            //    break;
                            //case 103:
                            //    this.Text = "F5";
                            //    break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        #region 事件
        private void SearchBox_Load(object sender, EventArgs e)
        {
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。  
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Ctrl, Keys.Q);
            ////注册热键Ctrl+B，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。  
            //HotKey.RegisterHotKey(Handle, 101, HotKey.KeyModifiers.Ctrl, Keys.B);
            ////注册热键Ctrl+Alt+D，Id号为102。HotKey.KeyModifiers.Alt也可以直接使用数字1来表示。  
            //HotKey.RegisterHotKey(Handle, 102, HotKey.KeyModifiers.Alt | HotKey.KeyModifiers.Ctrl, Keys.D);
            ////注册热键F5，Id号为103。  
            //HotKey.RegisterHotKey(Handle, 103, HotKey.KeyModifiers.None, Keys.F5);

            //initTextBoxSource();
        }

        /// <summary>
        /// 回车搜索事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string c = txtContent.Text.TrimEnd();
                if (!stringJudge(c) && c.IsNotEmpty())
                {
                    var strType = c.IsUrlOrIp();
                    if (strType == StrTypes.IP || strType == StrTypes.Url)
                    {
                        SearchContent(c, strType);
                    }
                    else
                    {
                        string defaultKey = "default".GetConfigValue();
                        int spaceIndex = c.IndexOf(" ");
                        if (defaultKey == "close")
                        {
                            if (spaceIndex > -1)
                            {
                                showSpecifySearch(c, spaceIndex, defaultKey);
                            }
                        }
                        else
                        {
                            //中间有空格
                            if (spaceIndex > -1)
                            {
                                showSpecifySearch(c, spaceIndex, defaultKey);
                            }
                            else
                            {
                                showDefaultSearch(c, defaultKey);
                            }
                        }
                    }
                }
                c.SaveSearchContent();
            }
            else if (!this.txtContent.Text.IsNotEmpty())
            {
                this.Height = WINDOW_HEIGHT;
            }
        }
        /// <summary>
        /// 操作窗体隐藏或者调用其它面板，配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.P) && e.Control)
            {
                try
                {
                    callSystemProgram("EditPlus.exe", "Config/config.json");
                }
                catch (Exception)
                {
                    //不存在调用默认的文本编辑器
                    callSystemProgram("Notepad.exe", "Config/config.json");
                }
                //SetPanel sp = new SetPanel();
                //sp.ShowDialog();
            }
            if ((e.KeyCode == Keys.T) && e.Control)
            {
                try
                {
                    AddSymbols addSymbols = new AddSymbols();
                    addSymbols.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
            else if (e.KeyCode == Keys.Enter && e.Control)
            {
                string content = txtContent.Text.Trim();
                if (ControlTypes.ip.ToString() == content.ToLower())
                {
                    this.txtContent.Text = Common.Common.GetLocalIP();
                }
                else
                {
                    int spaceIndex = content.IndexOf(" ");
                    if (spaceIndex > 0)
                    {
                        string key = content.Substring(0, spaceIndex).ToLower();

                        if (ControlTypes.f.ToString() == key)
                        {
                            content = content.Substring(spaceIndex);
                        }
                    }
                    string translate = "BaiduOnlineTranslateUrl".GetConfigValue();
                    string endStr = "en/zh/" + content;
                    if (content.isChinese())
                    {
                        endStr = "zh/en/" + content;
                    }
                    string fUrl = translate + endStr;
                    SearchContent(fUrl);
                }
            }
        }

        int index = 1;
        int counts = 0;
        private void txtContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (e.KeyCode == Keys.Up && index < counts)
                {
                    index++;
                }
                else if (e.KeyCode == Keys.Down && index > 1)
                {
                    index--;
                }
                txtContent.Text = FileOperation.readSearchContent(index, out counts);
            }
        }
        #endregion

        #region 方法

        string tempValue = string.Empty;
        /// <summary>
        /// 判断是输入字符串为计算表达式
        /// </summary>
        /// <returns></returns>
        private bool stringJudge(string str)
        {
            bool flag = false;

            try
            {
                //是否为计算表达式（20*98-7/0.23）
                //string reg = @"^([-]?\d{1,}\.?[-]?\d{0,}[\.,\+,\-,\*,\/][-]?\d{1,}\.?[-]?\d{0,})+$";
                //flag = str.isMatch(reg);
                //flag = str.IndexOfAny(new char[4] { '+', '-', '*', '/' }) > -1;
                flag = CalcExpression.HasOperator(str);
                if (flag)
                {
                    string res = calc(str);
                    Clipboard.SetDataObject(res);
                    MessageBox.Show(res);
                    this.txtContent.Focus();
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }
        private void callSystemProgram(string p1, string p2)
        {
            System.Diagnostics.Process.Start(p1, p2);
        }
        private void SearchContent(string fUrl, StrTypes strTypes = StrTypes.String)
        {
            //调用系统chrome的浏览器   
            //callSystemProgram("chrome.exe", fUrl.EscapeUrlStr());
            //调用系统默认浏览器
            try
            {
                //if (strTypes == StrTypes.Url && !fUrl.StartsWith("www."))
                //{
                //    System.Diagnostics.Process.Start("www." + fUrl.EscapeUrlStr());
                //}
                //else
                //{
                //    System.Diagnostics.Process.Start(fUrl.EscapeUrlStr());
                //}
                //this.Hide();


                string strUrl = string.Empty;
                if (strTypes == StrTypes.Url && !fUrl.StartsWith("www."))
                {
                    strUrl = fUrl.EscapeUrlStr();
                }
                else 
                {
                    strUrl = fUrl.EscapeUrlStr();
                    //System.Diagnostics.Process.Start("chrome.exe", "www." + fUrl.EscapeUrlStr());
                    //System.Diagnostics.Process.Start(fUrl.EscapeUrlStr());
                }
                Process process = new Process();
                process.StartInfo.FileName = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                process.StartInfo.Arguments = strUrl;// + " --new-window --incognito ";
                process.Start();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        Control ctl;
        /// <summary>
        /// 指定搜索
        /// </summary>
        /// <param name="c"></param>
        /// <param name="spaceIndex"></param>
        /// <param name="defaultKey"></param>
        private void showSpecifySearch(string c, int spaceIndex, string defaultKey)
        {
            //确定使用什么功能的键
            string key = c.Substring(0, spaceIndex).ToLower();

            if (ControlTypes.f.ToString() == key)
            {
                string t = c.Substring(spaceIndex + 1);
                try
                {
                    this.Controls.Remove(ctl);
                }
                catch (Exception)
                {
                }
                ICreateControl icc = CreateControlWithEnum.getShowControl(Enums.ControlTypes.f);
                TranslateTypes tt = TranslateTypes.English;
                if (t.isChinese())
                {
                    tt = TranslateTypes.Chinese;
                }
                ctl = icc.getResults(t, tt);
                ctl.Name = "test";
                ctl.Width = this.txtContent.Width;
                ctl.Height = 30;
                ctl.Left = 0;
                ctl.Top = this.txtContent.Height;
                this.Controls.Add(ctl);
                this.Height = this.txtContent.Height + ctl.Height + WINDOW_BORDER_HEIGHT;

            }
            else
            {
                string url = key.GetConfigValue();
                if (url.IsNotEmpty())
                {
                    string fUrl = url.Replace("{q}", c.Substring(spaceIndex + 1).EscapeStr());
                    SearchContent(fUrl);
                }
                else
                {
                    showDefaultSearch(c, defaultKey);
                }
            }
        }

        private void showDefaultSearch(string c, string defaultKey)
        {
            string url = defaultKey.GetConfigValue();
            string fUrl = url.Replace("{q}", c.EscapeStr());
            SearchContent(fUrl);
            index = 0;
        }



        private string calc(string str)
        {
            return CalcExpression.Calc(str);
        }

        #endregion

        private void SearchBox_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void SearchBox_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void SearchBox_Leave(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void SearchBox_Activated_1(object sender, EventArgs e)
        {
            //SendKeys.Send("^c"); //向当前活动窗口发送按键 ctrl+c，也就是复制
            //string str = Clipboard.GetText();//从剪贴板取到数据
            //txtContent.Text = str;
            //txtContent.Focus();
            //txtContent.SelectAll();
        }
    }
}

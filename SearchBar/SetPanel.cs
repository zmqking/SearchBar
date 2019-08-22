using SearchBar.Interface;
using SearchBar.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SearchBar.Enums;
using System.Net;
using System.Net.Sockets;

namespace SearchBar
{
    public partial class SetPanel : BaseForm
    {
        public SetPanel()
        {
            InitializeComponent();
            dynamicBuildControl();

            //this.SetVisibleCore
            //this.textBox1.Dock = DockStyle;
            //Point p = new Point();
            //p.X = 0;
            //p.Y = 8;
            //this.textBox1.Location = p;
            this.Width = this.textBox1.Width;
            this.Height = this.textBox1.Height;
        }

        private void dynamicBuildControl()
        {
            //Label lbl = new Label();
            //lbl.Text = "键:";
            //lbl.Width = 30;
            //lbl.Left = 10;
            //lbl.Top = 20;
            //grbTop.Controls.Add(lbl);
            //TextBox txt = new TextBox();
            //txt.Left = lbl.Left+lbl.Width;
            //txt.Top = lbl.Top;
            //grbTop.Controls.Add(txt);
        }

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public const int KEYEVENTF_KEYUP = 2;


        Control ctl;
        int index = 0;
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            // if (e.KeyCode == Keys.Down)
            //{
            //    if (index < listBox1.Items.Count-1)
            //    {
            //        index++;
            //    }
            //    this.listBox1.SelectedIndex = index;
            //}

            // if (e.KeyCode == Keys.Up)
            // {
            //     if (index > 0)
            //     {
            //         index--;
            //     }
            //     this.listBox1.SelectedIndex = index;
            // }

            if (textBox1.Text.isNotEmpty())
            {
            }
            else
            {
                this.Height = this.textBox1.Height;
                //this.panel1.Visible = false;
            }
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Text = GetLocalIP();
                //try
                //{
                //    this.Controls.Remove(ctl);
                //}
                //catch (Exception)
                //{
                //}
                //ICreateControl icc = CreateControlWithEnum.getShowControl(Enums.ControlTypes.fy);
                //string t = textBox1.Text;
                //TranslateTypes tt = TranslateTypes.English;
                //if (RegexJudge.isChinese(t))
                //{
                //    tt = TranslateTypes.Chinese;
                //}
                //ctl = icc.getResults(t, tt);
                //ctl.Name = "test";
                //ctl.Left = 0;
                //ctl.Top = this.textBox1.Height;
                ////Point p = new Point();
                ////p.X = 0;
                ////p.Y = 0;
                ////ctl.Location = p;
                //this.Controls.Add(ctl);
                //this.Height = this.textBox1.Height + ctl.Height;

            }
        }
        public string GetLocalIP()
        {
            try
            {
                string HostName = System.Net.Dns.GetHostName(); //得到主机名  
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址  
                    //AddressFamily.InterNetwork表示此IP为IPv4,  
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型  
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本机IP出错:" + ex.Message);
                return "";
            }
        }  



    }
}

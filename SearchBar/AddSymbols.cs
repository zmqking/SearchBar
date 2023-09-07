using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchBar
{
    public partial class AddSymbols : Form
    {
        readonly string propertyFrontSymbol = "=";
        readonly string propertyBackSymbol = ",";
        public AddSymbols()
        {
            InitializeComponent();
        }

        private void btnExChange_Click(object sender, EventArgs e)
        {
            txtResult.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(txtContent.Text.Trim()))
            {
                MessageBox.Show("替换内容不能为空！");
            }
            if (rdbPropert.Checked)
            {
                PropertExChange();
            }
            else if (rdbExChange.Checked)
            {
                ExChangeText();
            }
            else if (rdbSymbols.Checked)
            {
                AddStr();
            }
            else if (rdbRmDuplicate.Checked)
            {
                RemoveDuplicate();
            }
        }

        private void RemoveDuplicate()
        {
            List<string> listStr;
            StringBuilder sb;
            string last;
            GetList(out listStr, out sb, out last);
            HashSet<string> list = new HashSet<string>(listStr);
            foreach (var item in list)
            {
                sb.AppendLine($"{item}");
            }
            txtResult.Text = sb.ToString();
        }

        private void PropertExChange()
        {
            List<string> listStr;
            StringBuilder sb;
            string last;
            GetList(out listStr, out sb, out last);
            foreach (var item in listStr)
            {
                var strs = item.Trim().Split(' ');
                if (strs.Length > 1)
                {
                    if (last == item)
                    {
                        sb.Append($"{strs[2]}{txtFrontSymbols.Text.Trim()}{strs[2]}{txtBackSymbols.Text.Trim()}");
                    }
                    else
                    {
                        sb.AppendLine($"{strs[2]}{txtFrontSymbols.Text.Trim()}{strs[2]}{txtBackSymbols.Text.Trim()}");
                    }
                }
                else
                {
                    if (last == item)
                    {
                        sb.Append($"{strs[0]}{txtFrontSymbols.Text.Trim()}{strs[0]}{txtBackSymbols.Text.Trim()}");
                    }
                    else
                    {
                        sb.AppendLine($"{strs[0]}{txtFrontSymbols.Text.Trim()}{strs[0]}{txtBackSymbols.Text.Trim()}");
                    }
                }
            }
            txtResult.Text = sb.ToString().TrimEnd(',');
        }

        private void ExChangeText()
        {
            List<string> listStr;
            StringBuilder sb;
            string last;
            GetList(out listStr, out sb, out last);
            foreach (var item in listStr)
            {
                var strs = item.Split(txtFrontSymbols.Text.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (last == item)
                {
                    sb.Append($"{strs[1].TrimEnd(',', '\n')}{txtFrontSymbols.Text}{strs[0]}{txtBackSymbols.Text}");
                }
                else
                {
                    sb.AppendLine($"{strs[1]}{txtFrontSymbols.Text}{strs[0]}{txtBackSymbols.Text}");
                }

            }
            txtResult.Text = sb.ToString().TrimEnd(',');
        }

        private void AddStr()
        {
            List<string> listStr;
            StringBuilder sb;
            string last;
            GetList(out listStr, out sb, out last);
            foreach (var item in listStr)
            {
                if (last == item)
                {
                    sb.Append($"{txtFrontSymbols.Text.Trim()}{item}{txtBackSymbols.Text.Trim()}");
                }
                else
                {
                    sb.AppendLine($"{txtFrontSymbols.Text.Trim()}{item}{txtBackSymbols.Text.Trim()}");
                }

            }
            txtResult.Text = sb.ToString().TrimEnd(',');
        }

        private void GetList(out List<string> listStr, out StringBuilder sb, out string last)
        {
            var content = txtContent.Text.Trim();
            listStr = content.Split(new char[] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            sb = new StringBuilder();
            last = listStr.Last();
        }

        private void rdbPropert_Click(object sender, EventArgs e)
        {
            txtFrontSymbols.Text = propertyFrontSymbol;
            txtBackSymbols.Text = propertyBackSymbol;
            lblFrontStr.Text = "前面字符";
        }

        private void rdbSymbols_Click(object sender, EventArgs e)
        {
            txtBackSymbols.Text = string.Empty;
            txtFrontSymbols.Text = string.Empty;
            lblFrontStr.Text = "前面字符";
        }

        private void rdbExChange_Click(object sender, EventArgs e)
        {
            txtBackSymbols.Text = string.Empty;
            txtFrontSymbols.Text = string.Empty;
            lblFrontStr.Text = "分隔符:";
        }

        private void rdbRmDuplicate_Click(object sender, EventArgs e)
        {

        }
    }
}

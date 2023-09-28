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

        #region Event
        private void rdbPropert_Click(object sender, EventArgs e)
        {
            txtFrontSymbols.Text = propertyFrontSymbol;
            txtBackSymbols.Text = propertyBackSymbol;
            lblFrontStr.Text = "前面字符";
            txtBackSymbols.Visible = true;
            lblBackStr.Visible = true;
        }

        private void rdbSymbols_Click(object sender, EventArgs e)
        {
            txtBackSymbols.Text = string.Empty;
            txtFrontSymbols.Text = string.Empty;
            lblFrontStr.Text = "前面字符";
            txtBackSymbols.Visible = true;
            lblBackStr.Visible = true;
        }

        private void rdbExChange_Click(object sender, EventArgs e)
        {
            txtBackSymbols.Text = string.Empty;
            txtFrontSymbols.Text = string.Empty;
            lblFrontStr.Text = "分隔符:";
            txtBackSymbols.Visible = true;
            lblBackStr.Visible = true;
        }

        private void rdbRmDuplicate_Click(object sender, EventArgs e)
        {

        }

        private void rdbReplace_Click(object sender, EventArgs e)
        {
            txtBackSymbols.Text = string.Empty;
            txtFrontSymbols.Text = string.Empty;
            txtBackSymbols.Visible = true;
            lblBackStr.Visible = true;
        }

        private void rdbDynamic_Click(object sender, EventArgs e)
        {
            lblFrontStr.Text = "数量:";
            txtFrontSymbols.Text = string.Empty;
            txtBackSymbols.Visible = false;
            lblBackStr.Visible = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtResult.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(txtContent.Text.Trim()))
            {
                MessageBox.Show("左边内容不能为空！");
                txtContent.Focus();
                return;
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
            else if (rdbReplace.Checked)
            {
                ReplaceStr();
            }
            else if (rdbDynamic.Checked)
            {
                DynamicStr();
            }
        }
        #endregion

        #region Method
        private void ReplaceStr()
        {
            var content = txtContent.Text;
            txtResult.Text = content.Replace(txtFrontSymbols.Text, txtBackSymbols.Text);
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
                    sb.Append($"{txtFrontSymbols.Text}{item}{txtBackSymbols.Text}");
                }
                else
                {
                    sb.AppendLine($"{txtFrontSymbols.Text}{item}{txtBackSymbols.Text}");
                }

            }
            txtResult.Text = sb.ToString().TrimEnd(',');
        }

        private void GetList(out List<string> listStr, out StringBuilder sb, out string last)
        {
            var content = txtContent.Text.Trim();
            listStr = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            sb = new StringBuilder();
            last = listStr.Last();
        }

        private void DynamicStr()
        {
            var count = 0;
            var flag = int.TryParse(txtFrontSymbols.Text, out count);
            if (flag)
            {
                if (count > 9999)
                {
                    var drt = MessageBox.Show("数量比较大,会占用比较大内存，确定要继续吗？", "提示", MessageBoxButtons.OKCancel);
                    flag = drt == DialogResult.OK;
                }
                if (flag)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        sb.AppendLine(txtContent.Text);
                    }
                    txtResult.Text = sb.ToString().TrimEnd('\r', '\n');
                }
            }
            else
            {
                txtFrontSymbols.Focus();
                txtFrontSymbols.SelectAll();
                MessageBox.Show("请输入正确的数量！");
            }
        }
        #endregion

        private void txtResult_Click(object sender, EventArgs e)
        {
            txtResult.SelectAll();
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            var txt = txtContent.Text;

            txtContent.Text = txt.TrimEnd('\r', '\n');
        }
    }
}

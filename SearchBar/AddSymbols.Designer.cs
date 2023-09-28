namespace SearchBar
{
    partial class AddSymbols
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtContent = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBackStr = new System.Windows.Forms.Label();
            this.lblFrontStr = new System.Windows.Forms.Label();
            this.txtBackSymbols = new System.Windows.Forms.TextBox();
            this.txtFrontSymbols = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.rdbDynamic = new System.Windows.Forms.RadioButton();
            this.rdbReplace = new System.Windows.Forms.RadioButton();
            this.rdbRmDuplicate = new System.Windows.Forms.RadioButton();
            this.rdbSymbols = new System.Windows.Forms.RadioButton();
            this.rdbPropert = new System.Windows.Forms.RadioButton();
            this.rdbExChange = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(6, 20);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(404, 422);
            this.txtContent.TabIndex = 6;
            this.txtContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblBackStr);
            this.groupBox1.Controls.Add(this.lblFrontStr);
            this.groupBox1.Controls.Add(this.txtBackSymbols);
            this.groupBox1.Controls.Add(this.txtFrontSymbols);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.rdbDynamic);
            this.groupBox1.Controls.Add(this.rdbReplace);
            this.groupBox1.Controls.Add(this.rdbRmDuplicate);
            this.groupBox1.Controls.Add(this.rdbSymbols);
            this.groupBox1.Controls.Add(this.rdbPropert);
            this.groupBox1.Controls.Add(this.rdbExChange);
            this.groupBox1.Location = new System.Drawing.Point(15, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(826, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选项";
            // 
            // lblBackStr
            // 
            this.lblBackStr.AutoSize = true;
            this.lblBackStr.Location = new System.Drawing.Point(388, 62);
            this.lblBackStr.Name = "lblBackStr";
            this.lblBackStr.Size = new System.Drawing.Size(59, 12);
            this.lblBackStr.TabIndex = 3;
            this.lblBackStr.Text = "后面字符:";
            // 
            // lblFrontStr
            // 
            this.lblFrontStr.AutoSize = true;
            this.lblFrontStr.Location = new System.Drawing.Point(10, 62);
            this.lblFrontStr.Name = "lblFrontStr";
            this.lblFrontStr.Size = new System.Drawing.Size(59, 12);
            this.lblFrontStr.TabIndex = 3;
            this.lblFrontStr.Text = "前面字符:";
            // 
            // txtBackSymbols
            // 
            this.txtBackSymbols.Location = new System.Drawing.Point(448, 59);
            this.txtBackSymbols.Name = "txtBackSymbols";
            this.txtBackSymbols.Size = new System.Drawing.Size(372, 21);
            this.txtBackSymbols.TabIndex = 4;
            // 
            // txtFrontSymbols
            // 
            this.txtFrontSymbols.Location = new System.Drawing.Point(71, 58);
            this.txtFrontSymbols.Name = "txtFrontSymbols";
            this.txtFrontSymbols.Size = new System.Drawing.Size(311, 21);
            this.txtFrontSymbols.TabIndex = 3;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(725, 30);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // rdbDynamic
            // 
            this.rdbDynamic.AutoSize = true;
            this.rdbDynamic.Location = new System.Drawing.Point(359, 30);
            this.rdbDynamic.Name = "rdbDynamic";
            this.rdbDynamic.Size = new System.Drawing.Size(71, 16);
            this.rdbDynamic.TabIndex = 2;
            this.rdbDynamic.Text = "动态生成";
            this.rdbDynamic.UseVisualStyleBackColor = true;
            this.rdbDynamic.Click += new System.EventHandler(this.rdbDynamic_Click);
            // 
            // rdbReplace
            // 
            this.rdbReplace.AutoSize = true;
            this.rdbReplace.Location = new System.Drawing.Point(306, 30);
            this.rdbReplace.Name = "rdbReplace";
            this.rdbReplace.Size = new System.Drawing.Size(47, 16);
            this.rdbReplace.TabIndex = 2;
            this.rdbReplace.Text = "替换";
            this.rdbReplace.UseVisualStyleBackColor = true;
            this.rdbReplace.Click += new System.EventHandler(this.rdbReplace_Click);
            // 
            // rdbRmDuplicate
            // 
            this.rdbRmDuplicate.AutoSize = true;
            this.rdbRmDuplicate.Location = new System.Drawing.Point(253, 30);
            this.rdbRmDuplicate.Name = "rdbRmDuplicate";
            this.rdbRmDuplicate.Size = new System.Drawing.Size(47, 16);
            this.rdbRmDuplicate.TabIndex = 2;
            this.rdbRmDuplicate.Text = "去重";
            this.rdbRmDuplicate.UseVisualStyleBackColor = true;
            this.rdbRmDuplicate.Click += new System.EventHandler(this.rdbRmDuplicate_Click);
            // 
            // rdbSymbols
            // 
            this.rdbSymbols.AutoSize = true;
            this.rdbSymbols.Checked = true;
            this.rdbSymbols.Location = new System.Drawing.Point(176, 30);
            this.rdbSymbols.Name = "rdbSymbols";
            this.rdbSymbols.Size = new System.Drawing.Size(71, 16);
            this.rdbSymbols.TabIndex = 2;
            this.rdbSymbols.TabStop = true;
            this.rdbSymbols.Text = "加字符串";
            this.rdbSymbols.UseVisualStyleBackColor = true;
            this.rdbSymbols.Click += new System.EventHandler(this.rdbSymbols_Click);
            // 
            // rdbPropert
            // 
            this.rdbPropert.AutoSize = true;
            this.rdbPropert.Location = new System.Drawing.Point(99, 30);
            this.rdbPropert.Name = "rdbPropert";
            this.rdbPropert.Size = new System.Drawing.Size(71, 16);
            this.rdbPropert.TabIndex = 1;
            this.rdbPropert.Text = "属性赋值";
            this.rdbPropert.UseVisualStyleBackColor = true;
            this.rdbPropert.Click += new System.EventHandler(this.rdbPropert_Click);
            // 
            // rdbExChange
            // 
            this.rdbExChange.AutoSize = true;
            this.rdbExChange.Location = new System.Drawing.Point(21, 30);
            this.rdbExChange.Name = "rdbExChange";
            this.rdbExChange.Size = new System.Drawing.Size(71, 16);
            this.rdbExChange.TabIndex = 0;
            this.rdbExChange.Text = "左右互换";
            this.rdbExChange.UseVisualStyleBackColor = true;
            this.rdbExChange.Click += new System.EventHandler(this.rdbExChange_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtResult);
            this.groupBox2.Controls.Add(this.txtContent);
            this.groupBox2.Location = new System.Drawing.Point(15, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(826, 453);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "内容";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(416, 20);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(404, 422);
            this.txtResult.TabIndex = 7;
            this.txtResult.Click += new System.EventHandler(this.txtResult_Click);
            // 
            // AddSymbols
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 570);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AddSymbols";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文本处理";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdbExChange;
        private System.Windows.Forms.Label lblFrontStr;
        private System.Windows.Forms.TextBox txtFrontSymbols;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RadioButton rdbSymbols;
        private System.Windows.Forms.RadioButton rdbPropert;
        private System.Windows.Forms.Label lblBackStr;
        private System.Windows.Forms.TextBox txtBackSymbols;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.RadioButton rdbRmDuplicate;
        private System.Windows.Forms.RadioButton rdbReplace;
        private System.Windows.Forms.RadioButton rdbDynamic;
    }
}
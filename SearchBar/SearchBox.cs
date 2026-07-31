using Calc;
using SearchBar.Common;
using SearchBar.Enums;
using SearchBar.Interface;
using SearchBar.Realization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchBar
{
    public partial class SearchBox : BaseForm
    {
        public SearchBox()
        {
            InitializeComponent();
            nofityIcon.Click -= nofityIcon_Click;
            nofityIcon.MouseClick += nofityIcon_MouseClick;
            InitializeResultList();
            InitializeResponsiveLayout();
            InitializeStartupMenu();
            //this.ControlBox = false;   // 设置不出现关闭按钮
        }

        private const int WINDOW_BORDER_HEIGHT = 35;
        private const int WINDOW_HEIGHT = 63;
        private const int WM_HOTKEY = 0x312; //窗口消息-热键  
        private const int WM_CREATE = 0x1; //窗口消息-创建  
        private const int WM_DESTROY = 0x2; //窗口消息-销毁  
        private const int Space = 0x3572; //热键ID  
        private const int MAX_APPLICATION_RESULTS = 8;
        private const int MINIMUM_EXPANDED_CLIENT_HEIGHT = 260;
        private const int CONVERSATION_STATUS_HEIGHT = 58;
        private readonly DeepSeekTranslationService translationService = new DeepSeekTranslationService();
        private readonly WebSearchService webSearchService = new WebSearchService();
        private readonly List<ApplicationSearchResult> applications = new List<ApplicationSearchResult>();
        private ListView resultList;
        private ImageList resultImages;
        private WebBrowser translationResultBrowser;
        private Panel conversationStatusPanel;
        private Label conversationStatusLabel;
        private Button clearContextButton;
        private ToolStripMenuItem startupMenuItem;
        private ToolStripMenuItem openLogMenuItem;
        private bool isDeepSeekBusy;
        private bool isUpdatingResponsiveLayout;
        private Size expandedClientSize = new Size(484, 404);

        private void InitializeResultList()
        {
            resultImages = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(24, 24)
            };
            resultList = new ListView
            {
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.None,
                HideSelection = false,
                Location = new Point(0, txtContent.Bottom + 4),
                MultiSelect = false,
                ShowItemToolTips = true,
                SmallImageList = resultImages,
                Size = new Size(ClientSize.Width, 230),
                View = View.Details,
                Visible = false
            };
            resultList.Columns.Add("名称", 185);
            resultList.Columns.Add("详情", 285);
            resultList.DoubleClick += resultList_DoubleClick;
            Controls.Add(resultList);

            translationResultBrowser = new WebBrowser
            {
                AllowWebBrowserDrop = false,
                IsWebBrowserContextMenuEnabled = true,
                Location = resultList.Location,
                ScriptErrorsSuppressed = true,
                Size = new Size(resultList.Width, 320),
                Visible = false,
                WebBrowserShortcutsEnabled = true
            };
            Controls.Add(translationResultBrowser);

            conversationStatusPanel = new Panel
            {
                BackColor = Color.FromArgb(241, 245, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(0, translationResultBrowser.Bottom),
                Size = new Size(ClientSize.Width, 58),
                Visible = false
            };
            conversationStatusLabel = new Label
            {
                AutoEllipsis = true,
                Font = new Font("Microsoft YaHei", 9F, FontStyle.Regular, GraphicsUnit.Point, 134),
                Location = new Point(10, 7),
                Size = new Size(355, 44),
                TextAlign = ContentAlignment.MiddleLeft
            };
            clearContextButton = new Button
            {
                Location = new Point(375, 12),
                Size = new Size(96, 32),
                Text = "清除上下文",
                UseVisualStyleBackColor = true
            };
            clearContextButton.Click += clearContextButton_Click;
            conversationStatusPanel.Controls.Add(conversationStatusLabel);
            conversationStatusPanel.Controls.Add(clearContextButton);
            Controls.Add(conversationStatusPanel);
            txtContent.TextChanged += txtContent_TextChanged;
        }

        private void InitializeResponsiveLayout()
        {
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(360, WINDOW_HEIGHT);
            SizeGripStyle = SizeGripStyle.Show;
            txtContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Resize += SearchBox_Resize;
            LayoutResultControls();
        }

        private void SearchBox_Resize(object sender, EventArgs e)
        {
            if (isUpdatingResponsiveLayout)
            {
                return;
            }

            if (HasVisibleResult() && ClientSize.Height >= MINIMUM_EXPANDED_CLIENT_HEIGHT)
            {
                expandedClientSize = ClientSize;
            }

            LayoutResultControls();
        }

        private bool HasVisibleResult()
        {
            return resultList.Visible || translationResultBrowser.Visible;
        }

        private void EnsureExpandedResultSize()
        {
            if (ClientSize.Height > txtContent.Bottom + 10)
            {
                return;
            }

            int expandedHeight = Math.Max(MINIMUM_EXPANDED_CLIENT_HEIGHT, expandedClientSize.Height);
            isUpdatingResponsiveLayout = true;
            try
            {
                ClientSize = new Size(ClientSize.Width, expandedHeight);
            }
            finally
            {
                isUpdatingResponsiveLayout = false;
            }
        }

        private void LayoutResultControls()
        {
            if (resultList == null || translationResultBrowser == null
                || conversationStatusPanel == null)
            {
                return;
            }

            int contentWidth = Math.Max(1, ClientSize.Width);
            txtContent.Width = contentWidth;

            int resultTop = txtContent.Bottom + 4;
            int footerHeight = conversationStatusPanel.Visible
                ? CONVERSATION_STATUS_HEIGHT
                : 0;
            int resultHeight = Math.Max(40, ClientSize.Height - resultTop - footerHeight);
            var resultBounds = new Rectangle(0, resultTop, contentWidth, resultHeight);
            resultList.Bounds = resultBounds;
            translationResultBrowser.Bounds = resultBounds;

            if (resultList.Columns.Count >= 2)
            {
                int nameColumnWidth = Math.Max(140, contentWidth * 40 / 100);
                resultList.Columns[0].Width = nameColumnWidth;
                resultList.Columns[1].Width = Math.Max(120, contentWidth - nameColumnWidth - 8);
            }

            conversationStatusPanel.Bounds = new Rectangle(
                0,
                Math.Max(resultTop, ClientSize.Height - footerHeight),
                contentWidth,
                CONVERSATION_STATUS_HEIGHT);
            clearContextButton.Left = Math.Max(10, contentWidth - clearContextButton.Width - 12);
            conversationStatusLabel.Width = Math.Max(120, clearContextButton.Left - 20);
        }

        private void InitializeStartupMenu()
        {
            startupMenuItem = new ToolStripMenuItem("开机启动") { CheckOnClick = false };
            startupMenuItem.Click += startupMenuItem_Click;
            contextMenuStrip1.Items.Insert(1, startupMenuItem);

            openLogMenuItem = new ToolStripMenuItem("打开日志目录");
            openLogMenuItem.Click += openLogMenuItem_Click;
            contextMenuStrip1.Items.Insert(2, openLogMenuItem);
        }

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
        private async void SearchBox_Load(object sender, EventArgs e)
        {
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。  
            bool hotKeyRegistered = HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Ctrl, Keys.Q);
            ApplicationLogger.Info("SearchBox loaded. Ctrl+Q hotkey registered: " + hotKeyRegistered);
            ////注册热键Ctrl+B，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。  
            //HotKey.RegisterHotKey(Handle, 101, HotKey.KeyModifiers.Ctrl, Keys.B);
            ////注册热键Ctrl+Alt+D，Id号为102。HotKey.KeyModifiers.Alt也可以直接使用数字1来表示。  
            //HotKey.RegisterHotKey(Handle, 102, HotKey.KeyModifiers.Alt | HotKey.KeyModifiers.Ctrl, Keys.D);
            ////注册热键F5，Id号为103。  
            //HotKey.RegisterHotKey(Handle, 103, HotKey.KeyModifiers.None, Keys.F5);

            try
            {
                startupMenuItem.Checked = StartupManager.IsEnabled();
                ApplicationLogger.Info("Startup option loaded. Enabled: " + startupMenuItem.Checked);
            }
            catch (Exception ex)
            {
                startupMenuItem.Checked = false;
                ApplicationLogger.Error("Failed to read startup option.", ex);
            }

            try
            {
                ApplicationLogger.Info("Background application indexing requested.");
                List<ApplicationSearchResult> indexedApplications = await Task.Run(
                    () => ApplicationSearchService.LoadApplications());
                applications.Clear();
                applications.AddRange(indexedApplications);
                ApplicationLogger.Info("Application index assigned to UI. Count: " + applications.Count);
                UpdateApplicationResults();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error("Application indexing failed.", ex);
                ShowError("读取本地程序失败", ex);
            }
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
                e.Handled = true;
            }
        }
        /// <summary>
        /// 操作窗体隐藏或者调用其它面板，配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchBox_KeyDown(object sender, KeyEventArgs e)
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
                    await TranslateAndShowAsync(content.Trim());
                }
                e.SuppressKeyPress = true;
            }
        }

        int index = 1;
        int counts = 0;
        private async void txtContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                e.SuppressKeyPress = true;
                if (!OpenSelectedResult())
                {
                    await ExecuteInputAsync();
                }
                return;
            }

            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && resultList.Visible && resultList.Items.Count > 0)
            {
                MoveResultSelection(e.KeyCode == Keys.Down ? 1 : -1);
                e.SuppressKeyPress = true;
                return;
            }

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
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fUrl,
                    UseShellExecute = true
                });
                this.Hide();
            }
            catch (Exception ex)
            {
                ShowError("无法打开链接", ex);
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
                    var content = Uri.EscapeDataString(c.Substring(spaceIndex + 1));
                    Clipboard.SetText(content);
                    string fUrl = url.Replace("{q}", content);
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
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("默认搜索引擎配置无效。", "SearchBar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string fUrl = url.Replace("{q}", Uri.EscapeDataString(c));
            SearchContent(fUrl);
            index = 0;
        }

        private async Task ExecuteInputAsync()
        {
            string content = txtContent.Text.Trim();
            if (string.IsNullOrWhiteSpace(content) || stringJudge(content))
            {
                return;
            }

            try
            {
                if (string.Equals(content, "f", StringComparison.OrdinalIgnoreCase))
                {
                    ShowTextResult("请输入需要翻译的中文或英文内容。\n示例：f hello world", "翻译使用提示");
                    return;
                }
                if (string.Equals(content, "ds", StringComparison.OrdinalIgnoreCase))
                {
                    ShowTextResult(
                        "请输入需要向 DeepSeek 查询的问题。\n"
                        + "示例：ds 什么是依赖注入\n"
                        + "清空上下文：ds clear",
                        "DeepSeek 使用提示");
                    return;
                }

                int spaceIndex = content.IndexOfAny(new[] { ' ', '\t' });
                string command = spaceIndex > 0 ? content.Substring(0, spaceIndex) : string.Empty;
                if (string.Equals(command, "f", StringComparison.OrdinalIgnoreCase))
                {
                    await TranslateAndShowAsync(content.Substring(spaceIndex + 1).Trim());
                    return;
                }
                if (string.Equals(command, "ds", StringComparison.OrdinalIgnoreCase))
                {
                    string question = content.Substring(spaceIndex + 1).Trim();
                    if (string.Equals(question, "clear", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(question, "清空", StringComparison.Ordinal))
                    {
                        await ClearDeepSeekContextAsync();
                    }
                    else
                    {
                        await QueryDeepSeekAndShowAsync(question);
                    }
                    return;
                }

                string normalizedUrl;
                if (WebAddressParser.TryNormalize(content, out normalizedUrl))
                {
                    SearchContent(normalizedUrl, StrTypes.Url);
                    return;
                }

                string defaultKey = "default".GetConfigValue();
                if (spaceIndex > 0)
                {
                    string specifiedKey = content.Substring(0, spaceIndex).ToLowerInvariant();
                    string specifiedUrl = specifiedKey.GetConfigValue();
                    if (!string.IsNullOrWhiteSpace(specifiedUrl))
                    {
                        showSpecifySearch(content, spaceIndex, defaultKey);
                    }
                    else if (!string.Equals(defaultKey, "close", StringComparison.OrdinalIgnoreCase))
                    {
                        await ShowWebSearchChoicesAsync(content);
                    }
                }
                else if (!string.Equals(defaultKey, "close", StringComparison.OrdinalIgnoreCase))
                {
                    await ShowWebSearchChoicesAsync(content);
                }
            }
            catch (Exception ex)
            {
                ShowError("执行搜索失败", ex);
            }
            finally
            {
                content.SaveSearchContent();
            }
        }

        private async Task TranslateAndShowAsync(string content)
        {
            if (isDeepSeekBusy || string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            isDeepSeekBusy = true;
            ShowTextResult("正在翻译…", "DeepSeek");
            try
            {
                string result = await translationService.TranslateAsync(content);
                ShowTextResult(result, "DeepSeek 翻译结果（已复制）");
                try
                {
                    Clipboard.SetText(result);
                }
                catch (Exception)
                {
                    // Clipboard may temporarily be locked; the result remains visible.
                }
            }
            catch (Exception ex)
            {
                ShowTextResult(ex.Message, "翻译失败");
            }
            finally
            {
                isDeepSeekBusy = false;
            }
        }

        private async Task QueryDeepSeekAndShowAsync(string question)
        {
            if (isDeepSeekBusy || string.IsNullOrWhiteSpace(question))
            {
                return;
            }

            isDeepSeekBusy = true;
            ShowTextResult("正在查询 DeepSeek…", "DeepSeek 查询");
            try
            {
                string result = await translationService.AskAsync(question);
                ShowTextResult(result, "DeepSeek 回答（已复制）");
                try
                {
                    Clipboard.SetText(result);
                }
                catch (Exception)
                {
                    // Clipboard may temporarily be locked; the result remains visible.
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error("DeepSeek query failed.", ex);
                ShowTextResult(ex.Message, "DeepSeek 查询失败");
            }
            finally
            {
                isDeepSeekBusy = false;
            }
            await UpdateConversationStatusAsync();
        }

        private async Task ClearDeepSeekContextAsync()
        {
            if (isDeepSeekBusy)
            {
                return;
            }

            isDeepSeekBusy = true;
            try
            {
                await translationService.ClearConversationAsync();
                ShowTextResult(
                    "当前 DeepSeek 会话上下文已清空，下一次 ds 查询将开始新会话。",
                    "DeepSeek 会话");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error("Failed to clear DeepSeek conversation context.", ex);
                ShowTextResult(ex.Message, "DeepSeek 会话清空失败");
            }
            finally
            {
                isDeepSeekBusy = false;
            }
            await UpdateConversationStatusAsync();
        }

        private async Task UpdateConversationStatusAsync()
        {
            try
            {
                ConversationContextInfo contextInfo =
                    await translationService.GetConversationContextInfoAsync();
                conversationStatusLabel.Text =
                    "会话：" + contextInfo.TurnCount + "/6 轮"
                    + "    估算 Token：" + contextInfo.EstimatedTokenCount.ToString("N0")
                    + "/" + contextInfo.MaximumTokenCount.ToString("N0")
                    + "（" + contextInfo.UsagePercent + "%）"
                    + Environment.NewLine
                    + contextInfo.Recommendation;
                conversationStatusLabel.ForeColor = contextInfo.ShouldClear
                    ? Color.FromArgb(183, 70, 45)
                    : Color.FromArgb(56, 74, 118);
                conversationStatusPanel.BackColor = contextInfo.ShouldClear
                    ? Color.FromArgb(255, 239, 232)
                    : Color.FromArgb(241, 245, 255);
                clearContextButton.Enabled = contextInfo.TurnCount > 0;
                conversationStatusPanel.Visible = true;
                conversationStatusPanel.BringToFront();
                EnsureExpandedResultSize();
                LayoutResultControls();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error("Failed to update DeepSeek conversation status.", ex);
            }
        }

        private async void clearContextButton_Click(object sender, EventArgs e)
        {
            await ClearDeepSeekContextAsync();
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            UpdateApplicationResults();
        }

        private void UpdateApplicationResults()
        {
            translationResultBrowser.Visible = false;
            conversationStatusPanel.Visible = false;
            resultList.Items.Clear();
            resultImages.Images.Clear();

            List<ApplicationSearchResult> matches = ApplicationSearchService.Search(
                applications, txtContent.Text, MAX_APPLICATION_RESULTS).ToList();
            foreach (ApplicationSearchResult application in matches)
            {
                int imageIndex = -1;
                if (application.Icon != null)
                {
                    resultImages.Images.Add(application.Icon);
                    imageIndex = resultImages.Images.Count - 1;
                }

                var item = new ListViewItem(application.Name, imageIndex) { Tag = application };
                item.SubItems.Add(application.Path);
                resultList.Items.Add(item);
            }

            SetResultListVisible(resultList.Items.Count > 0);
            ApplicationLogger.Info(
                "Application query evaluated. Query length: " + txtContent.Text.Length
                + "; indexed count: " + applications.Count
                + "; matched count: " + matches.Count
                + "; result list visible: " + resultList.Visible);
        }

        private void ShowTextResult(string text, string details)
        {
            resultList.Items.Clear();
            resultImages.Images.Clear();
            resultList.Visible = false;
            conversationStatusPanel.Visible = false;
            translationResultBrowser.DocumentText = TranslationHtmlRenderer.Render(details, text);
            translationResultBrowser.Visible = true;
            translationResultBrowser.BringToFront();
            EnsureExpandedResultSize();
            LayoutResultControls();
        }

        private async Task ShowWebSearchChoicesAsync(string query)
        {
            List<WebSearchResult> results = webSearchService.GetProviderChoices(query);
            ShowWebSearchResults(results);

            try
            {
                List<WebSearchResult> googleResults = await webSearchService.SearchGoogleAsync(query, 8);
                if (!string.Equals(txtContent.Text.Trim(), query, StringComparison.Ordinal))
                {
                    return;
                }

                results.AddRange(googleResults);
                ShowWebSearchResults(results);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error("Failed to load selectable Google results.", ex);
            }
        }

        private void ShowWebSearchResults(IEnumerable<WebSearchResult> results)
        {
            translationResultBrowser.Visible = false;
            resultList.Items.Clear();
            resultImages.Images.Clear();
            resultImages.Images.Add(SystemIcons.Information);

            foreach (WebSearchResult result in results)
            {
                var item = new ListViewItem(result.Title, 0) { Tag = result };
                string snippet = (result.Snippet ?? string.Empty)
                    .Replace("\r", " ")
                    .Replace("\n", " ");
                item.ToolTipText = result.Title + Environment.NewLine + snippet;
                item.SubItems.Add(result.Provider + " · " + snippet);
                resultList.Items.Add(item);
            }

            SetResultListVisible(resultList.Items.Count > 0);
            ApplicationLogger.Info("Selectable web search results displayed. Count: " + resultList.Items.Count);
        }

        private void SetResultListVisible(bool visible)
        {
            translationResultBrowser.Visible = false;
            conversationStatusPanel.Visible = false;
            resultList.Visible = visible;
            if (visible)
            {
                EnsureExpandedResultSize();
                LayoutResultControls();
                return;
            }

            isUpdatingResponsiveLayout = true;
            try
            {
                ClientSize = new Size(ClientSize.Width, txtContent.Bottom + 3);
            }
            finally
            {
                isUpdatingResponsiveLayout = false;
            }
            LayoutResultControls();
        }

        private void MoveResultSelection(int direction)
        {
            int selectedIndex = resultList.SelectedIndices.Count > 0 ? resultList.SelectedIndices[0] : -1;
            int newIndex = selectedIndex < 0
                ? (direction > 0 ? 0 : resultList.Items.Count - 1)
                : Math.Max(0, Math.Min(resultList.Items.Count - 1, selectedIndex + direction));
            resultList.Items[newIndex].Selected = true;
            resultList.Items[newIndex].Focused = true;
            resultList.EnsureVisible(newIndex);
        }

        private bool OpenSelectedResult()
        {
            if (!resultList.Visible || resultList.SelectedItems.Count == 0)
            {
                return false;
            }

            object selectedResult = resultList.SelectedItems[0].Tag;
            var webResult = selectedResult as WebSearchResult;
            if (webResult != null)
            {
                ApplicationLogger.Info("Opening selected web result. Provider: " + webResult.Provider);
                SearchContent(webResult.Url, StrTypes.Url);
                return true;
            }

            var application = selectedResult as ApplicationSearchResult;
            if (application == null)
            {
                return false;
            }

            try
            {
                if (!File.Exists(application.Path))
                {
                    throw new FileNotFoundException("程序入口不存在。", application.Path);
                }
                Process.Start(new ProcessStartInfo { FileName = application.Path, UseShellExecute = true });
                ApplicationLogger.Info("Application launched from selected result. Name: " + application.Name);
                Hide();
                return true;
            }
            catch (Exception ex)
            {
                ApplicationLogger.Error("Failed to launch selected application. Name: " + application.Name, ex);
                ShowError("无法打开程序 “" + application.Name + "”", ex);
                return true;
            }
        }

        private void resultList_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedResult();
        }

        private void startupMenuItem_Click(object sender, EventArgs e)
        {
            bool enabled = !startupMenuItem.Checked;
            try
            {
                StartupManager.SetEnabled(enabled);
                startupMenuItem.Checked = enabled;
            }
            catch (Exception ex)
            {
                ShowError("设置开机启动失败", ex);
            }
        }

        private void openLogMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationLogger.Info("Opening log directory.");
                Process.Start(new ProcessStartInfo
                {
                    FileName = ApplicationLogger.LogDirectory,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ShowError("无法打开日志目录", ex);
            }
        }

        private static void ShowError(string context, Exception exception)
        {
            ApplicationLogger.Error(context, exception);
            MessageBox.Show(context + "：" + exception.Message, "SearchBar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        private string calc(string str)
        {
            return CalcExpression.Calc(str);
        }

        #endregion

        private void SearchBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationLogger.Info("SearchBox closing. Unregistering Ctrl+Q hotkey.");
            HotKey.UnregisterHotKey(Handle, 100);
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

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openSearch_Click(object sender, EventArgs e)
        {
            ShowSearchWindow();
        }

        private void nofityIcon_Click(object sender, EventArgs e)
        {
            // Kept for the designer event reference. Runtime uses MouseClick to distinguish buttons.
        }

        private void nofityIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowSearchWindow();
            }
        }

        private void ShowSearchWindow()
        {
            TopMost = true;
            Show();
            Activate();
            txtContent.Focus();
        }
    }
}

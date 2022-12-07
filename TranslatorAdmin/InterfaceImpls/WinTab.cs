using System.Runtime.Versioning;
using Translator;
using Translator.Core.Helpers;
using Translator.Helpers;
using Translator.UICompatibilityLayer;
using TranslatorAdmin.Properties;

namespace TranslatorAdmin.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    internal class WinTab : TabPage, ITab<WinLineItem>
    {
        private static Fenster MainForm { get; set; } = new Fenster() { Visible = false };

        private static int Number { get; set; } = 0;

        internal static void Initialize(Fenster fenster) => MainForm = fenster;

        public WinTab()
        {
            ++Number;
            BackColor = Color.Black;
            ForeColor = SystemColors.ScrollBar;
            BorderStyle = BorderStyle.FixedSingle;
            Location = new Point(4, 22);
            Name = $"TabPage{Number}";
            Padding = new Padding(3);
            TabIndex = 0;
            Text = $"Tab{Number}";
            var TranslateThis = new Button();
            var ApprovedBox = new CheckBox();
            var CheckListBoxLeft = new ColouredCheckedListBox();
            var ListContextMenu = new ContextMenuStrip();
            var CommentGroup = new GroupBox();
            var CharacterCountLabel = new Label();
            var SelectedFile = new Label();
            var WordsTranslated = new Label();
            var ProgressbarTranslated = new NoAnimationBar();
            var panel1 = new Panel();
            var panel2 = new Panel();
            var mainTableLayoutPanel = new TableLayoutPanel();
            var CommentTextBox = new TextBox();
            var TemplateTextBox = new TextBox();
            var TranslatedTextBox = new TextBox();
            var CopyAllContextMenuButton = new ToolStripMenuItem();
            var CopyAsOutputContextMenuButton = new ToolStripMenuItem();
            var CopyFileNameContextMenuButton = new ToolStripMenuItem();
            var CopyIdContextMenuButton = new ToolStripMenuItem();
            var CopyStoryNameContextMenuButton = new ToolStripMenuItem();
            var CopyTemplateContextMenuButton = new ToolStripMenuItem();
            var CopyTranslationContextMenuButton = new ToolStripMenuItem();
            mainTableLayoutPanel.SuspendLayout();
            CommentGroup.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // TranslatedTextBox
            // 
            TranslatedTextBox.AcceptsReturn = true;
            TranslatedTextBox.AllowDrop = true;
            TranslatedTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            TranslatedTextBox.BackColor = Utils.background;
            TranslatedTextBox.Dock = DockStyle.Fill;
            TranslatedTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TranslatedTextBox.ForeColor = Utils.brightText;
            TranslatedTextBox.ImeMode = ImeMode.On;
            TranslatedTextBox.Location = new Point(689, 294);
            TranslatedTextBox.Multiline = true;
            TranslatedTextBox.Name = "TranslatedTextBox";
            TranslatedTextBox.Size = new Size(678, 275);
            TranslatedTextBox.TabIndex = 0;
            TranslatedTextBox.Text = "edit here";
            TranslatedTextBox.TextChanged += new EventHandler(MainForm.TextBoxRight_TextChanged);
            TranslatedTextBox.MouseUp += new MouseEventHandler(MainForm.TextContextOpened);
            TranslatedTextBox.MouseEnter += new EventHandler(MainForm.TextContextOpened);
            // 
            // AutoTranslateThis
            // 
            TranslateThis.AutoSize = true;
            TranslateThis.BackColor = Utils.menu;
            TranslateThis.ForeColor = Utils.darkText;
            TranslateThis.Location = new Point(80, 1);
            TranslateThis.Name = "AutoTranslateThis";
            TranslateThis.Size = new Size(60, 20);
            TranslateThis.TabIndex = 13;
            TranslateThis.Text = "Automatic Translation";
            TranslateThis.UseVisualStyleBackColor = true;
            TranslateThis.Click += new EventHandler(MainForm.TranslateThis_Click);
            // 
            // TemplateTextBox
            // 
            TemplateTextBox.BackColor = Utils.background;
            TemplateTextBox.Dock = DockStyle.Fill;
            TemplateTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TemplateTextBox.ForeColor = Utils.brightText;
            TemplateTextBox.Location = new Point(689, 33);
            TemplateTextBox.Multiline = true;
            TemplateTextBox.Name = "TemplateTextBox";
            TemplateTextBox.ReadOnly = true;
            TemplateTextBox.Size = new Size(678, 255);
            TemplateTextBox.TabIndex = 9;
            TemplateTextBox.Text = "Lorem ipsum dolor sit amed";
            // 
            // CommentTextBox
            // 
            CommentTextBox.BackColor = Utils.background;
            CommentTextBox.Dock = DockStyle.Fill;
            CommentTextBox.Font = new Font("Consolas", 11F);
            CommentTextBox.ForeColor = Utils.brightText;
            CommentTextBox.Location = new Point(3, 16);
            CommentTextBox.Multiline = true;
            CommentTextBox.Name = "CommentTextBox";
            CommentTextBox.Size = new Size(672, 105);
            CommentTextBox.TabIndex = 13;
            CommentTextBox.TextChanged += new EventHandler(MainForm.Comments_TextChanged);
            CommentTextBox.MouseUp += new MouseEventHandler(MainForm.TextContextOpened);
            CommentTextBox.MouseEnter += new EventHandler(MainForm.TextContextOpened);
            // 
            // CharacterCountLabel
            // 
            CharacterCountLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CharacterCountLabel.AutoSize = true;
            CharacterCountLabel.ForeColor = Utils.brightText;
            CharacterCountLabel.Location = new Point(23, 5);
            CharacterCountLabel.Name = "CharacterCountLabel";
            CharacterCountLabel.Size = new Size(143, 13);
            CharacterCountLabel.TabIndex = 20;
            CharacterCountLabel.Text = "Template: xx | Translation: xx";
            // 
            // SelectedFile
            // 
            SelectedFile.AutoSize = true;
            SelectedFile.ForeColor = Utils.brightText;
            SelectedFile.Location = new Point(0, 6);
            SelectedFile.Name = "SelectedFile";
            SelectedFile.Size = new Size(98, 13);
            SelectedFile.TabIndex = 7;
            SelectedFile.Text = "Selected File: none";
            // 
            // WordsTranslated
            // 
            WordsTranslated.Anchor = AnchorStyles.Top;
            WordsTranslated.Parent = panel1;
            WordsTranslated.AutoSize = true;
            WordsTranslated.BackColor = Color.Transparent;
            WordsTranslated.ForeColor = Utils.brightText;
            WordsTranslated.Location = new Point(60, 6);
            WordsTranslated.Name = "WordsTranslated";
            WordsTranslated.Size = new Size(47, 13);
            WordsTranslated.TabIndex = 15;
            WordsTranslated.Text = "progress";
            // 
            // ApprovedBox
            // 
            ApprovedBox.AutoSize = true;
            ApprovedBox.ForeColor = Utils.brightText;
            ApprovedBox.Location = new Point(3, 5);
            ApprovedBox.Name = "ApprovedBox";
            ApprovedBox.Size = new Size(72, 17);
            ApprovedBox.TabIndex = 13;
            ApprovedBox.Text = Resources.Approved;
            ApprovedBox.UseVisualStyleBackColor = true;
            ApprovedBox.CheckedChanged += new EventHandler(MainForm.ApprovedBox_CheckedChanged);
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 2;
            _ = mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.07924F));
            _ = mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.92076F));
            mainTableLayoutPanel.Controls.Add(CommentGroup, 1, 3);
            mainTableLayoutPanel.Controls.Add(TranslatedTextBox, 1, 2);
            mainTableLayoutPanel.Controls.Add(TemplateTextBox, 1, 1);
            mainTableLayoutPanel.Controls.Add(CheckListBoxLeft, 0, 1);
            mainTableLayoutPanel.Controls.Add(panel1, 0, 0);
            mainTableLayoutPanel.Controls.Add(panel2, 1, 0);
            mainTableLayoutPanel.Parent = this;
            mainTableLayoutPanel.Dock = DockStyle.Fill;
            mainTableLayoutPanel.Location = new Point(3, 3);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowCount = 4;
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 38.94275F));
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 41.86569F));
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 19.19156F));
            mainTableLayoutPanel.Size = new Size(1370, 702);
            mainTableLayoutPanel.TabIndex = 18;
            // 
            // CommentGroup
            // 
            CommentGroup.Controls.Add(CommentTextBox);
            CommentGroup.Dock = DockStyle.Fill;
            CommentGroup.ForeColor = Utils.brightText;
            CommentGroup.Location = new Point(689, 575);
            CommentGroup.Name = "CommentGroup";
            CommentGroup.Size = new Size(678, 124);
            CommentGroup.TabIndex = 11;
            CommentGroup.TabStop = false;
            CommentGroup.Text = "Comments";
            // 
            // panel1
            // 
            panel1.Controls.Add(SelectedFile);
            panel1.Controls.Add(WordsTranslated);
            panel1.Controls.Add(ProgressbarTranslated);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(680, 24);
            panel1.TabIndex = 12;
            // 
            // panel2
            // 
            panel2.Controls.Add(ApprovedBox);
            panel2.Controls.Add(TranslateThis);
            panel2.Controls.Add(CharacterCountLabel);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(689, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(678, 24);
            panel2.TabIndex = 13;
            // 
            // CheckListBoxLeft
            // 
            CheckListBoxLeft.BackColor = Utils.frame;
            CheckListBoxLeft.Dock = DockStyle.Fill;
            CheckListBoxLeft.ForeColor = Utils.brightText;
            CheckListBoxLeft.FormattingEnabled = true;
            CheckListBoxLeft.Location = new Point(3, 33);
            CheckListBoxLeft.Name = "CheckListBoxLeft";
            mainTableLayoutPanel.SetRowSpan(CheckListBoxLeft, 3);
            CheckListBoxLeft.Size = new Size(680, 666);
            CheckListBoxLeft.TabIndex = 10;
            CheckListBoxLeft.ThreeDCheckBoxes = true;
            CheckListBoxLeft.ItemCheck += new ItemCheckEventHandler(MainForm.CheckListBoxLeft_ItemCheck);
            CheckListBoxLeft.SelectedIndexChanged += new EventHandler(MainForm.CheckListBoxLeft_SelectedIndexChanged);
            CheckListBoxLeft.ContextMenuStrip = ListContextMenu;
            CheckListBoxLeft.MouseDown += new MouseEventHandler(MainForm.OpeningContextMenu);
            // 
            // ProgressbarTranslated
            // 
            ProgressbarTranslated.BackColor = Utils.background;
            ProgressbarTranslated.Cursor = Cursors.Default;
            ProgressbarTranslated.Dock = DockStyle.Fill;
            ProgressbarTranslated.ForeColor = Utils.foreground;
            ProgressbarTranslated.Location = new Point(0, 0);
            ProgressbarTranslated.Name = "ProgressbarTranslated";
            ProgressbarTranslated.Size = new Size(680, 24);
            ProgressbarTranslated.Step = 1;
            ProgressbarTranslated.Style = ProgressBarStyle.Continuous;
            ProgressbarTranslated.TabIndex = 8;
            ProgressbarTranslated.Value = 50;
            // 
            // CopyIdContextMenuButton
            // 
            CopyIdContextMenuButton.Name = "CopyIdContextMenuButton";
            CopyIdContextMenuButton.Size = new Size(236, 22);
            CopyIdContextMenuButton.Text = "Copy Id";
            CopyIdContextMenuButton.Click += new EventHandler(MainForm.CopyIdContextMenuButton_Click);
            // 
            // CopyFileNameContextMenuButton
            // 
            CopyFileNameContextMenuButton.Name = "CopyFileNameContextMenuButton";
            CopyFileNameContextMenuButton.Size = new Size(236, 22);
            CopyFileNameContextMenuButton.Text = "Copy file name";
            CopyFileNameContextMenuButton.Click += new EventHandler(MainForm.CopyFileNameContextMenuButton_Click);
            // 
            // CopyStoryNameContextMenuButton
            // 
            CopyStoryNameContextMenuButton.Name = "CopyStoryNameContextMenuButton";
            CopyStoryNameContextMenuButton.Size = new Size(236, 22);
            CopyStoryNameContextMenuButton.Text = "Copy story name";
            CopyStoryNameContextMenuButton.Click += new EventHandler(MainForm.CopyStoryNameContextMenuButton_Click);
            // 
            // CopyAllContextMenuButton
            // 
            CopyAllContextMenuButton.Name = "CopyAllContextMenuButton";
            CopyAllContextMenuButton.Size = new Size(236, 22);
            CopyAllContextMenuButton.Text = "Copy everything";
            CopyAllContextMenuButton.Click += new EventHandler(MainForm.CopyAllContextMenuButton_Click);
            // 
            // CopyAsOutputContextMenuButton
            // 
            CopyAsOutputContextMenuButton.Name = "CopyAsOutputContextMenuButton";
            CopyAsOutputContextMenuButton.Size = new Size(236, 22);
            CopyAsOutputContextMenuButton.Text = "Copy as output";
            CopyAsOutputContextMenuButton.Click += new EventHandler(MainForm.CopyAsOutputContextMenuButton_Click);
            // 
            // CopyTemplateContextMenuButton
            // 
            CopyTemplateContextMenuButton.Name = "CopyTemplateContextMenuButton";
            CopyTemplateContextMenuButton.Size = new Size(236, 22);
            CopyTemplateContextMenuButton.Text = "Copy template";
            CopyTemplateContextMenuButton.Click += new EventHandler(MainForm.CopyTemplateContextMenuButton_Click);
            // 
            // CopyTranslationContextMenuButton
            // 
            CopyTranslationContextMenuButton.Name = "CopyTranslationContextMenuButton";
            CopyTranslationContextMenuButton.Size = new Size(236, 22);
            CopyTranslationContextMenuButton.Text = "Copy translation";
            CopyTranslationContextMenuButton.Click += new EventHandler(MainForm.CopyTranslationContextMenuButton_Click);
            //
            // ListContextMenu
            //
            ListContextMenu.Name = "ListContextMenu";
            ListContextMenu.BackColor = SystemColors.ScrollBar;
            ListContextMenu.ShowCheckMargin = false;
            ListContextMenu.ShowImageMargin = false;
            ListContextMenu.ForeColor = SystemColors.MenuText;
            ListContextMenu.Size = new Size(236, 160);
            ListContextMenu.Items.Clear();
            ListContextMenu.Items.AddRange(new ToolStripItem[]{
                CopyIdContextMenuButton,
                CopyFileNameContextMenuButton,
                CopyStoryNameContextMenuButton,
                CopyTemplateContextMenuButton,
                CopyTranslationContextMenuButton,
                CopyAsOutputContextMenuButton,
                CopyAllContextMenuButton
            });

            Controls.Add(mainTableLayoutPanel);

            mainTableLayoutPanel.ResumeLayout();
            mainTableLayoutPanel.PerformLayout();
            CommentGroup.ResumeLayout();
            CommentGroup.PerformLayout();
            panel1.ResumeLayout();
            panel1.PerformLayout();
            panel2.ResumeLayout();
            panel2.PerformLayout();
            ResumeLayout();
        }

        //todo
        public bool IsApproveButtonFocused => throw new NotImplementedException();

        public List<string> SimilarStringsToEnglish => throw new NotImplementedException();

        public NullLineList<WinLineItem> Lines { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SelectedLineIndex => throw new NotImplementedException();

        public WinLineItem SelectedLineItem => throw new NotImplementedException();

        public bool IsTranslationBoxFocused => throw new NotImplementedException();

        public bool IsCommentBoxFocused => throw new NotImplementedException();

        public int ProgressValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TranslationBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string SelectedTranslationBoxText => throw new NotImplementedException();

        public string TemplateBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string SelectedTemplateBoxText => throw new NotImplementedException();

        public string CommentBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string[] CommentBoxTextArr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ApprovedButtonChecked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        NullLineList<WinLineItem> ITab<WinLineItem>.Lines { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ApproveSelectedLine() => throw new NotImplementedException();
        public WinLineItem AtIndex(int index) => throw new NotImplementedException();
        public void ClearLines() => throw new NotImplementedException();
        public void FocusCommentBox() => throw new NotImplementedException();
        public void FocusTranslationBox() => throw new NotImplementedException();
        public string SelectedCommentBoxText() => throw new NotImplementedException();
        public void SelectLineItem(int index) => throw new NotImplementedException();
        public void SelectLineItem(WinLineItem item) => throw new NotImplementedException();
        public void SetApprovedLabelText(string text) => throw new NotImplementedException();
        public void SetCharacterCountLabelText(string text) => throw new NotImplementedException();
        public void SetCharacterLabelColor(Color lawnGreen) => throw new NotImplementedException();
        public void SetFileInfoText(string info) => throw new NotImplementedException();
        public void SetSelectedCommentBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTemplateBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTranslationBoxText(int start, int end) => throw new NotImplementedException();
        public void UnapproveSelectedLine() => throw new NotImplementedException();
        public void UpdateLines() => throw new NotImplementedException();
    }
}

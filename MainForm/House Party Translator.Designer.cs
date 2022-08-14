using HousePartyTranslator.Helpers;

namespace HousePartyTranslator
{
    partial class Fenster
    {
        private System.Windows.Forms.CheckBox ApprovedBox;
        private System.Windows.Forms.Label CharacterCountLabel;
        private ColouredCheckedListBox CheckListBoxLeft;
        private System.Windows.Forms.GroupBox CommentGroup;
        private System.Windows.Forms.TextBox CommentTextBox;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStripMenuItem CopyAllContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem CopyAsOutputContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem CopyFileNameContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem CopyIdContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem CopyStoryNameContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem CopyTemplateContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem CopyTranslationContextMenuButton;
        private System.Windows.Forms.ToolStripMenuItem customOpenStoryExplorer;
        private System.Windows.Forms.TextBox TenmplateTextBox;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox languageToolStripComboBox;
        private System.Windows.Forms.ContextMenuStrip ListContextMenu;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.ToolStripMenuItem openAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInNewTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private NoAnimationBar ProgressbarTranslated;
        private System.Windows.Forms.ToolStripMenuItem Recents;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCommentsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveCurrentStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox searchToolStripTextBox;
        private System.Windows.Forms.Label SelectedFile;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storyExplorerStripMenuItem1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripTextBox ToolStripMenuReplaceBox;
        private System.Windows.Forms.ToolStripMenuItem toolStripReplaceButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TextBox TranslatedTextBox;
        private System.Windows.Forms.Button AutoTranslateThis;
        private System.Windows.Forms.Label WordsTranslated;

        private System.Drawing.Color foreground = System.Drawing.SystemColors.Window;
        private System.Drawing.Color background = System.Drawing.SystemColors.ControlDarkDark;
        private System.Drawing.Color backgroundDarker = System.Drawing.SystemColors.MenuText;
        private System.Drawing.Color brightText = System.Drawing.SystemColors.HighlightText;
        private System.Drawing.Color darkText = System.Drawing.SystemColors.WindowText;
        private System.Drawing.Color menu = System.Drawing.SystemColors.ScrollBar;
        private System.Drawing.Color frame = System.Drawing.SystemColors.WindowFrame;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fenster));
            this.ApprovedBox = new System.Windows.Forms.CheckBox();
            this.CharacterCountLabel = new System.Windows.Forms.Label();
            this.CheckListBoxLeft = new HousePartyTranslator.Helpers.ColouredCheckedListBox();
            this.CommentGroup = new System.Windows.Forms.GroupBox();
            this.CommentTextBox = new System.Windows.Forms.TextBox();
            this.CopyAllContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyAsOutputContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyFileNameContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyIdContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyStoryNameContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyTemplateContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyTranslationContextMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.customOpenStoryExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.TenmplateTextBox = new System.Windows.Forms.TextBox();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.ListContextMenu = new System.Windows.Forms.ContextMenuStrip();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.openInNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ProgressbarTranslated = new HousePartyTranslator.Helpers.NoAnimationBar();
            this.Recents = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCommentsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrentStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.SelectedFile = new System.Windows.Forms.Label();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyExplorerStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ToolStripMenuReplaceBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripReplaceButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TranslatedTextBox = new System.Windows.Forms.TextBox();
            this.AutoTranslateThis = new System.Windows.Forms.Button();
            this.WordsTranslated = new System.Windows.Forms.Label();
            this.MainMenu.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            this.CommentGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TranslatedTextBox
            // 
            this.TranslatedTextBox.AcceptsReturn = true;
            this.TranslatedTextBox.AllowDrop = true;
            this.TranslatedTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TranslatedTextBox.BackColor = this.background;
            this.TranslatedTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TranslatedTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TranslatedTextBox.ForeColor = this.brightText;
            this.TranslatedTextBox.ImeMode = System.Windows.Forms.ImeMode.On;
            this.TranslatedTextBox.Location = new System.Drawing.Point(689, 294);
            this.TranslatedTextBox.Multiline = true;
            this.TranslatedTextBox.Name = "TranslatedTextBox";
            this.TranslatedTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TranslatedTextBox.Size = new System.Drawing.Size(678, 275);
            this.TranslatedTextBox.TabIndex = 0;
            this.TranslatedTextBox.Text = "edit here";
            this.TranslatedTextBox.TextChanged += new System.EventHandler(this.TextBoxRight_TextChanged);
            this.TranslatedTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextContextOpened);
            // 
            // EnglishTextBox
            // 
            this.TenmplateTextBox.BackColor = this.background;
            this.TenmplateTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TenmplateTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TenmplateTextBox.ForeColor = this.brightText;
            this.TenmplateTextBox.Location = new System.Drawing.Point(689, 33);
            this.TenmplateTextBox.Multiline = true;
            this.TenmplateTextBox.Name = "TemplateTextBox";
            this.TenmplateTextBox.ReadOnly = true;
            this.TenmplateTextBox.Size = new System.Drawing.Size(678, 255);
            this.TenmplateTextBox.TabIndex = 9;
            this.TenmplateTextBox.Text = "Lorem ipsum dolor sit amed";
            // 
            // CommentTextBox
            // 
            this.CommentTextBox.BackColor = this.background;
            this.CommentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentTextBox.Font = new System.Drawing.Font("Consolas", 11F);
            this.CommentTextBox.ForeColor = this.brightText;
            this.CommentTextBox.Location = new System.Drawing.Point(3, 16);
            this.CommentTextBox.Multiline = true;
            this.CommentTextBox.Name = "CommentTextBox";
            this.CommentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CommentTextBox.Size = new System.Drawing.Size(672, 105);
            this.CommentTextBox.TabIndex = 13;
            this.CommentTextBox.TextChanged += new System.EventHandler(this.Comments_TextChanged);
            this.CommentTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextContextOpened);
            // 
            // CharacterCountLabel
            // 
            this.CharacterCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CharacterCountLabel.AutoSize = true;
            this.CharacterCountLabel.ForeColor = this.brightText;
            this.CharacterCountLabel.Location = new System.Drawing.Point(501, 5);
            this.CharacterCountLabel.Name = "CharacterCountLabel";
            this.CharacterCountLabel.Size = new System.Drawing.Size(143, 13);
            this.CharacterCountLabel.TabIndex = 16;
            this.CharacterCountLabel.Text = "Template: xx | Translation: xx";
            // 
            // SelectedFile
            // 
            this.SelectedFile.AutoSize = true;
            this.SelectedFile.ForeColor = this.brightText;
            this.SelectedFile.Location = new System.Drawing.Point(0, 6);
            this.SelectedFile.Name = "SelectedFile";
            this.SelectedFile.Size = new System.Drawing.Size(98, 13);
            this.SelectedFile.TabIndex = 7;
            this.SelectedFile.Text = "Selected File: none";
            // 
            // WordsTranslated
            // 
            this.WordsTranslated.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WordsTranslated.AutoSize = true;
            this.WordsTranslated.BackColor = System.Drawing.Color.Transparent;
            this.WordsTranslated.ForeColor = this.brightText;
            this.WordsTranslated.Location = new System.Drawing.Point(302, 6);
            this.WordsTranslated.Name = "WordsTranslated";
            this.WordsTranslated.Size = new System.Drawing.Size(47, 13);
            this.WordsTranslated.TabIndex = 7;
            this.WordsTranslated.Text = "progress";
            // 
            // ApprovedBox
            // 
            this.ApprovedBox.AutoSize = true;
            this.ApprovedBox.ForeColor = this.brightText;
            this.ApprovedBox.Location = new System.Drawing.Point(3, 5);
            this.ApprovedBox.Name = "ApprovedBox";
            this.ApprovedBox.Size = new System.Drawing.Size(72, 17);
            this.ApprovedBox.TabIndex = 13;
            this.ApprovedBox.Text = global::HousePartyTranslator.Properties.Resources.Approved;
            this.ApprovedBox.UseVisualStyleBackColor = true;
            this.ApprovedBox.CheckedChanged += new System.EventHandler(this.ApprovedBox_CheckedChanged);
            // 
            // AutoTranslateThis
            // 
            this.AutoTranslateThis.AutoSize = true;
            this.AutoTranslateThis.BackColor = this.menu;
            this.AutoTranslateThis.ForeColor = this.darkText;
            this.AutoTranslateThis.Location = new System.Drawing.Point(80, 1);
            this.AutoTranslateThis.Name = "AutoTranslateThis";
            this.AutoTranslateThis.Size = new System.Drawing.Size(100, 20);
            this.AutoTranslateThis.TabIndex = 13;
            this.AutoTranslateThis.Text = "Automatic Translation";
            this.AutoTranslateThis.UseVisualStyleBackColor = true;
            this.AutoTranslateThis.Click += new System.EventHandler(this.TranslateThis_Click);
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.saveCurrentStringToolStripMenuItem,
            this.saveCommentsToolStripMenuItem1,
            this.searchToolStripTextBox,
            this.ToolStripMenuReplaceBox,
            this.toolStripReplaceButton,
            this.languageToolStripComboBox,
            this.storyExplorerStripMenuItem1,
            this.customOpenStoryExplorer,
            this.settingsToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(1384, 27);
            this.MainMenu.TabIndex = 17;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = this.menu;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openAllToolStripMenuItem,
            this.openInNewTabToolStripMenuItem,
            this.toolStripSeparator2,
            this.Recents,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Opens a dialog to select a file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // openAllToolStripMenuItem
            // 
            this.openAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openAllToolStripMenuItem.Name = "openAllToolStripMenuItem";
            this.openAllToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openAllToolStripMenuItem.Text = "Open &all";
            this.openAllToolStripMenuItem.ToolTipText = "Opens a dialog to select a file, all others will be discovered automatically. Usually.";
            this.openAllToolStripMenuItem.Click += new System.EventHandler(this.OpenAllToolStripMenuItem_Click);
            // 
            // openInNewTabToolStripMenuItem
            // 
            this.openInNewTabToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openInNewTabToolStripMenuItem.Image")));
            this.openInNewTabToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openInNewTabToolStripMenuItem.Name = "openInNewTabToolStripMenuItem";
            this.openInNewTabToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openInNewTabToolStripMenuItem.Text = "Open in new tab";
            this.openInNewTabToolStripMenuItem.ToolTipText = "Opens a dialog to select a file";
            this.openInNewTabToolStripMenuItem.Click += new System.EventHandler(this.OpenInNewTabToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(233, 6);
            // 
            // Recents
            // 
            this.Recents.Enabled = false;
            this.Recents.Name = "Recents";
            this.Recents.ShowShortcutKeys = false;
            this.Recents.Size = new System.Drawing.Size(236, 22);
            this.Recents.Text = "Recents";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(233, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveAllToolStripMenuItem.Text = "Sa&ve All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.SaveAllToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(233, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // saveCurrentStringToolStripMenuItem
            // 
            this.saveCurrentStringToolStripMenuItem.BackColor = this.menu;
            this.saveCurrentStringToolStripMenuItem.Name = "saveCurrentStringToolStripMenuItem";
            this.saveCurrentStringToolStripMenuItem.Size = new System.Drawing.Size(122, 23);
            this.saveCurrentStringToolStripMenuItem.Text = "&Save selected string";
            this.saveCurrentStringToolStripMenuItem.Click += new System.EventHandler(this.SaveCurrentStringToolStripMenuItem_Click);
            // 
            // saveCommentsToolStripMenuItem1
            // 
            this.saveCommentsToolStripMenuItem1.BackColor = this.menu;
            this.saveCommentsToolStripMenuItem1.Name = "saveCommentsToolStripMenuItem1";
            this.saveCommentsToolStripMenuItem1.Size = new System.Drawing.Size(103, 23);
            this.saveCommentsToolStripMenuItem1.Text = "Save &comments";
            this.saveCommentsToolStripMenuItem1.Click += new System.EventHandler(this.SaveCommentsToolStripMenuItem1_Click);
            // 
            // searchToolStripTextBox
            // 
            this.searchToolStripTextBox.BackColor = this.menu;
            this.searchToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchToolStripTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchToolStripTextBox.Name = "searchToolStripTextBox";
            this.searchToolStripTextBox.Size = new System.Drawing.Size(300, 23);
            this.searchToolStripTextBox.TextChanged += new System.EventHandler(this.SearchToolStripTextBox_TextChanged);
            // 
            // ToolStripMenuReplaceBox
            // 
            this.ToolStripMenuReplaceBox.BackColor = this.menu;
            this.ToolStripMenuReplaceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ToolStripMenuReplaceBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ToolStripMenuReplaceBox.Margin = new System.Windows.Forms.Padding(4, 0, 1, 0);
            this.ToolStripMenuReplaceBox.Name = "ToolStripMenuReplaceBox";
            this.ToolStripMenuReplaceBox.Size = new System.Drawing.Size(100, 23);
            this.ToolStripMenuReplaceBox.Visible = false;
            this.ToolStripMenuReplaceBox.TextChanged += new System.EventHandler(this.ToolStripMenuReplaceBox_TextChanged);
            // 
            // toolStripReplaceButton
            // 
            this.toolStripReplaceButton.BackColor = this.menu;
            this.toolStripReplaceButton.ForeColor = this.darkText;
            this.toolStripReplaceButton.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStripReplaceButton.Name = "toolStripReplaceButton";
            this.toolStripReplaceButton.Size = new System.Drawing.Size(63, 23);
            this.toolStripReplaceButton.Text = "Replace!";
            this.toolStripReplaceButton.Visible = false;
            this.toolStripReplaceButton.Click += new System.EventHandler(this.ToolStripReplaceButton_Click);
            // 
            // languageToolStripComboBox
            // 
            this.languageToolStripComboBox.BackColor = this.menu;
            this.languageToolStripComboBox.Items.AddRange(LanguageHelper.ShortLanguages);
            this.languageToolStripComboBox.Name = "languageToolStripComboBox";
            this.languageToolStripComboBox.Size = new System.Drawing.Size(75, 23);
            this.languageToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageToolStripComboBox_SelectedIndexChanged);
            // 
            // storyExplorerStripMenuItem1
            // 
            this.storyExplorerStripMenuItem1.BackColor = this.menu;
            this.storyExplorerStripMenuItem1.Name = "storyExplorerStripMenuItem1";
            this.storyExplorerStripMenuItem1.Size = new System.Drawing.Size(118, 23);
            this.storyExplorerStripMenuItem1.Text = "Auto Story&Explorer";
            this.storyExplorerStripMenuItem1.Click += new System.EventHandler(this.StoryExplorerStripMenuItem_Click);
            // 
            // customOpenStoryExplorer
            // 
            this.customOpenStoryExplorer.BackColor = this.menu;
            this.customOpenStoryExplorer.Name = "customOpenStoryExplorer";
            this.customOpenStoryExplorer.Size = new System.Drawing.Size(121, 23);
            this.customOpenStoryExplorer.Text = "Open StoryE&xplorer";
            this.customOpenStoryExplorer.Click += new System.EventHandler(this.CustomStoryExplorerStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.BackColor = this.menu;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.07924F));
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.92076F));
            this.mainTableLayoutPanel.Controls.Add(this.CommentGroup, 1, 3);
            this.mainTableLayoutPanel.Controls.Add(this.TranslatedTextBox, 1, 2);
            this.mainTableLayoutPanel.Controls.Add(this.TenmplateTextBox, 1, 1);
            this.mainTableLayoutPanel.Controls.Add(this.CheckListBoxLeft, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.panel1, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.panel2, 1, 0);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.94275F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.86569F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.19156F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(1370, 702);
            this.mainTableLayoutPanel.TabIndex = 18;
            // 
            // CommentGroup
            // 
            this.CommentGroup.Controls.Add(this.CommentTextBox);
            this.CommentGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentGroup.ForeColor = this.brightText;
            this.CommentGroup.Location = new System.Drawing.Point(689, 575);
            this.CommentGroup.Name = "CommentGroup";
            this.CommentGroup.Size = new System.Drawing.Size(678, 124);
            this.CommentGroup.TabIndex = 11;
            this.CommentGroup.TabStop = false;
            this.CommentGroup.Text = "Comments";
            // 
            // CheckListBoxLeft
            // 
            this.CheckListBoxLeft.BackColor = this.frame;
            this.CheckListBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CheckListBoxLeft.ForeColor = this.brightText;
            this.CheckListBoxLeft.FormattingEnabled = true;
            this.CheckListBoxLeft.Location = new System.Drawing.Point(3, 33);
            this.CheckListBoxLeft.Name = "CheckListBoxLeft";
            this.mainTableLayoutPanel.SetRowSpan(this.CheckListBoxLeft, 3);
            this.CheckListBoxLeft.Size = new System.Drawing.Size(680, 666);
            this.CheckListBoxLeft.TabIndex = 10;
            this.CheckListBoxLeft.ThreeDCheckBoxes = true;
            this.CheckListBoxLeft.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckListBoxLeft_ItemCheck);
            this.CheckListBoxLeft.SelectedIndexChanged += new System.EventHandler(this.CheckListBoxLeft_SelectedIndexChanged);
            this.CheckListBoxLeft.ContextMenuStrip = this.ListContextMenu;
            this.CheckListBoxLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OpeningContextMenu);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SelectedFile);
            this.panel1.Controls.Add(this.WordsTranslated);
            this.panel1.Controls.Add(this.ProgressbarTranslated);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(680, 24);
            this.panel1.TabIndex = 12;
            // 
            // ProgressbarTranslated
            // 
            this.ProgressbarTranslated.BackColor = this.background;
            this.ProgressbarTranslated.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressbarTranslated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressbarTranslated.ForeColor = this.foreground;
            this.ProgressbarTranslated.Location = new System.Drawing.Point(0, 0);
            this.ProgressbarTranslated.Name = "ProgressbarTranslated";
            this.ProgressbarTranslated.Size = new System.Drawing.Size(680, 24);
            this.ProgressbarTranslated.Step = 1;
            this.ProgressbarTranslated.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressbarTranslated.TabIndex = 8;
            this.ProgressbarTranslated.Value = 50;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ApprovedBox);
            this.panel2.Controls.Add(this.AutoTranslateThis);
            this.panel2.Controls.Add(this.CharacterCountLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(689, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(678, 24);
            this.panel2.TabIndex = 13;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 27);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(1384, 734);
            this.MainTabControl.TabIndex = 9;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            this.MainTabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CloseTab_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Black;
            this.tabPage1.Controls.Add(this.mainTableLayoutPanel);
            this.tabPage1.ForeColor = this.menu;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1376, 708);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tab1";
            // 
            // CopyIdContextMenuButton
            // 
            this.CopyIdContextMenuButton.Name = "CopyIdContextMenuButton";
            this.CopyIdContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyIdContextMenuButton.Text = "Copy Id";
            this.CopyIdContextMenuButton.Click += new System.EventHandler(this.CopyIdContextMenuButton_Click);
            // 
            // CopyFileNameContextMenuButton
            // 
            this.CopyFileNameContextMenuButton.Name = "CopyFileNameContextMenuButton";
            this.CopyFileNameContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyFileNameContextMenuButton.Text = "Copy file name";
            this.CopyFileNameContextMenuButton.Click += new System.EventHandler(this.CopyFileNameContextMenuButton_Click);
            // 
            // CopyStoryNameContextMenuButton
            // 
            this.CopyStoryNameContextMenuButton.Name = "CopyStoryNameContextMenuButton";
            this.CopyStoryNameContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyStoryNameContextMenuButton.Text = "Copy story name";
            this.CopyStoryNameContextMenuButton.Click += new System.EventHandler(this.CopyStoryNameContextMenuButton_Click);
            // 
            // CopyAllContextMenuButton
            // 
            this.CopyAllContextMenuButton.Name = "CopyAllContextMenuButton";
            this.CopyAllContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyAllContextMenuButton.Text = "Copy everything";
            this.CopyAllContextMenuButton.Click += new System.EventHandler(this.CopyAllContextMenuButton_Click);
            // 
            // CopyAsOutputContextMenuButton
            // 
            this.CopyAsOutputContextMenuButton.Name = "CopyAsOutputContextMenuButton";
            this.CopyAsOutputContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyAsOutputContextMenuButton.Text = "Copy as output";
            this.CopyAsOutputContextMenuButton.Click += new System.EventHandler(this.CopyAsOutputContextMenuButton_Click);
            // 
            // CopyTemplateContextMenuButton
            // 
            this.CopyTemplateContextMenuButton.Name = "CopyTemplateContextMenuButton";
            this.CopyTemplateContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyTemplateContextMenuButton.Text = "Copy template";
            this.CopyTemplateContextMenuButton.Click += new System.EventHandler(this.CopyTemplateContextMenuButton_Click);
            // 
            // CopyTranslationContextMenuButton
            // 
            this.CopyTranslationContextMenuButton.Name = "CopyTranslationContextMenuButton";
            this.CopyTranslationContextMenuButton.Size = new System.Drawing.Size(236, 22);
            this.CopyTranslationContextMenuButton.Text = "Copy translation";
            this.CopyTranslationContextMenuButton.Click += new System.EventHandler(this.CopyTranslationContextMenuButton_Click);
            //
            // ListContextMenu
            //
            this.ListContextMenu.Name = "ListContextMenu";
            this.ListContextMenu.BackColor = this.menu;
            this.ListContextMenu.ShowCheckMargin = false;
            this.ListContextMenu.ShowImageMargin = false;
            this.ListContextMenu.ForeColor = this.darkText;
            this.ListContextMenu.Size = new System.Drawing.Size(236, 160);
            this.ListContextMenu.Items.Clear();
            this.ListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]{
            CopyIdContextMenuButton,
            CopyFileNameContextMenuButton,
            CopyStoryNameContextMenuButton,
            CopyTemplateContextMenuButton,
            CopyTranslationContextMenuButton,
            CopyAsOutputContextMenuButton,
            CopyAllContextMenuButton});
            // 
            // Fenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = this.backgroundDarker;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.MainMenu);
            this.MinimumSize = new System.Drawing.Size(970, 470);
            this.Name = "Fenster";
            this.ShowIcon = false;
            this.Text = "HP Translator Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextContextOpened);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.CommentGroup.ResumeLayout(false);
            this.CommentGroup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.MainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        //private System.Windows.Forms.SplitContainer MainContainer;
    }
}

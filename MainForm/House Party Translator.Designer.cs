using HousePartyTranslator.Helpers;

namespace HousePartyTranslator
{
    partial class Fenster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fenster));
            this.TranslatedTextBox = new System.Windows.Forms.TextBox();
            this.EnglishTextBox = new System.Windows.Forms.TextBox();
            this.CommentTextBox = new System.Windows.Forms.TextBox();
            this.CharacterCountLabel = new System.Windows.Forms.Label();
            this.SelectedFile = new System.Windows.Forms.Label();
            this.WordsTranslated = new System.Windows.Forms.Label();
            this.ApprovedBox = new System.Windows.Forms.CheckBox();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Recents = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrentStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCommentsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.languageToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.storyExplorerStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.customOpenStoryExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CommentGroup = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.CheckListBoxLeft = new HousePartyTranslator.Helpers.ColouredCheckedListBox();
            this.ProgressbarTranslated = new HousePartyTranslator.Helpers.NoAnimationBar();
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
            this.TranslatedTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.TranslatedTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TranslatedTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TranslatedTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.TranslatedTextBox.ImeMode = System.Windows.Forms.ImeMode.On;
            this.TranslatedTextBox.Location = new System.Drawing.Point(689, 294);
            this.TranslatedTextBox.Multiline = true;
            this.TranslatedTextBox.Name = "TranslatedTextBox";
            this.TranslatedTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TranslatedTextBox.Size = new System.Drawing.Size(678, 275);
            this.TranslatedTextBox.TabIndex = 0;
            this.TranslatedTextBox.Text = "edit here";
            this.TranslatedTextBox.TextChanged += new System.EventHandler(this.TextBoxRight_TextChanged);
            // 
            // EnglishTextBox
            // 
            this.EnglishTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.EnglishTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EnglishTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnglishTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.EnglishTextBox.Location = new System.Drawing.Point(689, 33);
            this.EnglishTextBox.Multiline = true;
            this.EnglishTextBox.Name = "EnglishTextBox";
            this.EnglishTextBox.ReadOnly = true;
            this.EnglishTextBox.Size = new System.Drawing.Size(678, 255);
            this.EnglishTextBox.TabIndex = 9;
            this.EnglishTextBox.Text = "Lorem ipsum dolor sit amed";
            // 
            // CommentTextBox
            // 
            this.CommentTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CommentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentTextBox.Font = new System.Drawing.Font("Consolas", 11F);
            this.CommentTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.CommentTextBox.Location = new System.Drawing.Point(3, 16);
            this.CommentTextBox.Multiline = true;
            this.CommentTextBox.Name = "CommentTextBox";
            this.CommentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CommentTextBox.Size = new System.Drawing.Size(672, 105);
            this.CommentTextBox.TabIndex = 13;
            // 
            // CharacterCountLabel
            // 
            this.CharacterCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CharacterCountLabel.AutoSize = true;
            this.CharacterCountLabel.BackColor = System.Drawing.SystemColors.Desktop;
            this.CharacterCountLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.CharacterCountLabel.Location = new System.Drawing.Point(501, 5);
            this.CharacterCountLabel.Name = "CharacterCountLabel";
            this.CharacterCountLabel.Size = new System.Drawing.Size(143, 13);
            this.CharacterCountLabel.TabIndex = 16;
            this.CharacterCountLabel.Text = "Template: xx | Translation: xx";
            // 
            // SelectedFile
            // 
            this.SelectedFile.AutoSize = true;
            this.SelectedFile.ForeColor = System.Drawing.SystemColors.Control;
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
            this.WordsTranslated.ForeColor = System.Drawing.SystemColors.Control;
            this.WordsTranslated.Location = new System.Drawing.Point(302, 6);
            this.WordsTranslated.Name = "WordsTranslated";
            this.WordsTranslated.Size = new System.Drawing.Size(47, 13);
            this.WordsTranslated.TabIndex = 7;
            this.WordsTranslated.Text = "progress";
            // 
            // ApprovedBox
            // 
            this.ApprovedBox.AutoSize = true;
            this.ApprovedBox.ForeColor = System.Drawing.SystemColors.Control;
            this.ApprovedBox.Location = new System.Drawing.Point(3, 5);
            this.ApprovedBox.Name = "ApprovedBox";
            this.ApprovedBox.Size = new System.Drawing.Size(72, 17);
            this.ApprovedBox.TabIndex = 13;
            this.ApprovedBox.Text = global::HousePartyTranslator.Properties.Resources.Approved;
            this.ApprovedBox.UseVisualStyleBackColor = true;
            this.ApprovedBox.CheckedChanged += new System.EventHandler(this.ApprovedBox_CheckedChanged);
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.saveCurrentStringToolStripMenuItem,
            this.saveCommentsToolStripMenuItem1,
            this.searchToolStripTextBox,
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
            this.fileToolStripMenuItem.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openInNewTabToolStripMenuItem,
            this.toolStripSeparator2,
            this.Recents,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
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
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Opens a dialog to select a file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // openInNewTabToolStripMenuItem
            // 
            this.openInNewTabToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openInNewTabToolStripMenuItem.Image")));
            this.openInNewTabToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openInNewTabToolStripMenuItem.Name = "openInNewTabToolStripMenuItem";
            this.openInNewTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
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
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // saveCurrentStringToolStripMenuItem
            // 
            this.saveCurrentStringToolStripMenuItem.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.saveCurrentStringToolStripMenuItem.Name = "saveCurrentStringToolStripMenuItem";
            this.saveCurrentStringToolStripMenuItem.Size = new System.Drawing.Size(122, 23);
            this.saveCurrentStringToolStripMenuItem.Text = "&Save selected string";
            this.saveCurrentStringToolStripMenuItem.Click += new System.EventHandler(this.SaveCurrentStringToolStripMenuItem_Click);
            // 
            // saveCommentsToolStripMenuItem1
            // 
            this.saveCommentsToolStripMenuItem1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.saveCommentsToolStripMenuItem1.Name = "saveCommentsToolStripMenuItem1";
            this.saveCommentsToolStripMenuItem1.Size = new System.Drawing.Size(103, 23);
            this.saveCommentsToolStripMenuItem1.Text = "Save &comments";
            this.saveCommentsToolStripMenuItem1.Click += new System.EventHandler(this.SaveCommentsToolStripMenuItem1_Click);
            // 
            // searchToolStripTextBox
            // 
            this.searchToolStripTextBox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.searchToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchToolStripTextBox.Name = "searchToolStripTextBox";
            this.searchToolStripTextBox.Size = new System.Drawing.Size(300, 23);
            this.searchToolStripTextBox.TextChanged += new System.EventHandler(this.SearchToolStripTextBox_TextChanged);
            // 
            // languageToolStripComboBox
            // 
            this.languageToolStripComboBox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.languageToolStripComboBox.Items.AddRange(new object[] {
            "cs",
            "da",
            "de",
            "fi",
            "fr",
            "hu",
            "it",
            "ja",
            "ko",
            "pl",
            "pt",
            "ptbr",
            "es",
            "esmx",
            "tr",
            "cn"});
            this.languageToolStripComboBox.Name = "languageToolStripComboBox";
            this.languageToolStripComboBox.Size = new System.Drawing.Size(75, 23);
            this.languageToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageToolStripComboBox_SelectedIndexChanged);
            // 
            // storyExplorerStripMenuItem1
            // 
            this.storyExplorerStripMenuItem1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.storyExplorerStripMenuItem1.Name = "storyExplorerStripMenuItem1";
            this.storyExplorerStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.storyExplorerStripMenuItem1.Size = new System.Drawing.Size(118, 23);
            this.storyExplorerStripMenuItem1.Text = "Auto Story&Explorer";
            this.storyExplorerStripMenuItem1.Click += new System.EventHandler(this.StoryExplorerStripMenuItem_Click);
            // 
            // customOpenStoryExplorer
            // 
            this.customOpenStoryExplorer.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.customOpenStoryExplorer.Name = "customOpenStoryExplorer";
            this.customOpenStoryExplorer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.customOpenStoryExplorer.Size = new System.Drawing.Size(121, 23);
            this.customOpenStoryExplorer.Text = "Open StoryE&xplorer";
            this.customOpenStoryExplorer.Click += new System.EventHandler(this.CustomStoryExplorerStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.BackColor = System.Drawing.SystemColors.ScrollBar;
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
            this.mainTableLayoutPanel.Controls.Add(this.EnglishTextBox, 1, 1);
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
            this.CommentGroup.ForeColor = System.Drawing.SystemColors.Window;
            this.CommentGroup.Location = new System.Drawing.Point(689, 575);
            this.CommentGroup.Name = "CommentGroup";
            this.CommentGroup.Size = new System.Drawing.Size(678, 124);
            this.CommentGroup.TabIndex = 11;
            this.CommentGroup.TabStop = false;
            this.CommentGroup.Text = "Comments";
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
            // panel2
            // 
            this.panel2.Controls.Add(this.ApprovedBox);
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
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1376, 708);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tab1";
            // 
            // CheckListBoxLeft
            // 
            this.CheckListBoxLeft.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CheckListBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CheckListBoxLeft.ForeColor = System.Drawing.SystemColors.Window;
            this.CheckListBoxLeft.FormattingEnabled = true;
            this.CheckListBoxLeft.Location = new System.Drawing.Point(3, 33);
            this.CheckListBoxLeft.Name = "CheckListBoxLeft";
            this.mainTableLayoutPanel.SetRowSpan(this.CheckListBoxLeft, 3);
            this.CheckListBoxLeft.Size = new System.Drawing.Size(680, 666);
            this.CheckListBoxLeft.TabIndex = 10;
            this.CheckListBoxLeft.ThreeDCheckBoxes = true;
            this.CheckListBoxLeft.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckListBoxLeft_ItemCheck);
            this.CheckListBoxLeft.SelectedIndexChanged += new System.EventHandler(this.CheckListBoxLeft_SelectedIndexChanged);
            // 
            // ProgressbarTranslated
            // 
            this.ProgressbarTranslated.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ProgressbarTranslated.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressbarTranslated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressbarTranslated.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ProgressbarTranslated.Location = new System.Drawing.Point(0, 0);
            this.ProgressbarTranslated.Name = "ProgressbarTranslated";
            this.ProgressbarTranslated.Size = new System.Drawing.Size(680, 24);
            this.ProgressbarTranslated.Step = 1;
            this.ProgressbarTranslated.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressbarTranslated.TabIndex = 8;
            this.ProgressbarTranslated.Value = 50;
            // 
            // Fenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.MainMenu);
            this.MinimumSize = new System.Drawing.Size(970, 470);
            this.Name = "Fenster";
            this.ShowIcon = false;
            this.Text = "HP Translator Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fenster_FormClosing);
            this.Shown += new System.EventHandler(this.FormShown);
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

        private System.Windows.Forms.TextBox TranslatedTextBox;
        private System.Windows.Forms.TextBox EnglishTextBox;
        private NoAnimationBar ProgressbarTranslated;
        private System.Windows.Forms.TextBox CommentTextBox;
        private System.Windows.Forms.Label CharacterCountLabel;
        private System.Windows.Forms.Label SelectedFile;
        private System.Windows.Forms.Label WordsTranslated;
        private ColouredCheckedListBox CheckListBoxLeft;
        private System.Windows.Forms.CheckBox ApprovedBox;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox languageToolStripComboBox;
        private System.Windows.Forms.ToolStripTextBox searchToolStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem saveCurrentStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCommentsToolStripMenuItem1;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.GroupBox CommentGroup;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem storyExplorerStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem customOpenStoryExplorer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Recents;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInNewTabToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl MainTabControl;
        //private System.Windows.Forms.SplitContainer MainContainer;
    }
}


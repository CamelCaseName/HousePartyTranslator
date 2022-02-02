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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.CommentGroup = new System.Windows.Forms.GroupBox();
            this.CheckListBoxLeft = new HousePartyTranslator.Helpers.ColouredCheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ProgressbarTranslated = new HousePartyTranslator.Helpers.NoAnimationBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.MainMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.CommentGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.TranslatedTextBox.Location = new System.Drawing.Point(695, 307);
            this.TranslatedTextBox.Multiline = true;
            this.TranslatedTextBox.Name = "TranslatedTextBox";
            this.TranslatedTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TranslatedTextBox.Size = new System.Drawing.Size(686, 288);
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
            this.EnglishTextBox.Location = new System.Drawing.Point(695, 33);
            this.EnglishTextBox.Multiline = true;
            this.EnglishTextBox.Name = "EnglishTextBox";
            this.EnglishTextBox.ReadOnly = true;
            this.EnglishTextBox.Size = new System.Drawing.Size(686, 268);
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
            this.CommentTextBox.Size = new System.Drawing.Size(680, 111);
            this.CommentTextBox.TabIndex = 13;
            // 
            // CharacterCountLabel
            // 
            this.CharacterCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CharacterCountLabel.AutoSize = true;
            this.CharacterCountLabel.BackColor = System.Drawing.SystemColors.Desktop;
            this.CharacterCountLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.CharacterCountLabel.Location = new System.Drawing.Point(509, 5);
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
            this.WordsTranslated.Location = new System.Drawing.Point(305, 6);
            this.WordsTranslated.Name = "WordsTranslated";
            this.WordsTranslated.Size = new System.Drawing.Size(78, 13);
            this.WordsTranslated.TabIndex = 7;
            this.WordsTranslated.Text = "progress words";
            // 
            // ApprovedBox
            // 
            this.ApprovedBox.AutoSize = true;
            this.ApprovedBox.ForeColor = System.Drawing.SystemColors.Control;
            this.ApprovedBox.Location = new System.Drawing.Point(3, 5);
            this.ApprovedBox.Name = "ApprovedBox";
            this.ApprovedBox.Size = new System.Drawing.Size(72, 17);
            this.ApprovedBox.TabIndex = 13;
            this.ApprovedBox.Text = "Approved";
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
            this.customOpenStoryExplorer});
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Opens a dialog to select a file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
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
            "tr"});
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
            this.storyExplorerStripMenuItem1.Click += new System.EventHandler(this.StoryExplorerStripMenuItem1_Click);
            // 
            // customOpenStoryExplorer
            // 
            this.customOpenStoryExplorer.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.customOpenStoryExplorer.Name = "customOpenStoryExplorer";
            this.customOpenStoryExplorer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.customOpenStoryExplorer.Size = new System.Drawing.Size(121, 23);
            this.customOpenStoryExplorer.Text = "Open Sto&ryExplorer";
            this.customOpenStoryExplorer.Click += new System.EventHandler(this.CustomStoryExplorerStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.CommentGroup, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.TranslatedTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.EnglishTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CheckListBoxLeft, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 27);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.94275F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.86569F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.19156F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1384, 734);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // CommentGroup
            // 
            this.CommentGroup.Controls.Add(this.CommentTextBox);
            this.CommentGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentGroup.ForeColor = System.Drawing.SystemColors.Window;
            this.CommentGroup.Location = new System.Drawing.Point(695, 601);
            this.CommentGroup.Name = "CommentGroup";
            this.CommentGroup.Size = new System.Drawing.Size(686, 130);
            this.CommentGroup.TabIndex = 11;
            this.CommentGroup.TabStop = false;
            this.CommentGroup.Text = "Comments";
            // 
            // CheckListBoxLeft
            // 
            this.CheckListBoxLeft.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CheckListBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CheckListBoxLeft.ForeColor = System.Drawing.SystemColors.Window;
            this.CheckListBoxLeft.FormattingEnabled = true;
            this.CheckListBoxLeft.Location = new System.Drawing.Point(3, 33);
            this.CheckListBoxLeft.Name = "CheckListBoxLeft";
            this.tableLayoutPanel1.SetRowSpan(this.CheckListBoxLeft, 3);
            this.CheckListBoxLeft.Size = new System.Drawing.Size(686, 698);
            this.CheckListBoxLeft.TabIndex = 10;
            this.CheckListBoxLeft.ThreeDCheckBoxes = true;
            this.CheckListBoxLeft.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckListBoxLeft_ItemCheck);
            this.CheckListBoxLeft.SelectedIndexChanged += new System.EventHandler(this.CheckListBoxLeft_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SelectedFile);
            this.panel1.Controls.Add(this.WordsTranslated);
            this.panel1.Controls.Add(this.ProgressbarTranslated);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(686, 24);
            this.panel1.TabIndex = 12;
            // 
            // ProgressbarTranslated
            // 
            this.ProgressbarTranslated.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ProgressbarTranslated.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressbarTranslated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressbarTranslated.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ProgressbarTranslated.Location = new System.Drawing.Point(0, 0);
            this.ProgressbarTranslated.Name = "ProgressbarTranslated";
            this.ProgressbarTranslated.Size = new System.Drawing.Size(686, 24);
            this.ProgressbarTranslated.Step = 1;
            this.ProgressbarTranslated.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressbarTranslated.TabIndex = 8;
            this.ProgressbarTranslated.Value = 50;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ApprovedBox);
            this.panel2.Controls.Add(this.CharacterCountLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(695, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(686, 24);
            this.panel2.TabIndex = 13;
            // 
            // Fenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.MainMenu);
            this.Name = "Fenster";
            this.ShowIcon = false;
            this.Text = "HP Translator Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Fenster_FormClosing);
            this.Load += new System.EventHandler(this.Window_Load);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.CommentGroup.ResumeLayout(false);
            this.CommentGroup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox CommentGroup;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem storyExplorerStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem customOpenStoryExplorer;
        //private System.Windows.Forms.SplitContainer MainContainer;
    }
}


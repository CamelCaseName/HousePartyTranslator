
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
            this.TranslatedTextBox = new System.Windows.Forms.TextBox();
            this.SelectedFile = new System.Windows.Forms.Label();
            this.ProgressbarTranslated = new System.Windows.Forms.ProgressBar();
            this.WordsTranslated = new System.Windows.Forms.Label();
            this.CheckListBoxLeft = new System.Windows.Forms.CheckedListBox();
            this.OpenFile = new System.Windows.Forms.Button();
            this.SaveFile = new System.Windows.Forms.Button();
            this.SaveFileAs = new System.Windows.Forms.Button();
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.LanguageBox = new System.Windows.Forms.ComboBox();
            this.CharacterCountLabel = new System.Windows.Forms.Label();
            this.SaveCommentsButton = new System.Windows.Forms.Button();
            this.CommentLabel = new System.Windows.Forms.Label();
            this.CommentTextBox = new System.Windows.Forms.TextBox();
            this.SaveCurrentString = new System.Windows.Forms.Button();
            this.EnglishTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel1.SuspendLayout();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // TranslatedTextBox
            // 
            this.TranslatedTextBox.AcceptsReturn = true;
            this.TranslatedTextBox.AllowDrop = true;
            this.TranslatedTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TranslatedTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.TranslatedTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TranslatedTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.TranslatedTextBox.ImeMode = System.Windows.Forms.ImeMode.On;
            this.TranslatedTextBox.Location = new System.Drawing.Point(6, 262);
            this.TranslatedTextBox.Multiline = true;
            this.TranslatedTextBox.Name = "TranslatedTextBox";
            this.TranslatedTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TranslatedTextBox.Size = new System.Drawing.Size(703, 293);
            this.TranslatedTextBox.TabIndex = 0;
            this.TranslatedTextBox.Text = "edit here";
            this.TranslatedTextBox.TextChanged += new System.EventHandler(this.TextBoxRight_TextChanged);
            // 
            // SelectedFile
            // 
            this.SelectedFile.AutoSize = true;
            this.SelectedFile.ForeColor = System.Drawing.SystemColors.Control;
            this.SelectedFile.Location = new System.Drawing.Point(313, 16);
            this.SelectedFile.Name = "SelectedFile";
            this.SelectedFile.Size = new System.Drawing.Size(98, 13);
            this.SelectedFile.TabIndex = 7;
            this.SelectedFile.Text = "Selected File: none";
            // 
            // ProgressbarTranslated
            // 
            this.ProgressbarTranslated.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressbarTranslated.ForeColor = System.Drawing.SystemColors.Window;
            this.ProgressbarTranslated.Location = new System.Drawing.Point(282, 13);
            this.ProgressbarTranslated.Name = "ProgressbarTranslated";
            this.ProgressbarTranslated.Size = new System.Drawing.Size(427, 23);
            this.ProgressbarTranslated.TabIndex = 8;
            this.ProgressbarTranslated.Value = 50;
            this.ProgressbarTranslated.Visible = false;
            this.ProgressbarTranslated.Click += new System.EventHandler(this.ProgressbarTranslated_Click);
            // 
            // WordsTranslated
            // 
            this.WordsTranslated.AutoSize = true;
            this.WordsTranslated.BackColor = System.Drawing.Color.Transparent;
            this.WordsTranslated.ForeColor = System.Drawing.SystemColors.Control;
            this.WordsTranslated.Location = new System.Drawing.Point(516, 17);
            this.WordsTranslated.Name = "WordsTranslated";
            this.WordsTranslated.Size = new System.Drawing.Size(78, 13);
            this.WordsTranslated.TabIndex = 7;
            this.WordsTranslated.Text = "progress words";
            // 
            // CheckListBoxLeft
            // 
            this.CheckListBoxLeft.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CheckListBoxLeft.ForeColor = System.Drawing.SystemColors.Window;
            this.CheckListBoxLeft.FormattingEnabled = true;
            this.CheckListBoxLeft.Location = new System.Drawing.Point(3, 41);
            this.CheckListBoxLeft.Name = "CheckListBoxLeft";
            this.CheckListBoxLeft.Size = new System.Drawing.Size(617, 694);
            this.CheckListBoxLeft.TabIndex = 10;
            this.CheckListBoxLeft.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckListBoxLeft_ItemCheck);
            this.CheckListBoxLeft.SelectedIndexChanged += new System.EventHandler(this.CheckListBoxLeft_SelectedIndexChanged);
            // 
            // OpenFile
            // 
            this.OpenFile.ForeColor = System.Drawing.SystemColors.WindowText;
            this.OpenFile.Location = new System.Drawing.Point(3, 12);
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(75, 23);
            this.OpenFile.TabIndex = 1;
            this.OpenFile.Text = "Select file";
            this.OpenFile.UseVisualStyleBackColor = true;
            this.OpenFile.Click += new System.EventHandler(this.SelectFileLeftClick);
            // 
            // SaveFile
            // 
            this.SaveFile.ForeColor = System.Drawing.SystemColors.WindowText;
            this.SaveFile.Location = new System.Drawing.Point(84, 12);
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(75, 23);
            this.SaveFile.TabIndex = 2;
            this.SaveFile.Text = "Save file";
            this.SaveFile.UseVisualStyleBackColor = true;
            this.SaveFile.Click += new System.EventHandler(this.SaveFileLeftClick);
            // 
            // SaveFileAs
            // 
            this.SaveFileAs.BackColor = System.Drawing.SystemColors.Control;
            this.SaveFileAs.ForeColor = System.Drawing.SystemColors.WindowText;
            this.SaveFileAs.Location = new System.Drawing.Point(165, 12);
            this.SaveFileAs.Name = "SaveFileAs";
            this.SaveFileAs.Size = new System.Drawing.Size(75, 23);
            this.SaveFileAs.TabIndex = 3;
            this.SaveFileAs.Text = "Save file as";
            this.SaveFileAs.UseVisualStyleBackColor = false;
            this.SaveFileAs.Click += new System.EventHandler(this.SaveFileAsLeftClick);
            // 
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.Location = new System.Drawing.Point(0, 0);
            this.MainContainer.Name = "MainContainer";
            // 
            // MainContainer.Panel1
            // 
            this.MainContainer.Panel1.BackColor = System.Drawing.SystemColors.Desktop;
            this.MainContainer.Panel1.Controls.Add(this.LanguageBox);
            this.MainContainer.Panel1.Controls.Add(this.CheckListBoxLeft);
            this.MainContainer.Panel1.Controls.Add(this.SaveFileAs);
            this.MainContainer.Panel1.Controls.Add(this.WordsTranslated);
            this.MainContainer.Panel1.Controls.Add(this.SaveFile);
            this.MainContainer.Panel1.Controls.Add(this.SelectedFile);
            this.MainContainer.Panel1.Controls.Add(this.OpenFile);
            this.MainContainer.Panel1.ForeColor = System.Drawing.SystemColors.Window;
            // 
            // MainContainer.Panel2
            // 
            this.MainContainer.Panel2.BackColor = System.Drawing.SystemColors.Desktop;
            this.MainContainer.Panel2.Controls.Add(this.CharacterCountLabel);
            this.MainContainer.Panel2.Controls.Add(this.SaveCommentsButton);
            this.MainContainer.Panel2.Controls.Add(this.CommentLabel);
            this.MainContainer.Panel2.Controls.Add(this.CommentTextBox);
            this.MainContainer.Panel2.Controls.Add(this.SaveCurrentString);
            this.MainContainer.Panel2.Controls.Add(this.ProgressbarTranslated);
            this.MainContainer.Panel2.Controls.Add(this.EnglishTextBox);
            this.MainContainer.Panel2.Controls.Add(this.TranslatedTextBox);
            this.MainContainer.Size = new System.Drawing.Size(1339, 741);
            this.MainContainer.SplitterDistance = 623;
            this.MainContainer.TabIndex = 0;
            // 
            // LanguageBox
            // 
            this.LanguageBox.AllowDrop = true;
            this.LanguageBox.FormattingEnabled = true;
            this.LanguageBox.Items.AddRange(new object[] {
            "cs",
            "da",
            "de",
            "es",
            "esmx",
            "fi",
            "fr",
            "hu",
            "it",
            "ja",
            "ko",
            "nl",
            "pl",
            "pt",
            "ptrb",
            "ru",
            "tr"});
            this.LanguageBox.Location = new System.Drawing.Point(247, 13);
            this.LanguageBox.Name = "LanguageBox";
            this.LanguageBox.Size = new System.Drawing.Size(60, 21);
            this.LanguageBox.TabIndex = 11;
            this.LanguageBox.SelectedIndexChanged += new System.EventHandler(this.LanguageBox_SelectedIndexChanged);
            this.LanguageBox.SelectionChangeCommitted += new System.EventHandler(this.LanguageBox_SelectedIndexChanged);
            // 
            // CharacterCountLabel
            // 
            this.CharacterCountLabel.AutoSize = true;
            this.CharacterCountLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.CharacterCountLabel.Location = new System.Drawing.Point(108, 17);
            this.CharacterCountLabel.Name = "CharacterCountLabel";
            this.CharacterCountLabel.Size = new System.Drawing.Size(143, 13);
            this.CharacterCountLabel.TabIndex = 16;
            this.CharacterCountLabel.Text = "Template: xx | Translation: xx";
            // 
            // SaveCommentsButton
            // 
            this.SaveCommentsButton.Location = new System.Drawing.Point(81, 557);
            this.SaveCommentsButton.Name = "SaveCommentsButton";
            this.SaveCommentsButton.Size = new System.Drawing.Size(93, 22);
            this.SaveCommentsButton.TabIndex = 15;
            this.SaveCommentsButton.Text = "Save comments";
            this.SaveCommentsButton.UseVisualStyleBackColor = true;
            this.SaveCommentsButton.Click += new System.EventHandler(this.SaveCommentsButton_Click);
            // 
            // CommentLabel
            // 
            this.CommentLabel.AutoSize = true;
            this.CommentLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.CommentLabel.Location = new System.Drawing.Point(6, 562);
            this.CommentLabel.Name = "CommentLabel";
            this.CommentLabel.Size = new System.Drawing.Size(69, 13);
            this.CommentLabel.TabIndex = 14;
            this.CommentLabel.Text = "COMMENTS";
            // 
            // CommentTextBox
            // 
            this.CommentTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CommentTextBox.Font = new System.Drawing.Font("Consolas", 11F);
            this.CommentTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.CommentTextBox.Location = new System.Drawing.Point(6, 580);
            this.CommentTextBox.Multiline = true;
            this.CommentTextBox.Name = "CommentTextBox";
            this.CommentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CommentTextBox.Size = new System.Drawing.Size(703, 155);
            this.CommentTextBox.TabIndex = 13;
            // 
            // SaveCurrentString
            // 
            this.SaveCurrentString.ForeColor = System.Drawing.SystemColors.MenuText;
            this.SaveCurrentString.Location = new System.Drawing.Point(6, 13);
            this.SaveCurrentString.Name = "SaveCurrentString";
            this.SaveCurrentString.Size = new System.Drawing.Size(96, 23);
            this.SaveCurrentString.TabIndex = 12;
            this.SaveCurrentString.Text = "Save this string";
            this.SaveCurrentString.UseVisualStyleBackColor = true;
            this.SaveCurrentString.Click += new System.EventHandler(this.SaveCurrentString_Click);
            // 
            // EnglishTextBox
            // 
            this.EnglishTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.EnglishTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnglishTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.EnglishTextBox.Location = new System.Drawing.Point(6, 41);
            this.EnglishTextBox.Multiline = true;
            this.EnglishTextBox.Name = "EnglishTextBox";
            this.EnglishTextBox.ReadOnly = true;
            this.EnglishTextBox.Size = new System.Drawing.Size(703, 215);
            this.EnglishTextBox.TabIndex = 9;
            this.EnglishTextBox.Text = "Lorem ipsum dolor sit amed";
            // 
            // Fenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1339, 741);
            this.Controls.Add(this.MainContainer);
            this.Name = "Fenster";
            this.ShowIcon = false;
            this.Text = "HP Translator Helper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MainContainer.Panel1.ResumeLayout(false);
            this.MainContainer.Panel1.PerformLayout();
            this.MainContainer.Panel2.ResumeLayout(false);
            this.MainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox TranslatedTextBox;
        private System.Windows.Forms.Label SelectedFile;
        private System.Windows.Forms.ProgressBar ProgressbarTranslated;
        private System.Windows.Forms.Label WordsTranslated;
        private System.Windows.Forms.CheckedListBox CheckListBoxLeft;
        private System.Windows.Forms.Button OpenFile;
        private System.Windows.Forms.Button SaveFile;
        private System.Windows.Forms.Button SaveFileAs;
        private System.Windows.Forms.SplitContainer MainContainer;
        private System.Windows.Forms.TextBox EnglishTextBox;
        private System.Windows.Forms.ComboBox LanguageBox;
        private System.Windows.Forms.Button SaveCurrentString;
        private System.Windows.Forms.Label CommentLabel;
        private System.Windows.Forms.TextBox CommentTextBox;
        private System.Windows.Forms.Button SaveCommentsButton;
        private System.Windows.Forms.Label CharacterCountLabel;
    }
}


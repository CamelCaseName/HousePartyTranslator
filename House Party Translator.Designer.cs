
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
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.SelectedFileLeft = new System.Windows.Forms.Label();
            this.SaveFileAsLeft = new System.Windows.Forms.Button();
            this.SaveFileLeft = new System.Windows.Forms.Button();
            this.OpenFileLeft = new System.Windows.Forms.Button();
            this.TextBoxLeft = new System.Windows.Forms.TextBox();
            this.WordsTranslated = new System.Windows.Forms.Label();
            this.ProgressbarTranslated = new System.Windows.Forms.ProgressBar();
            this.SelectedFileRight = new System.Windows.Forms.Label();
            this.SaveFileAsRight = new System.Windows.Forms.Button();
            this.TextBoxRight = new System.Windows.Forms.TextBox();
            this.SaveFileRight = new System.Windows.Forms.Button();
            this.OpenFileRight = new System.Windows.Forms.Button();
            this.OpenFileDialogLeft = new System.Windows.Forms.OpenFileDialog();
            this.OpenFileDialogRight = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileAsDialogLeft = new System.Windows.Forms.SaveFileDialog();
            this.SafeFileAsDialogRight = new System.Windows.Forms.SaveFileDialog();
            this.ApproveTranslationButton = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel1.SuspendLayout();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.Location = new System.Drawing.Point(0, 0);
            this.MainContainer.Name = "MainContainer";
            // 
            // MainContainer.Panel1
            // 
            this.MainContainer.Panel1.Controls.Add(this.SelectedFileLeft);
            this.MainContainer.Panel1.Controls.Add(this.SaveFileAsLeft);
            this.MainContainer.Panel1.Controls.Add(this.SaveFileLeft);
            this.MainContainer.Panel1.Controls.Add(this.OpenFileLeft);
            this.MainContainer.Panel1.Controls.Add(this.TextBoxLeft);
            // 
            // MainContainer.Panel2
            // 
            this.MainContainer.Panel2.Controls.Add(this.ApproveTranslationButton);
            this.MainContainer.Panel2.Controls.Add(this.WordsTranslated);
            this.MainContainer.Panel2.Controls.Add(this.ProgressbarTranslated);
            this.MainContainer.Panel2.Controls.Add(this.SelectedFileRight);
            this.MainContainer.Panel2.Controls.Add(this.SaveFileAsRight);
            this.MainContainer.Panel2.Controls.Add(this.TextBoxRight);
            this.MainContainer.Panel2.Controls.Add(this.SaveFileRight);
            this.MainContainer.Panel2.Controls.Add(this.OpenFileRight);
            this.MainContainer.Size = new System.Drawing.Size(1257, 624);
            this.MainContainer.SplitterDistance = 585;
            this.MainContainer.TabIndex = 0;
            // 
            // SelectedFileLeft
            // 
            this.SelectedFileLeft.AutoSize = true;
            this.SelectedFileLeft.Location = new System.Drawing.Point(3, 38);
            this.SelectedFileLeft.Name = "SelectedFileLeft";
            this.SelectedFileLeft.Size = new System.Drawing.Size(37, 13);
            this.SelectedFileLeft.TabIndex = 4;
            this.SelectedFileLeft.Text = "leftFile";
            this.SelectedFileLeft.Click += new System.EventHandler(this.label1_Click);
            // 
            // SaveFileAsLeft
            // 
            this.SaveFileAsLeft.Location = new System.Drawing.Point(165, 12);
            this.SaveFileAsLeft.Name = "SaveFileAsLeft";
            this.SaveFileAsLeft.Size = new System.Drawing.Size(75, 23);
            this.SaveFileAsLeft.TabIndex = 3;
            this.SaveFileAsLeft.Text = "Save file as";
            this.SaveFileAsLeft.UseVisualStyleBackColor = true;
            this.SaveFileAsLeft.Click += new System.EventHandler(this.SaveFileAsLeftClick);
            // 
            // SaveFileLeft
            // 
            this.SaveFileLeft.Location = new System.Drawing.Point(84, 12);
            this.SaveFileLeft.Name = "SaveFileLeft";
            this.SaveFileLeft.Size = new System.Drawing.Size(75, 23);
            this.SaveFileLeft.TabIndex = 2;
            this.SaveFileLeft.Text = "Save file";
            this.SaveFileLeft.UseVisualStyleBackColor = true;
            this.SaveFileLeft.Click += new System.EventHandler(this.SaveFileLeftClick);
            // 
            // OpenFileLeft
            // 
            this.OpenFileLeft.Location = new System.Drawing.Point(3, 12);
            this.OpenFileLeft.Name = "OpenFileLeft";
            this.OpenFileLeft.Size = new System.Drawing.Size(75, 23);
            this.OpenFileLeft.TabIndex = 1;
            this.OpenFileLeft.Text = "Select file";
            this.OpenFileLeft.UseVisualStyleBackColor = true;
            this.OpenFileLeft.Click += new System.EventHandler(this.SelectFileLeftClick);
            // 
            // TextBoxLeft
            // 
            this.TextBoxLeft.BackColor = System.Drawing.SystemColors.ControlLight;
            this.TextBoxLeft.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxLeft.ImeMode = System.Windows.Forms.ImeMode.On;
            this.TextBoxLeft.Location = new System.Drawing.Point(33, 64);
            this.TextBoxLeft.Multiline = true;
            this.TextBoxLeft.Name = "TextBoxLeft";
            this.TextBoxLeft.ReadOnly = true;
            this.TextBoxLeft.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxLeft.Size = new System.Drawing.Size(539, 334);
            this.TextBoxLeft.TabIndex = 0;
            this.TextBoxLeft.Text = "Nicht editierbar, aber scrolling und so halt";
            this.TextBoxLeft.WordWrap = false;
            this.TextBoxLeft.TextChanged += new System.EventHandler(this.TextBoxLeft_TextChanged);
            // 
            // WordsTranslated
            // 
            this.WordsTranslated.AutoSize = true;
            this.WordsTranslated.BackColor = System.Drawing.Color.Transparent;
            this.WordsTranslated.Location = new System.Drawing.Point(316, 17);
            this.WordsTranslated.Name = "WordsTranslated";
            this.WordsTranslated.Size = new System.Drawing.Size(78, 13);
            this.WordsTranslated.TabIndex = 7;
            this.WordsTranslated.Text = "progress words";
            this.WordsTranslated.Click += new System.EventHandler(this.label2_Click);
            // 
            // ProgressbarTranslated
            // 
            this.ProgressbarTranslated.Location = new System.Drawing.Point(246, 12);
            this.ProgressbarTranslated.Name = "ProgressbarTranslated";
            this.ProgressbarTranslated.Size = new System.Drawing.Size(247, 23);
            this.ProgressbarTranslated.TabIndex = 8;
            this.ProgressbarTranslated.Value = 50;
            this.ProgressbarTranslated.Click += new System.EventHandler(this.ProgressbarTranslated_Click);
            // 
            // SelectedFileRight
            // 
            this.SelectedFileRight.AutoSize = true;
            this.SelectedFileRight.Location = new System.Drawing.Point(3, 38);
            this.SelectedFileRight.Name = "SelectedFileRight";
            this.SelectedFileRight.Size = new System.Drawing.Size(43, 13);
            this.SelectedFileRight.TabIndex = 7;
            this.SelectedFileRight.Text = "rightFile";
            // 
            // SaveFileAsRight
            // 
            this.SaveFileAsRight.Location = new System.Drawing.Point(165, 12);
            this.SaveFileAsRight.Name = "SaveFileAsRight";
            this.SaveFileAsRight.Size = new System.Drawing.Size(75, 23);
            this.SaveFileAsRight.TabIndex = 6;
            this.SaveFileAsRight.Text = "Save file as";
            this.SaveFileAsRight.UseVisualStyleBackColor = true;
            this.SaveFileAsRight.Click += new System.EventHandler(this.SaveFileAsRightClick);
            // 
            // TextBoxRight
            // 
            this.TextBoxRight.AcceptsReturn = true;
            this.TextBoxRight.AllowDrop = true;
            this.TextBoxRight.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TextBoxRight.BackColor = System.Drawing.SystemColors.ControlLight;
            this.TextBoxRight.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxRight.ImeMode = System.Windows.Forms.ImeMode.On;
            this.TextBoxRight.Location = new System.Drawing.Point(35, 64);
            this.TextBoxRight.Multiline = true;
            this.TextBoxRight.Name = "TextBoxRight";
            this.TextBoxRight.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxRight.Size = new System.Drawing.Size(621, 334);
            this.TextBoxRight.TabIndex = 0;
            this.TextBoxRight.Text = "editierbar und scroll mit links synchronisiert";
            this.TextBoxRight.WordWrap = false;
            this.TextBoxRight.Click += new System.EventHandler(this.TextBoxRight_Click);
            this.TextBoxRight.TextChanged += new System.EventHandler(this.TextBoxRight_TextChanged);
            // 
            // SaveFileRight
            // 
            this.SaveFileRight.Location = new System.Drawing.Point(84, 12);
            this.SaveFileRight.Name = "SaveFileRight";
            this.SaveFileRight.Size = new System.Drawing.Size(75, 23);
            this.SaveFileRight.TabIndex = 5;
            this.SaveFileRight.Text = "Save file";
            this.SaveFileRight.UseVisualStyleBackColor = true;
            this.SaveFileRight.Click += new System.EventHandler(this.SaveFileRightClick);
            // 
            // OpenFileRight
            // 
            this.OpenFileRight.Location = new System.Drawing.Point(3, 12);
            this.OpenFileRight.Name = "OpenFileRight";
            this.OpenFileRight.Size = new System.Drawing.Size(75, 23);
            this.OpenFileRight.TabIndex = 4;
            this.OpenFileRight.Text = "Select file";
            this.OpenFileRight.UseVisualStyleBackColor = true;
            this.OpenFileRight.Click += new System.EventHandler(this.SelectFileRightClick);
            // 
            // OpenFileDialogLeft
            // 
            this.OpenFileDialogLeft.FileName = "openFileDialog1";
            this.OpenFileDialogLeft.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialogLeft_FileOk);
            // 
            // OpenFileDialogRight
            // 
            this.OpenFileDialogRight.FileName = "openFileDialog2";
            this.OpenFileDialogRight.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialogRight_FileOk);
            // 
            // SaveFileAsDialogLeft
            // 
            this.SaveFileAsDialogLeft.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileAsDialogLeft_FileOk);
            // 
            // SafeFileAsDialogRight
            // 
            this.SafeFileAsDialogRight.FileOk += new System.ComponentModel.CancelEventHandler(this.SafeFileAsDialogRight_FileOk);
            // 
            // ApproveTranslationButton
            // 
            this.ApproveTranslationButton.AutoSize = true;
            this.ApproveTranslationButton.Location = new System.Drawing.Point(461, 41);
            this.ApproveTranslationButton.Name = "ApproveTranslationButton";
            this.ApproveTranslationButton.Size = new System.Drawing.Size(128, 17);
            this.ApproveTranslationButton.TabIndex = 9;
            this.ApproveTranslationButton.Text = "Approve selected line";
            this.ApproveTranslationButton.UseVisualStyleBackColor = true;
            this.ApproveTranslationButton.CheckedChanged += new System.EventHandler(this.ApproveTranslationButton_CheckedChanged);
            // 
            // Fenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1257, 624);
            this.Controls.Add(this.MainContainer);
            this.Name = "Fenster";
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

        private System.Windows.Forms.SplitContainer MainContainer;
        private System.Windows.Forms.TextBox TextBoxLeft;
        private System.Windows.Forms.TextBox TextBoxRight;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogLeft;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogRight;
        private System.Windows.Forms.SaveFileDialog SaveFileAsDialogLeft;
        private System.Windows.Forms.SaveFileDialog SafeFileAsDialogRight;
        private System.Windows.Forms.Button OpenFileLeft;
        private System.Windows.Forms.Button SaveFileLeft;
        private System.Windows.Forms.Button SaveFileAsLeft;
        private System.Windows.Forms.Button SaveFileAsRight;
        private System.Windows.Forms.Button SaveFileRight;
        private System.Windows.Forms.Button OpenFileRight;
        private System.Windows.Forms.Label SelectedFileLeft;
        private System.Windows.Forms.Label SelectedFileRight;
        private System.Windows.Forms.ProgressBar ProgressbarTranslated;
        private System.Windows.Forms.Label WordsTranslated;
        private System.Windows.Forms.CheckBox ApproveTranslationButton;
    }
}


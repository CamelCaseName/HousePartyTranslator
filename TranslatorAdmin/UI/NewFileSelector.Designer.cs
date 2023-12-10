using Google.Protobuf.Reflection;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Translator.Desktop.UI
{
    partial class NewFileSelector
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
            if (disposing && (components is not null))
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
            storyDropdown = new ComboBox();
            fileDropdown = new ComboBox();
            cancel = new Button();
            submit = new Button();
            storyLabel = new Label();
            fileLabel = new Label();
            SuspendLayout();
            // 
            // storyDropdown
            // 
            storyDropdown.FormattingEnabled = true;
            storyDropdown.Location = new Point(12, 29);
            storyDropdown.Name = "storyDropdown";
            storyDropdown.Size = new Size(369, 23);
            storyDropdown.TabIndex = 0;
            storyDropdown.SelectedIndexChanged += StoryDropdown_SelectedIndexChanged;
            // 
            // fileDropdown
            // 
            fileDropdown.FormattingEnabled = true;
            fileDropdown.Location = new Point(12, 78);
            fileDropdown.Name = "fileDropdown";
            fileDropdown.Size = new Size(369, 23);
            fileDropdown.TabIndex = 1;
            fileDropdown.SelectedIndexChanged += FileDropdown_SelectedIndexChanged;
            // 
            // cancel
            // 
            cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancel.Location = new Point(307, 116);
            cancel.Name = "cancel";
            cancel.Size = new Size(75, 23);
            cancel.TabIndex = 2;
            cancel.Text = "Cancel";
            cancel.UseVisualStyleBackColor = true;
            cancel.Click += Cancel_Click;
            // 
            // submit
            // 
            submit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            submit.Enabled = false;
            submit.Location = new Point(12, 116);
            submit.Name = "submit";
            submit.Size = new Size(75, 23);
            submit.TabIndex = 3;
            submit.Text = "Submit";
            submit.UseVisualStyleBackColor = true;
            submit.Click += Submit_click;
            // 
            // storyLabel
            // 
            storyLabel.AutoSize = true;
            storyLabel.Location = new Point(15, 11);
            storyLabel.Name = "storyLabel";
            storyLabel.Size = new Size(37, 15);
            storyLabel.TabIndex = 4;
            storyLabel.Text = "Story:";
            // 
            // fileLabel
            // 
            fileLabel.AutoSize = true;
            fileLabel.Location = new Point(12, 60);
            fileLabel.Name = "fileLabel";
            fileLabel.Size = new Size(28, 15);
            fileLabel.TabIndex = 5;
            fileLabel.Text = "File:";
            // 
            // NewFileSelector
            // 
            AcceptButton = submit;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancel;
            ClientSize = new Size(394, 151);
            Controls.Add(fileLabel);
            Controls.Add(storyLabel);
            Controls.Add(submit);
            Controls.Add(cancel);
            Controls.Add(fileDropdown);
            Controls.Add(storyDropdown);
            MaximizeBox = false;
            MaximumSize = new Size(410, 190);
            MinimumSize = new Size(410, 190);
            Name = "NewFileSelector";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create new file";
            Load += NewFileSelector_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox storyDropdown;
        private ComboBox fileDropdown;
        private Button cancel;
        private Button submit;
        private Label storyLabel;
        private Label fileLabel;
    }
}
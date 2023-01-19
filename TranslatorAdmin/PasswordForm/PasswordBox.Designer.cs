
namespace Translator
{
    partial class Password
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
			this.PasswordTextBox = new System.Windows.Forms.TextBox();
			this.MessageLabel = new System.Windows.Forms.Label();
			this.SubmitButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// PasswordTextBox
			// 
			this.PasswordTextBox.Location = new System.Drawing.Point(14, 73);
			this.PasswordTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.Size = new System.Drawing.Size(461, 23);
			this.PasswordTextBox.TabIndex = 0;
			// 
			// MessageLabel
			// 
			this.MessageLabel.AutoSize = true;
			this.MessageLabel.Location = new System.Drawing.Point(15, 15);
			this.MessageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.MessageLabel.Name = "MessageLabel";
			this.MessageLabel.Size = new System.Drawing.Size(234, 15);
			this.MessageLabel.TabIndex = 1;
			this.MessageLabel.Text = "Please enter the password for the database!";
			// 
			// SubmitButton
			// 
			this.SubmitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SubmitButton.Location = new System.Drawing.Point(192, 133);
			this.SubmitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.SubmitButton.Name = "SubmitButton";
			this.SubmitButton.Size = new System.Drawing.Size(88, 27);
			this.SubmitButton.TabIndex = 2;
			this.SubmitButton.Text = "Submit";
			this.SubmitButton.UseVisualStyleBackColor = true;
			this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
			// 
			// Password
			// 
			this.AcceptButton = this.SubmitButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 173);
			this.Controls.Add(this.SubmitButton);
			this.Controls.Add(this.MessageLabel);
			this.Controls.Add(this.PasswordTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "Password";
			this.ShowIcon = false;
			this.Text = "Enter Password";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Button SubmitButton;
    }
}

namespace Translator.Desktop.UI
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
            PasswordTextBox = new TextBox();
            MessageLabel = new Label();
            SubmitButton = new Button();
            SuspendLayout();
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Location = new Point(14, 73);
            PasswordTextBox.Margin = new Padding(4, 3, 4, 3);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.PasswordChar = '*';
            PasswordTextBox.Size = new Size(461, 23);
            PasswordTextBox.TabIndex = 0;
            PasswordTextBox.UseSystemPasswordChar = true;
            // 
            // MessageLabel
            // 
            MessageLabel.AutoSize = true;
            MessageLabel.Location = new Point(15, 15);
            MessageLabel.Margin = new Padding(4, 0, 4, 0);
            MessageLabel.Name = "MessageLabel";
            MessageLabel.Size = new Size(234, 15);
            MessageLabel.TabIndex = 1;
            MessageLabel.Text = "Please enter the password for the database!";
            // 
            // SubmitButton
            // 
            SubmitButton.DialogResult = DialogResult.OK;
            SubmitButton.Location = new Point(192, 133);
            SubmitButton.Margin = new Padding(4, 3, 4, 3);
            SubmitButton.Name = "SubmitButton";
            SubmitButton.Size = new Size(88, 27);
            SubmitButton.TabIndex = 2;
            SubmitButton.Text = "Submit";
            SubmitButton.UseVisualStyleBackColor = true;
            SubmitButton.Click += SubmitButton_Click;
            // 
            // Password
            // 
            AcceptButton = SubmitButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(490, 173);
            Controls.Add(SubmitButton);
            Controls.Add(MessageLabel);
            Controls.Add(PasswordTextBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            Name = "Password";
            ShowIcon = false;
            Text = "Enter Password";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Button SubmitButton;
    }
}
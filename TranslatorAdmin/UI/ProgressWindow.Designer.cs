using Translator.Desktop.UI.Components;

namespace Translator.Desktop.UI
{
    partial class ProgressWindow
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
            label1 = new System.Windows.Forms.Label();
            noAnimationBar1 = new NoAnimationBar();
            SuspendLayout();
            // 
            // label1
            // 
            label1.BackColor = System.Drawing.SystemColors.WindowFrame;
            label1.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.SystemColors.HighlightText;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(448, 52);
            label1.TabIndex = 1;
            label1.Text = "progress";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noAnimationBar1
            // 
            noAnimationBar1.BackColor = System.Drawing.SystemColors.WindowFrame;
            noAnimationBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            noAnimationBar1.ForeColor = System.Drawing.SystemColors.ControlLight;
            noAnimationBar1.Location = new System.Drawing.Point(0, 0);
            noAnimationBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            noAnimationBar1.Name = "noAnimationBar1";
            noAnimationBar1.SecondValue = 0;
            noAnimationBar1.Size = new System.Drawing.Size(448, 128);
            noAnimationBar1.TabIndex = 0;
            // 
            // ProgressWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlDarkDark;
            ClientSize = new System.Drawing.Size(448, 128);
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(noAnimationBar1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximumSize = new System.Drawing.Size(464, 167);
            MinimumSize = new System.Drawing.Size(464, 167);
            Name = "ProgressWindow";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Autosave";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private NoAnimationBar noAnimationBar1;
        private System.Windows.Forms.Label label1;
    }
}
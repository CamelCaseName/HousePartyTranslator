namespace HousePartyTranslator.ProgressbarForm
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
            this.noAnimationBar1 = new HousePartyTranslator.Helpers.NoAnimationBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // noAnimationBar1
            // 
            this.noAnimationBar1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.noAnimationBar1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.noAnimationBar1.Location = new System.Drawing.Point(-1, -1);
            this.noAnimationBar1.Name = "noAnimationBar1";
            this.noAnimationBar1.Size = new System.Drawing.Size(393, 105);
            this.noAnimationBar1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label1.Location = new System.Drawing.Point(128, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 34);
            this.label1.TabIndex = 1;
            this.label1.Text = "progress";
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(392, 102);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.noAnimationBar1);
            this.Name = "ProgressWindow";
            this.Text = "Autosave";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Helpers.NoAnimationBar noAnimationBar1;
        private System.Windows.Forms.Label label1;
    }
}
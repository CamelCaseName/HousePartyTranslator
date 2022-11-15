namespace Translator.ProgressbarForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.noAnimationBar1 = new Translator.Helpers.NoAnimationBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 111);
            this.label1.TabIndex = 1;
            this.label1.Text = "progress";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noAnimationBar1
            // 
            this.noAnimationBar1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.noAnimationBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noAnimationBar1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.noAnimationBar1.Location = new System.Drawing.Point(0, 0);
            this.noAnimationBar1.Name = "noAnimationBar1";
            this.noAnimationBar1.Size = new System.Drawing.Size(384, 111);
            this.noAnimationBar1.TabIndex = 0;
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(384, 111);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.noAnimationBar1);
            this.MaximumSize = new System.Drawing.Size(400, 150);
            this.MinimumSize = new System.Drawing.Size(400, 150);
            this.Name = "ProgressWindow";
            this.ShowIcon = false;
            this.Text = "Autosave";
            this.ResumeLayout(false);

        }

        #endregion

        private Helpers.NoAnimationBar noAnimationBar1;
        private System.Windows.Forms.Label label1;
    }
}
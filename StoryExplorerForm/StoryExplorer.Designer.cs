
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    partial class StoryExplorer
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
            this.SuspendLayout();
            // 
            // StoryExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "StoryExplorer";
            this.ShowIcon = false;
            this.Text = "Story Explorer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyBoard);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
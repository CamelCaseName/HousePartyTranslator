namespace Translator.Explorer.Window
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
			this.NodeInfoLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// NodeInfoLabel
			// 
			this.NodeInfoLabel.AutoSize = true;
			this.NodeInfoLabel.BackColor = System.Drawing.Color.Transparent;
			this.NodeInfoLabel.Dock = System.Windows.Forms.DockStyle.Right;
			this.NodeInfoLabel.Anchor = (System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top);
			this.NodeInfoLabel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.NodeInfoLabel.ForeColor = System.Drawing.SystemColors.MenuBar;
			this.NodeInfoLabel.Location = new System.Drawing.Point(1614, 0);
			this.NodeInfoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.NodeInfoLabel.Name = "NodeInfoLabel";
			this.NodeInfoLabel.Size = new System.Drawing.Size(0, 18);
			this.NodeInfoLabel.TabIndex = 0;
			// 
			// StoryExplorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Desktop;
			this.ClientSize = new System.Drawing.Size(1614, 800);
			this.Controls.Add(this.NodeInfoLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "StoryExplorer";
			this.ShowIcon = false;
			this.Text = "Story Explorer";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyBoard);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleKeyBoard);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private Label NodeInfoLabel;
    }
}
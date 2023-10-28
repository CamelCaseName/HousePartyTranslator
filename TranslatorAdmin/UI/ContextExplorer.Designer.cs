﻿namespace Translator.Desktop.UI.Components
{
    partial class ContextExplorer
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
            Lines = new System.Windows.Forms.TreeView();
            SuspendLayout();
            // 
            // Lines
            // 
            Lines.Dock = System.Windows.Forms.DockStyle.Fill;
            Lines.Location = new System.Drawing.Point(0, 0);
            Lines.Name = "Lines";
            Lines.Size = new System.Drawing.Size(800, 450);
            Lines.TabIndex = 0;
            // 
            // ContextExplorer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(Lines);
            Name = "ContextExplorer";
            ShowIcon = false;
            Text = "ContextExplorer";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TreeView Lines;
    }
}
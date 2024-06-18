using System.Drawing;
using System;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.Foundation;

namespace Translator.Desktop.UI.Components
{
    partial class LineContext
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
            ContextView = new TreeView();
            winTabController1 = new WinTabController<TabPage>();
            CommentsTab = new TabPage();
            ContextTab = new TabPage();
            CommentTextBox = new WinTextBox();
            winTabController1.SuspendLayout();
            CommentsTab.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            ContextView.Dock = DockStyle.Fill;
            ContextView.LineColor = System.Drawing.Color.Empty;
            ContextView.Location = new System.Drawing.Point(3, 3);
            ContextView.Name = "treeView1";
            ContextView.Size = new System.Drawing.Size(786, 416);
            ContextView.TabIndex = 0;
            // 
            // winTabController1
            // 
            winTabController1.Controls.Add(CommentsTab);
            winTabController1.Controls.Add(ContextTab);
            winTabController1.Dock = DockStyle.Fill;
            winTabController1.DrawMode = TabDrawMode.OwnerDrawFixed;
            winTabController1.Location = new System.Drawing.Point(0, 0);
            winTabController1.Margin = new Padding(0);
            winTabController1.Name = "winTabController1";
            winTabController1.Padding = new System.Drawing.Point(0, 0);
            winTabController1.SelectedIndex = 0;
            winTabController1.Size = new System.Drawing.Size(800, 450);
            winTabController1.TabIndex = 1;
            // 
            // tabPage1
            // 
            CommentsTab.Location = new System.Drawing.Point(4, 24);
            CommentsTab.Name = "CommentsTab";
            CommentsTab.Padding = new Padding(3);
            CommentsTab.Size = new System.Drawing.Size(792, 422);
            CommentsTab.TabIndex = 0;
            CommentsTab.Text = "Comments";
            CommentsTab.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            ContextTab.Controls.Add(ContextView);
            ContextTab.Location = new System.Drawing.Point(4, 24);
            ContextTab.Name = "ContextTab";
            ContextTab.Padding = new Padding(3);
            ContextTab.Size = new System.Drawing.Size(792, 422);
            ContextTab.TabIndex = 1;
            ContextTab.Text = "Context";
            ContextTab.UseVisualStyleBackColor = true;
            // 
            // CommentTextBox
            // 
            CommentTextBox.BackColor = Utils.background;
            CommentTextBox.Dock = DockStyle.Fill;
            CommentTextBox.Font = new Font("Consolas", 11F);
            CommentTextBox.ForeColor = Utils.brightText;
            CommentTextBox.Location = new Point(3, 16);
            CommentTextBox.Multiline = true;
            CommentTextBox.Name = "CommentTextBox";
            CommentTextBox.Size = new Size(672, 105);
            CommentTextBox.TabIndex = 13;
            //CommentTextBox.TextChanged += new EventHandler(MainForm.Comments_TextChanged);
            //CommentTextBox.MouseUp += new MouseEventHandler(MainForm.TextContextOpened);
            //CommentTextBox.MouseEnter += new EventHandler(MainForm.TextContextOpened);
            CommentTextBox.PlaceholderText = "No comments yet";
            CommentTextBox.PlaceholderColor = Utils.darkText;
            CommentTextBox.ScrollBars = ScrollBars.Vertical;
            // 
            // LineContext
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(winTabController1);
            Name = "LineContext";
            Text = "Line Context";
            winTabController1.ResumeLayout(false);
            CommentsTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TreeView ContextView;
        private WinTabController<TabPage> winTabController1;
        private System.Windows.Forms.TabPage CommentsTab;
        private System.Windows.Forms.TabPage ContextTab;
        public WinTextBox CommentTextBox;
    }
}
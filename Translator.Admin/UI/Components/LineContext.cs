using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Helpers;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    public class LineContext : WinTabController<TabPage>
    {
        private readonly TreeView ContextView = new();
        private readonly TabPage CommentsTab = new();
        private readonly TabPage ContextTab = new();
        public readonly WinTextBox CommentTextBox = new();

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public LineContext()
        {
            CommentsTab.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            ContextView.Dock = DockStyle.Fill;
            ContextView.LineColor = Color.Empty;
            ContextView.Location = new Point(3, 3);
            ContextView.Name = "Context";
            ContextView.Size = new Size(786, 416);
            ContextView.TabIndex = 0;
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
            CommentTextBox.PlaceholderText = "No comments yet";
            CommentTextBox.PlaceholderColor = Utils.darkText;
            CommentTextBox.ScrollBars = ScrollBars.Vertical;
            // 
            // tabPage1
            // 
            CommentsTab.Controls.Add(CommentTextBox);
            CommentsTab.Location = new Point(4, 24);
            CommentsTab.Name = "CommentsTab";
            CommentsTab.Padding = new Padding(3);
            CommentsTab.Size = new Size(792, 422);
            CommentsTab.TabIndex = 0;
            CommentsTab.Text = "Comments";
            CommentsTab.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            ContextTab.Controls.Add(ContextView);
            ContextTab.Location = new Point(4, 24);
            ContextTab.Name = "ContextTab";
            ContextTab.Padding = new Padding(3);
            ContextTab.Size = new Size(792, 422);
            ContextTab.TabIndex = 1;
            ContextTab.Text = "Context";
            ContextTab.UseVisualStyleBackColor = true;

            Controls.Add(CommentsTab);
            Controls.Add(ContextTab);
            Dock = DockStyle.Fill;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            Location = new Point(0, 0);
            Margin = new Padding(0);
            Name = "winTabController1";
            Padding = new Point(0, 0);
            SelectedIndex = 0;
            Size = new Size(800, 450);
            TabIndex = 1;
            // 
            // LineContext
            // 
            ClientSize = new Size(800, 450);
            Name = "LineContext";
            Text = "Line Context";
            CommentsTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
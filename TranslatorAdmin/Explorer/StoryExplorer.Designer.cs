using System.Windows.Forms;

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
            this.ColoringDepth = new System.Windows.Forms.NumericUpDown();
            this.IdealLength = new System.Windows.Forms.NumericUpDown();
            this.Start = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.ColorDepth = new System.Windows.Forms.Label();
            this.EdgeLength = new System.Windows.Forms.Label();
            this.NodeCalculations = new System.Windows.Forms.Label();
            this.MenuShowButton = new System.Windows.Forms.Button();
            this.SettingsBox = new System.Windows.Forms.GroupBox();
            this.NodeFilterLabel = new System.Windows.Forms.Label();
            this.NodeTypeFilterScrollPanel = new System.Windows.Forms.TableLayoutPanel();
            this.NodeTypeButtonsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.MoveDownButton = new System.Windows.Forms.Button();
            this.MoveUpButton = new System.Windows.Forms.Button();
            this.ShowExtendedInfo = new System.Windows.Forms.CheckBox();
            this.TextNodesOnly = new System.Windows.Forms.CheckBox();
            this.NodeSizeField = new System.Windows.Forms.NumericUpDown();
            this.NodeSize = new System.Windows.Forms.Label();
            this.SaveStory = new System.Windows.Forms.Button();
            this.CenterButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ColoringDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IdealLength)).BeginInit();
            this.SettingsBox.SuspendLayout();
            this.NodeTypeFilterScrollPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NodeSizeField)).BeginInit();
            this.SuspendLayout();
            // 
            // NodeInfoLabel
            // 
            this.NodeInfoLabel.AutoSize = true;
            this.NodeInfoLabel.BackColor = System.Drawing.SystemColors.Desktop;
            this.NodeInfoLabel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NodeInfoLabel.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.NodeInfoLabel.Location = new System.Drawing.Point(0, 0);
            this.NodeInfoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NodeInfoLabel.Name = "NodeInfoLabel";
            this.NodeInfoLabel.Size = new System.Drawing.Size(0, 18);
            this.NodeInfoLabel.TabIndex = 0;
            this.NodeInfoLabel.Visible = false;
            // 
            // ColoringDepth
            // 
            this.ColoringDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ColoringDepth.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ColoringDepth.Location = new System.Drawing.Point(105, 12);
            this.ColoringDepth.Name = "ColoringDepth";
            this.ColoringDepth.Size = new System.Drawing.Size(75, 23);
            this.ColoringDepth.TabIndex = 1;
            this.ColoringDepth.ValueChanged += new System.EventHandler(this.ColoringDepth_ValueChanged);
            // 
            // IdealLength
            // 
            this.IdealLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IdealLength.Location = new System.Drawing.Point(105, 41);
            this.IdealLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.IdealLength.Name = "IdealLength";
            this.IdealLength.Size = new System.Drawing.Size(75, 23);
            this.IdealLength.TabIndex = 2;
            this.IdealLength.ValueChanged += new System.EventHandler(this.IdealLength_ValueChanged);
            // 
            // Start
            // 
            this.Start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Start.Location = new System.Drawing.Point(794, 529);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 3;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Stop
            // 
            this.Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Stop.Location = new System.Drawing.Point(883, 529);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(75, 23);
            this.Stop.TabIndex = 4;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // ColorDepth
            // 
            this.ColorDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ColorDepth.AutoSize = true;
            this.ColorDepth.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.ColorDepth.Location = new System.Drawing.Point(12, 14);
            this.ColorDepth.Name = "ColorDepth";
            this.ColorDepth.Size = new System.Drawing.Size(71, 15);
            this.ColorDepth.TabIndex = 5;
            this.ColorDepth.Text = "Color Depth";
            // 
            // EdgeLength
            // 
            this.EdgeLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EdgeLength.AutoSize = true;
            this.EdgeLength.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.EdgeLength.Location = new System.Drawing.Point(12, 43);
            this.EdgeLength.Name = "EdgeLength";
            this.EdgeLength.Size = new System.Drawing.Size(73, 15);
            this.EdgeLength.TabIndex = 6;
            this.EdgeLength.Text = "Edge Length";
            // 
            // NodeCalculations
            // 
            this.NodeCalculations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NodeCalculations.AutoSize = true;
            this.NodeCalculations.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.NodeCalculations.Location = new System.Drawing.Point(816, 508);
            this.NodeCalculations.Name = "NodeCalculations";
            this.NodeCalculations.Size = new System.Drawing.Size(0, 15);
            this.NodeCalculations.TabIndex = 7;
            // 
            // MenuShowButton
            // 
            this.MenuShowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MenuShowButton.Location = new System.Drawing.Point(883, 12);
            this.MenuShowButton.Name = "MenuShowButton";
            this.MenuShowButton.Size = new System.Drawing.Size(89, 23);
            this.MenuShowButton.TabIndex = 8;
            this.MenuShowButton.Text = "Show Settings";
            this.MenuShowButton.UseVisualStyleBackColor = true;
            this.MenuShowButton.Click += new System.EventHandler(this.MenuShowButton_Click);
            // 
            // SettingsBox
            // 
            this.SettingsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsBox.Controls.Add(this.NodeFilterLabel);
            this.SettingsBox.Controls.Add(this.NodeTypeFilterScrollPanel);
            this.SettingsBox.Controls.Add(this.MoveDownButton);
            this.SettingsBox.Controls.Add(this.MoveUpButton);
            this.SettingsBox.Controls.Add(this.ShowExtendedInfo);
            this.SettingsBox.Controls.Add(this.TextNodesOnly);
            this.SettingsBox.Controls.Add(this.NodeSizeField);
            this.SettingsBox.Controls.Add(this.NodeSize);
            this.SettingsBox.Controls.Add(this.IdealLength);
            this.SettingsBox.Controls.Add(this.ColoringDepth);
            this.SettingsBox.Controls.Add(this.ColorDepth);
            this.SettingsBox.Controls.Add(this.EdgeLength);
            this.SettingsBox.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.SettingsBox.Location = new System.Drawing.Point(778, 41);
            this.SettingsBox.Name = "SettingsBox";
            this.SettingsBox.Size = new System.Drawing.Size(194, 482);
            this.SettingsBox.TabIndex = 10;
            this.SettingsBox.TabStop = false;
            this.SettingsBox.Text = "Settings";
            this.SettingsBox.Visible = false;
            // 
            // NodeFilterLabel
            // 
            this.NodeFilterLabel.AutoSize = true;
            this.NodeFilterLabel.Location = new System.Drawing.Point(6, 185);
            this.NodeFilterLabel.Name = "NodeFilterLabel";
            this.NodeFilterLabel.Size = new System.Drawing.Size(95, 15);
            this.NodeFilterLabel.TabIndex = 14;
            this.NodeFilterLabel.Text = "Node Type Filter:";
            // 
            // NodeTypeFilterScrollPanel
            // 
            this.NodeTypeFilterScrollPanel.AutoScroll = true;
            this.NodeTypeFilterScrollPanel.AutoSize = true;
            this.NodeTypeFilterScrollPanel.BackColor = System.Drawing.Color.Transparent;
            this.NodeTypeFilterScrollPanel.ColumnCount = 1;
            this.NodeTypeFilterScrollPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.NodeTypeFilterScrollPanel.Controls.Add(this.NodeTypeButtonsLayout, 0, 0);
            this.NodeTypeFilterScrollPanel.Location = new System.Drawing.Point(3, 200);
            this.NodeTypeFilterScrollPanel.Margin = new System.Windows.Forms.Padding(0);
            this.NodeTypeFilterScrollPanel.Name = "NodeTypeFilterScrollPanel";
            this.NodeTypeFilterScrollPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.NodeTypeFilterScrollPanel.Size = new System.Drawing.Size(188, 276);
            this.NodeTypeFilterScrollPanel.TabIndex = 13;
            // 
            // NodeTypeButtonsLayout
            // 
            this.NodeTypeButtonsLayout.AutoSize = true;
            this.NodeTypeButtonsLayout.BackColor = System.Drawing.Color.Transparent;
            this.NodeTypeButtonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NodeTypeButtonsLayout.Location = new System.Drawing.Point(0, 0);
            this.NodeTypeButtonsLayout.Margin = new System.Windows.Forms.Padding(0);
            this.NodeTypeButtonsLayout.MaximumSize = new System.Drawing.Size(188, 0);
            this.NodeTypeButtonsLayout.Name = "NodeTypeButtonsLayout";
            this.NodeTypeButtonsLayout.Size = new System.Drawing.Size(188, 276);
            this.NodeTypeButtonsLayout.TabIndex = 12;
            // 
            // MoveDownButton
            // 
            this.MoveDownButton.ForeColor = System.Drawing.SystemColors.MenuText;
            this.MoveDownButton.Location = new System.Drawing.Point(105, 153);
            this.MoveDownButton.Name = "MoveDownButton";
            this.MoveDownButton.Size = new System.Drawing.Size(75, 23);
            this.MoveDownButton.TabIndex = 12;
            this.MoveDownButton.Text = "Child node";
            this.MoveDownButton.UseVisualStyleBackColor = true;
            this.MoveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
            // 
            // MoveUpButton
            // 
            this.MoveUpButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.MoveUpButton.ForeColor = System.Drawing.SystemColors.MenuText;
            this.MoveUpButton.Location = new System.Drawing.Point(12, 153);
            this.MoveUpButton.Name = "MoveUpButton";
            this.MoveUpButton.Size = new System.Drawing.Size(79, 23);
            this.MoveUpButton.TabIndex = 11;
            this.MoveUpButton.Text = "Parent node";
            this.MoveUpButton.UseVisualStyleBackColor = false;
            this.MoveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
            // 
            // ShowExtendedInfo
            // 
            this.ShowExtendedInfo.AutoSize = true;
            this.ShowExtendedInfo.Location = new System.Drawing.Point(12, 128);
            this.ShowExtendedInfo.Name = "ShowExtendedInfo";
            this.ShowExtendedInfo.Size = new System.Drawing.Size(131, 19);
            this.ShowExtendedInfo.TabIndex = 10;
            this.ShowExtendedInfo.Text = "Show extended info";
            this.ShowExtendedInfo.UseVisualStyleBackColor = true;
            this.ShowExtendedInfo.CheckedChanged += new System.EventHandler(this.ShowExtendedInfo_CheckedChanged);
            // 
            // TextNodesOnly
            // 
            this.TextNodesOnly.AutoSize = true;
            this.TextNodesOnly.Location = new System.Drawing.Point(12, 103);
            this.TextNodesOnly.Name = "TextNodesOnly";
            this.TextNodesOnly.Size = new System.Drawing.Size(108, 19);
            this.TextNodesOnly.TabIndex = 9;
            this.TextNodesOnly.Text = "Text nodes only";
            this.TextNodesOnly.UseVisualStyleBackColor = true;
            this.TextNodesOnly.CheckedChanged += new System.EventHandler(this.TextNodesOnly_CheckedChanged);
            // 
            // NodeSizeField
            // 
            this.NodeSizeField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NodeSizeField.Location = new System.Drawing.Point(105, 70);
            this.NodeSizeField.Name = "NodeSizeField";
            this.NodeSizeField.Size = new System.Drawing.Size(75, 23);
            this.NodeSizeField.TabIndex = 7;
            this.NodeSizeField.ValueChanged += new System.EventHandler(this.NodeSizeField_ValueChanged);
            // 
            // NodeSize
            // 
            this.NodeSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NodeSize.AutoSize = true;
            this.NodeSize.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.NodeSize.Location = new System.Drawing.Point(12, 72);
            this.NodeSize.Name = "NodeSize";
            this.NodeSize.Size = new System.Drawing.Size(59, 15);
            this.NodeSize.TabIndex = 8;
            this.NodeSize.Text = "Node Count";
            // 
            // SaveStory
            // 
            this.SaveStory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveStory.Location = new System.Drawing.Point(802, 12);
            this.SaveStory.Name = "SaveStory";
            this.SaveStory.Size = new System.Drawing.Size(75, 23);
            this.SaveStory.TabIndex = 11;
            this.SaveStory.Text = "Save Story";
            this.SaveStory.UseVisualStyleBackColor = true;
            // 
            // CenterButton
            // 
            this.CenterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CenterButton.Location = new System.Drawing.Point(721, 12);
            this.CenterButton.Name = "CenterButton";
            this.CenterButton.Size = new System.Drawing.Size(75, 23);
            this.CenterButton.TabIndex = 12;
            this.CenterButton.Text = "Center";
            this.CenterButton.UseVisualStyleBackColor = true;
            this.CenterButton.Click += new System.EventHandler(this.CenterButton_Click);
            // 
            // StoryExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.CenterButton);
            this.Controls.Add(this.SaveStory);
            this.Controls.Add(this.SettingsBox);
            this.Controls.Add(this.MenuShowButton);
            this.Controls.Add(this.NodeCalculations);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.NodeInfoLabel);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "StoryExplorer";
            this.ShowIcon = false;
            this.Text = "Story Explorer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyBoard);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleKeyBoard);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.HandleMouseEvents);
            ((System.ComponentModel.ISupportInitialize)(this.ColoringDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IdealLength)).EndInit();
            this.SettingsBox.ResumeLayout(false);
            this.SettingsBox.PerformLayout();
            this.NodeTypeFilterScrollPanel.ResumeLayout(false);
            this.NodeTypeFilterScrollPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NodeSizeField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Label NodeInfoLabel;
		private NumericUpDown ColoringDepth;
		private NumericUpDown IdealLength;
		private Button Start;
		private Button Stop;
		private Label ColorDepth;
		private Label EdgeLength;
		private Label NodeCalculations;
		private Button MenuShowButton;
		private GroupBox SettingsBox;
		private NumericUpDown NodeSizeField;
		private Label NodeSize;
        private CheckBox ShowExtendedInfo;
        private CheckBox TextNodesOnly;
        private Button MoveDownButton;
        private Button MoveUpButton;
        private Button SaveStory;
        private FlowLayoutPanel NodeTypeButtonsLayout;
        private TableLayoutPanel NodeTypeFilterScrollPanel;
        private Label NodeFilterLabel;
        private Button CenterButton;
    }
}
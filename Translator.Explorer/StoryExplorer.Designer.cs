﻿using System.Windows.Forms;

namespace Translator.Explorer.Window
{
    public partial class StoryExplorer
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
            if (disposing && (components is not null))
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
            NodeInfoLabel = new Label();
            ColoringDepth = new NumericUpDown();
            IdealLength = new NumericUpDown();
            Start = new Button();
            Stop = new Button();
            ColorDepth = new Label();
            EdgeLength = new Label();
            NodeCalculations = new Label();
            MenuShowButton = new Button();
            MenuBox = new GroupBox();
            NodeTypeButtonsLayout = new FlowLayoutPanel();
            NodeFilterLabel = new Label();
            MoveDownButton = new Button();
            MoveUpButton = new Button();
            ShowExtendedInfo = new CheckBox();
            TextNodesOnly = new CheckBox();
            NodeSizeField = new NumericUpDown();
            NodeSize = new Label();
            SaveStory = new Button();
            CenterButton = new Button();
            ((System.ComponentModel.ISupportInitialize)ColoringDepth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)IdealLength).BeginInit();
            MenuBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NodeSizeField).BeginInit();
            SuspendLayout();
            // 
            // NodeInfoLabel
            // 
            NodeInfoLabel.AutoSize = true;
            NodeInfoLabel.BackColor = System.Drawing.SystemColors.Desktop;
            NodeInfoLabel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            NodeInfoLabel.ForeColor = System.Drawing.SystemColors.MenuBar;
            NodeInfoLabel.Location = new System.Drawing.Point(0, 0);
            NodeInfoLabel.Margin = new Padding(4, 0, 4, 0);
            NodeInfoLabel.Name = "NodeInfoLabel";
            NodeInfoLabel.Size = new System.Drawing.Size(0, 18);
            NodeInfoLabel.TabIndex = 0;
            NodeInfoLabel.Visible = false;
            // 
            // ColoringDepth
            // 
            ColoringDepth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ColoringDepth.ForeColor = System.Drawing.SystemColors.MenuText;
            ColoringDepth.Location = new System.Drawing.Point(113, 12);
            ColoringDepth.Name = "ColoringDepth";
            ColoringDepth.Size = new System.Drawing.Size(75, 23);
            ColoringDepth.TabIndex = 1;
            ColoringDepth.ValueChanged += ColoringDepth_ValueChanged;
            // 
            // IdealLength
            // 
            IdealLength.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            IdealLength.Location = new System.Drawing.Point(113, 41);
            IdealLength.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            IdealLength.Name = "IdealLength";
            IdealLength.Size = new System.Drawing.Size(75, 23);
            IdealLength.TabIndex = 2;
            IdealLength.ValueChanged += IdealLength_ValueChanged;
            // 
            // Start
            // 
            Start.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Start.Location = new System.Drawing.Point(788, 529);
            Start.Name = "Start";
            Start.Size = new System.Drawing.Size(75, 23);
            Start.TabIndex = 3;
            Start.Text = "Start";
            Start.UseVisualStyleBackColor = true;
            Start.Click += Start_Click;
            // 
            // Stop
            // 
            Stop.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Stop.Location = new System.Drawing.Point(881, 529);
            Stop.Name = "Stop";
            Stop.Size = new System.Drawing.Size(75, 23);
            Stop.TabIndex = 4;
            Stop.Text = "Stop";
            Stop.UseVisualStyleBackColor = true;
            Stop.Click += Stop_Click;
            // 
            // ColorDepth
            // 
            ColorDepth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ColorDepth.AutoSize = true;
            ColorDepth.ForeColor = System.Drawing.SystemColors.MenuBar;
            ColorDepth.Location = new System.Drawing.Point(12, 14);
            ColorDepth.Name = "ColorDepth";
            ColorDepth.Size = new System.Drawing.Size(71, 15);
            ColorDepth.TabIndex = 5;
            ColorDepth.Text = "Color Depth";
            // 
            // EdgeLength
            // 
            EdgeLength.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            EdgeLength.AutoSize = true;
            EdgeLength.ForeColor = System.Drawing.SystemColors.MenuBar;
            EdgeLength.Location = new System.Drawing.Point(12, 43);
            EdgeLength.Name = "EdgeLength";
            EdgeLength.Size = new System.Drawing.Size(73, 15);
            EdgeLength.TabIndex = 6;
            EdgeLength.Text = "Edge Length";
            // 
            // NodeCalculations
            // 
            NodeCalculations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            NodeCalculations.AutoSize = true;
            NodeCalculations.ForeColor = System.Drawing.SystemColors.MenuBar;
            NodeCalculations.Location = new System.Drawing.Point(12, 537);
            NodeCalculations.Name = "NodeCalculations";
            NodeCalculations.Size = new System.Drawing.Size(0, 15);
            NodeCalculations.TabIndex = 7;
            // 
            // MenuShowButton
            // 
            MenuShowButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MenuShowButton.Location = new System.Drawing.Point(883, 12);
            MenuShowButton.Name = "MenuShowButton";
            MenuShowButton.Size = new System.Drawing.Size(89, 23);
            MenuShowButton.TabIndex = 8;
            MenuShowButton.Text = "Show Menu";
            MenuShowButton.UseVisualStyleBackColor = true;
            MenuShowButton.Click += MenuShowButton_Click;
            // 
            // MenuBox
            // 
            MenuBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            MenuBox.Controls.Add(NodeTypeButtonsLayout);
            MenuBox.Controls.Add(NodeFilterLabel);
            MenuBox.Controls.Add(MoveDownButton);
            MenuBox.Controls.Add(MoveUpButton);
            MenuBox.Controls.Add(ShowExtendedInfo);
            MenuBox.Controls.Add(TextNodesOnly);
            MenuBox.Controls.Add(NodeSizeField);
            MenuBox.Controls.Add(NodeSize);
            MenuBox.Controls.Add(IdealLength);
            MenuBox.Controls.Add(ColoringDepth);
            MenuBox.Controls.Add(ColorDepth);
            MenuBox.Controls.Add(EdgeLength);
            MenuBox.ForeColor = System.Drawing.SystemColors.MenuBar;
            MenuBox.Location = new System.Drawing.Point(770, 41);
            MenuBox.Name = "MenuBox";
            MenuBox.Size = new System.Drawing.Size(202, 464);
            MenuBox.TabIndex = 10;
            MenuBox.TabStop = false;
            MenuBox.Text = "Menu";
            MenuBox.Visible = false;
            // 
            // NodeTypeButtonsLayout
            // 
            NodeTypeButtonsLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            NodeTypeButtonsLayout.AutoScroll = true;
            NodeTypeButtonsLayout.BackColor = System.Drawing.Color.Transparent;
            NodeTypeButtonsLayout.Location = new System.Drawing.Point(3, 200);
            NodeTypeButtonsLayout.Margin = new Padding(0);
            NodeTypeButtonsLayout.MaximumSize = new System.Drawing.Size(188, 0);
            NodeTypeButtonsLayout.Name = "NodeTypeButtonsLayout";
            NodeTypeButtonsLayout.Size = new System.Drawing.Size(188, 261);
            NodeTypeButtonsLayout.TabIndex = 12;
            // 
            // NodeFilterLabel
            // 
            NodeFilterLabel.AutoSize = true;
            NodeFilterLabel.Location = new System.Drawing.Point(12, 185);
            NodeFilterLabel.Name = "NodeFilterLabel";
            NodeFilterLabel.Size = new System.Drawing.Size(95, 15);
            NodeFilterLabel.TabIndex = 14;
            NodeFilterLabel.Text = "Node Type Filter:";
            // 
            // MoveDownButton
            // 
            MoveDownButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            MoveDownButton.ForeColor = System.Drawing.SystemColors.MenuText;
            MoveDownButton.Location = new System.Drawing.Point(105, 153);
            MoveDownButton.Name = "MoveDownButton";
            MoveDownButton.Size = new System.Drawing.Size(75, 23);
            MoveDownButton.TabIndex = 12;
            MoveDownButton.Text = "Child node";
            MoveDownButton.UseVisualStyleBackColor = true;
            MoveDownButton.Click += MoveDownButton_Click;
            // 
            // MoveUpButton
            // 
            MoveUpButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            MoveUpButton.ForeColor = System.Drawing.SystemColors.MenuText;
            MoveUpButton.Location = new System.Drawing.Point(12, 153);
            MoveUpButton.Name = "MoveUpButton";
            MoveUpButton.Size = new System.Drawing.Size(79, 23);
            MoveUpButton.TabIndex = 11;
            MoveUpButton.Text = "Parent node";
            MoveUpButton.UseVisualStyleBackColor = true;
            MoveUpButton.Click += MoveUpButton_Click;
            // 
            // ShowExtendedInfo
            // 
            ShowExtendedInfo.AutoSize = true;
            ShowExtendedInfo.Location = new System.Drawing.Point(12, 128);
            ShowExtendedInfo.Name = "ShowExtendedInfo";
            ShowExtendedInfo.Size = new System.Drawing.Size(131, 19);
            ShowExtendedInfo.TabIndex = 10;
            ShowExtendedInfo.Text = "Show extended info";
            ShowExtendedInfo.UseVisualStyleBackColor = true;
            ShowExtendedInfo.CheckedChanged += ShowExtendedInfo_CheckedChanged;
            // 
            // TextNodesOnly
            // 
            TextNodesOnly.AutoSize = true;
            TextNodesOnly.Checked = true;
            TextNodesOnly.Location = new System.Drawing.Point(12, 103);
            TextNodesOnly.Name = "TextNodesOnly";
            TextNodesOnly.Size = new System.Drawing.Size(108, 19);
            TextNodesOnly.TabIndex = 9;
            TextNodesOnly.Text = "Text nodes only";
            TextNodesOnly.UseVisualStyleBackColor = true;
            TextNodesOnly.CheckedChanged += TextNodesOnly_CheckedChanged;
            // 
            // NodeSizeField
            // 
            NodeSizeField.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            NodeSizeField.Location = new System.Drawing.Point(113, 70);
            NodeSizeField.Name = "NodeSizeField";
            NodeSizeField.Size = new System.Drawing.Size(75, 23);
            NodeSizeField.TabIndex = 7;
            NodeSizeField.ValueChanged += NodeSizeField_ValueChanged;
            // 
            // NodeSize
            // 
            NodeSize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            NodeSize.AutoSize = true;
            NodeSize.ForeColor = System.Drawing.SystemColors.MenuBar;
            NodeSize.Location = new System.Drawing.Point(12, 72);
            NodeSize.Name = "NodeSize";
            NodeSize.Size = new System.Drawing.Size(72, 15);
            NodeSize.TabIndex = 8;
            NodeSize.Text = "Node Size";
            // 
            // SaveStory
            // 
            SaveStory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SaveStory.Location = new System.Drawing.Point(802, 12);
            SaveStory.Name = "SaveStory";
            SaveStory.Size = new System.Drawing.Size(75, 23);
            SaveStory.TabIndex = 11;
            SaveStory.Text = "Save Story";
            SaveStory.UseVisualStyleBackColor = true;
            // 
            // CenterButton
            // 
            CenterButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CenterButton.Location = new System.Drawing.Point(721, 12);
            CenterButton.Name = "CenterButton";
            CenterButton.Size = new System.Drawing.Size(75, 23);
            CenterButton.TabIndex = 12;
            CenterButton.Text = "Center";
            CenterButton.UseVisualStyleBackColor = true;
            CenterButton.Click += CenterButton_Click;
            // 
            // StoryExplorer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Desktop;
            ClientSize = new System.Drawing.Size(984, 561);
            Controls.Add(CenterButton);
            Controls.Add(SaveStory);
            Controls.Add(MenuBox);
            Controls.Add(MenuShowButton);
            Controls.Add(NodeCalculations);
            Controls.Add(Stop);
            Controls.Add(Start);
            Controls.Add(NodeInfoLabel);
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(600, 400);
            Name = "StoryExplorer";
            ShowIcon = false;
            Text = "Story Explorer";
            KeyDown += HandleKeyBoard;
            KeyUp += HandleKeyBoard;
            MouseClick += HandleMouseEvents;
            MouseMove += HandleMouseEvents;
            MouseWheel += HandleMouseEvents;
            ((System.ComponentModel.ISupportInitialize)ColoringDepth).EndInit();
            ((System.ComponentModel.ISupportInitialize)IdealLength).EndInit();
            MenuBox.ResumeLayout(false);
            MenuBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NodeSizeField).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private GroupBox MenuBox;
        private NumericUpDown NodeSizeField;
        private Label NodeSize;
        private CheckBox ShowExtendedInfo;
        private CheckBox TextNodesOnly;
        private Button MoveDownButton;
        private Button MoveUpButton;
        private Button SaveStory;
        private FlowLayoutPanel NodeTypeButtonsLayout;
        private Label NodeFilterLabel;
        private Button CenterButton;
    }
}
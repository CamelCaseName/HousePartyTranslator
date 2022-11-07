﻿using HousePartyTranslator.Helpers;
using System;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    partial class Fenster
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripComboBox languageToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem customOpenStoryExplorer;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInNewTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem overrideCloudSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Recents;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCurrentStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storyExplorerStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripReplaceAllButton;
        private System.Windows.Forms.ToolStripMenuItem toolStripReplaceButton;
        private System.Windows.Forms.ToolStripTextBox searchToolStripTextBox;
        private System.Windows.Forms.ToolStripTextBox ToolStripMenuReplaceBox;

        private static System.Drawing.Color background = Utils.background;
        private static System.Drawing.Color backgroundDarker = Utils.backgroundDarker;
        private static System.Drawing.Color brightText = Utils.brightText;
        private static System.Drawing.Color darkText = Utils.darkText;
        private static System.Drawing.Color foreground = Utils.foreground;
        private static System.Drawing.Color frame = Utils.frame;
        private static System.Drawing.Color menu = Utils.menu;

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


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor. - fuck you designer -
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fenster));
            this.customOpenStoryExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.openAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overrideCloudSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Recents = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrentStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storyExplorerStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ToolStripMenuReplaceBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripReplaceAllButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripReplaceButton = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.saveCurrentStringToolStripMenuItem,
            this.searchToolStripTextBox,
            this.ToolStripMenuReplaceBox,
            this.toolStripReplaceButton,
            this.toolStripReplaceAllButton,
            this.languageToolStripComboBox,
            this.storyExplorerStripMenuItem,
            this.customOpenStoryExplorer,
            this.settingsToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.MainMenu.AutoSize = true;
            this.MainMenu.Margin = new System.Windows.Forms.Padding(0);
            this.MainMenu.ForeColor = backgroundDarker;
            this.MainMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainMenu.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.MainMenu.BackColor = backgroundDarker;
            this.MainMenu.TabIndex = 17;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = Fenster.menu;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openAllToolStripMenuItem,
            this.openInNewTabToolStripMenuItem,
            new ToolStripSeparator(),
            this.Recents,
            new ToolStripSeparator(),
            this.saveToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            new ToolStripSeparator(),
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.AutoSize = false;
            this.fileToolStripMenuItem.Margin = new Padding(1);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.ToolTipText = "All relevant controls for opening and saving a file";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.BackColor = Fenster.menu;
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchToolStripMenuItem,
            this.searchAllToolStripMenuItem,
            new ToolStripSeparator(),
            this.replaceToolStripMenuItem,
            new ToolStripSeparator(),
            this.overrideCloudSaveToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.editToolStripMenuItem.AutoSize = false;
            this.editToolStripMenuItem.Margin = new Padding(1);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.ToolTipText = "All relevant controls for editing a file, plus special controls";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            this.searchToolStripMenuItem.ToolTipText = "Selects the search bar";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.SearchToolStripMenuItem_click);
            // 
            // searchAllToolStripMenuItem
            // 
            this.searchAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchAllToolStripMenuItem.Name = "searchAllToolStripMenuItem";
            this.searchAllToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.searchAllToolStripMenuItem.Text = "Search &All";
            this.searchAllToolStripMenuItem.ToolTipText = "Selects the search bar with the search all open files mode";
            this.searchAllToolStripMenuItem.Click += new System.EventHandler(this.SearchAllToolStripMenuItem_click);
            // 
            // replaceToolStripMenuItem
            //// 
            this.replaceToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.replaceToolStripMenuItem.Text = "&Replace";
            this.replaceToolStripMenuItem.ToolTipText = "opens the searchbar in replace mode";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.ReplaceToolStripMenuItem_click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Opens a dialog to select a file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // openAllToolStripMenuItem
            // 
            this.openAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openAllToolStripMenuItem.Name = "openAllToolStripMenuItem";
            this.openAllToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openAllToolStripMenuItem.Text = "Open &all";
            this.openAllToolStripMenuItem.ToolTipText = "Opens a dialog to select a file, all others will be discovered automatically. Usually.";
            this.openAllToolStripMenuItem.Click += new System.EventHandler(this.OpenAllToolStripMenuItem_Click);
            // 
            // openInNewTabToolStripMenuItem
            // 
            this.openInNewTabToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openInNewTabToolStripMenuItem.Image")));
            this.openInNewTabToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openInNewTabToolStripMenuItem.Name = "openInNewTabToolStripMenuItem";
            this.openInNewTabToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.openInNewTabToolStripMenuItem.Text = "Open in new &tab";
            this.openInNewTabToolStripMenuItem.ToolTipText = "Opens a dialog to select a file";
            this.openInNewTabToolStripMenuItem.Click += new System.EventHandler(this.OpenInNewTabToolStripMenuItem_Click);
            // 
            // Recents
            // 
            this.Recents.Enabled = false;
            this.Recents.Name = "Recents";
            this.Recents.ShowShortcutKeys = false;
            this.Recents.Size = new System.Drawing.Size(236, 22);
            this.Recents.Text = "Recents:";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveAllToolStripMenuItem.Text = "Sa&ve All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.SaveAllToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // overrideCloudSaveToolStripMenuItem
            // 
            this.overrideCloudSaveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.overrideCloudSaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.overrideCloudSaveToolStripMenuItem.Name = "overrideCloudSaveAsToolStripMenuItem";
            this.overrideCloudSaveToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.overrideCloudSaveToolStripMenuItem.Text = "Override &Cloud Save";
            this.overrideCloudSaveToolStripMenuItem.Click += new System.EventHandler(this.OverrideCloudSaveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // saveCurrentStringToolStripMenuItem
            // 
            this.saveCurrentStringToolStripMenuItem.BackColor = Fenster.menu;
            this.saveCurrentStringToolStripMenuItem.Name = "saveCurrentStringToolStripMenuItem";
            this.saveCurrentStringToolStripMenuItem.Size = new System.Drawing.Size(122, 23);
            this.saveCurrentStringToolStripMenuItem.AutoSize = false;
            this.saveCurrentStringToolStripMenuItem.Margin = new Padding(1);
            this.saveCurrentStringToolStripMenuItem.Text = "&Save selected string";
            this.saveCurrentStringToolStripMenuItem.Click += new System.EventHandler(this.SaveCurrentStringToolStripMenuItem_Click);
            // 
            // searchToolStripTextBox
            // 
            this.searchToolStripTextBox.BackColor = Fenster.menu;
            this.searchToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchToolStripTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchToolStripTextBox.AutoSize = true;
            this.searchToolStripTextBox.Name = "searchToolStripTextBox";
            this.searchToolStripTextBox.Size = new System.Drawing.Size(150, 23);
            this.searchToolStripTextBox.Margin = new Padding(1);
            this.searchToolStripTextBox.TextChanged += new System.EventHandler(this.SearchToolStripTextBox_TextChanged);
            // 
            // ToolStripMenuReplaceBox
            // 
            this.ToolStripMenuReplaceBox.BackColor = Fenster.menu;
            this.ToolStripMenuReplaceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ToolStripMenuReplaceBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ToolStripMenuReplaceBox.Name = "ToolStripMenuReplaceBox";
            this.ToolStripMenuReplaceBox.Size = new System.Drawing.Size(100, 23);
            this.ToolStripMenuReplaceBox.AutoSize = true;
            this.ToolStripMenuReplaceBox.Margin = new Padding(1);
            this.ToolStripMenuReplaceBox.Visible = false;
            this.ToolStripMenuReplaceBox.TextChanged += new System.EventHandler(this.ToolStripMenuReplaceBox_TextChanged);
            // 
            // toolStripReplaceAllButton
            // 
            this.toolStripReplaceAllButton.BackColor = Fenster.menu;
            this.toolStripReplaceAllButton.ForeColor = Fenster.darkText;
            this.toolStripReplaceAllButton.Name = "toolStripReplaceAllButton";
            this.toolStripReplaceAllButton.Size = new System.Drawing.Size(63, 23);
            this.toolStripReplaceAllButton.AutoSize = false;
            this.toolStripReplaceAllButton.Margin = new Padding(1);
            this.toolStripReplaceAllButton.Text = "Replace all";
            this.toolStripReplaceAllButton.Visible = false;
            this.toolStripReplaceAllButton.Click += new System.EventHandler(this.ToolStripReplaceAllButton_Click);
            // 
            // toolStripReplaceButton
            // 
            this.toolStripReplaceButton.BackColor = Fenster.menu;
            this.toolStripReplaceButton.ForeColor = Fenster.darkText;
            this.toolStripReplaceButton.Name = "toolStripReplaceButton";
            this.toolStripReplaceButton.Size = new System.Drawing.Size(63, 23);
            this.toolStripReplaceButton.AutoSize = false;
            this.toolStripReplaceButton.Margin = new Padding(1);
            this.toolStripReplaceButton.Text = "Replace";
            this.toolStripReplaceButton.Visible = false;
            this.toolStripReplaceButton.Click += new System.EventHandler(this.ToolStripReplaceButton_Click);
            // 
            // languageToolStripComboBox
            // 
            this.languageToolStripComboBox.BackColor = Fenster.menu;
            this.languageToolStripComboBox.Items.AddRange(LanguageHelper.ShortLanguages);
            this.languageToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.languageToolStripComboBox.Name = "languageToolStripComboBox";
            this.languageToolStripComboBox.Size = new System.Drawing.Size(75, 23);
            this.languageToolStripComboBox.AutoSize = true;
            this.languageToolStripComboBox.Margin = new Padding(1);
            this.languageToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageToolStripComboBox_SelectedIndexChanged);
            // 
            // storyExplorerStripMenuItem
            // 
            this.storyExplorerStripMenuItem.BackColor = Fenster.menu;
            this.storyExplorerStripMenuItem.Name = "storyExplorerStripMenuItem";
            this.storyExplorerStripMenuItem.Size = new System.Drawing.Size(118, 23);
            this.storyExplorerStripMenuItem.AutoSize = false;
            this.storyExplorerStripMenuItem.Margin = new Padding(1);
            this.storyExplorerStripMenuItem.Text = "&Auto StoryExplorer";
            this.storyExplorerStripMenuItem.Click += new System.EventHandler(this.StoryExplorerStripMenuItem_Click);
            // 
            // customOpenStoryExplorer
            // 
            this.customOpenStoryExplorer.BackColor = Fenster.menu;
            this.customOpenStoryExplorer.Name = "customOpenStoryExplorer";
            this.customOpenStoryExplorer.Size = new System.Drawing.Size(121, 23);
            this.customOpenStoryExplorer.AutoSize = false;
            this.customOpenStoryExplorer.Margin = new Padding(1);
            this.customOpenStoryExplorer.Text = "Open StoryE&xplorer";
            this.customOpenStoryExplorer.Click += new System.EventHandler(this.CustomStoryExplorerStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.BackColor = Fenster.menu;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.settingsToolStripMenuItem.AutoSize = false;
            this.settingsToolStripMenuItem.Margin = new Padding(1);
            this.settingsToolStripMenuItem.Text = "Se&ttings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // tabPage1
            // 
            this.tabPage1 = Utils.CreateNewTab(1, this);
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 27);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.SizeMode = TabSizeMode.Normal;
            this.MainTabControl.TabIndex = 9;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            this.MainTabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CloseTab_Click);
            // 
            // Fenster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Fenster.backgroundDarker;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.MainMenu);
            this.MinimumSize = new System.Drawing.Size(640, 470);
            this.Name = "Fenster";
            this.ShowIcon = false;
            this.Text = "HP Translator Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextContextOpened);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.MainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

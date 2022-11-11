using HousePartyTranslator.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    partial class Fenster
    {
        private IContainer components = null;
        private MenuStrip MainMenu;
        private TabControl MainTabControl;
        private TabPage tabPage1;
        private ToolStripComboBox languageToolStripComboBox;
        private ToolStripMenuItem customOpenStoryExplorer;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openAllToolStripMenuItem;
        private ToolStripMenuItem openInNewTabToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem overrideCloudSaveToolStripMenuItem;
        private ToolStripMenuItem Recents;
        private ToolStripMenuItem replaceToolStripMenuItem;
        private ToolStripMenuItem saveAllToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem saveCurrentStringToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem searchAllToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem storyExplorerStripMenuItem;
        private ToolStripMenuItem toolStripReplaceAllButton;
        private ToolStripMenuItem toolStripReplaceButton;
        private ToolStripTextBox searchToolStripTextBox;
        private ToolStripTextBox ToolStripMenuReplaceBox;

        private static Color background = Utils.background;
        private static Color backgroundDarker = Utils.backgroundDarker;
        private static Color brightText = Utils.brightText;
        private static Color darkText = Utils.darkText;
        private static Color foreground = Utils.foreground;
        private static Color frame = Utils.frame;
        private static Color menu = Utils.menu;

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
            ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fenster));
            SuspendLayout();

            // searchToolStripMenuItem
            searchToolStripMenuItem = new ToolStripMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(searchToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Search",
                ToolTipText = "Selects the search bar"
            };
            searchToolStripMenuItem.Click += new EventHandler(SearchToolStripMenuItem_click);

            // searchAllToolStripMenuItem
            searchAllToolStripMenuItem = new ToolStripMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(searchAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Search &All",
                ToolTipText = "Selects the search bar with the search all open files mode"
            };
            searchAllToolStripMenuItem.Click += new EventHandler(SearchAllToolStripMenuItem_click);

            // replaceToolStripMenuItem
            replaceToolStripMenuItem = new ToolStripMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(replaceToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Replace",
                ToolTipText = "opens the searchbar in replace mode"
            };
            replaceToolStripMenuItem.Click += new EventHandler(ReplaceToolStripMenuItem_click);

            // openToolStripMenuItem
            openToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Open",
                ToolTipText = "Opens a dialog to select a file"
            };
            openToolStripMenuItem.Click += new EventHandler(OpenToolStripMenuItem_Click);

            // openAllToolStripMenuItem
            openAllToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("openToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Open &all",
                ToolTipText = "Opens a dialog to select a file, all others will be discovered automatically. Usually."
            };
            openAllToolStripMenuItem.Click += new EventHandler(OpenAllToolStripMenuItem_Click);

            // openInNewTabToolStripMenuItem
            openInNewTabToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openInNewTabToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Open in new &tab",
                ToolTipText = "Opens a dialog to select a file"
            };
            openInNewTabToolStripMenuItem.Click += new EventHandler(OpenInNewTabToolStripMenuItem_Click);

            // Recents
            Recents = new ToolStripMenuItem()
            {
                Enabled = false,
                Name = nameof(Recents),
                ShowShortcutKeys = false,
                Size = new Size(236, 22),
                Text = "Recents:"
            };

            // saveToolStripMenuItem
            saveToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Save"
            };
            saveToolStripMenuItem.Click += new EventHandler(SaveToolStripMenuItem_Click);

            // saveAllToolStripMenuItem
            saveAllToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Sa&ve All"
            };
            saveAllToolStripMenuItem.Click += new EventHandler(SaveAllToolStripMenuItem_Click);

            // saveAsToolStripMenuItem
            saveAsToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveAsToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAsToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Save &As"
            };
            saveAsToolStripMenuItem.Click += new EventHandler(SaveAsToolStripMenuItem_Click);

            // overrideCloudSaveToolStripMenuItem
            overrideCloudSaveToolStripMenuItem = new ToolStripMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveAsToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(overrideCloudSaveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Override &Cloud Save"
            };
            overrideCloudSaveToolStripMenuItem.Click += new EventHandler(OverrideCloudSaveToolStripMenuItem_Click);

            // exitToolStripMenuItem
            exitToolStripMenuItem = new ToolStripMenuItem()
            {
                Name = nameof(exitToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Exit"
            };
            exitToolStripMenuItem.Click += new EventHandler(ExitToolStripMenuItem_Click);

            // saveCurrentStringToolStripMenuItem
            saveCurrentStringToolStripMenuItem = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                Name = nameof(saveCurrentStringToolStripMenuItem),
                Size = new Size(122, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Save selected string"
            };
            saveCurrentStringToolStripMenuItem.Click += new EventHandler(SaveCurrentStringToolStripMenuItem_Click);

            // searchToolStripTextBox
            searchToolStripTextBox = new ToolStripTextBox()
            {
                BackColor = Fenster.menu,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Name = nameof(searchToolStripTextBox),
                Size = new Size(150, 23),
                Margin = new Padding(1)
            };
            searchToolStripTextBox.TextChanged += new EventHandler(SearchToolStripTextBox_TextChanged);

            // ToolStripMenuReplaceBox
            ToolStripMenuReplaceBox = new ToolStripTextBox()
            {
                BackColor = Fenster.menu,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Name = nameof(ToolStripMenuReplaceBox),
                Size = new Size(100, 23),
                AutoSize = true,
                Margin = new Padding(1),
                Visible = false
            };
            ToolStripMenuReplaceBox.TextChanged += new EventHandler(ToolStripMenuReplaceBox_TextChanged);

            // toolStripReplaceAllButton
            toolStripReplaceAllButton = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                ForeColor = Fenster.darkText,
                Name = nameof(toolStripReplaceAllButton),
                Size = new Size(63, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Replace all",
                Visible = false
            };
            toolStripReplaceAllButton.Click += new EventHandler(ToolStripReplaceAllButton_Click);

            // toolStripReplaceButton
            toolStripReplaceButton = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                ForeColor = Fenster.darkText,
                Name = nameof(toolStripReplaceButton),
                Size = new Size(63, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Replace",
                Visible = false
            };
            toolStripReplaceButton.Click += new EventHandler(ToolStripReplaceButton_Click);

            // languageToolStripComboBox
            languageToolStripComboBox = new ToolStripComboBox()
            {
                BackColor = Fenster.menu,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Name = nameof(languageToolStripComboBox),
                Size = new Size(50, 23),
                MaxLength = 4,
                AutoSize = true,
                Margin = new Padding(1)
            };
            languageToolStripComboBox.Items.AddRange(LanguageHelper.ShortLanguages);
            languageToolStripComboBox.SelectedIndexChanged += new EventHandler(LanguageToolStripComboBox_SelectedIndexChanged);

            // storyExplorerStripMenuItem
            storyExplorerStripMenuItem = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                Name = nameof(storyExplorerStripMenuItem),
                Size = new Size(118, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Auto StoryExplorer"
            };
            storyExplorerStripMenuItem.Click += new EventHandler(StoryExplorerStripMenuItem_Click);

            // customOpenStoryExplorer
            customOpenStoryExplorer = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                Name = nameof(customOpenStoryExplorer),
                Size = new Size(121, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Open StoryE&xplorer"
            };
            customOpenStoryExplorer.Click += new EventHandler(CustomStoryExplorerStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            settingsToolStripMenuItem = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                Name = nameof(settingsToolStripMenuItem),
                Size = new Size(61, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Se&ttings"
            };
            settingsToolStripMenuItem.Click += new EventHandler(SettingsToolStripMenuItem_Click);

            // editToolStripMenuItem
            editToolStripMenuItem = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                Name = nameof(editToolStripMenuItem),
                Size = new Size(37, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Edit",
                ToolTipText = "All relevant controls for editing a file, plus special controls"
            };
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                searchToolStripMenuItem,
                searchAllToolStripMenuItem,
                new ToolStripSeparator(),
                replaceToolStripMenuItem,
                new ToolStripSeparator(),
                overrideCloudSaveToolStripMenuItem
            });

            // fileToolStripMenuItem
            fileToolStripMenuItem = new ToolStripMenuItem()
            {
                BackColor = Fenster.menu,
                Name = nameof(fileToolStripMenuItem),
                Size = new Size(37, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&File",
                ToolTipText = "All relevant controls for opening and saving a file"
            };
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                openToolStripMenuItem,
                openAllToolStripMenuItem,
                openInNewTabToolStripMenuItem,
                new ToolStripSeparator(),
                Recents,
                new ToolStripSeparator(),
                saveToolStripMenuItem,
                saveAllToolStripMenuItem,
                saveAsToolStripMenuItem,
                new ToolStripSeparator(),
                exitToolStripMenuItem
            });

            // MainMenu
            MainMenu = new MenuStrip()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSize = true,
                BackColor = backgroundDarker,
                Dock = DockStyle.Top,
                ForeColor = backgroundDarker,
                LayoutStyle = ToolStripLayoutStyle.Flow,
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Name = nameof(MainMenu),
                TabIndex = 17
            };
            MainMenu.SuspendLayout();
            MainMenu.Items.AddRange(new ToolStripItem[] {
                fileToolStripMenuItem,
                editToolStripMenuItem,
                saveCurrentStringToolStripMenuItem,
                searchToolStripTextBox,
                ToolStripMenuReplaceBox,
                toolStripReplaceButton,
                toolStripReplaceAllButton,
                languageToolStripComboBox,
                storyExplorerStripMenuItem,
                customOpenStoryExplorer,
                settingsToolStripMenuItem
            });

            // tabPage1
            tabPage1 = Utils.CreateNewTab(1, this);
            tabPage1.SuspendLayout();

            // MainTabControl
            MainTabControl = new TabControl()
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 27),
                Name = nameof(MainTabControl),
                SelectedIndex = 0,
                SizeMode = TabSizeMode.Normal,
                TabIndex = 9
            };
            MainTabControl.SuspendLayout();
            MainTabControl.Controls.Add(tabPage1);
            MainTabControl.SelectedIndexChanged += new EventHandler(MainTabControl_SelectedIndexChanged);
            MainTabControl.MouseClick += new MouseEventHandler(CloseTab_Click);

            // Fenster
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Fenster.backgroundDarker;
            ClientSize = new Size(1400, 760);
            Controls.Add(MainMenu);
            Controls.Add(MainTabControl);
            MinimumSize = new Size(640, 470);
            Name = nameof(Fenster);
            ShowIcon = false;
            Text = "Translator";
            FormClosing += new FormClosingEventHandler(OnFormClosing);
            MouseUp += new MouseEventHandler(TextContextOpened);
            Shown += new EventHandler(OnFormShown);
            MainMenu.ResumeLayout(true);
            MainTabControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ResumeLayout(true);
        }
    }
}

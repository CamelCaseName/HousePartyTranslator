using System.ComponentModel;
using Translator.Core.Helpers;
using TranslatorAdmin.InterfaceImpls;

namespace Translator
{
    partial class Fenster
    {
        private IContainer components;
        private MenuStrip MainMenu;
        private ToolStripComboBox languageToolStripComboBox;
        private WinMenuItem customOpenStoryExplorer;
        private WinMenuItem editToolStripMenuItem;
        private WinMenuItem exitToolStripMenuItem;
        private WinMenuItem fileToolStripMenuItem;
        private WinMenuItem openAllToolStripMenuItem;
        private WinMenuItem openInNewTabToolStripMenuItem;
        private WinMenuItem openToolStripMenuItem;
        private WinMenuItem overrideCloudSaveToolStripMenuItem;
        private WinMenuItem Recents;
        private WinMenuItem replaceToolStripMenuItem;
        private WinMenuItem saveAllToolStripMenuItem;
        private WinMenuItem saveAsToolStripMenuItem;
        private WinMenuItem saveCurrentStringToolStripMenuItem;
        private WinMenuItem saveToolStripMenuItem;
        private WinMenuItem searchAllToolStripMenuItem;
        private WinMenuItem searchToolStripMenuItem;
        private WinMenuItem settingsToolStripMenuItem;
        private WinMenuItem storyExplorerStripMenuItem;
        private WinMenuItem toolStripReplaceAllButton;
        private WinMenuItem toolStripReplaceButton;
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
            searchToolStripMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(searchToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Search",
                ToolTipText = "Selects the search bar"
            };
            searchToolStripMenuItem.Click += new EventHandler(SearchToolStripMenuItem_click);

            // searchAllToolStripMenuItem
            searchAllToolStripMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(searchAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Search &All",
                ToolTipText = "Selects the search bar with the search all open files mode"
            };
            searchAllToolStripMenuItem.Click += new EventHandler(SearchAllToolStripMenuItem_click);

            // replaceToolStripMenuItem
            replaceToolStripMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(replaceToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Replace",
                ToolTipText = "opens the searchbar in replace mode"
            };
            replaceToolStripMenuItem.Click += new EventHandler(ReplaceToolStripMenuItem_click);

            // openToolStripMenuItem
            openToolStripMenuItem = new WinMenuItem()
            {
                Image = ((Image)(resources.GetObject("openToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Open",
                ToolTipText = "Opens a dialog to select a file"
            };
            openToolStripMenuItem.Click += new EventHandler(OpenToolStripMenuItem_Click);

            // openAllToolStripMenuItem
            openAllToolStripMenuItem = new WinMenuItem()
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
            openInNewTabToolStripMenuItem = new WinMenuItem()
            {
                Image = ((Image)(resources.GetObject("openToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openInNewTabToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Open in new &tab",
                ToolTipText = "Opens a dialog to select a file"
            };
            openInNewTabToolStripMenuItem.Click += new EventHandler(OpenInNewTabToolStripMenuItem_Click);

            // Recents
            Recents = new WinMenuItem()
            {
                Enabled = false,
                Name = nameof(Recents),
                ShowShortcutKeys = false,
                Size = new Size(236, 22),
                Text = "Recents:"
            };

            // saveToolStripMenuItem
            saveToolStripMenuItem = new WinMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Save"
            };
            saveToolStripMenuItem.Click += new EventHandler(SaveToolStripMenuItem_Click);

            // saveAllToolStripMenuItem
            saveAllToolStripMenuItem = new WinMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Sa&ve All"
            };
            saveAllToolStripMenuItem.Click += new EventHandler(SaveAllToolStripMenuItem_Click);

            // saveAsToolStripMenuItem
            saveAsToolStripMenuItem = new WinMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveAsToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAsToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Save &As"
            };
            saveAsToolStripMenuItem.Click += new EventHandler(SaveAsToolStripMenuItem_Click);

            // overrideCloudSaveToolStripMenuItem
            overrideCloudSaveToolStripMenuItem = new WinMenuItem()
            {
                Image = ((Image)(resources.GetObject("saveAsToolStripMenuItem.Image"))),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(overrideCloudSaveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Override &Cloud Save"
            };
            overrideCloudSaveToolStripMenuItem.Click += new EventHandler(OverrideCloudSaveToolStripMenuItem_Click);

            // exitToolStripMenuItem
            exitToolStripMenuItem = new WinMenuItem()
            {
                Name = nameof(exitToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Exit"
            };
            exitToolStripMenuItem.Click += new EventHandler(ExitToolStripMenuItem_Click);

            // saveCurrentStringToolStripMenuItem
            saveCurrentStringToolStripMenuItem = new WinMenuItem()
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
            toolStripReplaceAllButton = new WinMenuItem()
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
            toolStripReplaceButton = new WinMenuItem()
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
            storyExplorerStripMenuItem = new WinMenuItem()
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
            customOpenStoryExplorer = new WinMenuItem()
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
            settingsToolStripMenuItem = new WinMenuItem()
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
            editToolStripMenuItem = new WinMenuItem()
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
                new WinMenuSeperator(),
                replaceToolStripMenuItem,
                new WinMenuSeperator(),
                overrideCloudSaveToolStripMenuItem
            });

            // fileToolStripMenuItem
            fileToolStripMenuItem = new WinMenuItem()
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
                new WinMenuSeperator(),
                Recents,
                new WinMenuSeperator(),
                saveToolStripMenuItem,
                saveAllToolStripMenuItem,
                saveAsToolStripMenuItem,
                new WinMenuSeperator(),
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

            TabControl.SuspendLayout();
            TabControl.SelectedIndexChanged += new EventHandler(MainTabControl_SelectedIndexChanged);
            TabControl.MouseClick += new MouseEventHandler(CloseTab_Click);

            // Fenster
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Fenster.backgroundDarker;
            ClientSize = new Size(1400, 760);
            Controls.Add(TabControl);
            Controls.Add(MainMenu);
            MinimumSize = new Size(640, 470);
            Name = nameof(Fenster);
            ShowIcon = false;
            Text = "Translator";
            FormClosing += new FormClosingEventHandler(OnFormClosing);
            MouseUp += new MouseEventHandler(TextContextOpened);
            Shown += new EventHandler(OnFormShown);
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            TabControl.ResumeLayout(false);
            ResumeLayout();
            PerformLayout();
        }
    }
}

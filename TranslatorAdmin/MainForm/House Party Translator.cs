using System.ComponentModel;
using Translator.Core;
using Translator.Core.Helpers;
using Translator.Helpers;
using Translator.Managers;
using Translator.StoryExplorerForm;
using Translator.UICompatibilityLayer;
using TranslatorAdmin.InterfaceImpls;
using TranslatorAdmin.Managers;
using DataBase = Translator.Core.DataBase<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController, TranslatorAdmin.InterfaceImpls.WinTab>;
using Settings = TranslatorAdmin.Properties.Settings;
using TabManager = Translator.Core.TabManager<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController, TranslatorAdmin.InterfaceImpls.WinTab>;
using WinUtils = Translator.Core.Helpers.Utils<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController, TranslatorAdmin.InterfaceImpls.WinTab>;

namespace Translator
{
    /// <summary>
    /// The main class which handles the UI for the House Party Translator Window
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class Fenster : Form
    {
        public MenuStrip MainMenu;
        internal readonly WinTabController TabControl = new()
        {
            Dock = DockStyle.Fill,
            Location = new Point(0, 27),
            Name = nameof(TabControl),
            SelectedIndex = 0,
            SizeMode = TabSizeMode.Normal,
            TabIndex = 9
        };

        internal WinUIHandler UI;
        private static readonly Color background = Utils.background;
        private static readonly Color backgroundDarker = Utils.backgroundDarker;
        private static readonly Color brightText = Utils.brightText;
        private static readonly Color darkText = Utils.darkText;
        private static readonly Color foreground = Utils.foreground;
        private static readonly Color frame = Utils.frame;
        private static readonly Color menu = Utils.menu;
        private readonly CancellationTokenSource CancelTokens = new();
        private readonly LineList CheckListBoxLeft;
        private readonly ContextMenuStrip? ListContextMenu;
        private readonly System.Timers.Timer PresenceTimer = new(2000);
        private WinMenuItem customOpenStoryExplorer;
        private WinMenuItem editToolStripMenuItem;
        private WinMenuItem exitToolStripMenuItem;
        private WinMenuItem fileToolStripMenuItem;
        private ToolStripComboBox languageToolStripComboBox;
        private WinMenuItem openAllToolStripMenuItem;
        private WinMenuItem openInNewTabToolStripMenuItem;
        private WinMenuItem openToolStripMenuItem;
        private WinMenuItem overrideCloudSaveToolStripMenuItem;
        private DiscordPresenceManager? PresenceManager;
        private WinMenuItem Recents;
        private WinMenuItem replaceToolStripMenuItem;
        private WinMenuItem saveAllToolStripMenuItem;
        private WinMenuItem saveAsToolStripMenuItem;
        private WinMenuItem saveCurrentStringToolStripMenuItem;
        private WinMenuItem saveToolStripMenuItem;
        private WinMenuItem searchAllToolStripMenuItem;
        private WinMenuItem searchToolStripMenuItem;
        private ToolStripTextBox searchToolStripTextBox;
        private WinMenuItem settingsToolStripMenuItem;
        private StoryExplorer? SExplorer;
        private WinMenuItem storyExplorerStripMenuItem;
        private ToolStripTextBox ToolStripMenuReplaceBox;
        private WinMenuItem toolStripReplaceAllButton;
        private WinMenuItem toolStripReplaceButton;

        /// <summary>
        /// static constructor for static fields
        /// </summary>
        static Fenster()
        {
            ProgressbarWindow = new ProgressbarForm.ProgressWindow();
            ProgressbarWindow.Status.Text = "starting...";
            ProgressbarWindow.Text = "Startup";
            ProgressbarWindow.Hide();
        }

        /// <summary>
        /// Constructor for the main window of the translator, starts all other components in the correct order
        /// </summary>
        public Fenster()
        {
            ProgressbarWindow.Show();
            //custom exception handlers to handle mysql exceptions
#if RELEASE || USER_RELEASE
            AppDomain.CurrentDomain.UnhandledException += FensterUnhandledExceptionHandler;
            Application.ThreadException += ThreadExceptionHandler;
#endif
            UI = new(TabControl);

            //initialize ui, controllers, database and so on
            var tab = new WinTab(this);
            TabManager.Initialize(UI, typeof(WinMenuItem), typeof(WinMenuSeperator), GetPassword(), SoftwareVersionManager.LocalVersion, tab, new WinSettings());
            CheckListBoxLeft = tab.Lines;
            ListContextMenu = CheckListBoxLeft.ContextMenuStrip;

            //init all form components
            InitializeComponent();
            ProgressbarWindow.PerformStep();

            //check for update and replace if we want one
            SoftwareVersionManager.ReplaceFileIfNew();
            ProgressbarWindow.PerformStep();
        }

        public static ProgressbarForm.ProgressWindow ProgressbarWindow { get; private set; }

        /// <summary>
        /// Instance of the Story Explorer, but the owner is checked so only the Storyexplorer class itself can instantiate it.
        /// </summary>
        public StoryExplorer? Explorer
        {
            get
            {
                return SExplorer;
            }
            set
            {
                if (value != null)
                {
                    if (value.ParentName == Name)
                    {
                        SExplorer = value;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("You must only write to this object?from within the Explorer class");
                    }
                }
            }
        }

        internal WinMenuItem FileToolStripMenuItem => fileToolStripMenuItem;

        internal ToolStripComboBox LanguageBox => languageToolStripComboBox;

        internal WinMenuItem ReplaceAllButton => toolStripReplaceAllButton;

        internal ToolStripTextBox ReplaceBox => ToolStripMenuReplaceBox;

        internal WinMenuItem ReplaceButton => toolStripReplaceButton;

        internal ToolStripTextBox SearchBox => searchToolStripTextBox;

        public void ApprovedBox_CheckedChanged(object? sender, EventArgs? e)
        {
            KeypressManager.ApprovedButtonChanged();
        }

        public void CheckListBoxLeft_ItemCheck(object? sender, ItemCheckEventArgs? e)
        {
            KeypressManager.CheckItemChanged();
        }

        public void CheckListBoxLeft_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            KeypressManager.SelectedItemChanged((LineList)CheckListBoxLeft);
        }

        public void Comments_TextChanged(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.UpdateComments();
            KeypressManager.TextChangedCallback(this, CheckListBoxLeft.SelectedIndex);
        }

        public void CopyAllContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyAll();
        }

        public void CopyAsOutputContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyAsOutput();
        }

        public void CopyFileNameContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyFileName();
        }

        public void CopyIdContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyId();
        }

        public void CopyStoryNameContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyStoryName();
        }

        public void CopyTemplateContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyTemplate();
        }

        public void CopyTranslationContextMenuButton_Click(object? sender, EventArgs? e)
        {
            TabManager.CopyTranslation();
        }

        /// <summary>
        /// Tries to delete the word in let or right ofthe cursor in the currently selected TextBox.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="toLeft">true if deleting to the left</param>
        /// <returns>true if successfull</returns>
        public bool DeleteCharactersInText(bool toLeft)
        {
            Control? focused_control = ActiveControl;
            try
            {
                _ = (TextBox?)focused_control;
            }
            //ignore exception, really intended
            catch { return false; }
            if (focused_control == null) return false;
            var textBox = (TextBox)focused_control;
            if (toLeft)
            {
                return ((ITextBox)textBox).DeleteCharactersInTextLeft();
            }
            else
            {
                return ((ITextBox)textBox).DeleteCharactersInTextRight();
            }
        }

        /// <summary>
        /// Moves the cursor to the beginning/end of the next word in the specified direction
        /// </summary>
        /// <param name="form"></param>
        /// <param name="toLeft">true if to scan to the left</param>
        /// <returns>true if succeeded</returns>
        public bool MoveCursorInText(bool toLeft)
        {
            Control? focused_control = ActiveControl;
            try
            {
                _ = (TextBox?)focused_control;
            }
            //ignore exception, really intended
            catch { return false; }
            if (focused_control == null) return false;
            var textBox = (TextBox)focused_control;
            if (toLeft)
            {
                ((ITextBox)textBox).MoveCursorWordLeft();
                return true;
            }
            else
            {
                ((ITextBox)textBox).MoveCursorWordRight();
                return true;
            }
        }

        public void OpeningContextMenu(object? sender, MouseEventArgs? e)
        {
            if (e == null || ListContextMenu == null)
                return;
            KeypressManager.OpenContextMenu(ListContextMenu, e);
        }

        public void TextBoxRight_TextChanged(object? sender, EventArgs? e)
        {
            KeypressManager.TextChangedCallback(this, CheckListBoxLeft.SelectedIndex);
            KeypressManager.TranslationTextChanged();
        }

        public void TextContextOpened(object? sender, EventArgs? e)
        {
            if (sender == null) return;
            KeypressManager.PrepareTextChanged(sender);
        }

        public void TranslateThis_Click(object? sender, EventArgs? e)
        {
            KeypressManager.AutoTranslate();
        }

        /// <summary>
        /// Override to intercept the Keystrokes windows sends us.
        /// </summary>
        /// <param name="msg">The message containing relevant info</param>
        /// <param name="keyData">List of all Keys that were pressed in this event</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (KeypressManager.MainKeyPressHandler(ref msg, keyData, CancelTokens))
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void CloseTab_Click(object? sender, MouseEventArgs? e)
        {
            TabManager.CloseTab((WinTab?)sender ?? (WinTab)new object());
        }

        private async void CustomStoryExplorerStripMenuItem_Click(object? sender, EventArgs? e)
        {
            Explorer = await KeypressManager.CreateStoryExplorer(false, this, CancelTokens);
        }

        private void ExitToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            UI.SignalAppExit();
        }

        private void FensterUnhandledExceptionHandler(object? sender, UnhandledExceptionEventArgs? e)
        {
            if (e == null) { LogManager.Log("No eventargs on unhandled exception", LogManager.Level.Error); }
            else
            {
                LogManager.Log(e.ExceptionObject?.ToString() ?? "ExceptionObject is null", LogManager.Level.Error);

                if (e.ExceptionObject == null) return;

                if (e.ExceptionObject.GetType().IsAssignableTo(typeof(Exception)))
                {
                    WinUtils.DisplayExceptionMessage(((Exception)e.ExceptionObject).Message);
                }
                else
                {
                    WinUtils.DisplayExceptionMessage(e.ExceptionObject.ToString() ?? "ExceptionObject is null");
                }
            }
        }

        private string GetPassword()
        {
            if (Settings.Default.dbPassword.Length > 0) return Settings.Default.dbPassword;

            var Passwordbox = new Password();
            DialogResult passwordResult = Passwordbox.ShowDialog(this);
            if (passwordResult == DialogResult.OK)
            {
                Settings.Default.dbPassword = Passwordbox.GetPassword();
                Settings.Default.Save();
            }
            else
            {
                if (Passwordbox.GetPassword().Length > 1)
                {
                    _ = Msg.ErrorOk("Invalid password", "Wrong password");
                }
                else
                {
                    Environment.Exit(-1);
                }
            }
            return Settings.Default.dbPassword;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(Fenster));
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
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
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
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
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
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
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
                Image = (Image?)resources.GetObject("saveToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Save"
            };
            saveToolStripMenuItem.Click += new EventHandler(SaveToolStripMenuItem_Click);

            // saveAllToolStripMenuItem
            saveAllToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("saveToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Sa&ve All"
            };
            saveAllToolStripMenuItem.Click += new EventHandler(SaveAllToolStripMenuItem_Click);

            // saveAsToolStripMenuItem
            saveAsToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("saveAsToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAsToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Save &As"
            };
            saveAsToolStripMenuItem.Click += new EventHandler(SaveAsToolStripMenuItem_Click);

            // overrideCloudSaveToolStripMenuItem
            overrideCloudSaveToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image)resources.GetObject("saveAsToolStripMenuItem.Image"),
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
            languageToolStripComboBox.SelectedItem= Settings.Default.language;
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

        private void LanguageToolStripComboBox_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            KeypressManager.SelectedLanguageChanged();
        }

        private void MainTabControl_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            KeypressManager.SelectedTabChanged(PresenceManager);
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs? e)
        {
            Settings.Default.Save();

            //prevent discord from getting angry
            PresenceManager?.DeInitialize();

            RecentsManager.SaveRecents<WinLineItem, WinUIHandler, WinTabController, WinTab>();

            CancelTokens.Cancel();

            CancelTokens.Dispose();

            //show save unsaved changes dialog
            TabManager.ActiveTranslationManager.ShowAutoSaveDialog();

            LogManager.SaveLogFile();
        }

        private void OnFormShown(object? sender, EventArgs? e)
        {
            TabManager.FinalizeInitializer();

            Text = DataBase.AppTitle;

            ProgressbarWindow.PerformStep();
            LogManager.Log("Application initializing...");
            PresenceManager = new DiscordPresenceManager();

            WinTranslationManager.DiscordPresence = PresenceManager;

            ProgressbarWindow.PerformStep();

            //open most recent after db is initialized
            UpdateFileMenuItems();
            RecentsManager.OpenMostRecent<WinLineItem, WinUIHandler, WinTabController, WinTab>();

            //start timer to update presence
            PresenceTimer.Elapsed += (sender_, args) => { PresenceManager.Update(); };
            PresenceTimer.Start();

            PresenceManager.Update(TabManager.ActiveTranslationManager.StoryName ?? "None", TabManager.ActiveTranslationManager.FileName ?? "None");

            LogManager.Log($"Application initialized with app version:{SoftwareVersionManager.LocalVersion} db version:{(DataBase.IsOnline ? DataBase.DBVersion : "*offline*")} story version:{Settings.Default.version}");
            //ProgressbarWindow.Hide();
            //ProgressbarWindow.Status.Text = "progress";
            //ProgressbarWindow.Text = "Autosave";

            //hide override button if not in advanced mode
            if (!Settings.Default.advancedMode)
                overrideCloudSaveToolStripMenuItem.Enabled = false;
        }

        private void OpenAllToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            KeypressManager.OpenAll();
        }

        private void OpenInNewTabToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            KeypressManager.OpenNewTab();
        }

        private void OpenToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            KeypressManager.OpenNew();
        }

        private void OverrideCloudSaveToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.OverrideCloudSave();
        }

        private void ReplaceToolStripMenuItem_click(object? sender, EventArgs? e)
        {
            KeypressManager.ToggleReplaceUI();
        }

        private void SaveAllToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            _ = TabManager.SaveAllTabs();
        }

        private void SaveAsToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.SaveFileAs();
        }

        private void SaveCurrentStringToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.SaveCurrentString();
        }

        private void SaveToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.SaveFile();
        }

        private void SearchAllToolStripMenuItem_click(object? sender, EventArgs? e)
        {
            searchToolStripTextBox.Focus();
            if (searchToolStripTextBox.Text.Length == 0) searchToolStripTextBox.Text = "?search here";
        }

        private void SearchToolStripMenuItem_click(object? sender, EventArgs? e)
        {
            searchToolStripTextBox.Focus();
            if (searchToolStripTextBox.Text.Length == 0) searchToolStripTextBox.Text = "search here";
        }

        private void SearchToolStripTextBox_TextChanged(object? sender, EventArgs? e)
        {
            KeypressManager.TextChangedCallback(this, CheckListBoxLeft.SelectedIndex);
            TabManager.Search();
        }

        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            KeypressManager.ShowSettings();
        }

        private async void StoryExplorerStripMenuItem_Click(object? sender, EventArgs? e)
        {
            Explorer = await KeypressManager.CreateStoryExplorer(true, this, CancelTokens);
        }

        private void ThreadExceptionHandler(object? sender, ThreadExceptionEventArgs? e)
        {
            if (e == null) { LogManager.Log("No eventargs on unhandled exception", LogManager.Level.Error); return; }

            LogManager.Log(e.Exception.ToString(), LogManager.Level.Error);
            WinUtils.DisplayExceptionMessage(e.Exception.Message);
        }

        private void ToolStripMenuReplaceBox_TextChanged(object? sender, EventArgs? e)
        {
            KeypressManager.TextChangedCallback(this, CheckListBoxLeft.SelectedIndex);
        }

        private void ToolStripReplaceAllButton_Click(object? sender, EventArgs? e)
        {
            TabManager.ReplaceAll();
        }

        private void ToolStripReplaceButton_Click(object? sender, EventArgs? e)
        {
            TabManager.Replace();
        }

        private void UpdateFileMenuItems()
        {
            var items = RecentsManager.GetUpdatedMenuItems<WinLineItem, WinUIHandler, WinTabController, WinTab>(FileToolStripMenuItem.DropDownItems.ToMenuItems());
            FileToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                FileToolStripMenuItem.DropDownItems.Add((ToolStripItem)items[i]);
            }
        }
    }
}
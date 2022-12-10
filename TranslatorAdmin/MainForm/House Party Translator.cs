using Translator.Core;
using Translator.Core.Helpers;
using Translator.Helpers;
using Translator.Managers;
using Translator.StoryExplorerForm;
using Translator.UICompatibilityLayer;
using TranslatorAdmin.InterfaceImpls;
using TranslatorAdmin.Managers;
using Settings = TranslatorAdmin.Properties.Settings;
using TabManager = Translator.Core.TabManager<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController>;
using DataBase = Translator.Core.DataBase<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController>;
using WinUtils = Translator.Core.Helpers.Utils<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController>;

namespace Translator
{
    /// <summary>
    /// The main class which handles the UI for the House Party Translator Window
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class Fenster : Form
    {
        private readonly CancellationTokenSource CancelTokens = new();
        private readonly ColouredCheckedListBox CheckListBoxLeft;
        private readonly ContextMenuStrip? ListContextMenu;
        private readonly System.Timers.Timer PresenceTimer = new(2000);
        internal readonly WinTabController TabControl = new()
        {
            Dock = DockStyle.Fill,
            Location = new Point(0, 27),
            Name = nameof(TabControl),
            SelectedIndex = 0,
            SizeMode = TabSizeMode.Normal,
            TabIndex = 9
        };

        public static ProgressbarForm.ProgressWindow ProgressbarWindow { get; private set; }
        private DiscordPresenceManager? PresenceManager;
        private StoryExplorer? SExplorer;
        internal WinUIHandler UI;

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
            CheckListBoxLeft = tab.CheckedListBox;
            ListContextMenu = CheckListBoxLeft.ContextMenuStrip;

            Text = DataBase.AppTitle;

            //init all form components
            InitializeComponent();
            ProgressbarWindow.PerformStep();

            //check for update and replace if we want one
            SoftwareVersionManager.ReplaceFileIfNew();
            ProgressbarWindow.PerformStep();
        }

        private string GetPassword()
        {
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

        private void ReplaceToolStripMenuItem_click(object? sender, EventArgs? e)
        {
            KeypressManager.ToggleReplaceUI();
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
            //prevent discord from getting angry
            PresenceManager?.DeInitialize();

            RecentsManager.SaveRecents<WinLineItem, WinUIHandler, WinTabController>();

            CancelTokens.Cancel();

            CancelTokens.Dispose();

            //show save unsaved changes dialog
            TabManager.ActiveTranslationManager.ShowAutoSaveDialog();

            LogManager.SaveLogFile();
        }

        private void OnFormShown(object? sender, EventArgs? e)
        {
            TabManager.FinalizeInitializer();

            ProgressbarWindow.PerformStep();
            LogManager.Log("Application initializing...");
            PresenceManager = new DiscordPresenceManager();

            WinTranslationManager.DiscordPresence = PresenceManager;

            ProgressbarWindow.PerformStep();

            //Settings have to be loaded before the Database can be connected with
            ProgressbarWindow.PerformStep();

            //open most recent after db is initialized
            RecentsManager.UpdateMenuItems<WinLineItem, WinUIHandler, WinTabController>(FileToolStripMenuItem.DropDownItems.ToMenuItems());
            RecentsManager.OpenMostRecent<WinLineItem, WinUIHandler, WinTabController>();

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

        private void SaveAllToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            _ = TabManager.SaveAllTabs();
        }

        private void SaveAsToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.SaveFileAs();
        }

        private void OverrideCloudSaveToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.OverrideCloudSave();
        }

        private void SaveCurrentStringToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.SaveCurrentString();
        }

        private void SaveToolStripMenuItem_Click(object? sender, EventArgs? e)
        {
            TabManager.ActiveTranslationManager.SaveFile();
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
    }
}
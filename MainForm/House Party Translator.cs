using HousePartyTranslator.Managers;
using HousePartyTranslator.StoryExplorerForm;
using System;
using System.Threading;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    /// <summary>
    /// The main class which handles the UI for the House Party Translator Window
    /// </summary>
    public partial class Fenster : Form
    {
        private readonly System.Timers.Timer PresenceTimer = new System.Timers.Timer(2000);
        private DiscordPresenceManager PresenceManager;
        private StoryExplorer SExplorer;
        private readonly CancellationTokenSource CancelTokens = new CancellationTokenSource();

        /// <summary>
        /// Constructor for the main window of the translator, starts all other components in the correct order
        /// </summary>
        public Fenster()
        {
            //custom exception handlers to handle mysql exceptions
            AppDomain.CurrentDomain.UnhandledException += FensterUnhandledExceptionHandler;
            Application.ThreadException += ThreadExceptionHandler;

            //check for update and replace if we want one
            SoftwareVersionManager.ReplaceFileIfNew();

            //init all form components
            InitializeComponent();
        }

        /// <summary>
        /// Instance of the Story Explorer, but the owner is checked so only the Storyexplorer class itself can instantiate it.
        /// </summary>
        public StoryExplorer Explorer
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
                        throw new UnauthorizedAccessException("You must only write to this object from within the Explorer class");
                    }
                }
            }
        }

        public ToolStripComboBox LanguageBox { get { return languageToolStripComboBox; } }

        public ToolStripTextBox SearchBox { get { return searchToolStripTextBox; } }

        public ToolStripTextBox ReplaceBox { get { return ToolStripMenuReplaceBox; } }

        public ToolStripMenuItem ReplaceButton { get { return toolStripReplaceButton; } }

        public ToolStripMenuItem FileToolStripMenuItem { get { return fileToolStripMenuItem; } }

        public void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            KeypressManager.ApprovedButtonChanged();
        }

        public void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            KeypressManager.CheckItemChanged();
        }

        public void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeypressManager.SelectedItemChanged();
        }

        public void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            KeypressManager.TranslationTextChanged();
        }

        /// <summary>
        /// Override to intercept the Keystrokes windows sends us.
        /// </summary>
        /// <param name="msg">The message containing relevant info</param>
        /// <param name="keyData">List of all Keys that were pressed in this event</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (KeypressManager.MainKeyPressHandler(ref msg, keyData, PresenceManager, this, CancelTokens))
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void CloseTab_Click(object sender, MouseEventArgs e)
        {
            TabManager.CloseTab(sender, e);
        }

        private async void CustomStoryExplorerStripMenuItem_Click(object sender, EventArgs e)
        {
            Explorer = await KeypressManager.CreateStoryExplorer(false, this, CancelTokens);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            //prevent discord from getting angry
            if (PresenceManager != null) PresenceManager.DeInitialize();

            RecentsManager.SaveRecents();

            CancelTokens.Cancel();

            CancelTokens.Dispose();

            //show save unsaved changes dialog
            if (TabManager.ActiveTranslationManager != null) TabManager.ActiveTranslationManager.ShowAutoSaveDialog();
        }

        private void FensterUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.LogEvent(e.ExceptionObject.ToString());
            try //casting the object into an exception
            {
                TranslationManager.DisplayExceptionMessage(((Exception)e.ExceptionObject).Message);
            }
            catch //dirty dirty me... can't cast into an exception :)
            {
                TranslationManager.DisplayExceptionMessage(e.ExceptionObject.ToString());
            }
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            LogManager.LogEvent("Application started! hi there :D");

            //get translationmanager back
            TranslationManager translationManager = TabManager.Initialize(tabPage1);
            translationManager.SetLanguage();
            translationManager.SetMainForm(this);

            //initialize before password check so the saving doesnt break
            RecentsManager.Initialize();

            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);

            //open most recent after db is initialized
            RecentsManager.OpenMostRecent();

            PresenceManager = new DiscordPresenceManager();

            //start timer to update presence
            PresenceTimer.Elapsed += (sender_, args) => { PresenceManager.Update(); };
            PresenceTimer.Start();

            PresenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);

        }

        private void LanguageToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeypressManager.SelectedLanguageChanged();
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeypressManager.SelectedTabChanged(PresenceManager);
        }

        private void OpenInNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeypressManager.OpenNewTab(PresenceManager);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeypressManager.OpenNew(PresenceManager);
        }

        private void OpenAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeypressManager.OpenAll(PresenceManager);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveFileAs();
        }

        private void SaveCommentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveCurrentComment();
        }

        private void SaveCurrentStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveCurrentString();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveFile();
        }

        private void SaveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.SaveAllTabs();
        }

        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            TabManager.Search();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeypressManager.ShowSettings();
        }

        private async void StoryExplorerStripMenuItem_Click(object sender, EventArgs e)
        {
            Explorer = await KeypressManager.CreateStoryExplorer(true, this, CancelTokens);
        }

        private void ThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogManager.LogEvent(e.Exception.ToString());
            TranslationManager.DisplayExceptionMessage(e.Exception.Message);
        }

        private void ToolStripMenuReplaceBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void ToolStripReplaceButton_Click(object sender, EventArgs e)
        {
            TabManager.Replace();
        }

        public void TranslateThis_Click(object sender, EventArgs e)
        {
            KeypressManager.AutoTranslate();
        }

        public void CopyIdContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyId();
        }

        public void CopyAllContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyAll();
        }

        public void CopyFileNameContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyFileName();
        }

        public void CopyStoryNameContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyStoryName();
        }

        public void CopyAsOutputContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyAsOutput();
        }

        public void CopyTranslationContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyTranslation();
        }

        public void CopyTemplateContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyTemplate();
        }

        public void OpeningContextMenu(object sender, MouseEventArgs e)
        {
            KeypressManager.OpenContextMenu(ListContextMenu, e);
        }
    }
}

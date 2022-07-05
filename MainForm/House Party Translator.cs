using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using HousePartyTranslator.StoryExplorerForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
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
        private SettingsForm.SettingsForm settings;
        private StoryExplorer SExplorer;

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

        public ToolStripComboBox LanguageBox { get { return languageToolStripComboBox; } }

        public ToolStripTextBox SearchBox { get { return searchToolStripTextBox; } }

        public ToolStripTextBox ReplaceBox { get { return ToolStripMenuReplaceBox; } }

        public ToolStripMenuItem ReplaceButton { get { return toolStripReplaceButton; } }

        public ToolStripMenuItem FileToolStripMenuItem { get { return fileToolStripMenuItem; } }

        public void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.ApprovedButtonHandler();
        }

        public void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TabManager.ActiveTranslationManager.ApproveIfPossible(false);
        }

        public void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.PopulateTextBoxes();
        }

        public void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.UpdateTranslationString();
        }

        /// <summary>
        /// Override to intercept the Keystrokes windows sends us.
        /// </summary>
        /// <param name="msg">The message containing relevant info</param>
        /// <param name="keyData">List of all Keys that were pressed in this event</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (TabManager.ActiveTranslationManager.MainFormKeyPressHandler(ref msg, keyData))
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

        private void CreateStoryExplorer(bool autoOpen)
        {
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            Cursor = Cursors.WaitCursor;
            bool isStory = translationManager.StoryName.ToLowerInvariant() == translationManager.FileName.ToLowerInvariant();
            Explorer = new StoryExplorer(isStory, autoOpen, translationManager.FileName, translationManager.StoryName, this);
            if (!Explorer.IsDisposed) Explorer.Show();
            Cursor = Cursors.Default;
        }

        private void CustomStoryExplorerStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateStoryExplorer(false);
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

            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);

            PresenceManager = new DiscordPresenceManager();
            RecentsManager.Initialize();

            //start timer to update presence
            PresenceTimer.Elapsed += (sender_, args) => { PresenceManager.Update(); };
            PresenceTimer.Start();

            RecentsManager.OpenMostRecent();

            PresenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        private void LanguageToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SetLanguage();
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabManager.OnSwitchTabs();
            //update tabs
            if (TabManager.ActiveTranslationManager != null) PresenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        private void OpenInNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.OpenNewTab();

            //update presence and recents
            PresenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get currently active translationmanager
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.LoadFileIntoProgram();
            translationManager.SetLanguage();
            //update tab name
            if (translationManager.FileName.Length > 0) TabManager.TabControl.SelectedTab.Text = translationManager.FileName;

            //update presence and recents
            PresenceManager.Update(translationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
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

        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            TabManager.Search();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings = new SettingsForm.SettingsForm();
            if (!settings.IsDisposed) settings.Show();
        }

        private void StoryExplorerStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateStoryExplorer(true);
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

        private void TranslateThis_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.ReplaceTranslationTranslatedTask(TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex);
        }

        private void CopyIdContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyId();
        }

        private void CopyAllContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyAll();
        }

        private void CopyFileNameContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyFileName();
        }

        private void CopyStoryNameContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyStoryName();
        }

        private void CopyAsOutputContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyAsOutput();
        }

        private void CopyTranslationContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyTranslation();
        }

        private void CopyTemplateContextMenuButton_Click(object sender, EventArgs e)
        {
            TabManager.CopyTemplate();
        }

        private void OpeningContextMenu(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = TabManager.ActiveProperties.CheckListBoxLeft.IndexFromPoint(e.Location);
                if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex <= 0) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = 0;
                ListContextMenu.Show();
            }
        }
    }
}

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
        private RecentsManager RecentsManager;
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

        public void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.ApprovedButtonHandler(TabManager.ActiveProperties);
        }

        public void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TabManager.ActiveTranslationManager.ApproveIfPossible(TabManager.ActiveProperties, false);
        }

        public void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.PopulateTextBoxes(TabManager.ActiveProperties);
        }

        public void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.UpdateTranslationString(TabManager.ActiveProperties);
        }

        /// <summary>
        /// Override to intercept the Keystrokes windows sends us.
        /// </summary>
        /// <param name="msg">The message containing relevant info</param>
        /// <param name="keyData">List of all Keys that were pressed in this event</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (TabManager.ActiveTranslationManager.MainFormKeyPressHandler(ref msg, keyData, TabManager.ActiveProperties))
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

        private void Fenster_FormClosing(object sender, FormClosingEventArgs e)
        {
            //prevent discord from getting angry
            PresenceManager.DeInitialize();

            RecentsManager.SaveRecents();

            //save settings
            Properties.Settings.Default.Save();

            //show save unsaved changes dialog
            TabManager.ActiveTranslationManager.ShowAutoSaveDialog(TabManager.ActiveProperties);
        }

        private void FensterUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.LogEvent(e.ExceptionObject.ToString());
            try //casting the object into an exception
            {
                TranslationManager.DisplayExceptionMessage(((Exception)e.ExceptionObject).Message, TabManager.ActiveTranslationManager);
            }
            catch //dirty dirty me... can't cast into an exception :)
            {
                TranslationManager.DisplayExceptionMessage(e.ExceptionObject.ToString(), TabManager.ActiveTranslationManager);
            }
        }

        private void FormShown(object sender, EventArgs e)
        {
            LogManager.LogEvent("Application started! hi there :D");

            TabManager.Initialize(tabPage1);

            //get translationmanager back
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.SetLanguage(TabManager.ActiveProperties);
            translationManager.SetMainForm(this);

            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);

            PresenceManager = new DiscordPresenceManager();
            RecentsManager = new RecentsManager(TabManager.ActiveProperties);

            //start timer to update presence
            PresenceTimer.Elapsed += (sender_, args) => { PresenceManager.Update(); };
            PresenceTimer.Start();

            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);

            if (Properties.Settings.Default.autoLoadRecent)
            {
                if (RecentsManager.GetRecents().Length > 0)
                {
                    translationManager.LoadFileIntoProgram(TabManager.ActiveProperties, RecentsManager.GetRecents()[0].Text);
                    if (translationManager.FileName.Length > 0) tabPage1.Text = translationManager.FileName;
                }
            }
        }

        private void LanguageToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SetLanguage(TabManager.ActiveProperties);
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set search term to the one from the respective TranslationManager
            if (TabManager.ActiveTranslationManager != null) searchToolStripTextBox.Text = TabManager.ActiveTranslationManager.SearchQuery;
        }

        private void OpenInNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.OpenNewTab();

            //update presence and recents
            RecentsManager.SetMostRecent(TabManager.ActiveTranslationManager.SourceFilePath);
            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);
            PresenceManager.SetCharacterToShow(TabManager.ActiveTranslationManager.FileName);
            PresenceManager.SetStory(TabManager.ActiveTranslationManager.StoryName);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get currently active translationmanager
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.LoadFileIntoProgram(TabManager.ActiveProperties);
            translationManager.SetLanguage(TabManager.ActiveProperties);
            //update tab name
            if (translationManager.FileName.Length > 0) TabManager.TabControl.SelectedTab.Text = translationManager.FileName;

            //update presence and recents
            RecentsManager.SetMostRecent(translationManager.SourceFilePath);
            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);
            PresenceManager.SetCharacterToShow(translationManager.FileName);
            PresenceManager.SetStory(translationManager.StoryName);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveFileAs(TabManager.ActiveProperties);
        }

        private void SaveCommentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveCurrentComment(TabManager.ActiveProperties);
        }

        private void SaveCurrentStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveCurrentString(TabManager.ActiveProperties);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.SaveFile(TabManager.ActiveProperties);
        }

        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            TabManager.ActiveTranslationManager.Search(TabManager.ActiveProperties);
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
            TranslationManager.DisplayExceptionMessage(e.Exception.Message, TabManager.ActiveTranslationManager);
        }
    }
}

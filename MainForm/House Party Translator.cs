using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using HousePartyTranslator.StoryExplorerForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    /// <summary>
    /// The main class which handles the UI for the House Party Translator Window
    /// </summary>
    public partial class Fenster : Form
    {
        private static readonly Dictionary<TabPage, PropertyHelper> properties = new Dictionary<TabPage, PropertyHelper>();
        private static readonly Dictionary<TabPage, TranslationManager> translationManagers = new Dictionary<TabPage, TranslationManager>();
        private DiscordPresenceManager PresenceManager;
        private readonly System.Timers.Timer PresenceTimer = new System.Timers.Timer(2000);
        private RecentsManager RecentsManager;
        private static TabControl tabControl;
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

        /// <summary>
        /// Method returning a Propertyhelper containing all relevant UI elements.
        /// </summary>
        /// <returns>the relevant Propertyhelper</returns>
        public static PropertyHelper ActiveProperties()
        {
            if (properties.TryGetValue(tabControl.SelectedTab, out PropertyHelper property))
            {
                return property;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the active translation manager for the currently selected tab
        /// </summary>
        /// <returns>Current translationmanager</returns>
        public static TranslationManager ActiveTranslationManager()
        {
            if (translationManagers.TryGetValue(tabControl.SelectedTab, out TranslationManager translationManager))
            {
                return translationManager;
            }
            else
            {
                return null;
            }
        }

        public void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.ApprovedButtonHandler(ActiveProperties());
        }

        public void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.ApproveIfPossible(ActiveProperties(), false);
        }

        public void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.PopulateTextBoxes(ActiveProperties());
        }

        public void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.UpdateTranslationString(ActiveProperties());
        }

        /// <summary>
        /// Override to intercept the Keystrokes windows sends us.
        /// </summary>
        /// <param name="msg">The message containing relevant info</param>
        /// <param name="keyData">List of all Keys that were pressed in this event</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            if (translationManager.MainFormKeyPressHandler(ref msg, keyData, ActiveProperties()))
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private static PropertyHelper CreateActivePropertyHelper()
        {
            return new PropertyHelper(
                (CheckBox)tabControl.SelectedTab.Controls.Find("ApprovedBox", true)[0],
                (ColouredCheckedListBox)tabControl.SelectedTab.Controls.Find("CheckListBoxLeft", true)[0],
                ((Fenster)ActiveForm).languageToolStripComboBox,
                (Label)tabControl.SelectedTab.Controls.Find("WordsTranslated", true)[0],
                (Label)tabControl.SelectedTab.Controls.Find("CharacterCountLabel", true)[0],
                (Label)tabControl.SelectedTab.Controls.Find("SelectedFile", true)[0],
                (NoAnimationBar)tabControl.SelectedTab.Controls.Find("ProgressbarTranslated", true)[0],
                (TextBox)tabControl.SelectedTab.Controls.Find("CommentTextBox", true)[0],
                ((Fenster)ActiveForm).searchToolStripTextBox,
                (TextBox)tabControl.SelectedTab.Controls.Find("EnglishTextBox", true)[0],
                (TextBox)tabControl.SelectedTab.Controls.Find("TranslatedTextBox", true)[0]
                );
        }

        private void CreateStoryExplorer(bool autoOpen)
        {
            TranslationManager translationManager = ActiveTranslationManager();
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
            TranslationManager translationManager = ActiveTranslationManager();
            //prevent discord from getting angry
            PresenceManager.DeInitialize();

            RecentsManager.SaveRecents();

            //save settings
            Properties.Settings.Default.Save();

            //show save unsaved changes dialog
            translationManager.ShowAutoSaveDialog(ActiveProperties());
        }

        private void FensterUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.LogEvent(e.ExceptionObject.ToString());
            try //casting the object into an exception
            {
                TranslationManager.DisplayExceptionMessage(((Exception)e.ExceptionObject).Message, ActiveTranslationManager());
            }
            catch //dirty dirty me... can't cast into an exception :)
            {
                TranslationManager.DisplayExceptionMessage(e.ExceptionObject.ToString(), ActiveTranslationManager());
            }
        }

        private void LanguageToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.SetLanguage(ActiveProperties());
        }

        private void OpenInNewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create new support objects
            TranslationManager translationManager = new TranslationManager();
            TabPage newTab = Utils.CreateNewTab(translationManagers.Count + 1);
            //Add tab to form control
            tabControl.TabPages.Add(newTab);
            //select new tab
            tabControl.SelectedTab = newTab;
            //create support dict
            translationManagers.Add(newTab, translationManager);
            properties.Add(newTab, CreateActivePropertyHelper());

            //call startup for new translationmanager
            translationManager.SetLanguage(ActiveProperties());
            translationManager.LoadFileIntoProgram(ActiveProperties());

            //update tab name
            if (translationManager.FileName.Length > 0) newTab.Text = translationManager.FileName;

            //update presence and recents
            RecentsManager.SetMostRecent(translationManager.SourceFilePath);
            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);
            PresenceManager.SetCharacterToShow(translationManager.FileName);
            PresenceManager.SetStory(translationManager.StoryName);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            //get currently active translationmanager
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.LoadFileIntoProgram(ActiveProperties());
            translationManager.SetLanguage(ActiveProperties());
            //update tab name
            if (translationManager.FileName.Length > 0) tabControl.SelectedTab.Text = translationManager.FileName;

            //update presence and recents
            RecentsManager.SetMostRecent(translationManager.SourceFilePath);
            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);
            PresenceManager.SetCharacterToShow(translationManager.FileName);
            PresenceManager.SetStory(translationManager.StoryName);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.SaveFileAs(ActiveProperties());
        }

        private void SaveCommentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.SaveCurrentComment(ActiveProperties());
        }

        private void SaveCurrentStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.SaveCurrentString(ActiveProperties());
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.SaveFile(ActiveProperties());
        }

        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.Search(ActiveProperties());
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
            TranslationManager.DisplayExceptionMessage(e.Exception.Message, ActiveTranslationManager());
        }

        private void FormShown(object sender, EventArgs e)
        {
            LogManager.LogEvent("Application started! hi there :D");

            //get tabcontrol as a statc reference;
            tabControl = (TabControl)ActiveForm.Controls.Find("MainTabControl", true)[0];

            //create new translationmanager to use with the tab open right now
            translationManagers.Add(tabPage1, new TranslationManager());
            properties.Add(tabPage1, CreateActivePropertyHelper());

            //get translationmanager back
            TranslationManager translationManager = ActiveTranslationManager();
            translationManager.SetLanguage(ActiveProperties());
            translationManager.SetMainForm(this);

            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);

            PresenceManager = new DiscordPresenceManager();
            RecentsManager = new RecentsManager(ActiveProperties());

            //start timer to update presence
            PresenceTimer.Elapsed += (sender_, args) => { PresenceManager.Update(); };
            PresenceTimer.Start();

            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);

            if (Properties.Settings.Default.autoLoadRecent)
            {
                if (RecentsManager.GetRecents().Length > 0)
                {
                    translationManager.LoadFileIntoProgram(ActiveProperties(), RecentsManager.GetRecents()[0].Text);
                    if (translationManager.FileName.Length > 0) tabPage1.Text = translationManager.FileName;
                }
            }
        }

        private void CloseTab_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tabControl.TabCount > 1)
            {
                for (int ix = 0; ix < tabControl.TabCount; ++ix)
                {
                    if (tabControl.GetTabRect(ix).Contains(e.Location))
                    {
                        //remove manager for the tab, save first
                        ActiveTranslationManager().SaveFile(ActiveProperties());
                        translationManagers.Remove(tabControl.TabPages[ix]);
                        properties.Remove(tabControl.TabPages[ix]);

                        tabControl.TabPages[ix].Dispose();
                        break;
                    }
                }
            }
        }
    }
}

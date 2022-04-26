﻿using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using HousePartyTranslator.StoryExplorerForm;
using System;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public partial class Fenster : Form
    {
        public readonly PropertyHelper MainProperties;
        private StoryExplorer SExplorer;
        private readonly DiscordPresenceManager PresenceManager;
        private readonly RecentsManager RecentsManager;
        private readonly System.Timers.Timer PresenceTimer = new System.Timers.Timer(2000);
        private SettingsForm.SettingsForm settings;

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

        public Fenster()
        {
            //custom exception handlers to handle mysql exceptions
            AppDomain.CurrentDomain.UnhandledException += FensterUnhandledExceptionHandler;
            Application.ThreadException += ThreadExceptionHandler;

            //init all form components
            InitializeComponent();

            //create propertyhelper
            MainProperties = new PropertyHelper(
                ApprovedBox,
                CheckListBoxLeft,
                languageToolStripComboBox,
                WordsTranslated,
                CharacterCountLabel,
                SelectedFile,
                ProgressbarTranslated,
                CommentTextBox,
                searchToolStripTextBox,
                EnglishTextBox,
                TranslatedTextBox
                );

            TranslationManager.main.SetLanguage(MainProperties);
            TranslationManager.main.SetMainForm(this);
            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);

            PresenceManager = new DiscordPresenceManager();
            RecentsManager = new RecentsManager(MainProperties);

            //start timer to update presence
            PresenceTimer.Elapsed += (sender, args) => { PresenceManager.Update(); };
            PresenceTimer.Start();

            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);

            if (Properties.Settings.Default.autoLoadRecent)
            {
                if(RecentsManager.GetRecents().Length > 0) TranslationManager.main.LoadFileIntoProgram(MainProperties, RecentsManager.GetRecents()[0].Text);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (TranslationManager.main.MainFormKeyPressHandler(ref msg, keyData, MainProperties))
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void ThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogManager.LogEvent(e.Exception.ToString());
            TranslationManager.DisplayExceptionMessage(e.Exception.Message);
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

        private void Window_Load(object sender, EventArgs e)
        {
            LogManager.LogEvent("Application started! hi there :D");
        }

        private void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.UpdateTranslationString(MainProperties);
        }

        private void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.PopulateTextBoxes(MainProperties);
        }

        private void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TranslationManager.main.ApproveIfPossible(MainProperties, false);
        }

        private void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            TranslationManager.ApprovedButtonHandler(MainProperties);
        }

        private void SaveCurrentStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveCurrentString(MainProperties);
        }

        private void SaveCommentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveCurrentComment(MainProperties);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager.main.LoadFileIntoProgram(MainProperties);
            TranslationManager.main.SetLanguage(MainProperties);

            //update presence
            RecentsManager.SetMostRecent(TranslationManager.main.SourceFilePath);
            RecentsManager.UpdateMenuItems(fileToolStripMenuItem.DropDownItems);
            PresenceManager.SetCharacterToShow(TranslationManager.main.FileName);
            PresenceManager.SetStory(TranslationManager.main.StoryName);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveFile(MainProperties);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveFileAs(MainProperties);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LanguageToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.SetLanguage(MainProperties);
        }

        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.Search(MainProperties);
        }

        private void Fenster_FormClosing(object sender, FormClosingEventArgs e)
        {
            //prevent discord from getting angry
            PresenceManager.DeInitialize();

            RecentsManager.SaveRecents();
            //save settings
            Properties.Settings.Default.Save();

            if (MessageBox.Show("You may have unsaved changes. Do you want to save all changes?", "Save changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                TranslationManager.main.SaveFile(MainProperties);
            }
        }

        private void StoryExplorerStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreateStoryExplorer(true);
        }

        private void CustomStoryExplorerStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateStoryExplorer(false);
        }

        private void CreateStoryExplorer(bool autoOpen)
        {
            Cursor = Cursors.WaitCursor;
            bool isStory = TranslationManager.main.StoryName.ToLowerInvariant() == TranslationManager.main.FileName.ToLowerInvariant();
            Explorer = new StoryExplorer(isStory, autoOpen, TranslationManager.main.FileName, TranslationManager.main.StoryName, this);
            if (!Explorer.IsDisposed) Explorer.Show();
            Cursor = Cursors.Default;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings = new SettingsForm.SettingsForm();
            if (!settings.IsDisposed) settings.Show();
        }
    }
}

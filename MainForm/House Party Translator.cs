using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using System;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public partial class Fenster : Form
    {
        private readonly PropertyHelper MainProperties;

        public Fenster()
        {
            InitializeComponent();
            //initialize and open db connection (should not take too long)
            SettingsManager.LoadSettings();

            //create propertyhelper
            MainProperties = new PropertyHelper(
                ApprovedBox,
                CheckListBoxLeft,
                LanguageBox,
                WordsTranslated,
                CharacterCountLabel,
                SelectedFile,
                ProgressbarTranslated,
                CommentTextBox,
                SearchBox,
                EnglishTextBox,
                TranslatedTextBox
                );

            TranslationManager.main.SetLanguage(MainProperties);
            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);

            //custom exception handlers to handle mysql exceptions
            AppDomain.CurrentDomain.UnhandledException += FensterUnhandledExceptionHandler;
            Application.ThreadException += ThreadExceptionHandler;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (TranslationManager.main.HandleKeyPressMainForm(ref msg, keyData, MainProperties))
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

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.LoadFileIntoProgram(MainProperties);
            TranslationManager.main.SetLanguage(MainProperties);
        }

        private void SaveFileLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.SaveFile(MainProperties);
        }

        private void SaveFileAsLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.SaveFileAs(MainProperties);
        }

        private void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.PopulateTextBoxes(MainProperties);
        }

        private void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TranslationManager.main.ApproveIfPossible(MainProperties, false);
        }

        private void LanguageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.SetLanguage(MainProperties);
        }

        private void SaveCurrentString_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveCurrentString(MainProperties);
        }

        private void SaveCommentsButton_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveCurrentComment(MainProperties);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.Search(MainProperties);
        }

        private void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            TranslationManager.ApprovedButtonHandler(this, MainProperties);
        }
    }
}

using System;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public partial class Fenster : Form
    {

        public Fenster()
        {
            InitializeComponent();
            //initialize and open db connection (should not take too long)
            SettingsManager.LoadSettings();
            TranslationManager.main.SetLanguage(LanguageBox);
            //Settings have to be loaded before the Database can be connected with
            DataBaseManager.InitializeDB(this);
            //save last string edited
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        //doesnt really work because the form is already gone when this is called :(
        private void OnProcessExit(object sender, EventArgs e)
        {
            CheckListBoxLeft.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.UpdateTranslationString(TranslatedTextBox, EnglishTextBox, CheckListBoxLeft, CharacterCountLabel);
        }

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.LoadFileIntoProgram(CheckListBoxLeft, SelectedFile);
            TranslationManager.main.SetLanguage(LanguageBox);
        }

        private void SaveFileLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.SaveFile(CheckListBoxLeft);
        }

        private void SaveFileAsLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.SaveFileAs(CheckListBoxLeft);
        }

        private void ProgressbarTranslated_Click(object sender, EventArgs e)
        {

        }

        private void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.PopulateTextBoxes(CheckListBoxLeft, EnglishTextBox, TranslatedTextBox, CommentTextBox, CharacterCountLabel, WordsTranslated);
        }

        private void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TranslationManager.main.ApproveIfPossible(CheckListBoxLeft, WordsTranslated);
        }

        private void LanguageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.SetLanguage(LanguageBox);
        }

        private void SaveCurrentString_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveCurrentString(CheckListBoxLeft);
        }

        private void SaveCommentsButton_Click(object sender, EventArgs e)
        {
            TranslationManager.main.SaveCurrentComment(CheckListBoxLeft, CommentTextBox);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.Search(CheckListBoxLeft, SearchBox);
        }
    }
}

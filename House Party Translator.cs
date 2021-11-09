using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public partial class Fenster : Form
    {

        public Fenster()
        {
            InitializeComponent();
            //initialize and open db connection (should not take too long)
            ProofreadDB.InitializeDB(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.UpdateTranslationString(TranslatedTextBox);
        }

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            string filePath = TranslationManager.SelectFileFromSystem();
            if (filePath != "")
            {
                TranslationManager.main.LoadFileIntoProgram(filePath, CheckListBoxLeft);
            }
        }

        private void SaveFileLeftClick(object sender, EventArgs e)
        {

        }

        private void SaveFileAsLeftClick(object sender, EventArgs e)
        {

        }

        private void ProgressbarTranslated_Click(object sender, EventArgs e)
        {

        }

        private void OpenFileDialogLeft_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void SaveFileAsDialogLeft_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.PopulateTextBoxes(CheckListBoxLeft, EnglishTextBox, TranslatedTextBox);
        }

        private void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TranslationManager.main.ApproveIfPossible(CheckListBoxLeft);
        }
    }
}

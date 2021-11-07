using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public partial class Fenster : Form
    {

        public Fenster()
        {
            InitializeComponent();
            //initialize and open db connection (should not take too long)
            Cursor = Cursors.WaitCursor;
            ProofreadDB.InitializeDB();
            Cursor = Cursors.Default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TextBoxLeft_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = TranslatedTextBox.GetFirstCharIndexOfCurrentLine();
            int CurrentLine = TranslatedTextBox.GetLineFromCharIndex(selectionStart);
            Console.WriteLine(CurrentLine.ToString());

            int selectionEnd = TranslatedTextBox.GetFirstCharIndexFromLine(CurrentLine + 1) - 1;
            string approvedString = "";
            if (selectionEnd - selectionStart > 0) { approvedString = TranslatedTextBox.Text.Substring(selectionStart, selectionEnd); }
            else { approvedString = TranslatedTextBox.Text.Substring(selectionStart, TranslatedTextBox.Text.Length); }

            if (approvedString.Contains('|'))
            {
                string ID = approvedString.Split('|')[0];
            }
        }

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            string filePath = SelectFileFromSystem();
            if (filePath != "")
            {
                Console.WriteLine("Selected path is " + filePath);
                TranslationManager.main.SourceFilePath = filePath;
                TranslationManager.main.TemplateFileString = System.IO.File.ReadAllText(filePath);
                TranslationManager.main.TranslationData.Clear();

                foreach (string line in System.IO.File.ReadAllLines(filePath))
                {
                    if (line.Contains('|'))
                    {
                        string[] Splitted = line.Split('|');
                        TranslationManager.main.TranslationData.Add(new LineData(Splitted[0], Splitted[1]));
                    }
                }

                CheckListBoxLeft.Items.Clear();
                foreach (LineData lineD in TranslationManager.main.TranslationData)
                {
                    CheckListBoxLeft.Items.Add(lineD.ID, false);
                }
            }
        }

        private string SelectFileFromSystem()
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog
            {
                Title = "Choose a template file for translation",
                Filter = "Text files (*.txt)|*.txt",
                InitialDirectory = @"C:\Users\%USER%\Documents"
            };

            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                return selectFileDialog.FileName;
            }
            return "";
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
            EnglishTextBox.Text = TranslationManager.main.TranslationData[CheckListBoxLeft.SelectedIndex].english;
        }

        private void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (CheckListBoxLeft.GetItemChecked(CheckListBoxLeft.SelectedIndex))
            {
                string ID = TranslationManager.main.TranslationData[CheckListBoxLeft.SelectedIndex].ID;
                string Story = TranslationManager.main.SourceFilePath.Split('\\')[TranslationManager.main.SourceFilePath.Split('\\').Length - 2];
                string FileName = TranslationManager.main.FileName;
                ProofreadDB.SetStringAccepted(ID, FileName, Story);
            }
        }

        /*private void ApproveTranslationButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ApproveTranslationButton.Checked)
            {
                int CurrentLine = TranslatedTextBox.GetLineFromCharIndex(TranslatedTextBox.GetFirstCharIndexOfCurrentLine());
                int selectionStart = TranslatedTextBox.GetFirstCharIndexOfCurrentLine();
                int selectionEnd = TranslatedTextBox.GetFirstCharIndexFromLine(CurrentLine + 1) - 1;

                TranslatedTextBox.SelectionStart = selectionStart;
                if (selectionEnd - selectionStart > 0) { TranslatedTextBox.SelectionLength = selectionEnd - selectionStart; }
                else { TranslatedTextBox.SelectionLength = TranslatedTextBox.Text.Length - 1 - selectionStart; }

                string approvedString = TranslatedTextBox.SelectedText;
                if (approvedString.Contains('|'))
                {
                    string ID = approvedString.Split('|')[0];
                    if (!ProofreadDB.SetStringAccepted(ID, TranslationManager.main.FileName, "OS", ""))
                    {
                        Console.WriteLine($"Could not approve string {ID}");
                    }
                }
            }
            else
            {

            }
        }
        */
    }
}

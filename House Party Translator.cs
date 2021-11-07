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
            int selectionStart = TextBoxRight.GetFirstCharIndexOfCurrentLine();
            int CurrentLine = TextBoxRight.GetLineFromCharIndex(selectionStart);

            //move left box to same line
            TextBoxLeft.SelectionStart = TextBoxRight.GetFirstCharIndexFromLine(CurrentLine);
            TextBoxLeft.ScrollToCaret();

            //get end of line
            int selectionEnd = TextBoxRight.GetFirstCharIndexFromLine(CurrentLine + 1) - 1;
            string approvedString = "";
            //extract string of current line
            if (selectionEnd - selectionStart > 0) { approvedString = TextBoxRight.Text.Substring(selectionStart, selectionEnd); }
            else { approvedString = TextBoxRight.Text.Substring(selectionStart, TextBoxRight.Text.Length); }

            //extract id
            if (approvedString.Contains('|'))
            {
                string ID = approvedString.Split('|')[0];
                ApproveTranslationButton.Text = $"Approve string {ID}";
            }
        }

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            string filePath = SelectFileFromSystem(true);
            if (filePath != "")
            {
                Console.WriteLine("Selected path is " + filePath);
                TranslationManager.main.SourceFilePath = filePath;
                TranslationManager.main.TemplateFileString = System.IO.File.ReadAllText(filePath);
                TextBoxLeft.Text = TranslationManager.main.TemplateFileString;
            }
        }

        private string SelectFileFromSystem(bool isEnglishFile)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
            string title = isEnglishFile ? "Choose a source file for translation" : "Choose a target file for translation";
            selectFileDialog.Title = title;
            selectFileDialog.Filter = "Text files (*.txt)|*.txt";
            selectFileDialog.InitialDirectory = @"C:\Users\%USER%\Documents";

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

        private void SelectFileRightClick(object sender, EventArgs e)
        {
            string filePath = SelectFileFromSystem(false);
            if (filePath != "")
            {
                //Load file here
                TranslationManager.main.SourceFilePath = filePath;
                Console.WriteLine("Selected path is " + filePath);
                TranslationManager.main.TranslationFileString = System.IO.File.ReadAllText(filePath);
                TextBoxRight.Text = TranslationManager.main.TranslationFileString;
            }
        }

        private void SaveFileRightClick(object sender, EventArgs e)
        {

        }

        private void SaveFileAsRightClick(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ProgressbarTranslated_Click(object sender, EventArgs e)
        {

        }

        private void OpenFileDialogLeft_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void OpenFileDialogRight_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void SaveFileAsDialogLeft_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void SafeFileAsDialogRight_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void TextBoxRight_Click(object sender, EventArgs e)
        {

        }

        private void ApproveTranslationButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ApproveTranslationButton.Checked)
            {
                int CurrentLine = TextBoxRight.GetLineFromCharIndex(TextBoxRight.GetFirstCharIndexOfCurrentLine());
                int selectionStart = TextBoxRight.GetFirstCharIndexOfCurrentLine();
                int selectionEnd = TextBoxRight.GetFirstCharIndexFromLine(CurrentLine + 1) - 1;

                TextBoxRight.SelectionStart = selectionStart;
                if (selectionEnd - selectionStart > 0) { TextBoxRight.SelectionLength = selectionEnd - selectionStart; }
                else { TextBoxRight.SelectionLength = TextBoxRight.Text.Length - 1 - selectionStart; }

                string approvedString = TextBoxRight.SelectedText;
                if (approvedString.Contains('|'))
                {
                    string ID = approvedString.Split('|')[0];
                    if (!ProofreadDB.SetStringAccepted(ID, TranslationManager.main.FileName, "OS", "", approvedString.Split('|')[1]))
                    {
                        Console.WriteLine($"Could not approve string {ID}");
                    }
                }
            }
            else
            {

            }
        }
    }
}

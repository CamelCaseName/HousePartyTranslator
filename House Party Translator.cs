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

        }

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            string filePath = SelectFileFromSystem(true);
            if (filePath != "") 
            {
                //Load file here
                Console.WriteLine("Selected path is " + filePath);
                TranslationManager.main.SourceFilePath = filePath;
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
    }
}

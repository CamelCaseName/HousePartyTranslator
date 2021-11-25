﻿using HousePartyTranslator.Managers;
using System;
using System.Windows.Forms;
using System.Drawing;

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

            //custom exception handlers to handle mysql exceptions
            AppDomain.CurrentDomain.UnhandledException += FensterUnhandledExceptionHandler;
            Application.ThreadException += ThreadExceptionHandler;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (TranslationManager.main.HandleKeyPressMainForm(ref msg, keyData, SearchBox, TranslatedTextBox, CheckListBoxLeft))
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

        private void Form1_Load(object sender, EventArgs e)
        {
            LogManager.LogEvent("Application started! hi there :D");
        }

        private void TextBoxRight_TextChanged(object sender, EventArgs e)
        {
            TranslationManager.main.UpdateTranslationString(TranslatedTextBox, EnglishTextBox, CheckListBoxLeft, CharacterCountLabel);
        }

        private void SelectFileLeftClick(object sender, EventArgs e)
        {
            TranslationManager.main.LoadFileIntoProgram(CheckListBoxLeft, SelectedFile, WordsTranslated, ProgressbarTranslated);
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

        private void CheckListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            TranslationManager.main.PopulateTextBoxes(
                CheckListBoxLeft,
                EnglishTextBox,
                TranslatedTextBox,
                CommentTextBox,
                CharacterCountLabel,
                WordsTranslated,
                ProgressbarTranslated,
                ApprovedBox);
        }

        private void CheckListBoxLeft_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TranslationManager.main.ApproveIfPossible(CheckListBoxLeft, WordsTranslated, ProgressbarTranslated, false, ApprovedBox);
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

        private void ApprovedBox_CheckedChanged(object sender, EventArgs e)
        {
            //get title bar height
            Rectangle ScreenRectangle = RectangleToScreen(ClientRectangle);
            int TitleHeight = ScreenRectangle.Top - Top;

            //check whether cursor is on approved button or not
            int deltaX = Cursor.Position.X - ApprovedBox.Location.X - Location.X;
            int deltaY = Cursor.Position.Y - ApprovedBox.Location.Y - Location.Y - TitleHeight;
            bool isInX = 0 <= deltaX && deltaX <= ApprovedBox.Width;
            bool isInY = 0 <= deltaY && deltaY <= ApprovedBox.Height;

            //actually do the checking state change
            if (isInX && isInY)
            {
                int Index = CheckListBoxLeft.SelectedIndex;
                //inverse checked state at the selected index
                CheckListBoxLeft.SetItemChecked(Index, !CheckListBoxLeft.GetItemChecked(Index));
            }
        }
    }
}

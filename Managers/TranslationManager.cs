using HousePartyTranslator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A class providing functions for loading, approving, and workign with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>
    public class TranslationManager
    {
        public static TranslationManager main;
        public List<LineData> TranslationData = new List<LineData>();
        public List<StringCategory> CategoriesInFile = new List<StringCategory>();
        public bool IsUpToDate = false;
        public bool isTemplate = false;
        public bool AutoSave = true;
        private int LastIndex = -1;
        private bool isSaveAs = false;
        private int ExceptionCount = 0;
        private int SelectedSearchResult = 0;

        /// <summary>
        /// The Language of the current translation.
        /// </summary>
        public string Language
        {
            get
            {
                if (language.Length == 0)
                {
                    MessageBox.Show("Please enter a valid language or select one.", "Enter valid language");
                }
                return language;
            }
            set
            {
                language = value;
                ((StringSetting)SettingsManager.main.Settings.Find(pS => pS.GetKey() == "language")).UpdateValue(language);
                SettingsManager.main.UpdateSettings();
            }
        }
        private string language = "";

        /// <summary>
        /// The path to the file currently loaded.
        /// </summary>
        public string SourceFilePath
        {
            get
            {
                return sourceFilePath;
            }
            set
            {
                if (sourceFilePath != "")
                {
                    //TODO close opened file here
                }
                sourceFilePath = value;
                if (!isSaveAs) FileName = Path.GetFileNameWithoutExtension(value);
            }
        }
        private string sourceFilePath = "";

        /// <summary>
        /// The Name of the file laoded, without the extension.
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }
        private string fileName = "";

        /// <summary>
        /// The name of the parent folder of the loaded file, MUST BE the story it is from.
        /// </summary>
        public string StoryName
        {
            get
            {
                return storyName;
            }
            set
            {
                storyName = value;
            }
        }
        private string storyName = "";

        /// <summary>
        /// The Constructor for this class. Takes no arguments.
        /// </summary>
        public TranslationManager()
        {
            if (main != null)
            {
                return;
            }
            main = this;
        }

        /// <summary>
        /// Update the currently selected translation string in the TranslationData.
        /// </summary>
        /// <param name="EditorTextBox">The TextBox to read the string from.</param>
        /// <param name="TemplateTextBox">The TexBox containing the template.</param>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        /// <param name="CharacterCountLabel">The Label with the character count.</param>
        public void UpdateTranslationString(TextBox EditorTextBox, TextBox TemplateTextBox, ColouredCheckedListBox ColouredCheckedListBoxLeft, Label CharacterCountLabel)
        {
            int internalIndex = ColouredCheckedListBoxLeft.SelectedIndex;
            if (internalIndex >= 0)
            {
                TranslationData[internalIndex].TranslationString = EditorTextBox.Text.Replace(Environment.NewLine, "\n");
                UpdateCharacterCountLabel(TemplateTextBox.Text.Count(), EditorTextBox.Text.Count(), CharacterCountLabel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        /// <param name="SelectedFile">A label to display the selected file.</param>
        /// <param name="ApprovedCountLabel">A label displaying a ration of approved strings to normal strings.</param>
        /// <param name="NoProgressBar">The progressbar to show approval progress.</param>
        public void LoadFileIntoProgram(ColouredCheckedListBox ColouredCheckedListBoxLeft, Label SelectedFile, Label ApprovedCountLabel, NoAnimationBar NoProgressBar)
        {
            TranslationData.Clear();
            ColouredCheckedListBoxLeft.Items.Clear();
            CategoriesInFile.Clear();
            LastIndex = -1;
            SelectedSearchResult = 0;

            ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
            if (IsUpToDate)
            {
                SourceFilePath = SelectFileFromSystem();
                if (SourceFilePath != "")
                {

                    string[] paths = SourceFilePath.Split('\\');
                    //get parent folder name
                    StoryName = paths[paths.Length - 2];
                    HandleStringReadingFromFile();

                    SelectedFile.Text = "File: " + FileName + ".txt";

                    //is up to date, so we can start translation
                    HandleTranslationLoading(ColouredCheckedListBoxLeft);
                    UpdateApprovedCountLabel(ColouredCheckedListBoxLeft.CheckedIndices.Count, ColouredCheckedListBoxLeft.Items.Count, ApprovedCountLabel, NoProgressBar);
                }
            }
            else
            {
                string folderPath = SelectFolderFromSystem();
                string templateFolderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];
                if (templateFolderName == "TEMPLATE")
                {
                    isTemplate = true;
                    HandleTemplateLoading(folderPath);
                }
                else
                {
                    isTemplate = false;
                    MessageBox.Show(
                        $"Please the template folder named 'TEMPLATE' so we can upload them. " +
                        $"Your Current Folder shows as {templateFolderName}.",
                        "Updating string database"
                        );
                }
            }

            ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        /// <param name="TextBoxReadOnly">The TexBox containing the template.</param>
        /// <param name="TextBoxEditable">The TextBox to read the string from.</param>
        /// <param name="CommentBox">The TextBox containing the comments for the current string.</param>
        /// <param name="CharacterCountLabel">The Label with the character count.</param>
        /// <param name="ApprovedStringLabel">A label displaying a ration of approved strings to normal strings.</param>
        /// <param name="NoProgressbar">The progressbar to show approval progress.</param>
        /// <param name="ApprovedBox">The Button in the top to approve strings with. synced to the selected item checked state.</param>
        public void PopulateTextBoxes(ColouredCheckedListBox ColouredCheckedListBoxLeft, TextBox TextBoxReadOnly, TextBox TextBoxEditable, TextBox CommentBox, Label CharacterCountLabel, Label ApprovedStringLabel, NoAnimationBar NoProgressbar, CheckBox ApprovedBox)
        {
            TextBoxReadOnly.FindForm().Cursor = Cursors.WaitCursor;
            int currentIndex = ColouredCheckedListBoxLeft.SelectedIndex;
            if (LastIndex < 0)
            {
                //sets index the first time/when we click elsewhere
                LastIndex = currentIndex;
            }
            else
            {
                //if we changed the selection and have autsave enabled
                if (LastIndex != currentIndex && AutoSave)
                {

                    //update translation in the database
                    DataBaseManager.SetStringTranslation(
                        TranslationData[LastIndex].ID,
                        FileName,
                        StoryName,
                        TranslationData[LastIndex].Category,
                        TranslationData[LastIndex].TranslationString,
                        Language);

                    //upload comment on change
                    DataBaseManager.SetTranslationComments(
                        TranslationData[LastIndex].ID,
                        FileName,
                        StoryName,
                        CommentBox.Lines,
                        Language
                        );

                    LastIndex = currentIndex;

                    //clear comment after save
                    CommentBox.Lines = new string[0];
                }
            }

            if (currentIndex >= 0)
            {
                if (isTemplate)
                {
                    TextBoxReadOnly.Text = TranslationData[currentIndex].EnglishString.Replace("\n", Environment.NewLine);
                }
                else
                {
                    string id = TranslationData[currentIndex].ID;
                    //read latest version from the database
                    if (DataBaseManager.GetStringTranslation(id, FileName, StoryName, out string translation, Language))
                    {
                        //replace older one in file by new one from database
                        TranslationData[currentIndex].TranslationString = translation;
                    }

                    //display the string in the editable window
                    TextBoxEditable.Text = TranslationData[currentIndex].TranslationString.Replace("\n", Environment.NewLine);

                    if (DataBaseManager.GetStringTemplate(id, FileName, StoryName, out string templateString))
                    {
                        //read the template form the db and display it if it exists
                        TextBoxReadOnly.Text = templateString.Replace("\n", Environment.NewLine);

                        //clear text box if it is the template (not translated yet)
                        if (TextBoxReadOnly.Text == TextBoxEditable.Text && TranslationData[currentIndex].ID != "Name")
                        {
                            TextBoxEditable.Clear();
                        }
                    }

                    if (DataBaseManager.GetTranslationComments(id, FileName, StoryName, out string[] comments, Language))
                    {
                        CommentBox.Lines = comments;
                    }

                    ApprovedBox.Checked = ColouredCheckedListBoxLeft.GetItemChecked(currentIndex);

                    UpdateCharacterCountLabel(TextBoxReadOnly.Text.Count(), TextBoxEditable.Text.Count(), CharacterCountLabel);
                    UpdateApprovedCountLabel(ColouredCheckedListBoxLeft.CheckedIndices.Count, ColouredCheckedListBoxLeft.Items.Count, ApprovedStringLabel, NoProgressbar);
                }
            }
            TextBoxReadOnly.FindForm().Cursor = Cursors.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        public void SaveCurrentString(ColouredCheckedListBox ColouredCheckedListBoxLeft)
        {
            int currentIndex = ColouredCheckedListBoxLeft.SelectedIndex;

            //if we changed the eselction and have autsave enabled
            if (currentIndex >= 0)
            {
                ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

                //update translation in the database
                DataBaseManager.SetStringTranslation(
                    TranslationData[LastIndex].ID,
                    FileName,
                    StoryName,
                    TranslationData[LastIndex].Category,
                    TranslationData[LastIndex].TranslationString,
                    Language);

                ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        /// <param name="CommentBox">The TextBox containing the comments for the current string.</param>
        public void SaveCurrentComment(ColouredCheckedListBox ColouredCheckedListBoxLeft, TextBox CommentBox)
        {
            int currentIndex = ColouredCheckedListBoxLeft.SelectedIndex;

            //if we changed the eselction and have autsave enabled
            if (currentIndex >= 0)
            {
                ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

                //upload comment
                DataBaseManager.SetTranslationComments(
                    TranslationData[currentIndex].ID,
                    FileName,
                    StoryName,
                    CommentBox.Lines,
                    Language
                    );

                ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LanguageBox">The dropdown selection box for the language.</param>
        public void SetLanguage(ComboBox LanguageBox)
        {
            if (LanguageBox.SelectedIndex > -1)
            {
                Language = LanguageBox.GetItemText(LanguageBox.SelectedItem);
            }
            else if (SettingsManager.main.Settings.Exists(pS => pS.GetKey() == "language"))
            {
                string languageFromFile = ((StringSetting)SettingsManager.main.Settings.Find(pS => pS.GetKey() == "language")).GetValue();
                if (languageFromFile != "")
                {
                    Language = languageFromFile;
                    LanguageBox.Text = Language;
                }
            }
            LanguageBox.Text = Language;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        /// <param name="ApprovedCountLabel">A label displaying a ration of approved strings to normal strings.</param>
        /// <param name="NoProgressbar">The progressbar to show approval progress.</param>
        /// <param name="SelectNewAfter">A bool to determine if a new string should be selected after approval.</param>
        /// <param name="ApprovedBox">The Button in the top to approve strings with. synced to the selected item checked state.</param>
        public void ApproveIfPossible(ColouredCheckedListBox ColouredCheckedListBoxLeft, Label ApprovedCountLabel, NoAnimationBar NoProgressbar, bool SelectNewAfter, CheckBox ApprovedBox)
        {
            int currentIndex = ColouredCheckedListBoxLeft.SelectedIndex;
            if (currentIndex >= 0)
            {
                //UpdateApprovedCountLabel(ColouredCheckedListBoxLeft.CheckedIndices.Count, ColouredCheckedListBoxLeft.Items.Count, ApprovedCountLabel, NoProgressbar);

                string ID = TranslationData[currentIndex].ID;
                DataBaseManager.SetStringTranslation(ID, FileName, StoryName, TranslationData[currentIndex].Category, TranslationData[currentIndex].TranslationString, main.Language);
                if (!DataBaseManager.SetStringApprovedState(ID, FileName, StoryName, TranslationData[currentIndex].Category, !ColouredCheckedListBoxLeft.GetItemChecked(currentIndex), main.Language))
                {
                    MessageBox.Show("Could not set approved state of string " + ID);
                }

                //set checkbox state
                ApprovedBox.Checked = !ColouredCheckedListBoxLeft.GetItemChecked(currentIndex);

                //move one string down if possible
                if (!ColouredCheckedListBoxLeft.GetItemChecked(currentIndex) && SelectNewAfter)
                {
                    if (currentIndex < ColouredCheckedListBoxLeft.Items.Count - 1) ColouredCheckedListBoxLeft.SelectedIndex = currentIndex + 1;
                }

                UpdateApprovedCountLabel(ColouredCheckedListBoxLeft.CheckedIndices.Count, ColouredCheckedListBoxLeft.Items.Count, ApprovedCountLabel, NoProgressbar);
                NoProgressbar.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        public void SaveFile(ColouredCheckedListBox ColouredCheckedListBoxLeft)
        {
            if (SourceFilePath != "")
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
                ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
                List<Tuple<List<LineData>, StringCategory>> CategorizedStrings = new List<Tuple<List<LineData>, StringCategory>>();

                foreach (StringCategory category in CategoriesInFile)
                {//add a list for every category we have in the file, so we can then add the strings to these.
                    CategorizedStrings.Add(new Tuple<List<LineData>, StringCategory>(new List<LineData>(), category));
                }

                StreamWriter OutputWriter = new StreamWriter(SourceFilePath);

                DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);

                foreach (LineData item in IdsToExport)
                {
                    LineData lineDataResult = TranslationData.Find(predicateLine => predicateLine.ID == item.ID);
                    if (lineDataResult != null)
                    {
                        //add translation to the list in the correct category if present
                        int intCategory = CategoriesInFile.FindIndex(predicateCategory => predicateCategory == item.Category);
                        CategorizedStrings[intCategory].Item1.Add(lineDataResult);
                    }
                    else// if id is not found
                    {
                        string templateString = "";
                        //add template to list if no translation is in the file
                        if (!DataBaseManager.GetStringTranslation(item.ID, FileName, StoryName, out templateString, Language))
                        {
                            DataBaseManager.GetStringTemplate(item.ID, FileName, StoryName, out templateString);
                        }
                        item.TranslationString = templateString;
                        int intCategory = CategoriesInFile.FindIndex(predicateCategory => predicateCategory == item.Category);
                        if (intCategory < CategorizedStrings.Count && intCategory >= 0)
                        {
                            CategorizedStrings[intCategory].Item1.Add(item);
                        }
                        else
                        {
                            CategorizedStrings.Add(new Tuple<List<LineData>, StringCategory>(new List<LineData>(), StringCategory.Neither));
                            CategorizedStrings.Last().Item1.Add(item);
                        }
                    }
                }

                foreach (Tuple<List<LineData>, StringCategory> CategorizedLines in CategorizedStrings)
                {
                    //write category 
                    OutputWriter.WriteLine(GetStringFromCategory(CategorizedLines.Item2));

                    //sort strings depending on category
                    if (CategorizedLines.Item2 == StringCategory.Dialogue)
                    {
                        CategorizedLines.Item1.Sort((line1, line2) => decimal.Parse(line1.ID, culture).CompareTo(decimal.Parse(line2.ID, culture)));
                    }
                    else if (CategorizedLines.Item2 == StringCategory.BGC)
                    {
                        //sort using custom IComparer
                        CategorizedLines.Item1.Sort(new BGCComparer());
                    }
                    else if (CategorizedLines.Item2 == StringCategory.General)
                    {
                        //hints have to be sortet a bit different because the numbers can contain a u
                        CategorizedLines.Item1.Sort(new GeneralComparer());
                    }
                    else if (CategorizedLines.Item2 == StringCategory.Quest || CategorizedLines.Item2 == StringCategory.Achievement)
                    {
                        CategorizedLines.Item1.Sort((line1, line2) => line2.ID.CompareTo(line1.ID));
                    }

                    //iterate through each and print them
                    foreach (LineData line in CategorizedLines.Item1)
                    {
                        OutputWriter.WriteLine(line.ToString());
                    }
                    //newline after each category
                    OutputWriter.WriteLine();
                }

                OutputWriter.Close();

                ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.Default;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        public void SaveFileAs(ColouredCheckedListBox ColouredCheckedListBoxLeft)
        {
            if (SourceFilePath != "")
            {
                isSaveAs = true;
                string oldFile = main.SourceFilePath;
                string SaveFile = SaveFileOnSystem();
                main.SourceFilePath = SaveFile;
                main.SaveFile(ColouredCheckedListBoxLeft);
                main.SourceFilePath = oldFile;
                isSaveAs = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CheckedListBox">The list of strings.</param>
        /// <param name="SearchBox">The Textbox to search with.</param>
        public void Search(ColouredCheckedListBox CheckedListBox, TextBox SearchBox)
        {
            //reset list if no search is performed
            if (SearchBox.TextLength != 0)
            {
                //clear results
                CheckedListBox.SearchResults.Clear();
                //methodolgy: highlight items which fulfill search and show count
                //TranslationData.Where(a => a.TranslationString.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0||a.ID.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList().ForEach(b => CheckedListBox.SearchResults.Add(TranslationData.IndexOf(b)));

                TranslationData.Where(/*Get a list of all strings where*/
                    a => a.TranslationString.IndexOf(/*the text contains what is searched*/
                        SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    ||/*or*/
                    a.ID.IndexOf(/*the id contains what is searched*/
                        SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList().ForEach(/*then for all the result items add the index of the item in the main translation data list to the list of result indices*/
                        b => CheckedListBox.SearchResults.Add(TranslationData.IndexOf(b)
                    )
                );
            }
            else
            {
                CheckedListBox.SearchResults.Clear();
                SelectedSearchResult = 0;
            }

            CheckedListBox.Invalidate(CheckedListBox.Region);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">Windows message contaning the info on the event.</param>
        /// <param name="keyData">Keydata containing all currently pressed keys.</param>
        /// <param name="SearchBox">The Textbox to search with.</param>
        /// <param name="EditorBox">The TextBox to read the string from.</param>
        /// <param name="checkedListBox">The list of strings.</param>
        /// <returns></returns>
        public bool HandleKeyPressMainForm(ref Message msg, Keys keyData, TextBox SearchBox, TextBox EditorBox, ColouredCheckedListBox checkedListBox, TextBox CommentBox)
        {
            switch (keyData)
            {
                //handle enter as jumping to first search result if searched something, and focus is not on text editor.
                case (Keys.Enter):
                    if (!EditorBox.Focused || !CommentBox.Focused)
                    {
                        if (checkedListBox.SearchResults.Any())
                        {
                            if (SelectedSearchResult < checkedListBox.SearchResults.Count)
                            {
                                checkedListBox.SelectedIndex = checkedListBox.SearchResults[SelectedSearchResult++];
                            }
                            else
                            {
                                SelectedSearchResult = 0;
                                checkedListBox.SelectedIndex = checkedListBox.SearchResults[SelectedSearchResult++];
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                //set selected string as search string and place cursor in search box
                case (Keys.Control | Keys.F):
                    if (EditorBox.SelectedText.Length > 0)
                    {
                        SearchBox.Text = EditorBox.SelectedText;
                    }
                    SearchBox.Focus();
                    return true;

                //save current file
                case (Keys.Control | Keys.S):
                    SaveFile(checkedListBox);
                    return true;

                //save current string
                case (Keys.Control | Keys.Shift | Keys.S):
                    SaveCurrentString(checkedListBox);
                    return true;

                //reload currently loaded file
                case (Keys.Control | Keys.R):
                    ReloadFile(checkedListBox);
                    return true;

                //select string above current selection
                case (Keys.Control | Keys.Up):
                    if (checkedListBox.SelectedIndex > 0) checkedListBox.SelectedIndex--;
                    return true;

                //select string below current selection
                case (Keys.Control | Keys.Down):
                    if (checkedListBox.SelectedIndex < checkedListBox.Items.Count - 1) checkedListBox.SelectedIndex++;
                    return true;

                //save translation and move down one
                case (Keys.Control | Keys.Enter):
                    SaveCurrentString(checkedListBox);
                    if (checkedListBox.SelectedIndex < checkedListBox.Items.Count - 1) checkedListBox.SelectedIndex++;
                    return true;

                //save translation, approve and move down one
                case (Keys.Control | Keys.Shift | Keys.Enter):
                    checkedListBox.SetItemChecked(checkedListBox.SelectedIndex, true);
                    if (checkedListBox.SelectedIndex < checkedListBox.Items.Count - 1) checkedListBox.SelectedIndex++;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            main.ExceptionCount++;
            MessageBox.Show(
                $"The application encountered a Problem. Probably the database can not be reached, or you did something too quickly :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Some Error found (Nr. {main.ExceptionCount})",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );


            Application.OpenForms[0].Cursor = Cursors.Default;

            if (main.ExceptionCount > 3)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FensterRef"></param>
        /// <param name="ApprovedBox">The Button in the top to approve strings with. synced to the selected item checked state.</param>
        /// <param name="CheckListBox">The list of strings.</param>
        public static void ApprovedButtonHandler(Fenster FensterRef, CheckBox ApprovedBox, ColouredCheckedListBox CheckListBox)
        {
            //get title bar height
            Rectangle ScreenRectangle = FensterRef.RectangleToScreen(FensterRef.ClientRectangle);
            int TitleHeight = ScreenRectangle.Top - FensterRef.Top;

            //check whether cursor is on approved button or not
            int deltaX = Cursor.Position.X - ApprovedBox.Location.X - FensterRef.Location.X;
            int deltaY = Cursor.Position.Y - ApprovedBox.Location.Y - FensterRef.Location.Y - TitleHeight;
            bool isInX = 0 <= deltaX && deltaX <= ApprovedBox.Width;
            bool isInY = 0 <= deltaY && deltaY <= ApprovedBox.Height;

            //actually do the checking state change
            if (isInX && isInY)
            {
                int Index = CheckListBox.SelectedIndex;
                //inverse checked state at the selected index
                if (Index >= 0) CheckListBox.SetItemChecked(Index, !CheckListBox.GetItemChecked(Index));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkedListBox">The list of strings.</param>
        private void ReloadFile(ColouredCheckedListBox checkedListBox)
        {
            TranslationData.Clear();
            checkedListBox.Items.Clear();
            CategoriesInFile.Clear();
            LastIndex = -1;
            ReadStringsTranslationsFromFile();
            //is up to date, so we can start translation
            HandleTranslationLoading(checkedListBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TemplateCount">The number of chars in the template string.</param>
        /// <param name="TranslationCount">The number of chars in the translated string.</param>
        /// <param name="CharacterCountLabel">The Label with the character count.</param>
        private void UpdateCharacterCountLabel(int TemplateCount, int TranslationCount, Label CharacterCountLabel)
        {
            if (TemplateCount >= TranslationCount)
            {
                CharacterCountLabel.ForeColor = System.Drawing.Color.LawnGreen;
            }//if bigger by no more than 20 percent
            else if ((TranslationCount - TemplateCount) < (TemplateCount / 20))
            {
                CharacterCountLabel.ForeColor = System.Drawing.Color.DarkOrange;
            }
            else
            {
                CharacterCountLabel.ForeColor = System.Drawing.Color.Red;
            }
            CharacterCountLabel.Text = $"Template: {TemplateCount} | Translation: {TranslationCount}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Approved">The number of approved strings.</param>
        /// <param name="Total">The total number of strings.</param>
        /// <param name="ApprovedCountLabel">A label displaying a ration of approved strings to normal strings.</param>
        /// <param name="NoProgressbar">The progressbar to show approval progress.</param>
        private void UpdateApprovedCountLabel(int Approved, int Total, Label ApprovedCountLabel, NoAnimationBar NoProgressbar)
        {
            ApprovedCountLabel.Text = $"Approved: {Approved} / {Total}";
            int ProgressValue = (int)((float)Approved / (float)Total * 100);
            if (ProgressValue > 0 && ProgressValue < 100)
            {
                NoProgressbar.Value = ProgressValue;
            }
            else
            {
                NoProgressbar.Value = 0;
            }
            NoProgressbar.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleStringReadingFromFile()
        {
            //read in all strings with IDs
            if (isTemplate)//read in templates
            {
                ReadStringsTemplateFromFile();
            }
            else //read in translations
            {
                ReadStringsTranslationsFromFile();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadStringsTemplateFromFile()
        {
            StringCategory currentCategory = StringCategory.General;
            string multiLineCollector = "";
            string[] lastLine = { };
            foreach (string line in File.ReadAllLines(SourceFilePath))
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true));

                    //get current line
                    lastLine = line.Split('|');

                    //reset multiline collector
                    multiLineCollector = "";
                }
                else
                {
                    StringCategory tempCategory = GetCategoryFromString(line);
                    if (tempCategory == StringCategory.Neither)
                    {
                        //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                        multiLineCollector += "\n" + line;
                    }
                    else
                    {
                        //if we reach a category, we can add the old string to the translation manager
                        if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true));

                        multiLineCollector = "";
                        currentCategory = tempCategory;
                    }
                }
            }
            //add last line (if it does not exist)
            if (!TranslationData.Exists(predicateLine => predicateLine.ID == lastLine[0]))
            {
                if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1], true));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadStringsTranslationsFromFile()
        {
            StringCategory currentCategory = StringCategory.General;
            string multiLineCollector = "";
            string[] lastLine = { };

            DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);

            foreach (string line in File.ReadAllLines(SourceFilePath))
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector));

                    //get current line
                    lastLine = line.Split('|');

                    //reset multiline collector
                    multiLineCollector = "";
                }
                else
                {
                    StringCategory tempCategory = GetCategoryFromString(line);
                    if (tempCategory == StringCategory.Neither)
                    {
                        //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                        multiLineCollector += "\n" + line;
                    }
                    else
                    {
                        //if we reach a category, we can add the old string to the translation manager
                        if (lastLine.Length != 0)
                        {
                            if (multiLineCollector.Length > 2)
                            {//write last string with id plus all lines after that minus the last new line char
                                TranslationData.Add(
                                  new LineData(
                                      lastLine[0],
                                      StoryName,
                                      FileName,
                                      currentCategory,
                                      lastLine[1] + multiLineCollector.Remove(multiLineCollector.Length - 2, 1)));
                            }
                            else
                            {//write last line with id if no real line of text is afterwards
                                TranslationData.Add(
                                  new LineData(
                                      lastLine[0],
                                      StoryName,
                                      FileName,
                                      currentCategory,
                                      lastLine[1]));
                            }
                        }
                        lastLine = new string[0];
                        multiLineCollector = "";
                        currentCategory = tempCategory;
                        CategoriesInFile.Add(currentCategory);
                    }
                }
            }

            if (lastLine.Length > 0)
            {
                //add last line (if it does not exist)
                if (!TranslationData.Exists(predicateLine => predicateLine.ID == lastLine[0]))
                {
                    if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1]));
                }
            }

            //set categories if file is a hint file
            if (StoryName == "Hints") CategoriesInFile = new List<StringCategory>() { StringCategory.General };

            if (IdsToExport.Count != TranslationData.Count)
            {//inform user the issing translations will be added after export. i see no viable way to add them before having them all read in,
             //and it would take a lot o time to get them all. so just have the user save it once and reload. we might do this automatically, but i don't know if that is ok to do :)
                MessageBox.Show(
                    @"Some strings are missing from your translation, they will be added with the english version when you first save the file!",
                    "Some strings missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColouredCheckedListBoxLeft">The list of strings.</param>
        private void HandleTranslationLoading(ColouredCheckedListBox ColouredCheckedListBoxLeft)
        {
            ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

            bool lineIsApproved = false;
            bool gotApprovedStates = DataBaseManager.GetAllApprovalStatesForFile(main.FileName, main.StoryName, out List<LineData> internalLines, main.Language);

            foreach (LineData lineD in main.TranslationData)
            {
                if (gotApprovedStates)
                {
                    LineData tempLine = internalLines.Find(predicateLine => predicateLine.ID == lineD.ID);
                    if (tempLine != null)
                    {
                        lineIsApproved = tempLine.IsApproved;
                    }
                }

                ColouredCheckedListBoxLeft.Items.Add(lineD.ID, lineIsApproved);
                lineD.IsApproved = lineIsApproved;
            }
            ColouredCheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        private void HandleTemplateLoading(string folderPath)
        {
            //upload all new strings
            try
            {
                IterativeReadFiles(folderPath);
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("Encountered " + e.Message);
            }

            MessageBox.Show(
                "This is going to take some time(15+ minutes), get a tea or sth.\n" +
                "A Message will appear once it is done.\n" +
                "Click OK if you want to start.",
                "Updating string database...",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk
                );

            //add all the new strings
            foreach (LineData lineD in TranslationData)
            {
                DataBaseManager.SetStringTemplate(lineD.ID, lineD.FileName, lineD.Story, lineD.Category, lineD.EnglishString);
            }

            DialogResult result = MessageBox.Show(
                "This should be done uploading. It should be :)\n" +
                "If you are done uploading, please select YES. ELSE NO. Be wise please!",
                "Updating string database...",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
                );

            //update if successfull
            if (result == DialogResult.Yes)
            {
                if (DataBaseManager.UpdateDBVersion())
                {
                    IsUpToDate = true;
                    isTemplate = false;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        private void IterativeReadFiles(string folderPath)
        {
            DirectoryInfo templateDir = new DirectoryInfo(folderPath);
            if (templateDir != null)
            {
                DirectoryInfo[] storyDirs = templateDir.GetDirectories();
                if (storyDirs != null)
                {
                    foreach (DirectoryInfo storyDir in storyDirs)
                    {
                        if (storyDir != null)
                        {
                            StoryName = storyDir.Name;
                            FileInfo[] templateFiles = storyDir.GetFiles();
                            if (templateFiles != null)
                            {
                                foreach (FileInfo templateFile in templateFiles)
                                {
                                    if (templateFile != null)
                                    {
                                        SourceFilePath = templateFile.FullName;
                                        FileName = Path.GetFileNameWithoutExtension(SourceFilePath);
                                        HandleStringReadingFromFile();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private StringCategory GetCategoryFromString(string line)
        {
            StringCategory internalCategory = StringCategory.Neither;
            if (line.Contains("["))
            {
                if (line == "[General]")
                {
                    internalCategory = StringCategory.General;
                }
                else if (line == "[Dialogues]")
                {
                    internalCategory = StringCategory.Dialogue;
                }
                else if (line == "[Responses]")
                {
                    internalCategory = StringCategory.Response;
                }
                else if (line == "[Quests]")
                {
                    internalCategory = StringCategory.Quest;
                }
                else if (line == "[Events]")
                {
                    internalCategory = StringCategory.Event;
                }
                else if (line == "[Background Chatter]")
                {
                    internalCategory = StringCategory.BGC;
                }
                else if (line == "[Item Names]")
                {
                    internalCategory = StringCategory.ItemName;
                }
                else if (line == "[Item Actions]")
                {
                    internalCategory = StringCategory.ItemAction;
                }
                else if (line == "[Item Group Actions]")
                {
                    internalCategory = StringCategory.ItemGroupAction;
                }
                else if (line == "[Achievements]")
                {
                    internalCategory = StringCategory.Achievement;
                }
            }
            return internalCategory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private string GetStringFromCategory(StringCategory category)
        {
            string returnedString = "";
            switch (category)
            {
                case StringCategory.General:
                    returnedString = "[General]";
                    break;
                case StringCategory.Dialogue:
                    returnedString = "[Dialogues]";
                    break;
                case StringCategory.Response:
                    returnedString = "[Responses]";
                    break;
                case StringCategory.Quest:
                    returnedString = "[Quests]";
                    break;
                case StringCategory.Event:
                    returnedString = "[Events]";
                    break;
                case StringCategory.BGC:
                    returnedString = "[Background Chatter]";
                    break;
                case StringCategory.ItemName:
                    returnedString = "[Item Names]";
                    break;
                case StringCategory.ItemAction:
                    returnedString = "[Item Actions]";
                    break;
                case StringCategory.ItemGroupAction:
                    returnedString = "[Item Group Actions]";
                    break;
                case StringCategory.Achievement:
                    returnedString = "[Achievements]";
                    break;
                case StringCategory.Neither:
                    //do nothing hehehehe
                    break;
                default:
                    //do nothing hehehehe
                    break;
            }
            return returnedString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string SelectFileFromSystem()
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog
            {
                Title = "Choose a file for translation",
                Filter = "Text files (*.txt)|*.txt",
                InitialDirectory = @"C:\Users\%USER%\Documents"
            };

            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                return selectFileDialog.FileName;
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string SelectFolderFromSystem()
        {
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog
            {
                Description = "Please select the 'TEMPLATE' folder like in the repo",
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            if (selectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                return selectFolderDialog.SelectedPath;
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string SaveFileOnSystem()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Choose a file to save the translations to",
                AddExtension = true,
                DefaultExt = "txt",
                CheckPathExists = true,
                CreatePrompt = true,
                OverwritePrompt = true,
                FileName = main.FileName,
                InitialDirectory = main.SourceFilePath
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return "";
        }
    }
}
using HousePartyTranslator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

//TODO add tests?
//TODO create local db in ressources to download the tempaltes to
//TODO create a task queue for the database in case of internet outage

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A class providing functions for loading, approving, and workign with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>
    public class TranslationManager
    {
        public static TranslationManager main;
        public bool AutoSave = true;
        public List<StringCategory> CategoriesInFile = new List<StringCategory>();
        public bool isTemplate = false;
        public bool IsUpToDate = false;
        public List<LineData> TranslationData = new List<LineData>();
        private int ExceptionCount = 0;
        private string fileName = "";
        private bool isSaveAs = false;
        private string language = "";
        private int LastIndex = -1;
        private int SelectedSearchResult = 0;
        private string sourceFilePath = "";

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
        /// The Name of the file loaded, without the extension.
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

        /// <summary>
        /// The Language of the current translation.
        /// </summary>
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                if (value.Length == 0)
                {
                    MessageBox.Show("Please enter a valid language or select one.", "Enter valid language");
                }
                else
                {
                    language = value;
                    Properties.Settings.Default.language = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

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
                sourceFilePath = value;
                if (!isSaveAs) FileName = Path.GetFileNameWithoutExtension(value);
            }
        }

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

        /// <summary>
        /// Depending on the cursor location the line is approved or not (on checkbox or not).
        /// </summary>
        /// <param name="FensterRef">The window of type fenster</param>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public static void ApprovedButtonHandler(Fenster FensterRef, PropertyHelper helper)
        {
            //get title bar height
            Rectangle ScreenRectangle = FensterRef.RectangleToScreen(FensterRef.ClientRectangle);
            int TitleHeight = ScreenRectangle.Top - FensterRef.Top;

            //check whether cursor is on approved button or not
            int deltaX = Cursor.Position.X - helper.ApprovedBox.Location.X - FensterRef.Location.X;
            int deltaY = Cursor.Position.Y - helper.ApprovedBox.Location.Y - FensterRef.Location.Y - TitleHeight;
            bool isInX = 0 <= deltaX && deltaX <= helper.ApprovedBox.Width;
            bool isInY = 0 <= deltaY && deltaY <= helper.ApprovedBox.Height;

            //actually do the checking state change
            if (isInX && isInY)
            {
                int Index = helper.CheckListBoxLeft.SelectedIndex;
                //inverse checked state at the selected index
                if (Index >= 0) helper.CheckListBoxLeft.SetItemChecked(Index, !helper.CheckListBoxLeft.GetItemChecked(Index));
            }
        }

        /// <summary>
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.LogEvent("Exception message shown: " + message);
            LogManager.LogEvent("Current exceptioon count: " + main.ExceptionCount + 1);
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
        /// Opens a save file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the file to save to.</returns>
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

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string SelectFileFromSystem()
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog
            {
                Title = "Choose a file for translation",
                Filter = "Text files (*.txt)|*.txt",
                InitialDirectory = Properties.Settings.Default.translation_path.Length > 0 ? Path.GetDirectoryName(Properties.Settings.Default.translation_path) : @"C:\Users\%USER%\Documents",
                RestoreDirectory = false
            };

            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.translation_path = selectFileDialog.FileName;
                Properties.Settings.Default.Save();
                return selectFileDialog.FileName;
            }
            return "";
        }

        /// <summary>
        /// Opens a select folder dialogue and returns the selected folder as a path.
        /// </summary>
        /// <returns>The folder path selected.</returns>
        public static string SelectFolderFromSystem()
        {
            string templatePath = Properties.Settings.Default.template_path;
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog
            {
                Description = "Please select the 'TEMPLATE' folder like in the repo",
                SelectedPath = templatePath == "" ? Environment.SpecialFolder.UserProfile.ToString() : templatePath,
            };

            if (selectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                string t = selectFolderDialog.SelectedPath;
                if (templatePath != null)
                {
                    Properties.Settings.Default.template_path = t;
                    Properties.Settings.Default.Save();
                }
                return t;
            }
            return "";
        }

        /// <summary>
        /// Approves the string in the db, if possible. Also updates UI.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        /// <param name="SelectNewAfter">A bool to determine if a new string should be selected after approval.</param>
        public void ApproveIfPossible(PropertyHelper helper, bool SelectNewAfter)
        {
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;
            if (currentIndex >= 0)
            {
                //UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count, helper.ApprovedCountLabel, helper.NoProgressbar);

                string ID = TranslationData[currentIndex].ID;
                DataBaseManager.SetStringTranslation(ID, FileName, StoryName, TranslationData[currentIndex].Category, TranslationData[currentIndex].TranslationString, main.Language);
                if (!DataBaseManager.SetStringApprovedState(ID, FileName, StoryName, TranslationData[currentIndex].Category, !helper.CheckListBoxLeft.GetItemChecked(currentIndex), main.Language))
                {
                    MessageBox.Show("Could not set approved state of string " + ID);
                }

                //set checkbox state
                helper.ApprovedBox.Checked = !helper.CheckListBoxLeft.GetItemChecked(currentIndex);

                //move one string down if possible
                if (!helper.CheckListBoxLeft.GetItemChecked(currentIndex) && SelectNewAfter)
                {
                    if (currentIndex < helper.CheckListBoxLeft.Items.Count - 1) helper.CheckListBoxLeft.SelectedIndex = currentIndex + 1;
                }

                UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count, helper);
                helper.NoProgressbar.Update();
            }
        }

        /// <summary>
        /// Detects for hotkeys used, else with the return value the defualt WndPrc is called.
        /// </summary>
        /// <param name="msg">Windows message contaning the info on the event.</param>
        /// <param name="keyData">Keydata containing all currently pressed keys.</param>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        /// <returns></returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter

        public bool HandleKeyPressMainForm(ref Message msg, Keys keyData, PropertyHelper helper)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            switch (keyData)
            {
                //handle enter as jumping to first search result if searched something, and focus is not on text editor.
                case (Keys.Enter):
                    if (!helper.TranslationTextBox.Focused || !helper.CommentBox.Focused)
                    {
                        if (helper.CheckListBoxLeft.SearchResults.Any())
                        {
                            if (SelectedSearchResult < helper.CheckListBoxLeft.SearchResults.Count)
                            {
                                helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult++];
                            }
                            else
                            {
                                SelectedSearchResult = 0;
                                helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult++];
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
                    if (helper.TranslationTextBox.SelectedText.Length > 0)
                    {
                        helper.SearchBox.Text = helper.TranslationTextBox.SelectedText;
                    }
                    helper.SearchBox.Focus();
                    return true;

                //save current file
                case (Keys.Control | Keys.S):
                    SaveFile(helper);
                    return true;

                //save current string
                case (Keys.Control | Keys.Shift | Keys.S):
                    SaveCurrentString(helper);
                    return true;

                //reload currently loaded file
                case (Keys.Control | Keys.R):
                    ReloadFile(helper);
                    return true;

                //select string above current selection
                case (Keys.Control | Keys.Up):
                    if (helper.CheckListBoxLeft.SelectedIndex > 0) helper.CheckListBoxLeft.SelectedIndex--;
                    return true;

                //select string below current selection
                case (Keys.Control | Keys.Down):
                    if (helper.CheckListBoxLeft.SelectedIndex < helper.CheckListBoxLeft.Items.Count - 1) helper.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //save translation and move down one
                case (Keys.Control | Keys.Enter):
                    SaveCurrentString(helper);
                    if (helper.CheckListBoxLeft.SelectedIndex < helper.CheckListBoxLeft.Items.Count - 1) helper.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //save translation, approve and move down one
                case (Keys.Control | Keys.Shift | Keys.Enter):
                    helper.CheckListBoxLeft.SetItemChecked(helper.CheckListBoxLeft.SelectedIndex, true);
                    if (helper.CheckListBoxLeft.SelectedIndex < helper.CheckListBoxLeft.Items.Count - 1) helper.CheckListBoxLeft.SelectedIndex++;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Loads a file into the program and calls all helper routines
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void LoadFileIntoProgram(PropertyHelper helper)
        {
            ResetTranslationManager(helper);

            helper.CheckListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
            if (IsUpToDate)
            {
                SourceFilePath = SelectFileFromSystem();
                if (SourceFilePath != "")
                {
                    string[] paths = SourceFilePath.Split('\\');

                    //get parent folder name
                    string tempStoryName = paths[paths.Length - 2];
                    //get language text representation
                    bool gotLanguage = LanguageHelper.Languages.TryGetValue(Language, out string languageAsText);
                    //compare
                    if (tempStoryName == languageAsText && gotLanguage)
                    {
                        //get foler one more up
                        tempStoryName = paths[paths.Length - 3];
                    }

                    if (tempStoryName == "Languages")
                    {
                        //get foler one more up
                        tempStoryName = "UI";
                    }

                    StoryName = tempStoryName;

                    //actually load all strings into the program
                    HandleStringReadingFromFile();

                    //update UI (cut folder name short if it is too long)
                    int lengthOfFileName = FileName.Length, lengthOfStoryName = StoryName.Length;
                    if (FileName.Length > 15)
                    {
                        lengthOfFileName = 15;
                    }
                    if (StoryName.Length > 15)
                    {
                        lengthOfStoryName = 15;
                    }

                    string storyNameToDisplay = StoryName.Length > lengthOfStoryName ? StoryName.Substring(0, lengthOfStoryName).Trim() + "..." : StoryName;
                    string fileNameToDisplay = FileName.Length > lengthOfFileName ? FileName.Substring(0, lengthOfFileName).Trim() + "..." : FileName;
                    helper.SelectedFileLabel.Text = $"File: {storyNameToDisplay}/{fileNameToDisplay}.txt";

                    //is up to date, so we can start translation
                    HandleTranslationLoading(helper);
                    UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count, helper);
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
                        "Updating string database",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
                }
            }
            //register load
            LogManager.LogEvent($"File opened: {StoryName}/{FileName} at {DateTime.Now}");

            helper.CheckListBoxLeft.FindForm().Cursor = Cursors.Default;
        }

        /// <summary>
        /// Populates the Editor/Template text boxes and does some basic set/reset logic.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void PopulateTextBoxes(PropertyHelper helper)
        {
            helper.TemplateTextBox.FindForm().Cursor = Cursors.WaitCursor;
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;
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
                        helper.CommentBox.Lines,
                        Language
                        );

                    LastIndex = currentIndex;

                    //clear comment after save
                    helper.CommentBox.Lines = new string[0];
                }
            }

            if (currentIndex >= 0)
            {
                if (isTemplate)
                {
                    helper.TemplateTextBox.Text = TranslationData[currentIndex].EnglishString.Replace("\n", Environment.NewLine);
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
                    helper.TranslationTextBox.Text = TranslationData[currentIndex].TranslationString.Replace("\n", Environment.NewLine);

                    if (DataBaseManager.GetStringTemplate(id, FileName, StoryName, out string templateString))
                    {
                        //read the template form the db and display it if it exists
                        helper.TemplateTextBox.Text = templateString.Replace("\n", Environment.NewLine);

                        //clear text box if it is the template (not translated yet)
                        //  sql makes this easy, can just write all commands to a file that have not been sent, then send all on connection resume.
                        if (helper.TemplateTextBox.Text == helper.TranslationTextBox.Text && TranslationData[currentIndex].Category != StringCategory.General)
                        {
                            helper.CheckListBoxLeft.SimilarStringsToEnglish.Add(currentIndex);
                            //TranslationData[currentIndex].IsTranslated = false;
                        }
                        else
                        {
                            if (helper.CheckListBoxLeft.SimilarStringsToEnglish.Contains(currentIndex)) helper.CheckListBoxLeft.SimilarStringsToEnglish.Remove(currentIndex);
                        }
                    }

                    if (DataBaseManager.GetTranslationComments(id, FileName, StoryName, out string[] comments, Language))
                    {
                        helper.CommentBox.Lines = comments;
                    }

                    helper.ApprovedBox.Checked = helper.CheckListBoxLeft.GetItemChecked(currentIndex);

                    UpdateCharacterCountLabel(helper.TemplateTextBox.Text.Count(), helper.TranslationTextBox.Text.Count(), helper);
                    UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count, helper);
                }
            }
            helper.TemplateTextBox.FindForm().Cursor = Cursors.Default;
        }

        /// <summary>
        /// Saves the current comment to the db
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SaveCurrentComment(PropertyHelper helper)
        {
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;

            //if we changed the eselction and have autsave enabled
            if (currentIndex >= 0)
            {
                helper.CheckListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

                //upload comment
                DataBaseManager.SetTranslationComments(
                    TranslationData[currentIndex].ID,
                    FileName,
                    StoryName,
                    helper.CommentBox.Lines,
                    Language
                    );

                helper.CheckListBoxLeft.FindForm().Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves the current string to the db
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SaveCurrentString(PropertyHelper helper)
        {
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;

            //if we changed the eselction and have autsave enabled
            if (currentIndex >= 0)
            {
                helper.CheckListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

                //update translation in the database
                DataBaseManager.SetStringTranslation(
                    TranslationData[LastIndex].ID,
                    FileName,
                    StoryName,
                    TranslationData[LastIndex].Category,
                    TranslationData[LastIndex].TranslationString,
                    Language);

                if (helper.CheckListBoxLeft.SimilarStringsToEnglish.Contains(currentIndex)) helper.CheckListBoxLeft.SimilarStringsToEnglish.Remove(currentIndex);

                helper.CheckListBoxLeft.FindForm().Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves all strings to the file we read from.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SaveFile(PropertyHelper helper)
        {
            if (SourceFilePath != "")
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
                helper.CheckListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
                List<Tuple<List<LineData>, StringCategory>> CategorizedStrings = new List<Tuple<List<LineData>, StringCategory>>();

                //we need to check whether the file has any strings at all, expecially the categories, if no, add them first or shit breaks.
                if (CategoriesInFile.Count == 0)
                {
                    GenerateCategories();
                }

                foreach (StringCategory category in CategoriesInFile)
                {//add a list for every category we have in the file, so we can then add the strings to these.
                    CategorizedStrings.Add(new Tuple<List<LineData>, StringCategory>(new List<LineData>(), category));
                }

                StreamWriter OutputWriter = new StreamWriter(SourceFilePath);

                //can take some time
                DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);
                DataBaseManager.GetAllTranslatedStringForFile(FileName, StoryName, out List<LineData> TranslationsFromDatabase, Language);

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
                        //add template to list if no translation is in the file
                        LineData TempResult = TranslationsFromDatabase.Find(pL => pL.ID == item.ID);

                        if (TempResult == null)
                        {
                            DataBaseManager.GetStringTemplate(item.ID, FileName, StoryName, out item.TranslationString);
                        }
                        else
                        {
                            item.TranslationString = TempResult.TranslationString;
                        }

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

                helper.CheckListBoxLeft.FindForm().Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves all strings to a specified file location.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SaveFileAs(PropertyHelper helper)
        {
            if (SourceFilePath != "")
            {
                isSaveAs = true;
                string oldFile = main.SourceFilePath;
                string SaveFile = SaveFileOnSystem();
                main.SourceFilePath = SaveFile;
                main.SaveFile(helper);
                main.SourceFilePath = oldFile;
                isSaveAs = false;
            }
        }

        /// <summary>
        /// Performs a search through all lines currently loaded.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void Search(PropertyHelper helper)
        {
            //reset list if no search is performed
            if (helper.SearchBox.TextLength != 0)
            {
                //clear results
                helper.CheckListBoxLeft.SearchResults.Clear();
                //methodolgy: highlight items which fulfill search and show count
                //TranslationData.Where(a => a.TranslationString.IndexOf(helper.SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0||a.ID.IndexOf(helper.SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList().ForEach(b => helper.CheckListBoxLeft.SearchResults.Add(TranslationData.IndexOf(b)));

                TranslationData.Where(/*Get a list of all strings where*/
                    a => a.TranslationString.IndexOf(/*the text contains what is searched*/
                        helper.SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    ||/*or*/
                    a.ID.IndexOf(/*the id contains what is searched*/
                        helper.SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList().ForEach(/*then for all the result items add the index of the item in the main translation data list to the list of result indices*/
                        b => helper.CheckListBoxLeft.SearchResults.Add(TranslationData.IndexOf(b)
                    )
                );
            }
            else
            {
                helper.CheckListBoxLeft.SearchResults.Clear();
                SelectedSearchResult = 0;
            }

            helper.CheckListBoxLeft.Invalidate(helper.CheckListBoxLeft.Region);
        }

        /// <summary>
        /// Sets the language the translation is associated with
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SetLanguage(PropertyHelper helper)
        {
            if (helper.LanguageBox.SelectedIndex >= 0)
            {
                Language = helper.LanguageBox.Text;
            }
            else if (Properties.Settings.Default.language != "")
            {
                string languageFromFile = Properties.Settings.Default.language;
                if (languageFromFile != "")
                {
                    Language = languageFromFile;
                }
            }
            helper.LanguageBox.Text = Language;
        }

        /// <summary>
        /// Update the currently selected translation string in the TranslationData.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void UpdateTranslationString(PropertyHelper helper)
        {
            int internalIndex = helper.CheckListBoxLeft.SelectedIndex;
            if (internalIndex >= 0)
            {
                //remove pipe to not break saving/export
                helper.TranslationTextBox.Text.Replace('|', ' ');
                TranslationData[internalIndex].TranslationString = helper.TranslationTextBox.Text.Replace(Environment.NewLine, "\n");
                UpdateCharacterCountLabel(helper.TemplateTextBox.Text.Count(), helper.TranslationTextBox.Text.Count(), helper);
            }
        }

        /// <summary>
        /// Generates a list of all string categories depending on the filename.
        /// </summary>
        private void GenerateCategories()
        {
            if (StoryName == "UI" || StoryName == "Hints")
            {
                CategoriesInFile = new List<StringCategory>() { StringCategory.General };
            }
            else if (FileName == StoryName)
            {
                CategoriesInFile = new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.ItemName,
                            StringCategory.ItemAction,
                            StringCategory.ItemGroupAction,
                            StringCategory.Event,
                            StringCategory.Achievement };
            }
            else
            {
                CategoriesInFile = new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.Dialogue,
                            StringCategory.Response,
                            StringCategory.Quest,
                            StringCategory.Event,
                            StringCategory.BGC};
            }
        }

        /// <summary>
        /// Tries to parse a line into the category it indicates.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The category representing the string, or none.</returns>
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
        /// Returns the string representatio of a category.
        /// </summary>
        /// <param name="category">The Category to parse.</param>
        /// <returns>The string representing the category.</returns>
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
        /// Reads the strings depending on whether its a template or not.
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
        /// Loads the templates by combining all lines from all files into one, then sending them one by one to the db.
        /// </summary>
        /// <param name="folderPath">The path to the folders to load the templates from.</param>
        private void HandleTemplateLoading(string folderPath)
        {
            //upload all new strings
            try
            {
                IterativeReadFiles(folderPath);
            }
            catch (UnauthorizedAccessException e)
            {
                LogManager.LogEvent(e.ToString());
                DisplayExceptionMessage(e.ToString());
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
                if (lineD.EnglishString != "")
                {
                    DataBaseManager.SetStringTemplate(lineD.ID, lineD.FileName, lineD.Story, lineD.Category, lineD.EnglishString);
                }
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
        /// Loads the strings and does some work around to ensure smooth sailing.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        private void HandleTranslationLoading(PropertyHelper helper)
        {
            helper.CheckListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

            bool lineIsApproved = false;
            bool gotApprovedStates = DataBaseManager.GetAllStatesForFile(main.FileName, main.StoryName, out List<LineData> internalLines, main.Language);
            int currentIndex = 0;

            foreach (LineData lineD in main.TranslationData)
            {
                if (gotApprovedStates)
                {
                    LineData tempLine = internalLines.Find(predicateLine => predicateLine.ID == lineD.ID);
                    if (tempLine != null)
                    {
                        lineIsApproved = tempLine.IsApproved;
                        lineD.IsTranslated = tempLine.TranslationString.Trim().Length > 1;
                    }
                }

                helper.CheckListBoxLeft.Items.Add(lineD.ID, lineIsApproved);

                //do after adding or it will trigger reset
                lineD.IsApproved = lineIsApproved;

                //colour string if similar to the english one
                if (!lineD.IsTranslated && !lineD.IsApproved)
                {
                    helper.CheckListBoxLeft.SimilarStringsToEnglish.Add(currentIndex);
                }

                //increase index to aid colouring
                currentIndex++;
                lineIsApproved = false;
            }
            helper.CheckListBoxLeft.FindForm().Cursor = Cursors.Default;
        }

        /// <summary>
        /// Reads all files in all subfolders below the given path.
        /// </summary>
        /// <param name="folderPath">The path to the folder to find all files in (iterative).</param>
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
        /// tldr: magic
        ///
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        private void ReadStringsTemplateFromFile()
        {
            StringCategory currentCategory = StringCategory.General;
            string multiLineCollector = "";
            string[] lastLine = { };
            //string[] lastLastLine = { };
            //read in lines
            List<string> LinesFromFile = File.ReadAllLines(SourceFilePath).ToList();
            //remove last if empty, breaks line lioading for the last
            while (LinesFromFile.Last() == "") LinesFromFile.Remove(LinesFromFile.Last());
            //load lines and their data and split accordingly
            foreach (string line in LinesFromFile)
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
            //add last line (dont care about duplicates because sql will get rid of them)
            if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1], true));
        }

        /// <summary>
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        private void ReadStringsTranslationsFromFile()
        {
            StringCategory currentCategory = StringCategory.General;
            string multiLineCollector = "";
            string[] lastLine = { };

            DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);

            //read in lines
            List<string> LinesFromFile = File.ReadAllLines(SourceFilePath).ToList();
            //remove last if empty, breaks line lioading for the last
            while (LinesFromFile.Last() == "") LinesFromFile.RemoveAt(LinesFromFile.Count - 1);
            //load lines and their data and split accordingly
            foreach (string line in LinesFromFile)
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
                //add last line (dont care about duplicates because sql will get rid of them)
                TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1]));
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
        /// Reloads the file into the program as if it were selected.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        private void ReloadFile(PropertyHelper helper)
        {
            ResetTranslationManager(helper);
            ReadStringsTranslationsFromFile();
            HandleTranslationLoading(helper);
        }

        /// <summary>
        /// Resets the translation manager.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        private void ResetTranslationManager(PropertyHelper helper)
        {
            TranslationData.Clear();
            helper.CheckListBoxLeft.Items.Clear();
            CategoriesInFile.Clear();
            helper.CheckListBoxLeft.SimilarStringsToEnglish.Clear();
            LastIndex = -1;
            SelectedSearchResult = 0;
        }

        /// <summary>
        /// Updates the label schowing the number lines approved so far
        /// </summary>
        /// <param name="Approved">The number of approved strings.</param>
        /// <param name="Total">The total number of strings.</param>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        private void UpdateApprovedCountLabel(int Approved, int Total, PropertyHelper helper)
        {
            helper.ApprovedCountLabel.Text = $"Approved: {Approved} / {Total}";
            int ProgressValue = (int)((float)Approved / (float)Total * 100);
            if (ProgressValue > 0 && ProgressValue < 100)
            {
                helper.NoProgressbar.Value = ProgressValue;
            }
            else
            {
                helper.NoProgressbar.Value = 0;
            }
            helper.NoProgressbar.Invalidate();
        }

        /// <summary>
        /// Updates the label displaying the character count
        /// </summary>
        /// <param name="TemplateCount">The number of chars in the template string.</param>
        /// <param name="TranslationCount">The number of chars in the translated string.</param>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        private void UpdateCharacterCountLabel(int TemplateCount, int TranslationCount, PropertyHelper helper)
        {
            if (TemplateCount >= TranslationCount)
            {
                helper.CharacterCountLabel.ForeColor = Color.LawnGreen;
            }//if bigger by no more than 20 percent
            else if ((TranslationCount - TemplateCount) < (TemplateCount / 20))
            {
                helper.CharacterCountLabel.ForeColor = Color.DarkOrange;
            }
            else
            {
                helper.CharacterCountLabel.ForeColor = Color.Red;
            }
            helper.CharacterCountLabel.Text = $"Template: {TemplateCount} | Translation: {TranslationCount}";
        }
    }
}
using HousePartyTranslator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibreTranslate.Net;
using System.Threading.Tasks;

//TODO add tests?

//TODO create local db in ressources to download the tempaltes to

//TODO create a task queue for the database in case of internet outage

//TODO add recents to file drop down menu, add auto open setting

//TODO add settings menu window to edit settings

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A class providing functions for loading, approving, and workign with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>
    public class TranslationManager
    {
        private bool isSaveAs = false;
        private int ExceptionCount = 0;
        private int LastIndex = -1;
        private int SelectedSearchResult = 0;
        private static Fenster MainWindow;
        private string fileName = "";
        private string language = "";
        private string sourceFilePath = "";
        private string storyName = "";
        public bool AutoSave = true;//setting
        public bool AutoTranslate = true;//setting
        public bool AutoLoadRecent = true;//setting
        public bool isTemplate = false;
        public bool IsUpToDate = false;
        public bool UpdateStoryExplorerSelection = true;
        public List<LineData> TranslationData = new List<LineData>();
        public List<StringCategory> CategoriesInFile = new List<StringCategory>();
        public static TranslationManager main;
        private readonly LibreTranslate.Net.LibreTranslate Translator = new LibreTranslate.Net.LibreTranslate("https://translate.rinderha.cc");

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
        /// Sets the main form this translationmanager will work on (cursor, fields, etc)
        /// </summary>
        /// <param name="mainWindow">The form to work on.</param>
        public void SetMainForm(Fenster mainWindow)
        {
            MainWindow = mainWindow;
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
                if (language.Length == 0)
                {
                    MessageBox.Show("Please enter a valid language or select one.", "Enter valid language");
                    return "";
                }
                else
                {
                    return language;
                }
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
        public static void ApprovedButtonHandler(PropertyHelper helper)
        {
            //change checked state for the selected item,
            //but only if we are on the button with the mouse.
            //(prevents an infinite loop when we get the change state from setting the button state in code)
            if (helper.ApprovedBox.Focused)
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
                    SaveFile();
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
            LoadFileIntoProgram(helper, SelectFileFromSystem());
        }

        /// <summary>
        /// Loads a file into the program and calls all helper routines
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        /// <param name="path">The path to the file to translate</param>
        public void LoadFileIntoProgram(PropertyHelper helper, string path)
        {
            ResetTranslationManager(helper);

            MainWindow.Cursor = Cursors.WaitCursor;
            if (IsUpToDate)
            {
                SourceFilePath = path;
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

            MainWindow.Cursor = Cursors.Default;
        }

        /// <summary>
        /// eplaces the template version of the string with a computer translated one to speed up translation.
        /// </summary>
        /// <param name="currentIndex">The selected index of the string not yet translated</param>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public async void ReplaceTranslationTranslatedTask(int currentIndex, PropertyHelper helper)
        {
            if (main.AutoTranslate)
            {
                string _1 = "";
                _1 = await Translator.TranslateAsync(new Translate()
                {
                    ApiKey = "",
                    Source = LanguageCode.English,
                    Target = LanguageCode.FromString(Language),
                    Text = TranslationData[currentIndex].TranslationString
                });
                if (_1.Length > 0)
                {
                    TranslationData[currentIndex].TranslationString = _1;
                    if (currentIndex == helper.CheckListBoxLeft.SelectedIndex && helper.TranslationTextBox.Text == helper.TemplateTextBox.Text)
                    {
                        helper.TranslationTextBox.Text = TranslationData[currentIndex].TranslationString;
                    }
                }
            }
        }

        /// <summary>
        /// Populates the Editor/Template text boxes and does some basic set/reset logic.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void PopulateTextBoxes(PropertyHelper helper)
        {
            MainWindow.Cursor = Cursors.WaitCursor;
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

                        if (helper.TranslationTextBox.Text == helper.TemplateTextBox.Text)
                        {
                            ReplaceTranslationTranslatedTask(currentIndex, helper);
                        }

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

                    if (UpdateStoryExplorerSelection)
                    {
                        SetHighlightedNode(helper);
                    }
                    else
                    {
                        UpdateStoryExplorerSelection = true;
                    }
                }
            }
            MainWindow.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Sets the node whose tree gets highlighted to the one representing the currently selected string;
        /// </summary>
        /// <param name="helper">A Propertyhelper to get access to the form controls.</param>
        private void SetHighlightedNode(PropertyHelper helper)
        {
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;
            string id = TranslationData[currentIndex].ID;
            //Highlights the node representign the selected string in the story explorer window
            if (MainWindow.Explorer != null && !MainWindow.Explorer.IsDisposed)
            {
                MainWindow.Explorer.Grapher.HighlightedNode = MainWindow.Explorer.Grapher.Context.GetNodes().Find(n => n.ID == id);
            }
        }

        /// <summary>
        /// Selects a string in the listview given the id
        /// </summary>
        /// <param name="id">The id to look for.</param>
        public void SelectLine(string id)
        {
            //select line which correspondends to id
            int index = TranslationData.FindIndex(n => n.ID == id);
            if (index >= 0) MainWindow.MainProperties.CheckListBoxLeft.SelectedIndex = index;
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
                MainWindow.Cursor = Cursors.WaitCursor;

                //upload comment
                DataBaseManager.SetTranslationComments(
                    TranslationData[currentIndex].ID,
                    FileName,
                    StoryName,
                    helper.CommentBox.Lines,
                    Language
                    );

                MainWindow.Cursor = Cursors.Default;
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
                MainWindow.Cursor = Cursors.WaitCursor;

                //update translation in the database
                DataBaseManager.SetStringTranslation(
                    TranslationData[LastIndex].ID,
                    FileName,
                    StoryName,
                    TranslationData[LastIndex].Category,
                    TranslationData[LastIndex].TranslationString,
                    Language);

                if (helper.CheckListBoxLeft.SimilarStringsToEnglish.Contains(currentIndex)) helper.CheckListBoxLeft.SimilarStringsToEnglish.Remove(currentIndex);

                MainWindow.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves all strings to the file we read from.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SaveFile()
        {
            if (SourceFilePath != "" && Language != "")
            {
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
                MainWindow.Cursor = Cursors.WaitCursor;
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

                StreamWriter OutputWriter = new StreamWriter(SourceFilePath, false, new UTF8Encoding(true));

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

                MainWindow.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves all strings to a specified file location.
        /// </summary>
        /// <param name="helper">A reference to an instance of the helper class which exposes all necesseray UI elements</param>
        public void SaveFileAs()
        {
            if (SourceFilePath != "")
            {
                isSaveAs = true;
                string oldFile = main.SourceFilePath;
                string SaveFile = SaveFileOnSystem();
                main.SourceFilePath = SaveFile;
                main.SaveFile();
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
                for (int i = 0; i < TranslationData.Count; i++)
                {
                    if (TranslationData[i].TranslationString.ToLowerInvariant().Contains(helper.SearchBox.Text.ToLowerInvariant()) /*if the translated text contaisn the search string*/
                        || (TranslationData[i].EnglishString != null
                        ? TranslationData[i].EnglishString.ToLowerInvariant().Contains(helper.SearchBox.Text.ToLowerInvariant())
                        : false)/*if the english string is not null and contaisn the searched part*/
                        || TranslationData[i].ID.ToLowerInvariant().Contains(helper.SearchBox.Text.ToLowerInvariant()))/*if the id contains the searched part*/
                    {
                        helper.CheckListBoxLeft.SearchResults.Add(i);//add index to highligh list
                    }
                }
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
            MainWindow.Cursor = Cursors.WaitCursor;

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
            MainWindow.Cursor = Cursors.Default;
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
using HousePartyTranslator.Helpers;
using LibreTranslate.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//TODO add tests

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A class providing functions for loading, approving, and working with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>
    public class TranslationManager
    {
        public static bool ChangesPending = false;
        public static bool IsUpToDate = false;
        public List<StringCategory> CategoriesInFile = new List<StringCategory>();
        public bool isTemplate = false;
        public string SearchQuery = "";

        public int SelectedSearchResult = 0;

        //setting?
        public readonly List<LineData> TranslationData = new List<LineData>();
        public bool UpdateStoryExplorerSelection = true;
        private static readonly LibreTranslate.Net.LibreTranslate Translator = new LibreTranslate.Net.LibreTranslate("https://translate.rinderha.cc");
        private static int ExceptionCount = 0;
        private static Fenster MainWindow;
        private string fileName = "";
        private bool isSaveAs = false;
        private string language = "";
        private int LastIndex = -1;
        private int searchTabIndex = 0;
        private string sourceFilePath = "";
        private string storyName = "";
        private readonly PropertyHelper helper;
        private bool triedFixingOnce = false;

        /// <summary>
        /// The Constructor for this class. Takes no arguments.
        /// </summary>
        public TranslationManager(PropertyHelper _helper)
        {
            this.helper = _helper;
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
        /// Provides the id of the currently selected line
        /// </summary>
        public string SelectedId { get { return helper.CheckListBoxLeft.SelectedItem.ToString(); } }

        /// <summary>
        /// The Language of the current translation.
        /// </summary>
        public string Language
        {
            get
            {
                if (language.Length == 0)
                {
                    if (!SoftwareVersionManager.UpdatePending) MessageBox.Show("Please enter a valid language or select one.", "Enter valid language");
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
                    if (!SoftwareVersionManager.UpdatePending) MessageBox.Show("Please enter a valid language or select one.", "Enter valid language");
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
                if (Utils.IsOfficialStory(value))
                {
                    storyName = value;
                }
                else
                {
                    MessageBox.Show($"No valid story name found \"{value}\", assuming Original Story.");
                    storyName = "Original Story";
                }
            }
        }

        /// <summary>
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.LogEvent("Exception message shown: " + message);
            LogManager.LogEvent("Current exception count: " + ExceptionCount++);
            MessageBox.Show(
                $"The application encountered a Problem. Probably the database can not be reached, or you did something too quickly :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Some Error found (Nr. {ExceptionCount})",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            Application.OpenForms[0].Cursor = Cursors.Default;

            if (ExceptionCount > 3)
            {
                Application.Exit();
            }
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
        /// Opens a select folder dialogue to find the template folder and returns the selected folder as a path.
        /// </summary>
        /// <returns>The folder path selected.</returns>
        public static string SelectTemplateFolderFromSystem()
        {
            return SelectFolderFromSystem("Please select the 'TEMPLATE' folder like in the repo");
        }

        /// <summary>
        /// Opens a select folder dialogue and returns the selected folder as a path.
        /// </summary>
        /// <param name="message">The description of the dialogue to display</param>
        /// <returns>The folder path selected.</returns>
        public static string SelectFolderFromSystem(string message)
        {
            string templatePath = Properties.Settings.Default.template_path;
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog
            {
                Description = message,
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
        /// Depending on the cursor location the line is approved or not (on checkbox or not).
        /// </summary>
        /// <param name="FensterRef">The window of type fenster</param>
        public void ApprovedButtonHandler()
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
        /// Gets the location of a single file, then tries to discover all others and load them.
        /// </summary>
        public static void LoadAllFilesIntoProgram()
        {
            string basePath = SelectFolderFromSystem("Select the folder named like the Story you want to translate. It has to contain the Translation files, optionally under a folder named after the language");

            if (basePath.Length > 0)
            {
                foreach (string path in Directory.GetDirectories(basePath))
                {
                    string[] folders = path.Split('\\');

                    //get parent folder name
                    string tempName = folders[folders.Length - 1];
                    //get language text representation
                    bool gotLanguage = LanguageHelper.Languages.TryGetValue(TabManager.ActiveTranslationManager.Language, out string languageAsText);
                    //compare
                    if (tempName == languageAsText && gotLanguage)
                    {
                        //get foler one more up
                        basePath = path;
                        break;
                    }
                }

                foreach (string filePath in Directory.GetFiles(basePath))
                {
                    if (Path.GetExtension(filePath) == ".txt")
                    {
                        TabManager.OpenInNewTab(filePath);
                    }
                }
            }
        }

        /// <summary>
        /// Approves the string in the db, if possible. Also updates UI.
        /// </summary>
        /// <param name="SelectNewAfter">A bool to determine if a new string should be selected after approval.</param>
        public void ApproveIfPossible(bool SelectNewAfter)
        {
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;
            if (currentIndex >= 0)
            {
                //UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count.CheckListBoxLeft.Items.Count.ApprovedCountLabel.NoProgressbar);

                string ID = TranslationData[currentIndex].ID;
                DataBaseManager.SetStringTranslation(ID, FileName, StoryName, TranslationData[currentIndex].Category, TranslationData[currentIndex].TranslationString, Language);
                if (!DataBaseManager.SetStringApprovedState(ID, FileName, StoryName, TranslationData[currentIndex].Category, !helper.CheckListBoxLeft.GetItemChecked(currentIndex), Language))
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

                UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count);
                helper.NoProgressbar.Update();
            }
        }

        /// <summary>
        /// Tries to delete the word in let or right ofthe cursor in the currently selected TextBox.
        /// </summary>
        /// <param name="toLeft">true if deleting to the left</param>
        /// <returns>true if successfull</returns>
        public bool DeleteCharactersInText(bool toLeft)
        {
            if (MainWindow.ContainsFocus)
            {
                Control focused_control = MainWindow.ActiveControl;
                try
                {
                    TextBox _ = (TextBox)focused_control;
                }
                //ignore exception, really intended
                catch { return false; }
                TextBox textBox = (TextBox)focused_control;
                if (toLeft)
                {
                    return Utils.DeleteCharactersInTextLeft(textBox);
                }
                else
                {
                    return Utils.DeleteCharactersInTextRight(textBox);
                }
            }
            return false;
        }

        /// <summary>
        /// Moves the cursor to the beginning/end of the next word in the specified direction
        /// </summary>
        /// <param name="toLeft">true if to scan to the left</param>
        /// <returns>true if succeeded</returns>
        public bool MoveCursorInText(bool toLeft)
        {
            if (MainWindow.ContainsFocus)
            {
                Control focused_control = MainWindow.ActiveControl;
                try
                {
                    TextBox _ = (TextBox)focused_control;
                }
                //ignore exception, really intended
                catch { return false; }
                TextBox textBox = (TextBox)focused_control;
                if (toLeft)
                {
                    Utils.MoveCursorWordLeft(textBox);
                    return true;
                }
                else
                {
                    Utils.MoveCursorWordRight(textBox);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Loads a file into the program and calls all helper routines
        /// </summary>
        public void LoadFileIntoProgram()
        {
            LoadFileIntoProgram(SelectFileFromSystem());
        }

        /// <summary>
        /// Loads a file into the program and calls all helper routines
        /// </summary>
        /// <param name="path">The path to the file to translate</param>
        public void LoadFileIntoProgram(string path)
        {
            if (path.Length > 0)
            {
                ShowAutoSaveDialog();
                ResetTranslationManager();

                MainWindow.Cursor = Cursors.WaitCursor;

                if (IsUpToDate)
                {
                    SourceFilePath = path;
                    LoadTranslationFile();
                }
                else
                {
                    string folderPath = SelectTemplateFolderFromSystem();
                    string templateFolderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];
                    if (templateFolderName == "TEMPLATE")
                    {
                        isTemplate = true;
                        LoadAndSyncTemplates(folderPath);
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

                if (TranslationData.Count > 0)
                {
                    //log file loading if successfull
                    LogManager.LogEvent($"File opened: {StoryName}/{FileName} at {DateTime.Now}");

                    //update tab name
                    TabManager.UpdateTabTitle(FileName);

                    //update recents
                    RecentsManager.SetMostRecent(SourceFilePath);
                    RecentsManager.UpdateMenuItems(MainWindow.FileToolStripMenuItem.DropDownItems);
                }
                //reset cursor
                MainWindow.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Populates the Editor/Template text boxes and does some basic set/reset logic.
        /// </summary>
        public void PopulateTextBoxes()
        {
            //todo make smaller/compartmentalize
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
                if (LastIndex != currentIndex && Properties.Settings.Default.autoSave)
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
                    helper.TemplateTextBox.Text = TranslationData[currentIndex].TemplateString.Replace("\n", Environment.NewLine);
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

                    string templateString;
                    //if we have no template here
                    if (TranslationData[currentIndex].TemplateString?.Length == 0 || TranslationData[currentIndex].TemplateString?.Length == null)
                    {
                        //read the template form the db and display it if it exists
                        DataBaseManager.GetStringTemplate(id, FileName, StoryName, out templateString);
                        TranslationData[currentIndex].TemplateString = templateString;
                    }
                    else
                    {
                        templateString = TranslationData[currentIndex].TemplateString;
                    }

                    helper.TemplateTextBox.Text = templateString.Replace("\n", Environment.NewLine);

                    //translate if useful and possible
                    if (helper.TranslationTextBox.Text == helper.TemplateTextBox.Text && !TranslationData[currentIndex].IsTranslated && !TranslationData[currentIndex].IsApproved && helper.TemplateTextBox.Text.Length > 0)
                    {
                        ReplaceTranslationTranslatedTask(currentIndex);
                    }

                    //mark text if similar to english (not translated yet)
                    if (helper.TemplateTextBox.Text == helper.TranslationTextBox.Text && !TranslationData[currentIndex].IsTranslated && !TranslationData[currentIndex].IsApproved)
                    {
                        helper.CheckListBoxLeft.SimilarStringsToEnglish.Add(currentIndex);
                    }
                    else
                    {
                        if (helper.CheckListBoxLeft.SimilarStringsToEnglish.Contains(currentIndex)) helper.CheckListBoxLeft.SimilarStringsToEnglish.Remove(currentIndex);
                    }

                    if (DataBaseManager.GetTranslationComments(id, FileName, StoryName, out string[] comments, Language))
                    {
                        helper.CommentBox.Lines = comments;
                    }

                    helper.ApprovedBox.Checked = helper.CheckListBoxLeft.GetItemChecked(currentIndex);

                    UpdateCharacterCountLabel(helper.TemplateTextBox.Text.Count(), helper.TranslationTextBox.Text.Count());

                    if (UpdateStoryExplorerSelection)
                    {
                        SetHighlightedNode();
                    }
                    else
                    {
                        UpdateStoryExplorerSelection = true;
                    }
                }
            }
            UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count);
            MainWindow.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Replaces a searched string in all applicable lines by the replacement provided using the invariant culture.
        /// </summary>
        /// <param name="replacement">The string to replace all search matches with</param>
        public void Replace(string replacement)
        {
            foreach (var i in helper.CheckListBoxLeft.SearchResults)
            {
                TranslationData[i].TranslationString = Utils.Replace(TranslationData[i].TranslationString, replacement, SearchQuery);
            }
            //update translations also on the database
            DataBaseManager.SetStringSelectedTranslations(TranslationData, FileName, StoryName, Language, helper.CheckListBoxLeft.SearchResults);

            //update search results
            Search();

            //show confirmation
            MessageBox.Show("Replace successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// eplaces the template version of the string with a computer translated one to speed up translation.
        /// </summary>
        /// <param name="currentIndex">The selected index of the string not yet translated</param>
        public async void ReplaceTranslationTranslatedTask(int currentIndex)
        {
            if (Properties.Settings.Default.autoTranslate)
            {
                try
                {
                    string result = "";
                    result = await Translator.TranslateAsync(new Translate()
                    {
                        ApiKey = "",
                        Source = LanguageCode.English,
                        Target = LanguageCode.FromString(Language),
                        Text = TranslationData[currentIndex].TemplateString
                    });
                    if (result.Length > 0)
                    {
                        TranslationData[currentIndex].TranslationString = result;
                        if (currentIndex == helper.CheckListBoxLeft.SelectedIndex && helper.TranslationTextBox.Text == helper.TemplateTextBox.Text)
                        {
                            helper.TranslationTextBox.Text = TranslationData[currentIndex].TranslationString;
                        }
                    }
                }
                catch
                {
                    if (MessageBox.Show("The translator seems to be unavailable. Turn off autotranslation? (needs to be turned back on manually!)", "Turn off autotranslation", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Properties.Settings.Default.autoTranslate = false;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current comment to the db
        /// </summary>
        public void SaveCurrentComment()
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
        public void SaveCurrentString()
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
        public void SaveFile()
        {
            if (SourceFilePath != "" && Language != "")
            {
                //save current string
                SaveCurrentString();

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


                //can take some time
                DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);
                DataBaseManager.GetAllTranslatedStringForFile(FileName, StoryName, out List<LineData> TranslationsFromDatabase, Language);

                foreach (LineData item in IdsToExport)
                {
                    LineData lineDataResult = TranslationsFromDatabase.Find(predicateLine => predicateLine.ID == item.ID);
                    if (lineDataResult != null)
                    {
                        //add translation to the list in the correct category if present
                        int intCategory = CategoriesInFile.FindIndex(predicateCategory => predicateCategory == item.Category);
                        CategorizedStrings[intCategory].Item1.Add(lineDataResult);
                    }
                    else// if id is not found
                    {
                        //add template to list if no translation is in the file
                        LineData TempResult = TranslationData.Find(pL => pL.ID == item.ID);

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

                StreamWriter OutputWriter = new StreamWriter(SourceFilePath, false, new UTF8Encoding(true));

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

                //copy file to game rather than writing again
                if (Properties.Settings.Default.alsoSaveToGame)
                {
                    //get language path
                    LanguageHelper.Languages.TryGetValue(Language, out string languageAsText);
                    //add new to langauge if wanted
                    if (Properties.Settings.Default.useFalseFolder)
                    {
                        languageAsText += " new";
                    }

                    //create path to file
                    string gameFilePath = "Eek\\House Party\\Mods\\";
                    if (StoryName != "Hints" && StoryName != "UI")
                    {
                        //combine all paths
                        gameFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), gameFilePath, "Languages", StoryName, languageAsText, FileName + ".txt");
                    }
                    else if (StoryName == "UI")
                    {
                        //combine all paths
                        gameFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), gameFilePath, "Languages", languageAsText, FileName + ".txt");
                    }
                    else
                    {
                        //combine all paths
                        gameFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), gameFilePath, "Hints", FileName + ".txt");
                    }

                    //copy file if we are not already in it lol
                    if (gameFilePath != SourceFilePath)
                    {
                        if (File.Exists(gameFilePath))
                        {
                            File.Copy(SourceFilePath, gameFilePath, true);
                        }
                        else
                        {
                            //create file if it does not exist, as well as all folders
                            Directory.CreateDirectory(Path.GetDirectoryName(gameFilePath));
                            File.Copy(SourceFilePath, gameFilePath, true);
                        }
                    }
                }

                ChangesPending = false;

                MainWindow.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves all strings to a specified file location.
        /// </summary>
        public void SaveFileAs()
        {
            if (SourceFilePath != "")
            {
                isSaveAs = true;
                string oldFile = SourceFilePath;
                string SaveFile = SaveFileOnSystem();
                SourceFilePath = SaveFile;
                this.SaveFile();
                SourceFilePath = oldFile;
                isSaveAs = false;
            }
        }

        /// <summary>
        /// Opens a save file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the file to save to.</returns>
        public string SaveFileOnSystem()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Choose a file to save the translations to",
                AddExtension = true,
                DefaultExt = "txt",
                CheckPathExists = true,
                CreatePrompt = true,
                OverwritePrompt = true,
                FileName = FileName,
                InitialDirectory = SourceFilePath
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return "";
        }

        /// <summary>
        /// Performs a search through all lines currently loaded.
        /// </summary>
        public void Search()
        {
            SearchQuery = helper.SearchBox.Text;
            Search(helper.SearchBox.Text);
        }

        /// <summary>
        /// Performs a search through all lines currently loaded.
        /// </summary>
        /// <param name="query">The search temr to look for</param>
        public void Search(string query)
        {
            //reset list if no search is performed
            if (query.Length > 0)
            {
                //clear results
                helper.CheckListBoxLeft.SearchResults.Clear();

                //decide on case sensitivity
                if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
                {
                    query = query.Substring(1);
                    //methodolgy: highlight items which fulfill search and show count
                    for (int i = 0; i < TranslationData.Count; i++)
                    {
                        if (TranslationData[i].TranslationString.Contains(query) /*if the translated text contaisn the search string*/
                            || (TranslationData[i].TemplateString != null
                            && TranslationData[i].TemplateString.Contains(query))/*if the english string is not null and contaisn the searched part*/
                            || TranslationData[i].ID.Contains(query))/*if the id contains the searched part*/
                        {
                            helper.CheckListBoxLeft.SearchResults.Add(i);//add index to highligh list
                        }
                    }
                }
                else if (query[0] != '!')
                {
                    if (query[0] == '\\') // we have an escaped flag following, so we chop of escaper and continue
                    {
                        query = query.Substring(1);
                    }
                    //methodolgy: highlight items which fulfill search and show count
                    for (int i = 0; i < TranslationData.Count; i++)
                    {
                        if (TranslationData[i].TranslationString.ToLowerInvariant().Contains(query.ToLowerInvariant()) /*if the translated text contaisn the search string*/
                            || (TranslationData[i].TemplateString != null
                            && TranslationData[i].TemplateString.ToLowerInvariant().Contains(query.ToLowerInvariant()))/*if the english string is not null and contaisn the searched part*/
                            || TranslationData[i].ID.ToLowerInvariant().Contains(query.ToLowerInvariant()))/*if the id contains the searched part*/
                        {
                            helper.CheckListBoxLeft.SearchResults.Add(i);//add index to highligh list
                        }
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
        /// Selects a string in the listview given the id
        /// </summary>
        /// <param name="id">The id to look for.</param>
        public void SelectLine(string id)
        {
            //select line which correspondends to id
            int index = TranslationData.FindIndex(n => n.ID == id);
            if (index >= 0) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = index;
        }

        /// <summary>
        /// Selects the index given in the list of strings
        /// </summary>
        /// <param name="index">The index to select</param>
        public void SelectLine(int index)
        {
            if (index >= 0 && index < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = index;
        }

        /// <summary>
        /// Sets the language the translation is associated with
        /// </summary>
        public void SetLanguage()
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
        /// Sets the main form this translationmanager will work on (cursor, fields, etc)
        /// </summary>
        /// <param name="mainWindow">The form to work on.</param>
        public void SetMainForm(Fenster mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Shows a save all changes dialogue (intended to be used before quit) if settings allow.
        /// </summary>
        public void ShowAutoSaveDialog()
        {
            if (Properties.Settings.Default.askForSaveDialog && ChangesPending)
            {
                if (MessageBox.Show("You may have unsaved changes. Do you want to save all changes?", "Save changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    if (!TabManager.SaveAllTabs())
                    {
                        SaveFile();
                    }
                }
            }
        }

        /// <summary>
        /// Update the currently selected translation string in the TranslationData.
        /// </summary>
        public void UpdateTranslationString()
        {
            int internalIndex = helper.CheckListBoxLeft.SelectedIndex;
            if (internalIndex >= 0)
            {
                //remove pipe to not break saving/export
                helper.TranslationTextBox.Text.Replace('|', ' ');
                TranslationData[internalIndex].TranslationString = helper.TranslationTextBox.Text.Replace(Environment.NewLine, "\n");
                UpdateCharacterCountLabel(helper.TemplateTextBox.Text.Count(), helper.TranslationTextBox.Text.Count());
                ChangesPending = true;
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
                                        ReadInStringsFromFile();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the templates by combining all lines from all files into one, then sending them one by one to the db.
        /// </summary>
        /// <param name="folderPath">The path to the folders to load the templates from.</param>
        private void LoadAndSyncTemplates(string folderPath)
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
                if (lineD.TemplateString != "")
                {
                    DataBaseManager.SetStringTemplate(lineD.ID, lineD.FileName, lineD.Story, lineD.Category, lineD.TemplateString);
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
        /// Prepares the values for reading of the strings, and calls the methods necessary after successfully loading a file.
        /// </summary>
        private void LoadTranslationFile()
        {
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
                    //get folder one more up
                    tempStoryName = paths[paths.Length - 3];
                }

                if (tempStoryName == "Languages")
                {
                    //get foler one more up
                    tempStoryName = "UI";
                }

                StoryName = tempStoryName;

                //actually load all strings into the program
                ReadInStringsFromFile();

                if (TranslationData.Count() > 0)
                {
                    string storyNameToDisplay = Utils.TrimWithDelim(StoryName);
                    string fileNameToDisplay = Utils.TrimWithDelim(FileName);
                    helper.SelectedFileLabel.Text = $"File: {storyNameToDisplay}/{fileNameToDisplay}.txt";

                    //is up to date, so we can start translation
                    LoadTranslations();
                    UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count);
                }
            }
        }

        /// <summary>
        /// Loads the strings and does some work around to ensure smooth sailing.
        /// </summary>
        private void LoadTranslations()
        {
            MainWindow.Cursor = Cursors.WaitCursor;

            bool lineIsApproved = false;
            bool gotApprovedStates = DataBaseManager.GetAllStatesForFile(FileName, StoryName, out List<LineData> internalLines, Language);
            int currentIndex = 0;

            foreach (LineData lineD in TranslationData)
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
            //reload once so the order of lines is correct after we fixed an empty or broken file
            if (triedFixingOnce)
            {
                triedFixingOnce = false;
                ReloadFile();
            }

            MainWindow.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Reads the strings depending on whether its a template or not.
        /// </summary>
        private void ReadInStringsFromFile()
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
            string[] lastLine = { };

            DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);
            List<string> LinesFromFile;
            try
            {
                //read in lines
                LinesFromFile = File.ReadAllLines(SourceFilePath).ToList();
            }
            catch (Exception e)
            {
                LogManager.LogEvent($"File not found under {SourceFilePath}.\n{e}");
                MessageBox.Show($"File not found under {SourceFilePath}. Please reopen.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetTranslationManager();
                return;
            }

            //if we got lines at all
            if (LinesFromFile.Count > 0)
            {
                SplitReadTranslations(LinesFromFile, lastLine, currentCategory, IdsToExport);
            }
            else
            {
                TryFixEmptyFile();
            }

            if (lastLine.Length > 0)
            {
                //add last line (dont care about duplicates because sql will get rid of them)
                TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1]));
            }

            //set categories if file is a hint file
            if (StoryName == "Hints") CategoriesInFile = new List<StringCategory>() { StringCategory.General };

            if (IdsToExport.Count != TranslationData.Count)
            {
                if (TranslationData.Count == 0)
                {
                    TryFixEmptyFile();
                }
                else if (!SoftwareVersionManager.UpdatePending && Form.ActiveForm != null)
                    //inform user the issing translations will be added after export. i see no viable way to add them before having them all read in,
                    //and it would take a lot o time to get them all. so just have the user save it once and reload. we might do this automatically, but i don't know if that is ok to do :)
                    MessageBox.Show(
                    "Some strings are missing from your translation, they will be added with the english version when you first save the file!",
                    "Some strings missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
            }
        }

        private void SplitReadTranslations(List<string> LinesFromFile, string[] lastLine, StringCategory category, List<LineData> IdsToExport)
        {
            string multiLineCollector = "";
            //remove last if empty, breaks line lioading for the last
            while (LinesFromFile.Count > 0)
            {
                if (LinesFromFile.Last() == "")
                    LinesFromFile.RemoveAt(LinesFromFile.Count - 1);
                else break;
            }
            //load lines and their data and split accordingly
            foreach (string line in LinesFromFile)
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0)
                    {
                        TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, category, IdsToExport.Find(p => p.ID == lastLine[0])?.TemplateString, lastLine[1] + multiLineCollector));
                    }
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
                                      category,
                                      IdsToExport.Find(p => p.ID == lastLine[0])?.TemplateString,
                                      lastLine[1] + multiLineCollector.Remove(multiLineCollector.Length - 2, 1)));
                            }
                            else
                            {//write last line with id if no real line of text is afterwards
                                TranslationData.Add(
                                  new LineData(
                                      lastLine[0],
                                      StoryName,
                                      FileName,
                                      category,
                                      IdsToExport.Find(p => p.ID == lastLine[0])?.TemplateString,
                                      lastLine[1]));
                            }
                        }
                        lastLine = new string[0];
                        multiLineCollector = "";
                        category = tempCategory;
                        CategoriesInFile.Add(category);
                    }
                }
            }
        }

        private void TryFixEmptyFile()
        {
            if (!triedFixingOnce)
            {
                triedFixingOnce = true;
                DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);
                foreach (var item in IdsToExport)
                {
                    TranslationData.Add(new LineData(item.ID, StoryName, FileName, item.Category));
                }
                SaveFile();
                ReadStringsTranslationsFromFile();
            }
        }

        /// <summary>
        /// Reloads the file into the program as if it were selected.
        /// </summary>
        internal void ReloadFile()
        {
            ResetTranslationManager();
            ReadStringsTranslationsFromFile();
            LoadTranslations();
            //select recent index
            if (Properties.Settings.Default.recent_index > 0 && Properties.Settings.Default.recent_index < TranslationData.Count) helper.CheckListBoxLeft.SelectedIndex = Properties.Settings.Default.recent_index;
        }

        /// <summary>
        /// Resets the translation manager.
        /// </summary>
        private void ResetTranslationManager()
        {
            Properties.Settings.Default.recent_index = helper.CheckListBoxLeft.SelectedIndex;
            TranslationData.Clear();
            helper.CheckListBoxLeft.Items.Clear();
            CategoriesInFile.Clear();
            helper.CheckListBoxLeft.SimilarStringsToEnglish.Clear();
            LastIndex = -1;
            SelectedSearchResult = 0;
            TabManager.TabControl.SelectedTab.Text = "Tab";
            UpdateApprovedCountLabel(1, 1);
        }

        /// <summary>
        /// Selects the next search result if applicable
        /// </summary>
        /// <returns>True if a new result could be selected</returns>
        internal bool SelectNextResultIfApplicable()
        {
            if (!helper.TranslationTextBox.Focused && !helper.CommentBox.Focused && helper.CheckListBoxLeft.SearchResults.Any())
            {
                //loop back to start
                if (SelectedSearchResult > helper.CheckListBoxLeft.SearchResults.Count - 1)
                {
                    SelectedSearchResult = 0;
                    //loop over to new tab when in global search
                    if (TabManager.InGlobalSearch)
                    {
                        searchTabIndex = TabManager.TabControl.SelectedIndex;
                        TabManager.SwitchToTab(++searchTabIndex);
                    }
                    else
                    {
                        //select next index from list of matches
                        if (helper.CheckListBoxLeft.SearchResults[SelectedSearchResult] < helper.CheckListBoxLeft.Items.Count)
                        {
                            helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult];
                        }
                    }
                }
                else
                {
                    if (helper.CheckListBoxLeft.SearchResults[SelectedSearchResult] < helper.CheckListBoxLeft.Items.Count)
                    {
                        helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult++];
                    }
                    else
                    {
                        SelectedSearchResult = 0;
                        helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult++];
                    }
                }

                return true;
            }
            else if (TabManager.InGlobalSearch && TabManager.TabControl.TabCount > 1)
            {
                searchTabIndex = TabManager.TabControl.SelectedIndex;
                TabManager.SwitchToTab(++searchTabIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the node whose tree gets highlighted to the one representing the currently selected string;
        /// </summary>
        /// <param name="helper">A Propertyhelper to get access to the form controls.</param>
        public void SetHighlightedNode()
        {
            if (TranslationData.Count > 0)
            {
                int currentIndex = helper.CheckListBoxLeft.SelectedIndex;
                string id = currentIndex < TranslationData.Count && currentIndex >= 0 ? TranslationData[currentIndex].ID : TranslationData[0].ID;
                //Highlights the node representign the selected string in the story explorer window
                if (MainWindow.Explorer != null && !MainWindow.Explorer.IsDisposed)
                {
                    MainWindow.Explorer.Grapher.HighlightedNode = MainWindow.Explorer.Grapher.Context.Nodes.Find(n => n.ID == id);
                }
            }
        }

        /// <summary>
        /// Does some logic to figure out wether to show or hide the replacing ui
        /// </summary>
        internal void ToggleReplaceUI()
        {
            if (!helper.ReplaceBox.Visible)
            {
                if (helper.TranslationTextBox.SelectedText.Length > 0)
                {
                    helper.SearchBox.Text = helper.TranslationTextBox.SelectedText;
                }
                helper.ReplaceBox.Visible = true;
                helper.ReplaceButton.Visible = true;

                //set focus to most needed text box, search first
                if (helper.SearchBox.Text.Length > 0) helper.ReplaceBox.Focus();
                else helper.SearchBox.Focus();
            }
            else
            {
                helper.ReplaceButton.Visible = false;
                helper.ReplaceBox.Visible = false;
                helper.TranslationTextBox.Focus();
            }
        }

        /// <summary>
        /// Updates the label schowing the number lines approved so far
        /// </summary>
        /// <param name="Approved">The number of approved strings.</param>
        /// <param name="Total">The total number of strings.</param>
        private void UpdateApprovedCountLabel(int Approved, int Total)
        {
            float percentage = Approved / (float)Total;
            helper.ApprovedCountLabel.Text = $"Approved: {Approved} / {Total} {(int)(percentage * 100)}%";
            int ProgressValue = (int)(Approved / (float)Total * 100);
            if (ProgressValue != helper.NoProgressbar.Value)
            {
                if (ProgressValue > 0 && ProgressValue <= 100)
                {
                    helper.NoProgressbar.Value = ProgressValue;
                }
                else
                {
                    helper.NoProgressbar.Value = 0;
                }
                helper.NoProgressbar.Invalidate();
            }
        }

        /// <summary>
        /// Updates the label displaying the character count
        /// </summary>
        /// <param name="TemplateCount">The number of chars in the template string.</param>
        /// <param name="TranslationCount">The number of chars in the translated string.</param>
        private void UpdateCharacterCountLabel(int TemplateCount, int TranslationCount)
        {
            if (TranslationCount <= TemplateCount)
            {
                helper.CharacterCountLabel.ForeColor = Color.LawnGreen;
            }//if bigger by no more than 20 percent
            else if (TranslationCount <= TemplateCount * 1.2f)
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
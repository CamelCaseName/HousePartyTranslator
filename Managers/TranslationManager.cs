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
//todo add indicator for unsaved changes
//todo still save file even when offline

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A class providing functions for loading, approving, and working with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>
    internal class TranslationManager
    {
        public static bool ChangesPending = false;
        public static bool IsUpToDate = false;
        public List<StringCategory> CategoriesInFile = new List<StringCategory>();
        public bool isTemplate = false;
        public string SearchQuery = "";

        public static Timer AutoSaveTimer = new Timer();

        public int SelectedSearchResult = 0;


        //counter so we dont get multiple ids, we dont use the dictionary as ids anyways when uploading templates
        private int templateCounter = 0;

        //setting?
        public Dictionary<string, LineData> TranslationData = new Dictionary<string, LineData>();
        public bool UpdateStoryExplorerSelection = true;
        private static readonly LibreTranslate.Net.LibreTranslate Translator = new LibreTranslate.Net.LibreTranslate("https://translate.rinderha.cc");
        private static Fenster MainWindow;
        private string fileName = "";
        private bool isSaveAs = false;
        private string language = "";
        private int searchTabIndex = 0;
        private string sourceFilePath = "";
        private string storyName = "";
        private readonly PropertyHelper helper;
        private bool triedFixingOnce = false;
        private bool triedSavingFixOnce = false;

        /// <summary>
        /// The Constructor for this class. Takes no arguments.
        /// </summary>
        public TranslationManager(PropertyHelper _helper)
        {
            this.helper = _helper;
            AutoSaveTimer.Tick += SaveFileHandler;
        }

        static TranslationManager()
        {
            if (Properties.Settings.Default.AutoSaveInterval > TimeSpan.FromMinutes(1))
            {
                AutoSaveTimer.Interval = (int)Properties.Settings.Default.AutoSaveInterval.TotalMilliseconds;
                AutoSaveTimer.Start();
            }
        }

        internal void SaveFileHandler(object sender, EventArgs e)
        {
            SaveFile();
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
                    storyName = value;
                    //custom story?
                    if (!CustomStoryTemplateHandle(value))
                    {
                        MessageBox.Show($"Not flagged as custom story, can't find \"{value}\", assuming Original Story.");
                        storyName = "Original Story";
                    }
                }
            }
        }

        public bool CustomStoryTemplateHandle(string story)
        {
            DialogResult result;
            if (Properties.Settings.Default.EnableCustomStories)
            {
                result = DialogResult.Yes;
            }
            else
            {
                result = MessageBox.Show($"Detected {story} as the story to use, if this is a custom story you want to translate, you can do so. Choose yes if you want to do that. If not, select no and we will assume this is the Original Story", "Custom story?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            }
            if (result == DialogResult.Yes)
            {
                //check if the template has been added and generated once before
                DataBase.GetAllLineDataTemplate(fileName, story, out var data);
                if (data.Count == 0)
                {//its a custom story but no template so far on the server
                 //use contextprovider to extract the story object and get the lines
                    result = MessageBox.Show($"You will now be prompted to select the corresponding .story or .character file for the translation you want to do. Is {FileName} a character?", "Custom story?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result != DialogResult.Cancel)
                    {
                        bool isStory = result == DialogResult.No;
                        ContextProvider explorer = new ContextProvider(isStory, false, FileName, StoryName, null);
                        var nodes = explorer.GetTemplateNodes();
                        if (nodes != null)
                        {
                            Application.UseWaitCursor = true;
                            //remove old lines from server
                            DataBase.RemoveOldTemplates(FileName, story);

                            Dictionary<string, LineData> templates = new Dictionary<string, LineData>();

                            //add name as first template (its not in the file)
                            templates.Add("Name", new LineData("Name", story, FileName, StringCategory.General, FileName));

                            //Add all new lines, but check if they are relevant
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                //filter out irrelevant nodes
                                if (!((int.TryParse(nodes[i].Text, out _) || nodes[i].Text.Length < 2) && nodes[i].Type == NodeType.Event)
                                    && Utils.CategoryFromNode(nodes[i].Type) != StringCategory.Neither
                                    && nodes[i].ID != "")
                                {
                                    templates.Add(nodes[i].ID, new LineData(nodes[i].ID, story, FileName, Utils.CategoryFromNode(nodes[i].Type), nodes[i].Text, true));
                                }
                            }

                            DataBase.UploadTemplates(templates);

                            Application.UseWaitCursor = false;
                            return true;
                        }
                        MessageBox.Show("Something broke, please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return true;
                }
            }

            Application.UseWaitCursor = false;
            return false;
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
            string basePath = Utils.SelectFolderFromSystem("Select the folder named like the Story you want to translate. It has to contain the Translation files, optionally under a folder named after the language");

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
                //set checkbox state
                helper.ApprovedBox.Checked = !helper.CheckListBoxLeft.GetItemChecked(currentIndex);
                TranslationData[SelectedId].IsApproved = helper.ApprovedBox.Checked;

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
        public void LoadFileIntoProgram(DiscordPresenceManager presenceManager)
        {
            LoadFileIntoProgram(Utils.SelectFileFromSystem(), presenceManager);
        }

        /// <summary>
        /// Loads a file into the program and calls all helper routines
        /// </summary>
        /// <param name="path">The path to the file to translate</param>
        public void LoadFileIntoProgram(string path, DiscordPresenceManager presenceManager)
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
                    LoadTemplates();
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

                    //update presence and recents
                    presenceManager.Update(StoryName, FileName);
                }
                //reset cursor
                MainWindow.Cursor = Cursors.Default;
            }
        }

        private void LoadTemplates()
        {
            string folderPath = Utils.SelectTemplateFolderFromSystem();
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

        private void MarkSimilarLine(int currentIndex)
        {
            if (helper.TemplateTextBox.Text == helper.TranslationTextBox.Text && !TranslationData[SelectedId].IsTranslated && !TranslationData[SelectedId].IsApproved)
            {
                helper.CheckListBoxLeft.SimilarStringsToEnglish.Add(currentIndex);
            }
            else
            {
                if (helper.CheckListBoxLeft.SimilarStringsToEnglish.Contains(currentIndex)) helper.CheckListBoxLeft.SimilarStringsToEnglish.Remove(currentIndex);
            }
        }

        private void UpdateStoryExplorerNode()
        {
            if (UpdateStoryExplorerSelection)
            {
                SetHighlightedNode();
            }
            else
            {
                UpdateStoryExplorerSelection = true;
            }
        }

        /// <summary>
        /// Populates the Editor/Template text boxes and does some basic set/reset logic.
        /// </summary>
        public void PopulateTextBoxes()
        {
            MainWindow.Cursor = Cursors.WaitCursor;
            int currentIndex = helper.CheckListBoxLeft.SelectedIndex;

            if (currentIndex >= 0)
            {
                if (isTemplate)
                {
                    helper.TemplateTextBox.Text = TranslationData[SelectedId].TemplateString.Replace("\n", Environment.NewLine);
                }
                else
                {
                    helper.TemplateTextBox.Text = TranslationData[SelectedId].TemplateString?.Replace("\n", Environment.NewLine);

                    //display the string in the editable window
                    helper.TranslationTextBox.Text = TranslationData[SelectedId].TranslationString.Replace("\n", Environment.NewLine);

                    //translate if useful and possible
                    if (helper.TranslationTextBox.Text == helper.TemplateTextBox.Text && !TranslationData[SelectedId].IsTranslated && !TranslationData[SelectedId].IsApproved && helper.TemplateTextBox.Text.Length > 0)
                        ReplaceTranslationTranslatedTask(SelectedId);

                    //mark text if similar to english (not translated yet)
                    MarkSimilarLine(currentIndex);

                    helper.CommentBox.Lines = TranslationData[SelectedId].Comments;

                    //sync approvedbox and list
                    helper.ApprovedBox.Checked = helper.CheckListBoxLeft.GetItemChecked(currentIndex);

                    //update label
                    UpdateCharacterCountLabel(helper.TemplateTextBox.Text.Count(), helper.TranslationTextBox.Text.Count());

                    //update explorer
                    UpdateStoryExplorerNode();

                    //renew search result if possible
                    int t = helper.CheckListBoxLeft.SearchResults.IndexOf(currentIndex);
                    if (t >= 0) { SelectedSearchResult = t; }
                }
            }
            UpdateApprovedCountLabel(helper.CheckListBoxLeft.CheckedIndices.Count, helper.CheckListBoxLeft.Items.Count);
            MainWindow.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Replaces a searched string in all applicable lines by the replacement provided using the invariant culture.
        /// </summary>
        /// <param name="replacement">The string to replace all search matches with</param>
        public void ReplaceAll(string replacement)
        {
            var old = TranslationData;

            foreach (var i in helper.CheckListBoxLeft.SearchResults)
            {
                TranslationData[helper.CheckListBoxLeft.Items[i].ToString()].TranslationString = Utils.Replace(TranslationData[helper.CheckListBoxLeft.Items[i].ToString()].TranslationString, replacement, SearchQuery);
            }

            History.AddAction(new AllTranslationsChanged(this, old, TranslationData));

            //update search results
            Search();

            //update textbox
            ReloadTranslationTextbox();

            //show confirmation
            MessageBox.Show("Replace successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Replaces a searched string in the selected line if it is a search result by the replacement provided using the invariant culture.
        /// </summary>
        /// <param name="replacement">The string to replace all search matches with</param>
        public void ReplaceSingle(string replacement)
        {
            int i = helper.CheckListBoxLeft.SelectedIndex;
            if (helper.CheckListBoxLeft.SearchResults.Contains(i))
            {
                var temp = Utils.Replace(TranslationData[SelectedId].TranslationString, replacement, SearchQuery);
                History.AddAction(new TranslationChanged(this, SelectedId, TranslationData[SelectedId].TranslationString, temp));
                TranslationData[SelectedId].TranslationString = temp;

                //update search results
                Search();

                //update textbox
                ReloadTranslationTextbox();
            }
        }

        public void ReloadTranslationTextbox()
        {
            //update textbox
            helper.TranslationTextBox.Text = TranslationData[SelectedId].TranslationString.Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// eplaces the template version of the string with a computer translated one to speed up translation.
        /// </summary>
        /// <param name="currentIndex">The selected index of the string not yet translated</param>
        public async void ReplaceTranslationTranslatedTask(string currentId)
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
                        Text = TranslationData[currentId].TemplateString
                    });
                    if (result.Length > 0)
                    {
                        TranslationData[currentId].TranslationString = result;
                        if (currentId == SelectedId && helper.TranslationTextBox.Text == helper.TemplateTextBox.Text)
                        {
                            helper.TranslationTextBox.Text = TranslationData[currentId].TranslationString;
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
                DataBase.UpdateTranslation(TranslationData[SelectedId], Language);

                if (helper.CheckListBoxLeft.SimilarStringsToEnglish.Contains(currentIndex)) helper.CheckListBoxLeft.SimilarStringsToEnglish.Remove(currentIndex);

                MainWindow.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Saves all strings to the file we read from.
        /// </summary>
        public void SaveFile()
        {
            if (!Fenster.ProgressbarWindow.Visible) Fenster.ProgressbarWindow.Show(MainWindow);
            Fenster.ProgressbarWindow.Focus();
            Fenster.ProgressbarWindow.ProgressBar.Value = 10;
            MainWindow.Cursor = Cursors.WaitCursor;

            if (SourceFilePath == "" || Language == "") return;


            DataBase.UpdateTranslations(TranslationData, Language);

            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
            var CategorizedStrings = new List<CategorizedLines>();

            //we need to check whether the file has any strings at all, expecially the categories, if no, add them first or shit breaks.
            if (CategoriesInFile.Count == 0)
            {
                GenerateCategories();
            }

            foreach (StringCategory category in CategoriesInFile)
            {//add a list for every category we have in the file, so we can then add the strings to these.
                CategorizedStrings.Add((new List<LineData>(), category));
            }

            //can take some time
            DataBase.GetAllLineDataTemplate(FileName, StoryName, out Dictionary<string, LineData> IdsToExport);

            foreach (LineData item in IdsToExport.Values)
            {
                //add template to list if no translation is in the file
                if (!TranslationData.TryGetValue(item.ID, out var TempResult))
                {
                    item.TranslationString = IdsToExport[item.ID].TemplateString;
                }
                else
                {
                    item.TranslationString = TempResult.TranslationString;
                }

                int intCategory = CategoriesInFile.FindIndex(predicateCategory => predicateCategory == item.Category);

                if (intCategory < CategorizedStrings.Count && intCategory >= 0)
                    CategorizedStrings[intCategory].lines.Add(item);
                else if (!triedSavingFixOnce)
                {
                    triedSavingFixOnce = true;
                    GenerateCategories();
                    SaveFile();
                    triedSavingFixOnce = false;
                }
                else
                {
                    CategorizedStrings.Add((new List<LineData>(), StringCategory.Neither));
                    CategorizedStrings.Last().lines.Add(item);
                }
            }

            StreamWriter OutputWriter = new StreamWriter(SourceFilePath, false, new UTF8Encoding(true));

            foreach (var CategorizedLines in CategorizedStrings)
            {
                //write category if it has any lines, else we skip the category
                if (CategorizedLines.lines.Count > 0) OutputWriter.WriteLine(GetStringFromCategory(CategorizedLines.category));
                else continue;

                //sort strings depending on category
                if (CategorizedLines.category == StringCategory.Dialogue)
                {
                    CategorizedLines.lines.Sort((line1, line2) => decimal.Parse(line1.ID, culture).CompareTo(decimal.Parse(line2.ID, culture)));
                }
                else if (CategorizedLines.category == StringCategory.BGC)
                {
                    //sort using custom IComparer
                    CategorizedLines.lines.Sort(new BGCComparer());
                }
                else if (CategorizedLines.category == StringCategory.General)
                {
                    //hints have to be sortet a bit different because the numbers can contain a u
                    CategorizedLines.lines.Sort(new GeneralComparer());
                }
                else if (CategorizedLines.category == StringCategory.Quest || CategorizedLines.category == StringCategory.Achievement)
                {
                    CategorizedLines.lines.Sort((line1, line2) => line2.ID.CompareTo(line1.ID));
                }

                //iterate through each and print them
                foreach (LineData line in CategorizedLines.lines)
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
            Fenster.ProgressbarWindow.Hide();
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
                    int x = 0;
                    foreach (var item in TranslationData.Values)
                    {
                        if (item.TranslationString.Contains(query) /*if the translated text contaisn the search string*/
                            || (item.TemplateString != null
                            && item.TemplateString.Contains(query))/*if the english string is not null and contaisn the searched part*/
                            || item.ID.Contains(query))/*if the id contains the searched part*/
                        {
                            helper.CheckListBoxLeft.SearchResults.Add(x);//add index to highligh list
                        }
                        ++x;
                    }
                }
                else if (query[0] != '!')
                {
                    if (query[0] == '\\') // we have an escaped flag following, so we chop of escaper and continue
                    {
                        query = query.Substring(1);
                    }
                    //methodolgy: highlight items which fulfill search and show count
                    int x = 0;
                    foreach (var item in TranslationData.Values)
                    {
                        if (item.TranslationString.ToLowerInvariant().Contains(query.ToLowerInvariant()) /*if the translated text contaisn the search string*/
                            || (item.TemplateString != null
                            && item.TemplateString.ToLowerInvariant().Contains(query.ToLowerInvariant()))/*if the english string is not null and contaisn the searched part*/
                            || item.ID.ToLowerInvariant().Contains(query.ToLowerInvariant()))/*if the id contains the searched part*/
                        {
                            helper.CheckListBoxLeft.SearchResults.Add(x);//add index to highligh list
                        }
                        ++x;
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
            foreach (var item in helper.CheckListBoxLeft.Items)
            {
                if ((string)item == id)
                {
                    helper.CheckListBoxLeft.SelectedItem = item;
                    break;
                }
            };
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
            //remove pipe to not break saving/export
            helper.TranslationTextBox.Text.Replace('|', ' ');
            TranslationData[SelectedId].TranslationString = helper.TranslationTextBox.Text.Replace(Environment.NewLine, "\n");
            UpdateCharacterCountLabel(helper.TemplateTextBox.Text.Count(), helper.TranslationTextBox.Text.Count());
            ChangesPending = true;
        }

        public void UpdateComments()
        {
            //remove pipe to not break saving/export
            TranslationData[SelectedId].Comments = helper.CommentBox.Lines;
            ChangesPending = true;
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
            if (line.Contains("["))
            {
                if (line == "[General]")
                {
                    return StringCategory.General;
                }
                else if (line == "[Dialogues]")
                {
                    return StringCategory.Dialogue;
                }
                else if (line == "[Responses]")
                {
                    return StringCategory.Response;
                }
                else if (line == "[Quests]")
                {
                    return StringCategory.Quest;
                }
                else if (line == "[Events]")
                {
                    return StringCategory.Event;
                }
                else if (line == "[Background Chatter]")
                {
                    return StringCategory.BGC;
                }
                else if (line == "[Item Names]")
                {
                    return StringCategory.ItemName;
                }
                else if (line == "[Item Actions]")
                {
                    return StringCategory.ItemAction;
                }
                else if (line == "[Item Group Actions]")
                {
                    return StringCategory.ItemGroupAction;
                }
                else if (line == "[Achievements]")
                {
                    return StringCategory.Achievement;
                }
            }
            return StringCategory.Neither;
        }

        /// <summary>
        /// Returns the string representatio of a category.
        /// </summary>
        /// <param name="category">The Category to parse.</param>
        /// <returns>The string representing the category.</returns>
        private string GetStringFromCategory(StringCategory category)
        {
            switch (category)
            {
                case StringCategory.General:
                    return "[General]";

                case StringCategory.Dialogue:
                    return "[Dialogues]";

                case StringCategory.Response:
                    return "[Responses]";

                case StringCategory.Quest:
                    return "[Quests]";

                case StringCategory.Event:
                    return "[Events]";

                case StringCategory.BGC:
                    return "[Background Chatter]";

                case StringCategory.ItemName:
                    return "[Item Names]";

                case StringCategory.ItemAction:
                    return "[Item Actions]";

                case StringCategory.ItemGroupAction:
                    return "[Item Group Actions]";

                case StringCategory.Achievement:
                    return "[Achievements]";

                case StringCategory.Neither:
                    //do nothing hehehehe
                    return "";

                default:
                    //do nothing hehehehe
                    return "";
            }
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
                LogManager.LogEvent(e.ToString(), LogManager.Level.Warning);
                Utils.DisplayExceptionMessage(e.ToString());
            }

            //remove old lines from server
            DataBase.RemoveOldTemplates(StoryName);

            //add all the new strings
            DataBase.UploadTemplates(TranslationData);

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
                if (DataBase.UpdateDBVersion())
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
            TranslationData.Clear();
            helper.CheckListBoxLeft.Items.Clear();
            if (SourceFilePath != "")
            {
                string[] paths = SourceFilePath.Split('\\');

                //get parent folder name
                string tempStoryName = paths[paths.Length - 2];
                //get language text representation
                bool gotLanguage = LanguageHelper.Languages.TryGetValue(Language, out string languageAsText);
                //compare
                if ((tempStoryName == languageAsText || tempStoryName == (languageAsText + " new")) && gotLanguage)
                    //get folder one more up
                    tempStoryName = paths[paths.Length - 3];

                if (tempStoryName == "Languages")
                {
                    //get folder one more up
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
                //update tab name
                TabManager.UpdateTabTitle(FileName);
            }
        }

        /// <summary>
        /// Loads the strings and does some work around to ensure smooth sailing.
        /// </summary>
        private void LoadTranslations()
        {
            MainWindow.Cursor = Cursors.WaitCursor;

            bool lineIsApproved = false;
            DataBase.GetAllLineData(FileName, StoryName, out Dictionary<string, LineData> onlineLines, Language);
            int currentIndex = 0;

            foreach (var key in TranslationData.Keys)
            {
                if (onlineLines.TryGetValue(key, out var tempLine))
                {
                    lineIsApproved = tempLine.IsApproved;
                    TranslationData[key].Category = tempLine.Category;
                    TranslationData[key].Comments = tempLine.Comments;
                    TranslationData[key].FileName = tempLine.FileName;
                    TranslationData[key].ID = tempLine.ID;
                    TranslationData[key].IsTemplate = false;
                    TranslationData[key].IsTranslated = tempLine.IsTranslated;
                    TranslationData[key].Story = tempLine.Story;
                    TranslationData[key].TranslationString = tempLine.TranslationString;
                    TranslationData[key].IsApproved = false;
                }

                if (TranslationData[key].TemplateString == null) TranslationData[key].TemplateString = "";

                helper.CheckListBoxLeft.Items.Add(TranslationData[key].ID, lineIsApproved);

                //do after adding or it will trigger reset
                TranslationData[key].IsApproved = lineIsApproved;

                //colour string if similar to the english one
                if (!TranslationData[key].IsTranslated && !TranslationData[key].IsApproved)
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
                    if (lastLine.Length != 0) TranslationData.Add((++templateCounter).ToString() + lastLine[0], new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true));

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
                        if (lastLine.Length != 0) TranslationData.Add((++templateCounter).ToString() + lastLine[0], new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true));
                        lastLine = new string[0];
                        multiLineCollector = "";
                        currentCategory = tempCategory;
                    }
                }
            }
            //add last line (dont care about duplicates because sql will get rid of them)
            if (lastLine.Length != 0) TranslationData.Add((++templateCounter).ToString() + lastLine[0], new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1], true));
        }

        /// <summary>
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        private void ReadStringsTranslationsFromFile()
        {
            StringCategory currentCategory = StringCategory.General;
            string[] lastLine = { };

            DataBase.GetAllLineDataTemplate(FileName, StoryName, out Dictionary<string, LineData> IdsToExport);
            List<string> LinesFromFile;
            try
            {
                //read in lines
                LinesFromFile = File.ReadAllLines(SourceFilePath).ToList();
            }
            catch (Exception e)
            {
                LogManager.LogEvent($"File not found under {SourceFilePath}.\n{e}", LogManager.Level.Warning);
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

        private void SplitReadTranslations(List<string> LinesFromFile, string[] lastLine, StringCategory category, Dictionary<string, LineData> IdsToExport)
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
                        if (IdsToExport.TryGetValue(lastLine[0], out var line1))
                            TranslationData[lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, category, line1.TemplateString, lastLine[1] + multiLineCollector);
                        else
                            TranslationData.Add(lastLine[0], new LineData(lastLine[0], StoryName, FileName, category, "", lastLine[1] + multiLineCollector));
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
                                if (IdsToExport.TryGetValue(lastLine[0], out _))
                                    TranslationData[lastLine[0]] =
                                        new LineData(lastLine[0], StoryName, FileName, category, IdsToExport[lastLine[0]]?.TemplateString, lastLine[1] + multiLineCollector.Remove(multiLineCollector.Length - 2, 1));
                                else
                                    TranslationData.Add(lastLine[0],
                                        new LineData(lastLine[0], StoryName, FileName, category, IdsToExport[lastLine[0]]?.TemplateString, lastLine[1] + multiLineCollector.Remove(multiLineCollector.Length - 2, 1)));
                            }
                            else
                            {//write last line with id if no real line of text is afterwards
                                if (IdsToExport.TryGetValue(lastLine[0], out _))
                                    TranslationData[lastLine[0]] =
                                  new LineData(lastLine[0], StoryName, FileName, category, IdsToExport[lastLine[0]]?.TemplateString, lastLine[1]);
                                else
                                    TranslationData.Add(lastLine[0],
                                  new LineData(lastLine[0], StoryName, FileName, category, IdsToExport[lastLine[0]]?.TemplateString, lastLine[1]));
                            }
                        }
                        lastLine = new string[0];
                        multiLineCollector = "";
                        category = tempCategory;
                        CategoriesInFile.Add(category);
                    }
                }
            }

            if (lastLine.Length > 0)
            {
                //add last line (dont care about duplicates because sql will get rid of them)
                if (IdsToExport.TryGetValue(lastLine[0], out var line1))
                    TranslationData[lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, category, line1.TemplateString, lastLine[1]);
                else
                    TranslationData.Add(lastLine[0], new LineData(lastLine[0], StoryName, FileName, category, "", lastLine[1]));
            }
        }

        private void TryFixEmptyFile()
        {
            if (!triedFixingOnce)
            {
                triedFixingOnce = true;
                DataBase.GetAllLineDataTemplate(FileName, StoryName, out Dictionary<string, LineData> IdsToExport);
                foreach (var item in IdsToExport.Values)
                {
                    TranslationData.Add(item.ID, new LineData(item.ID, StoryName, FileName, item.Category));
                }
                SaveFile();
                TranslationData.Clear();
                ReadStringsTranslationsFromFile();
            }
        }

        /// <summary>
        /// Reloads the file into the program as if it were selected.
        /// </summary>
        internal void ReloadFile()
        {
            ShowAutoSaveDialog();
            LoadTranslationFile();
            CategoriesInFile.Clear();
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
                        helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult];
                        ++SelectedSearchResult;
                    }
                    else if (TabManager.InGlobalSearch && TabManager.TabControl.TabCount > 1)
                    {
                        searchTabIndex = TabManager.TabControl.SelectedIndex;
                        TabManager.SwitchToTab(++searchTabIndex);
                        return true;
                    }
                    else
                    {
                        SelectedSearchResult = 0;
                        helper.CheckListBoxLeft.SelectedIndex = helper.CheckListBoxLeft.SearchResults[SelectedSearchResult];
                        ++SelectedSearchResult;
                    }
                }

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
                string id = currentIndex < TranslationData.Count && currentIndex >= 0 ? TranslationData[SelectedId].ID : "Name";
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
                helper.ReplaceAllButton.Visible = true;
                helper.ReplaceButton.Visible = true;

                //set focus to most needed text box, search first
                if (helper.SearchBox.Text.Length > 0) helper.ReplaceBox.Focus();
                else helper.SearchBox.Focus();
            }
            else
            {
                helper.ReplaceButton.Visible = false;
                helper.ReplaceAllButton.Visible = false;
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

    internal struct CategorizedLines
    {
        public List<LineData> lines;
        public StringCategory category;

        public CategorizedLines(List<LineData> lines, StringCategory category)
        {
            this.lines = lines;
            this.category = category;
        }

        public override bool Equals(object obj) => obj is CategorizedLines other && EqualityComparer<List<LineData>>.Default.Equals(lines, other.lines) && category == other.category;

        public override int GetHashCode()
        {
            int hashCode = 458706445;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<LineData>>.Default.GetHashCode(lines);
            hashCode = hashCode * -1521134295 + category.GetHashCode();
            return hashCode;
        }

        public void Deconstruct(out List<LineData> lines, out StringCategory category)
        {
            lines = this.lines;
            category = this.category;
        }

        public static implicit operator (List<LineData> lines, StringCategory category)(CategorizedLines value) => (value.lines, value.category);
        public static implicit operator CategorizedLines((List<LineData> lines, StringCategory category) value) => new CategorizedLines(value.lines, value.category);
    }
}
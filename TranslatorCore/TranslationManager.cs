using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Translator.Core.Data;
using Translator.Core.DefaultImpls;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using Timer = System.Timers.Timer;

//TODO add tests

namespace Translator.Core
{
    public delegate bool CreateTemplateFromStoryDelegate(string story, string filename, string path, out FileData data);

    /// <summary>
    /// A class providing functions for loading, approving, and working with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>

    public partial class TranslationManager
    {
        public static bool IsUpToDate { get; internal set; } = false;
        internal static readonly Timer AutoSaveTimer = new();
        internal static string language = Settings.Default.Language;

        /// <summary>
        /// The Language of the current translation.
        /// </summary>
        public static string Language
        {
            get
            {
                if (language.Length == 0)
                {
                    throw new LanguageHelper.LanguageException();
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
                    throw new LanguageHelper.LanguageException();
                }
                else
                {
                    language = value;
                    Settings.Default.Language = value;
                    Settings.Default.Save();
                }
            }
        }

        public bool ChangesPending
        {
            get { return _changesPending; }
            set
            {
                if (value && !_changesPending)
                    TabManager.UpdateTabTitle(this, FileName + "*");
                else if(!value && _changesPending)
                    TabManager.UpdateTabTitle(this, FileName ?? string.Empty);
                _changesPending = value;
            }
        }
        private bool _changesPending = false;
        public string SearchQuery { get; private set; } = string.Empty;
        public string CleanedSearchQuery { get; private set; } = string.Empty;
        public bool CaseSensitiveSearch { get; private set; } = false;

        public int SelectedResultIndex = 0;
        public ILineItem SelectedSearchResultItem => SelectedResultIndex < TabUI.LineCount ? TabUI.Lines[SelectedResultIndex] : new DefaultLineItem();

        //counter so we dont get multiple ids, we dont use the dictionary as ids anyways when uploading templates
        private static int templateCounter = 0;

        public FileData TranslationData = new(string.Empty, string.Empty);
        private string fileName = string.Empty;
        private bool isSaveAs = false;
        private int searchTabIndex = 0;
        private bool selectedNew = false;
        private string sourceFilePath = string.Empty;
        private string storyName = string.Empty;
#nullable disable
        private static IUIHandler UI;
#nullable restore
        private static bool StaticUIInitialized = false;
        private readonly ITab TabUI;
        private bool triedFixingOnce = false;

        public TranslationManager(IUIHandler ui, ITab tab)
        {
            if (!StaticUIInitialized) { UI = ui; StaticUIInitialized = true; }
            TabUI = tab;
            AutoSaveTimer.Elapsed += SaveFileHandler;

            if (!IsUpToDate && Settings.Default.AdvancedModeEnabled)
                GenerateOfficialTemplates();
        }

        static TranslationManager()
        {
            if (Settings.Default.AutoSaveInterval > TimeSpan.FromMinutes(1))
            {
                AutoSaveTimer.Interval = (int)Settings.Default.AutoSaveInterval.TotalMilliseconds;
                AutoSaveTimer.Start();
            }
            else
            {
                Settings.Default.AutoSaveInterval = TimeSpan.FromMinutes(1);
                AutoSaveTimer.Interval = (int)Settings.Default.AutoSaveInterval.TotalMilliseconds;
                AutoSaveTimer.Start();
            }
        }

        public void SaveFileHandler(object? sender, ElapsedEventArgs? e)
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
        public string SelectedId { get { return TabUI.SelectedLineItem.Text; } }

        public LineData SelectedLine { get { return TranslationData[SelectedId]; } }

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
                if (!isSaveAs) FileName = Utils.ExtractFileName(value);
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
                if (value.IsOfficialStory())
                {
                    storyName = value;
                }
                else
                {
                    storyName = value;
                    //custom story?
                    if (!CustomStoryTemplateHandle(value))
                    {
                        _ = UI.InfoOk($"Not an official story, custom stories disabled. You can enable them in the settings and try again. Will use \"Original Story\" for now");
                        storyName = "Original Story";
                    }
                }
            }
        }

        public bool CustomStoryTemplateHandle(string story)
        {
            PopupResult result;
            if (!Settings.Default.AllowCustomStories)
            {
                result = PopupResult.NO;
            }
            else if (Settings.Default.IgnoreCustomStoryWarning)
            {
                result = PopupResult.YES;
            }
            else
            {
                result = UI.InfoYesNo($"Detected {story} as the story to use, if this is a custom story you want to translate, you can do so. Choose yes if you want to do that. If not, select no and we will assume this is the Original Story. (You can disable this warning in the settings)", "Custom story?");
            }
            if (result == PopupResult.YES)
            {
                //replace by template generation method
                UI.SignalUserWait();
                //check if the template has been added and generated once before
                _ = DataBase.GetAllLineDataTemplate(fileName, story, out FileData data);
                if (data.Count == 0)
                {//its a custom story but no template so far on the server
                 //use contextprovider to extract the story object and get the lines
                    PopupResult typeResult;
                    if ((typeResult = UI.InfoYesNoCancel($"You will now be prompted to select the corresponding .story or .character file for the translation you want to do. Is {FileName} a character?", "Custom story?")) != PopupResult.CANCEL)
                    {
                        if (UI.CreateTemplateFromStory(story, fileName, SourceFilePath, out FileData templates))
                            if (templates.Count > 0)
                            {
                                _ = DataBase.RemoveOldTemplates(FileName, story);
                                _ = DataBase.UploadTemplates(templates);

                                UI.SignalUserEndWait();
                                return true;
                            }
                        _ = UI.ErrorOk("Something broke, please try again.");
                    }
                }
                else
                {
                    UI.SignalUserEndWait();
                    return true;
                }
            }

            UI.SignalUserEndWait();
            return false;
        }

        /// <summary>
        /// Depending on the cursor location the line is approved or not (on checkbox or not).
        /// </summary>
        public void ApprovedButtonHandler()
        {
            //change checked state for the selected item,
            //but only if we are on the button with the mouse.
            //(prevents an infinite loop when we get the change state from setting the button state in code)
            if (TabUI.IsApproveButtonFocused)
            {
                int Index = TabUI.SelectedLineIndex;
                //inverse checked state at the selected index
                if (Index >= 0) TabUI.Lines.SetApprovalState(Index, TabUI.ApprovedButtonChecked);

                TabUI.SetApprovedCount(TabUI.Lines.ApprovedCount, TabUI.Lines.Count);
                UI.UpdateTranslationProgressIndicator();
            }
        }

        /// <summary>
        /// Approves the string in the db, if possible. Also updates UI.
        /// </summary>
        /// <param name="SelectNewAfter">A bool to determine if a new string should be selected after approval.</param>
        public void ApproveIfPossible(bool SelectNewAfter)
        {
            int currentIndex = TabUI.SelectedLineIndex;
            if (currentIndex >= 0)
            {
                //set checkbox state
                TabUI.ApprovedButtonChecked = TabUI.Lines.GetApprovalState(currentIndex);
                //set checkbox state
                SelectedLine.IsApproved = TabUI.ApprovedButtonChecked;

                //move one string down if possible
                if (SelectNewAfter)
                {
                    if (currentIndex < TabUI.Lines.Count - 1) TabUI.SelectLineItem(currentIndex + 1);
                }

                TabUI.SetApprovedCount(TabUI.Lines.ApprovedCount, TabUI.Lines.Count);
                UI.UpdateTranslationProgressIndicator();
            }
        }

        /// <summary>
        /// Loads a file into the program and calls all UI routines
        /// </summary>
        /// <param name="path">The path to the file to translate</param>
        public void LoadFileIntoProgram(string path)
        {
            if (path.Length > 0 && File.Exists(path))
            {
                if (TranslationData.Count > 0) TabManager.ShowAutoSaveDialog();
                //clear history if we have a new file, we dont need old one anymore
                if (path != SourceFilePath && FileName != string.Empty && StoryName != string.Empty)
                    History.ClearForFile(FileName, StoryName);
                ResetTranslationManager();

                SourceFilePath = path;
                LoadTranslationFile();

                if (TranslationData.Count > 0)
                {
                    //log file loading if successfull
                    LogManager.Log($"File opened: {StoryName}/{FileName} at {DateTime.Now}");

                    //update recents
                    RecentsManager.SetMostRecent(SourceFilePath);
                    UI.SetFileMenuItems(RecentsManager.GetUpdatedMenuItems(UI.FileMenuItems));
                }
            }
            else
            {
                UI.ErrorOk("File doesnt exist");
            }
        }

        private void MarkSimilarLine()
        {
            if (TabUI.TranslationBoxText == TabUI.TemplateBoxText && !SelectedLine.IsTranslated && !SelectedLine.IsApproved)
            {
                TabUI.SimilarStringsToEnglish.Add(SelectedId);
            }
            else
            {
                _ = TabUI.SimilarStringsToEnglish.Remove(SelectedId);
            }
        }

        /// <summary>
        /// Populates the Editor/Template text boxes and does some basic set/reset logic.
        /// </summary>
        public void PopulateTextBoxes()
        {
            int currentIndex = TabUI.SelectedLineIndex;

            if (currentIndex >= 0)
            {
                if (Settings.Default.DisplayVoiceActorHints)
                    TabUI.TemplateBoxText = SelectedLine.TemplateString.Replace("\n", Environment.NewLine);
                else
                    TabUI.TemplateBoxText = SelectedLine.TemplateString.Replace("\n", Environment.NewLine).RemoveVAHints();

                selectedNew = true;

                //display the string in the editable window
                TabUI.TranslationBoxText = SelectedLine.TranslationString.Replace("\n", Environment.NewLine);

                //translate if useful and possible
                ConvenienceAutomaticTranslation();

                //mark text if similar to english (not translated yet)
                MarkSimilarLine();

                TabUI.CommentBoxTextArr = SelectedLine.Comments;

                //sync approvedbox and list
                TabUI.ApprovedButtonChecked = SelectedLine.IsApproved;

                //update label
                TabUI.UpdateCharacterCounts(SelectedLine.TemplateLength, SelectedLine.TranslationLength);

                TabUI.SetSelectedTranslationBoxText(SelectedLine.TranslationLength, SelectedLine.TranslationLength);

                UpdateSearchAndSearchHighlight();
            }
            else
            {
                if (TabUI.LineCount > 0) TabUI.SelectLineItem(0);
            }
            TabUI.SetApprovedCount(TabUI.Lines.ApprovedCount, TabUI.LineCount);
        }

        private void UpdateSearchAndSearchHighlight()
        {
            //renew search result if possible
            int t = TabUI.Lines.SearchResults.IndexOf(SelectedId);
            if (t >= 0)
            {
                SelectedResultIndex = t; int TemplateTextQueryLocation;
                if (CaseSensitiveSearch)
                    TemplateTextQueryLocation = TabUI.TemplateBoxText.IndexOf(CleanedSearchQuery);
                else
                    TemplateTextQueryLocation = TabUI.TemplateBoxText.ToLowerInvariant().IndexOf(CleanedSearchQuery.ToLowerInvariant());
                if (TemplateTextQueryLocation >= 0)
                {
                    TabUI.Template.HighlightStart = TemplateTextQueryLocation;
                    TabUI.Template.HighlightEnd = TemplateTextQueryLocation + CleanedSearchQuery.Length;
                    TabUI.Template.ShowHighlight = true;
                }
                else
                {
                    TabUI.Template.ShowHighlight = false;
                }
            }
            else
            {
                TabUI.Template.ShowHighlight = false;
            }
        }

        /// <summary>
        /// Replaces a searched string in all applicable lines by the replacement provided using the invariant culture.
        /// </summary>
        /// <param name="replacement">The string to replace all search matches with</param>
        public void ReplaceAll(string replacement)
        {
            //save old lines for history
            FileData old = new(TranslationData, StoryName, FileName);

            for (int i = 0; i < TabUI.Lines.SearchResults.Count; ++i)
            {
                if (TabUI.Lines.SearchResults[i].Length == 0) continue;
                TranslationData[TabUI.Lines.SearchResults[i]].TranslationString = TranslationData[TabUI.Lines.SearchResults[i]].TranslationString.ReplaceImpl(replacement, CleanedSearchQuery);
            }

            History.AddAction(new AllTranslationsChanged(this, old, TranslationData));

            //update search results
            Search();

            //update textbox
            ReloadTranslationTextbox();

            //show confirmation
            _ = UI.InfoOk("Replace successful!", "Success");
        }

        /// <summary>
        /// Replaces a searched string in the selected line if it is a search result by the replacement provided using the invariant culture.
        /// </summary>
        /// <param name="replacement">The string to replace all search matches with</param>
        public void ReplaceSingle(string replacement)
        {
            if (TabUI.Lines.SearchResults.Contains(SelectedId))
            {
                string temp = SelectedLine.TranslationString.ReplaceImpl(replacement, CleanedSearchQuery);
                History.AddAction(new TranslationChanged(this, SelectedId, SelectedLine.TranslationString, temp));
                SelectedLine.TranslationString = temp;

                //update search results
                Search();

                //update textbox
                ReloadTranslationTextbox();
            }
        }

        public void ReloadTranslationTextbox()
        {
            //update textbox
            if (SelectedId != string.Empty)
                TabUI.TranslationBoxText = SelectedLine.TranslationString.Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// replaces the template version of the string with a computer translated one to speed up translation.
        /// </summary>
        public void RequestAutomaticTranslation()
        {
            if (SelectedId != string.Empty)
                AutoTranslation.AutoTranslationAsync(SelectedLine, Language, AutoTranslationCallback);
        }

        /// <summary>
        /// replaces the template version of the string with a computer translated one to speed up translation, but only if no human input has been given on the new line so far.
        /// </summary>
        private void ConvenienceAutomaticTranslation()
        {
            if (TabUI.TemplateBoxText == TabUI.TranslationBoxText && !SelectedLine.IsTranslated && !SelectedLine.IsApproved && SelectedLine.TemplateLength > 0)
                AutoTranslation.AutoTranslationAsync(SelectedLine, Language, ConvenienceTranslationCallback);
        }

        private void AutoTranslationCallback(bool successfull, LineData data)
        {
            if (successfull)
            {
                TranslationData[data.ID] = data;
                ReloadTranslationTextbox();
            }
            else
            {
                if (UI.WarningYesNo("The translator seems to be unavailable. Turn off autotranslation? (needs to be turned back on manually!)", "Turn off autotranslation", PopupResult.YES))
                {
                    Settings.Default.AutoTranslate = false;
                }
            }
        }

        private void ConvenienceTranslationCallback(bool successfull, LineData data)
        {
            if (successfull && TranslationData[data.ID].TranslationString == data.TemplateString && TranslationData[data.ID].TranslationString.Length == 0)
            {
                TranslationData[data.ID] = data;
                ReloadTranslationTextbox();
            }
            else
            {
                if (UI.WarningYesNo("The translator seems to be unavailable. Turn off autotranslation? (needs to be turned back on manually!)", "Turn off autotranslation", PopupResult.YES))
                {
                    Settings.Default.AutoTranslate = false;
                }
            }
        }

        /// <summary>
        /// Saves the current string to the db
        /// </summary>
        public void SaveCurrentString()
        {
            int currentIndex = TabUI.SelectedLineIndex;

            //if we changed the eselction and have autsave enabled
            if (currentIndex >= 0)
            {
                UI.SignalUserWait();

                //update translation in the DataBase
                _ = DataBase.UpdateTranslation(SelectedLine, Language);

                if (TabUI.SimilarStringsToEnglish.Contains(SelectedId)) _ = TabUI.SimilarStringsToEnglish.Remove(SelectedId);

                UI.SignalUserEndWait();
            }
        }

        /// <summary>
        /// Saves all strings to the file we read from.
        /// </summary>
        public void SaveFile(bool doOnlineUpdate = true)
        {
            UI.SignalUserWait();

            LogManager.Log("saving file " + StoryName + "/" + FileName);
            History.ClearForFile(FileName, StoryName);

            if (SourceFilePath == string.Empty || Language == string.Empty)
            {
                UI.SignalUserEndWait();
                return;
            }
            if (doOnlineUpdate) _ = Task.Run(RemoteUpdate);

            List<CategorizedLines> CategorizedStrings = InitializeCategories(StoryName, FileName);

            //sort online line ids into translations but use local values for translations if applicable
            if (DataBase.GetAllLineDataTemplate(FileName, StoryName, out FileData IdsToExport) && DataBase.IsOnline)
                SortIntoCategories(ref CategorizedStrings, IdsToExport, TranslationData); //export only ids from db
            else
                SortIntoCategories(ref CategorizedStrings, TranslationData, TranslationData); //eyxport all ids we have

            //save all categorized lines to disk
            WriteCategorizedLinesToDisk(CategorizedStrings, SourceFilePath);

            //copy file to game rather than writing again
            if (Settings.Default.AlsoSaveToGame)
            {
                CopyToGameModsFolder();
            }
            UI.SignalUserEndWait();
            ChangesPending = false;
            LogManager.Log("Successfully saved the file locally");

            void RemoteUpdate()
            {
                UI.SignalUserWait();
                if (!DataBase.UpdateTranslations(TranslationData, Language) || !DataBase.IsOnline) _ = UI.InfoOk("You seem to be offline, translations are going to be saved locally but not remotely.");
                UI.SignalUserEndWait();
                LogManager.Log("Successfully saved the file remotely");
            }
        }

        private void CopyToGameModsFolder()
        {
            //get language path
            if (LanguageHelper.Languages.TryGetValue(Language, out string? languageAsText))
            {
                //add new to langauge if wanted
                if (Settings.Default.UseFalseFolder)
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
                        _ = Directory.CreateDirectory(Path.GetDirectoryName(gameFilePath) ?? string.Empty);
                        File.Copy(SourceFilePath, gameFilePath, true);
                    }
                }
            }
        }

        /// <summary>
        /// Saves all strings to a specified file location.
        /// </summary>
        public void SaveFileAs()
        {
            if (SourceFilePath != string.Empty)
            {
                isSaveAs = true;
                string oldFile = SourceFilePath;
                string SaveFile = Utils.SelectSaveLocation("Choose a file to save the translation to");
                SourceFilePath = SaveFile;
                this.SaveFile();
                SourceFilePath = oldFile;
                isSaveAs = false;
            }
        }

        /// <summary>
        /// Performs a search through all lines currently loaded.
        /// </summary>
        public void Search()
        {
            SearchQuery = UI.SearchBarText;
            Search(SearchQuery);
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
                TabUI.Lines.SearchResults.Clear();

                CaseSensitiveSearch = false;
                //decide on case sensitivity
                if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
                {
                    CaseSensitiveSearch = true;
                    query = query[1..];
                    //methodolgy: highlight items which fulfill search and show count
                    int x = 0;
                    foreach (LineData item in TranslationData.Values)
                    {
                        if ((item.TranslationString.Contains(query) /*if the translated text contaisn the search string*/
                            || (item.TemplateString != null
                            && item.TemplateString.Contains(query))/*if the english string is not null and contaisn the searched part*/
                            || item.ID.Contains(query))
                            && item.TranslationLength > 0)/*if the id contains the searched part*/
                        {
                            TabUI.Lines.SearchResults.Add(TabUI.Lines[x].Text);//add index to highligh list
                        }
                        ++x;
                    }
                }
                else if (query[0] != '!')
                {
                    if (query[0] == '\\') // we have an escaped flag following, so we chop of escaper and continue
                    {
                        query = query[1..];
                    }
                    //methodolgy: highlight items which fulfill search
                    int x = 0;
                    foreach (LineData item in TranslationData.Values)
                    {
                        if (item.TranslationString.ToLowerInvariant().Contains(query.ToLowerInvariant()) /*if the translated text contaisn the search string*/
                            || (item.TemplateString != null
                            && item.TemplateString.ToLowerInvariant().Contains(query.ToLowerInvariant()))/*if the english string is not null and contaisn the searched part*/
                            || item.ID.ToLowerInvariant().Contains(query.ToLowerInvariant())
                            && item.TranslationLength > 0)/*if the id contains the searched part*/
                        {
                            TabUI.Lines.SearchResults.Add(TabUI.Lines[x].Text);//add index to highligh list
                        }
                        ++x;
                    }
                }
                CleanedSearchQuery = query;

                UpdateSearchAndSearchHighlight();
            }
            else
            {
                TabUI.Lines.SearchResults.Clear();
                SelectedResultIndex = 0;
                SearchQuery = string.Empty;
                CleanedSearchQuery = string.Empty;
                TabUI.Template.ShowHighlight = false;
            }

            UI.UpdateResults();
        }

        /// <summary>
        /// Selects a string in the listview given the id
        /// </summary>
        /// <param name="id">The id to look for.</param>
        public void SelectLine(string id)
        {
            //select line which correspondends to id
            for (int i = 0; i < TabUI.LineCount; i++)
            {
                if (TabUI.Lines[i].Text == id) TabUI.SelectLineItem(i);
            }
        }

        /// <summary>
        /// Update the currently selected translation string in the TranslationData.
        /// </summary>
        public void UpdateTranslationString()
        {
            //remove pipe to not break saving/export
            TabUI.TranslationBoxText = TabUI.TranslationBoxText.Replace('|', ' ');
            SelectedLine.TranslationString = TabUI.TranslationBoxText.Replace(Environment.NewLine, "\n");
            TabUI.UpdateCharacterCounts(SelectedLine.TemplateLength, SelectedLine.TranslationLength);
            ChangesPending = !selectedNew || ChangesPending;
            selectedNew = false;
        }

        public void UpdateComments()
        {
            //remove pipe to not break saving/export
            SelectedLine.Comments = TabUI.CommentBoxTextArr;
        }

        /// <summary>
        /// Prepares the values for reading of the strings, and calls the methods necessary after successfully loading a file.
        /// </summary>
        private void LoadTranslationFile(bool localTakesPriority = false)
        {
            TranslationData.Clear();
            TabUI.Lines.Clear();
            if (SourceFilePath != string.Empty)
            {
                //get parent folder name and check if it is the story, else search around a bit
                //get language text representation
                StoryName = Utils.ExtractStoryName(SourceFilePath);
                //actually load all strings into the program
                ReadStringsTranslationsFromFile();

                if (TranslationData.Count > 0)
                {
                    string storyNameToDisplay = StoryName.TrimWithDelim();
                    string fileNameToDisplay = FileName.TrimWithDelim();
                    TabUI.SetFileInfoText($"File: {storyNameToDisplay}/{fileNameToDisplay}.txt");

                    //is up to date, so we can start translation
                    IntegrateOnlineTranslations(localTakesPriority);
                    TabUI.SetApprovedCount(TabUI.Lines.ApprovedCount, TabUI.Lines.Count);
                }
                //update tab name
                TabManager.UpdateSelectedTabTitle(FileName);
            }
        }

        /// <summary>
        /// Loads the strings and does some work around to ensure smooth sailing.
        /// </summary>
        private void IntegrateOnlineTranslations(bool localTakesPriority = false)
        {
            int currentIndex = 0;
            UI.SignalUserWait();
            TabUI.Lines.FreezeLayout();

            FileData onlineLines = new(StoryName, FileName);
            if (DataBase.IsOnline) _ = DataBase.GetAllLineData(FileName, StoryName, out onlineLines, Language);

            foreach (string key in TranslationData.Keys)
            {
                if (onlineLines.TryGetValue(key, out LineData? tempLine))
                {
                    TranslationData[key].Category = tempLine.Category;
                    if (DataBase.IsOnline) TranslationData[key].Comments = tempLine.Comments;
                    TranslationData[key].FileName = tempLine.FileName;
                    TranslationData[key].ID = key;
                    TranslationData[key].IsTemplate = false;
                    TranslationData[key].IsTranslated = tempLine.IsTranslated;
                    TranslationData[key].Story = tempLine.Story;
                    if (localTakesPriority
                        && DataBase.IsOnline
                        && tempLine.TranslationLength > 0)
                        TranslationData[key].TranslationString = tempLine.TranslationString;
                    else if (!DataBase.IsOnline) TranslationData[key].TemplateString = tempLine.TemplateString;
                    TranslationData[key].IsApproved = tempLine.IsApproved;
                }

                if (TranslationData[key].TemplateString == null) TranslationData[key].TemplateString = string.Empty;

                TabUI.Lines.Add(key, TranslationData[key].IsApproved);

                //colour string if similar to the english one
                if (!TranslationData[key].IsTranslated && !TranslationData[key].IsApproved)
                {
                    TabUI.SimilarStringsToEnglish.Add(key);
                }

                //increase index to aid colouring
                currentIndex++;
            }

            TabUI.Lines.UnFreezeLayout();

            //reload once so the order of lines is correct after we fixed an empty or broken file
            if (triedFixingOnce)
            {
                triedFixingOnce = false;
                ReloadFile();
            }

            UI.SignalUserEndWait();
        }

        private FileData GetTemplatesFromUser()
        {
            if (UI.InfoYesNo("Do you have the translation template from Don/Eek available? If so, we can use those if you hit yes, if you hit no we can generate templates from the game's story files.", "Templates available?", PopupResult.YES))
            {
                return GetTemplateFromFile(Utils.SelectFileFromSystem(false, $"Choose the template for {StoryName}/{FileName}.", FileName + ".txt"), StoryName, FileName, false);
            }
            return new FileData(StoryName, FileName);
        }

        /// <summary>
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        private void ReadStringsTranslationsFromFile()
        {
            FileData templates;
            StringCategory currentCategory = StringCategory.General;
            if (DataBase.IsOnline)
            {
                _ = DataBase.GetAllLineDataTemplate(FileName, StoryName, out templates);
            }
            else
            {
                templates = GetTemplatesFromUser();
            }
            List<string> LinesFromFile;
            try
            {
                //read in lines
                LinesFromFile = new List<string>(File.ReadAllLines(SourceFilePath));
            }
            catch (Exception e)
            {
                LogManager.Log($"File not found under {SourceFilePath}.\n{e}", LogManager.Level.Warning);
                _ = UI.InfoOk($"File not found under {SourceFilePath}. Please reopen.", "Invalid path");
                ResetTranslationManager();
                return;
            }

            //if we got lines at all
            if (LinesFromFile.Count > 0)
            {
                SplitReadTranslations(LinesFromFile, currentCategory, templates);
            }
            else
            {
                TryFixEmptyFile();
            }

            if (templates.Count != TranslationData.Count)
            {
                if (TranslationData.Count == 0)
                {
                    TryFixEmptyFile();
                }
                else if (DataBase.IsOnline && !Settings.Default.IgnoreMissingLinesWarning)
                //inform user the issing translations will be added after export. i see no viable way to add them before having them all read in,
                //and it would take a lot o time to get them all. so just have the user save it once and reload. we might do this automatically, but i don't know if that is ok to do :)
                {
                    _ = UI.InfoOk(
                    "Some strings are missing from your translation, they will be added with the english version when you first save the file!",
                    "Some strings missing");
                    ChangesPending = true;
                }
            }
        }

        private void SplitReadTranslations(List<string> LinesFromFile, StringCategory category, FileData IdsToExport)
        {
            string[] lastLine = Array.Empty<string>();
            string multiLineCollector = string.Empty;
            //remove last if empty, breaks line loading for the last
            while (LinesFromFile.Count > 0)
            {
                if (LinesFromFile[^1] == string.Empty)
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
                        multiLineCollector = multiLineCollector.TrimEnd(new char[] { '\n', '\r', ' ' });
                        CreateLineInTranslations(lastLine, category, IdsToExport, multiLineCollector);
                    }
                    //get current line
                    lastLine = line.Split('|');
                    //reset multiline collector
                    multiLineCollector = string.Empty;
                }
                else
                {
                    StringCategory tempCategory = line.AsCategory();
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
                            {//write last string with id plus all lines after that minus the last new line char(s)
                                multiLineCollector = multiLineCollector.TrimEnd(new char[] { '\n', '\r', ' ' });
                                CreateLineInTranslations(lastLine, category, IdsToExport, multiLineCollector);
                            }
                            else
                            {//write last line with id if no real line of text is afterwards
                                CreateLineInTranslations(lastLine, category, IdsToExport, string.Empty);
                            }
                        }
                        //resetting for next iteration
                        lastLine = Array.Empty<string>();
                        multiLineCollector = string.Empty;
                    }
                }
            }

            if (lastLine.Length > 0)
            {
                //add last line (dont care about duplicates because the dict)
                CreateLineInTranslations(lastLine, category, IdsToExport, lastLine[1]);
            }
        }

        private void CreateLineInTranslations(string[] lastLine, StringCategory category, FileData IdsToExport, string translation)
        {
            if (lastLine[0] == string.Empty) return;
            if (IdsToExport.TryGetValue(lastLine[0], out LineData? templateLine))
                TranslationData[lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, category, templateLine.TemplateString, lastLine[1] + translation);
            else
                TranslationData[lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, category, string.Empty, lastLine[1] + translation);
        }

        private void TryFixEmptyFile()
        {
            if (!triedFixingOnce)
            {
                triedFixingOnce = true;
                _ = DataBase.GetAllLineDataTemplate(FileName, StoryName, out FileData IdsToExport);
                foreach (LineData item in IdsToExport.Values)
                {
                    TranslationData[item.ID] = new LineData(
                        item.ID,
                        StoryName,
                        FileName,
                        item.Category,
                        TranslationData.TryGetValue(item.ID, out LineData? tempLineData) ?
                        (tempLineData?.TranslationLength > 0 ?
                        tempLineData?.TranslationString ?? item.TemplateString.RemoveVAHints()
                        : item.TemplateString.RemoveVAHints())
                        : item.TemplateString.RemoveVAHints()
                        );
                }
                SaveFile(false);
                TranslationData.Clear();
                ReadStringsTranslationsFromFile();
            }
        }

        /// <summary>
        /// Reloads the file into the program as if it were selected.
        /// </summary>
        public void ReloadFile()
        {
            TabManager.ShowAutoSaveDialog();
            LoadTranslationFile();
            if (UI == null) return;
            //select recent index
            if (Settings.Default.RecentIndex > 0 && Settings.Default.RecentIndex < TranslationData.Count) TabUI.SelectLineItem(Settings.Default.RecentIndex);
            LogManager.Log($"Reloaded {StoryName}/{FileName}");
        }

        /// <summary>
        /// Resets the translation manager.
        /// </summary>
        private void ResetTranslationManager()
        {
            Settings.Default.RecentIndex = TabUI.SelectedLineIndex;
            TranslationData.Clear();
            TabUI.Lines.Clear();
            TabUI.SimilarStringsToEnglish.Clear();
            SelectedResultIndex = 0;
            TabManager.UpdateSelectedTabTitle("Tab");
            TabUI.SetApprovedCount(1, 1);
        }

        /// <summary>
        /// Selects the next search result if applicable
        /// </summary>
        /// <returns>True if a new result could be selected</returns>
        public bool SelectNextResultIfApplicable()
        {
            if (!TabUI.IsTranslationBoxFocused && !TabUI.IsCommentBoxFocused && TabUI.Lines.SearchResults.Count > 0)
            {
                //loop back to start
                if (SelectedResultIndex > TabUI.Lines.SearchResults.Count - 1)
                {
                    SelectedResultIndex = 0;
                    //loop over to new tab when in global search
                    if (TabManager.InGlobalSearch)
                    {
                        searchTabIndex = TabManager.SelectedTabIndex;
                        TabManager.SwitchToTab(++searchTabIndex);
                    }
                    else
                    {
                        //select next index from list of matches
                        if (SelectedResultIndex < TabUI.LineCount)
                        {
                            SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                        }
                    }
                }
                else
                {
                    if (SelectedResultIndex < TabUI.LineCount)
                    {
                        SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                        ++SelectedResultIndex;
                    }
                    else if (TabManager.InGlobalSearch && TabManager.TabCount > 1)
                    {
                        searchTabIndex = TabManager.SelectedTabIndex;
                        TabManager.SwitchToTab(++searchTabIndex);
                        return true;
                    }
                    else
                    {
                        SelectedResultIndex = 1;
                        SelectLine(TabUI.Lines.SearchResults[0]);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void OverrideCloudSave()
        {
            if (Settings.Default.AdvancedModeEnabled)
            {
                //show warning
                if (UI.WarningYesNo("This will override the lines saved online for the opened file with your local verison! Please be careful. If you read this and want to continue, please select yes", result: PopupResult.YES))
                {
                    //force load local version
                    LoadTranslationFile(true);
                    //select recent index
                    if (Settings.Default.RecentIndex > 0 && Settings.Default.RecentIndex < TranslationData.Count) TabUI.Lines.SelectedIndex = Settings.Default.RecentIndex;
                    //update to online
                    SaveFile();
                    //reload latest online, should be the same as local by then
                    ReloadFile();
                }
            }
        }

        /// <summary>
        /// tldr: magic
        ///
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        private static FileData GetTemplateFromFile(string path, string story = "", string fileName = "", bool doIterNumbers = true)
        {
            if (Utils.ExtractFileName(path) != fileName)
            {
                _ = UI.WarningOk("The template file must have the same name as the file you want to translate!");
                return new FileData(story, fileName);
            }
            if(story == string.Empty) story = Utils.ExtractStoryName(path);
            if(fileName == string.Empty) fileName = Utils.ExtractFileName(path);

            var fileData = new FileData(story, fileName);
            StringCategory currentCategory = StringCategory.General;
            string multiLineCollector = string.Empty;
            string[] lastLine = Array.Empty<string>();
            //string[] lastLastLine = { };
            //read in lines
            var LinesFromFile = new List<string>(File.ReadAllLines(path));
            //remove last if empty, breaks line loading for the last
            while (LinesFromFile[^1] == string.Empty) _ = LinesFromFile.Remove(LinesFromFile[^1]);
            //load lines and their data and split accordingly
            foreach (string line in LinesFromFile)
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : string.Empty + lastLine[0]] = new LineData(lastLine[0], story, fileName, currentCategory, lastLine[1] + multiLineCollector, true);

                    //get current line
                    lastLine = line.Split('|');

                    //reset multiline collector
                    multiLineCollector = string.Empty;
                }
                else
                {
                    StringCategory tempCategory = line.AsCategory();
                    if (tempCategory == StringCategory.Neither)
                    {
                        //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                        multiLineCollector += "\n" + line;
                    }
                    else
                    {
                        //if we reach a category, we can add the old string to the translation manager
                        if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : string.Empty + lastLine[0]] = new LineData(lastLine[0], story, fileName, currentCategory, lastLine[1] + multiLineCollector, true);
                        lastLine = Array.Empty<string>();
                        multiLineCollector = string.Empty;
                        currentCategory = tempCategory;
                    }
                }
            }
            //add last line (dont care about duplicates because sql will get rid of them)
            if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : string.Empty + lastLine[0]] = new LineData(lastLine[0], story, fileName, currentCategory, lastLine[1], true);

            return fileData;
        }

        private static void GenerateOfficialTemplates()
        {
            if (UI.InfoYesNoCancel($"You will now be prompted to select any folder in the folder which contains all Official Stories and UI/Hints.", "Create templates for a official stories") != PopupResult.YES)
                return;

            //set up 
            string path = Utils.SelectTemplateFolderFromSystem();
            if (path.Length == 0) return;

            UI.SignalUserWait();

            LogManager.Log("Creating official templates for version " + Settings.Default.FileVersion);

            //create translation and open it
            FileData templates;
            foreach (var folder_path in Directory.GetDirectories(Directory.GetParent(path)?.FullName ?? string.Empty))
            {
                string story = Utils.ExtractStoryName(folder_path);
                foreach (var file_path in Directory.GetFiles(folder_path))
                {
                    var file = Utils.ExtractFileName(file_path);
                    if (Path.GetExtension(file_path) != ".txt") continue;

                    //create and upload templates
                    templates = GetTemplateFromFile(file_path, story, file, false);
                    if (templates.Count > 0)
                    {
                        if (!DataBase.RemoveOldTemplates(file, story))
                        {
                            _ = UI.ErrorOk("New official templates were not removed, please try again.");
                            UI.SignalUserEndWait();
                            return;
                        }
                        if (!DataBase.UploadTemplates(templates))
                        {
                            _ = UI.ErrorOk("New official templates were not uploaded, please try again.");
                            UI.SignalUserEndWait();
                            return;
                        }
                        LogManager.Log($"Successfully read and uploaded templates for {story}/{file}");
                    }
                    else
                    {
                        _ = UI.ErrorOk($"{story}/{file} contained no templates, skipping");
                    }
                }
            }
            DataBase.UpdateDBVersion();
            UI.InfoOk("Successfully created and uploaded offical templates");
            LogManager.Log("Successfully created and uploaded offical templates");
            UI.SignalUserEndWait();
        }

        public static void GenerateTemplateForSingleFile(bool SaveOnline = false)
        {
            PopupResult typeResult;
            if ((typeResult = UI.InfoYesNoCancel($"You will now be prompted to select the corresponding .story or .character file you want to create the template for. (Note: templates of official stories can only be created for local use)", "Create a template")) != PopupResult.CANCEL)
            {
                //set up 
                string path = Utils.SelectFileFromSystem(false, "Select the file to create the template for", filter: "Character/Story files (*.character;*.story)|*.character;*.story");
                if (path.Length == 0) return;

                UI.SignalUserWait();
                (string story, string file) = Utils.ExtractFileAndStoryName(path);

                if (story.IsOfficialStory() && !Settings.Default.AdvancedModeEnabled) SaveOnline = false;

                LogManager.Log("creating template for " + story + "/" + file);
                //create and upload templates
                if (UI.CreateTemplateFromStory(story, file, path, out FileData templates))
                {
                    if (templates.Count > 0 && SaveOnline)
                    {
                        _ = DataBase.RemoveOldTemplates(file, story);
                        _ = DataBase.UploadTemplates(templates);
                    }
                    else if (SaveOnline)
                    {
                        _ = UI.ErrorOk("No template resulted from the generation, please try again.");
                        UI.SignalUserEndWait();
                        return;
                    }
                }
                else
                {
                    _ = UI.ErrorOk("Template was not created, please try again.");
                    UI.SignalUserEndWait();
                    return;
                }

                LogManager.Log("successfully created template");

                //create translation and open it
                string newFile = Utils.SelectSaveLocation("Select a file to save the generated templates to", path, file, "txt", false, false);
                if (newFile != string.Empty)
                {
                    var sortedLines = InitializeCategories(story, file);
                    SortIntoCategories(ref sortedLines, templates, templates);

                    WriteCategorizedLinesToDisk(sortedLines, newFile);
                }
                UI.SignalUserEndWait();
            }
        }

        public static void GenerateTemplateForAllFiles(bool SaveOnline = false)
        {
            PopupResult typeResult;
            if ((typeResult = UI.InfoYesNoCancel($"You will now be prompted to select any file in the story folder you want to create the templates for. After that you must select the folder in which the translations will be placed.", "Create templates for a complete story")) != PopupResult.CANCEL)
            {
                //set up 
                string path = Utils.SelectFileFromSystem(false, "Select a file in the folder you want to create templates for", filter: "Character/Story files (*.character;*.story)|*.character;*.story");
                if (path.Length == 0) return;

                UI.SignalUserWait();
                string story = Utils.ExtractStoryName(path);

                if (story.IsOfficialStory() && !Settings.Default.AdvancedModeEnabled) SaveOnline = false;
                LogManager.Log("creating templates for " + story);

                //create translation and open it
                string newFiles_dir = Directory.GetParent(Utils.SelectSaveLocation("Select the folder where you want the generated templates to go", path, "template export", string.Empty, false, false))?.FullName
                    ?? SpecialDirectories.MyDocuments;
                foreach (var file_path in Directory.GetFiles(Directory.GetParent(path)?.FullName ?? string.Empty))
                {
                    string file = Utils.ExtractFileName(file_path);
                    if (Path.GetExtension(file_path) != ".character" && Path.GetExtension(file_path) != ".story") continue;

                    //create and upload templates
                    if (UI.CreateTemplateFromStory(story, file, file_path, out FileData templates))
                    {
                        if (templates.Count > 0 && SaveOnline)
                        {
                            _ = DataBase.RemoveOldTemplates(file, story);
                            _ = DataBase.UploadTemplates(templates);
                        }
                        else if (SaveOnline)
                        {
                            _ = UI.ErrorOk("No templates resulted from the generation, please try again.");
                            UI.SignalUserEndWait();
                            return;
                        }
                    }
                    else
                    {
                        _ = UI.ErrorOk("Templates were not created, please try again.");
                        UI.SignalUserEndWait();
                        return;
                    }

                    if (newFiles_dir != string.Empty)
                    {
                        var sortedLines = InitializeCategories(story, file);
                        SortIntoCategories(ref sortedLines, templates, templates);
                        WriteCategorizedLinesToDisk(sortedLines, Path.Combine(newFiles_dir, file + ".txt"));
                    }
                }

                LogManager.Log("successfully created templates");
                UI.SignalUserEndWait();
            }
        }

        /// <summary>
        /// Sets the language the translation is associated with
        /// </summary>
        public static void SetLanguage()
        {
            if (UI.Language.Length >= 0)
            {
                Language = UI.Language;
            }
            else if (Settings.Default.Language != string.Empty)
            {
                string languageFromFile = Settings.Default.Language;
                if (languageFromFile != string.Empty)
                {
                    Language = languageFromFile;
                }
            }
            UI.Language = Language;
        }

        /// <summary>
        /// Generates a list of all string categories depending on the filename.
        /// </summary>
        public static List<StringCategory> GetCategories(string story, string file)
        {
            if (story == "UI" || story == "Hints")
            {
                return new List<StringCategory>() { StringCategory.General };
            }
            else if (file == story)
            {
                return new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.ItemName,
                            StringCategory.ItemAction,
                            StringCategory.ItemGroupAction,
                            StringCategory.Event,
                            StringCategory.Achievement };
            }
            else
            {
                return new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.Dialogue,
                            StringCategory.Response,
                            StringCategory.Quest,
                            StringCategory.Event,
                            StringCategory.BGC};
            }
        }

        internal static void SortIntoCategories(ref List<CategorizedLines> CategorizedStrings, FileData IdsToExport, FileData translationData)
        {
            foreach (LineData item in IdsToExport.Values)
            {
                if (item.ID == string.Empty) continue;
                if (translationData.TryGetValue(item.ID, out LineData? TempResult))
                {
                    if (TempResult?.TranslationLength > 0)
                        item.TranslationString = TempResult?.TranslationString ?? item.TemplateString.RemoveVAHints();
                    else
                        item.TranslationString = item.TemplateString.RemoveVAHints();
                }
                else
                {
                    item.TranslationString = item.TemplateString.RemoveVAHints();
                }

                int intCategory = CategorizedStrings.FindIndex(predicateCategory => predicateCategory.category == item.Category);

                if (intCategory < CategorizedStrings.Count && intCategory >= 0)
                    CategorizedStrings[intCategory].lines.Add(item);
                else
                {
                    CategorizedStrings.Add((new List<LineData>(), item.Category));
                    CategorizedStrings[^1].lines.Add(item);
                }
            }
        }

        internal static List<CategorizedLines> InitializeCategories(string story, string file)
        {
            var CategorizedStrings = new List<CategorizedLines>();

            foreach (StringCategory category in GetCategories(story, file))
            {//add a list for every category we have in the file, so we can then add the strings to these.
                CategorizedStrings.Add((new List<LineData>(), category));
            }

            return CategorizedStrings;
        }

        internal static void WriteCategorizedLinesToDisk(List<CategorizedLines> CategorizedStrings, string path, bool warnOnOverwrite = false)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (warnOnOverwrite)
                if (File.OpenRead(path).Length > 0)
                    if (UI.WarningYesNo("You are about to overwrite " + path + " \nAre you sure?", "Warning", PopupResult.NO))
                        return;
            using var OutputWriter = new StreamWriter(path, false, new UTF8Encoding(true));
            foreach (CategorizedLines CategorizedLines in CategorizedStrings)
            {
                //write category if it has any lines, else we skip the category
                if (CategorizedLines.lines.Count > 0) OutputWriter.WriteLine(CategorizedLines.category.AsString());
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
                    OutputWriter.WriteLine(line.ToString().RemoveVAHints());
                }
                //newline after each category
                OutputWriter.WriteLine();
            }
        }

        public static void ExportTemplatesForStory()
        {
            string path = Utils.SelectSaveLocation("Select a file or folder to export the templates to", checkFileExists: false, checkPathExists: false);
            if (Path.GetExtension(path) != string.Empty) ExportTemplate(path);
            else ExportTemplatesForStory(path);
        }

        public static void ExportTemplate(string path, string story = "", string file = "", bool warnOnOverwrite = false, bool confirmSuccess = true)
        {
            if (path == string.Empty) return;
            if (!File.Exists(path)) File.OpenWrite(path).Close();
            else if (warnOnOverwrite)
                if (UI.WarningYesNo("You are about to overwrite " + path + "\n Are you sure?", "Warning!", PopupResult.NO)) return;

            if (story == string.Empty) story = Utils.ExtractStoryName(path);
            if (file == string.Empty) file = Utils.ExtractFileName(path);

            if (story == "Hints")
                file = "Hints";

            LogManager.Log("Exporting template for" + story + "/" + file + " to " + path);
            if (!DataBase.GetAllLineDataTemplate(file, story, out FileData templates))
            {
                UI.WarningOk("No template found for that story/file, nothing exported.");
                LogManager.Log("\tNo template found for that file");
                return;
            }

            var sortedLines = InitializeCategories(story, file);
            SortIntoCategories(ref sortedLines, templates, templates);

            WriteCategorizedLinesToDisk(sortedLines, path);

            if (confirmSuccess) UI.InfoOk("Template exported to " + path);
            LogManager.Log("\tSucessfully exported the template");
        }

        public static void ExportTemplatesForStory(string path, string story = "")
        {
            if (path == string.Empty) return;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (story == string.Empty) story = Utils.ExtractStoryName(path);
            //todo add fallback to have the user enter the story if the story cant be found
            LogManager.Log("Exporting all templates for " + story + " to " + path);

            //export templates as hints.txt if we have the hints, no need to get filenames
            if (story == "Hints")
                ExportTemplate(Path.Combine(path, "Hints.txt"), story, story, confirmSuccess: false);
            else if (Directory.GetFiles(path).Length > 0)
                foreach (var file in Directory.GetFiles(path))
                {
                    ExportTemplate(file, story, warnOnOverwrite: true, confirmSuccess: false);
                }
            else if (DataBase.GetFilesForStory(story, out string[] names))
                foreach (var item in names)
                {
                    ExportTemplate(Path.Combine(path, item + ".txt"), story, item, confirmSuccess: false);
                }
            else
            {
                UI.WarningOk("No templates found for that story, nothing exported.");
                LogManager.Log("\tNo templates found for that story");
                return;
            }

            UI.InfoOk("Templates exported to " + path);
            LogManager.Log("\tSucessfully exported the templates");
        }
    }
}

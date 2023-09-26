using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using static System.Runtime.InteropServices.JavaScript.JSType;

//TODO add tests

namespace Translator.Core
{
    public delegate bool CreateTemplateFromStoryDelegate(string story, string filename, string path, out FileData data);

    /// <summary>
    /// A class providing functions for loading, approving, and working with strings to be translated. Heavily integrated in all other parts of this application.
    /// </summary>

    public sealed class TranslationManager
    {
        public FileData TranslationData = new(string.Empty, string.Empty);
        private static readonly Timer AutoSaveTimer = new();
        private static string language = Settings.Default.Language;
        private static bool StaticUIInitialized = false;
        private static IUIHandler UI = null!;
        private readonly List<string> SearchQueries = new() { string.Empty };
        private readonly ITab TabUI;
        private bool _changesPending = false;
        private int currentSearchQuery = 0;
        private string fileName = string.Empty;
        private bool isSaveAs = false;
        private bool SearchNeedsCleanup = false;
        private int searchTabIndex = 0;
        private bool selectedNew = false;
        private int SelectedResultIndex = 0;
        private string sourceFilePath = string.Empty;
        private string storyName = string.Empty;
        private bool triedFixingOnce = false;
        private static int returnedTasks = 0;
        private bool multiTranslationRunning = false;
        private bool abortedAutoTranslation = false;

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

        public TranslationManager(IUIHandler ui, ITab tab)
        {
            if (!StaticUIInitialized) { UI = ui; StaticUIInitialized = true; }
            TabUI = tab;
            AutoSaveTimer.Elapsed += SaveFileHandler;

            if (!IsUpToDate && Settings.Default.AdvancedModeEnabled)
                SaveAndExportManager.GenerateOfficialTemplates();
        }

        public static bool IsUpToDate { get; internal set; } = false;
        /// <summary>
        /// The Language of the current translation.
        /// </summary>
        public static string Language
        {
            get
            {
                return language.Length == 0 ? throw new LanguageHelper.LanguageException() : language;
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
        public bool CaseSensitiveSearch { get; private set; } = false;
        public bool ChangesPending
        {
            get { return _changesPending; }
            set
            {
                if (TranslationData.Count > 0)
                {
                    if (value != _changesPending)
                    {
                        _changesPending = value;
                        TabManager.UpdateTabTitle(this, GetTabName());
                    }
                }
            }
        }
        public string CleanedSearchQuery { get; private set; } = string.Empty;
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
        public string SearchQuery { get; private set; } = string.Empty;
        /// <summary>
        /// Provides the id of the currently selected line
        /// </summary>
        public string SelectedId { get { return TabUI.SelectedLineItem.Text; } }
        public LineData SelectedLine
        {
            get
            {
                return SelectedId == string.Empty ? new() : TranslationData[SelectedId];
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
                UpdateApprovedAndTabName();
                UpdateSearchForCurrentEditedLine();
            }
        }

        /// <summary>
        /// Approves the string in the db, if possible. Also updates UI.
        /// </summary>
        /// <param name="SelectNewAfter">A bool to determine if a new string should be selected after approval.</param>
        public void ApproveIfPossible()
        {
            int currentIndex = TabUI.SelectedLineIndex;
            if (currentIndex >= 0)
            {
                //update history
                History.AddAction(new ApprovedChanged(currentIndex, TabUI.Lines, this, fileName, storyName));
                //set checkbox state
                TabUI.ApprovedButtonChecked = TabUI.Lines.GetApprovalState(currentIndex);
                //set checkbox state
                SelectedLine.IsApproved = TabUI.ApprovedButtonChecked;

                UpdateApprovedAndTabName();
                Search();
            }
        }

        public void ExportMissinglinesForCurrentFile()
        {
            SaveAndExportManager.ExportMissingLinesForFile(Utils.SelectSaveLocation(message: "Please select where you want to save the missing lines to", file: FileName + "_missing.txt", createPrompt: true, checkFileExists: false), StoryName, FileName);
        }

        public void ExportMissingLinesForCurrentStory(bool folder)
        {
            if (folder)
                SaveAndExportManager.ExportAllMissinglinesForStoryIntoFolder(Utils.SelectFolderFromSystem("Please select where you want to save the missing lines to"), StoryName);
            else
                SaveAndExportManager.ExportAllMissinglinesForStoryIntoFile(Utils.SelectSaveLocation(message: "Please select where you want to save the missing lines to", file: "all_missing.txt", createPrompt: true, checkFileExists: false), StoryName);
        }

        public string GetTabName()
        {
            float percentage = TabUI.Lines.ApprovedCount / (float)TabUI.Lines.Count;
            return FileName + $" ({(int)(percentage * 100),000}" + (ChangesPending ? "%)*" : "%)");
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
                    _ = UI.InfoOk("Local version saved to database, reload to see changed version.(CTRL+R)");
                }
            }
        }

        /// <summary>
        /// Reloads the file into the program as if it were selected.
        /// </summary>
        public void ReloadFile()
        {
            Settings.Default.RecentIndex = TabUI.SelectedLineIndex;
            TabManager.ShowAutoSaveDialog();
            LoadTranslationFile();
            if (UI is null) return;
            //select recent index
            if (Settings.Default.RecentIndex > 0 && Settings.Default.RecentIndex < TranslationData.Count) TabUI.SelectLineItem(Settings.Default.RecentIndex);
            LogManager.Log($"Reloaded {StoryName}/{FileName}");
        }

        /// <summary>
        /// replaces the template version of the string with a computer translated one to speed up translation.
        /// </summary>
        public void RequestAutomaticTranslation()
        {
            if (SelectedId != string.Empty)
            {
                LogManager.LogDebug("manual autotranslation started for " + SelectedId);
                AutoTranslation.AutoTranslationAsync(new(SelectedLine), Language, AutoTranslationCallback);
                abortedAutoTranslation = false;
            }
        }

        /// <summary>
        /// sets all unapproved translations to an automatic translation of the template
        /// </summary>
        public void RequestAutomaticTranslationForAllUnapproved()
        {
            if (multiTranslationRunning)
            {
                UI.WarningOk("There is already a multi line translation running, please wait on it to finish");
                return;
            }
            multiTranslationRunning = true;
            abortedAutoTranslation = false;
            int NumberOfUnapprovedLines = TabUI.LineCount - TabUI.Lines.ApprovedCount;
            List<LineData> replaced;
            if (NumberOfUnapprovedLines < 0)
                replaced = new();
            else
                replaced = new(NumberOfUnapprovedLines);
            returnedTasks = 0;

            UI.SignalUserWait();
            LogManager.Log($"Starting automatic translation for {NumberOfUnapprovedLines} unapproved lines");
            foreach (var line in TranslationData.Values)
            {
                if (!line.IsApproved && line.TranslationString == line.TemplateString)
                    AutoTranslation.AutoTranslationAsync(line, Language, (bool successfull, LineData data) =>
                    {
                        if (successfull)
                            replaced.Add(data);
                        addReturned();
                    });
            }
            //seperate updates from ui thread
            Task.Factory.StartNew(() => WaitOnAutomaticTranslationsToFinish(NumberOfUnapprovedLines, replaced));

            //methods only used by this here, so embedded :D
            void addReturned() => returnedTasks++;
            void WaitOnAutomaticTranslationsToFinish(int NumberOfUnapprovedLines, List<LineData> replaced)
            {
                //wait on all translations to end
                while (returnedTasks < NumberOfUnapprovedLines && !abortedAutoTranslation) ;
                if (abortedAutoTranslation) return;

                //add changes to history
                var oldData = TranslationData;
                foreach (var translated in replaced)
                {
                    if (abortedAutoTranslation) return;
                    if (!TranslationData[translated.ID].IsApproved)
                    {
                        TranslationData[translated.ID] = translated;
                        UpdateSimilarityMarking(translated.ID);
                    }
                }
                ChangesPending = true;
                History.AddAction(new AllTranslationsChanged(this, oldData, TranslationData));
                TabUI.TranslationsSimilarToTemplate.Clear();
                UI.SignalUserEndWait();
                UI.InfoOk($"{replaced.Count} lines out of {NumberOfUnapprovedLines} unapproved lines were automatically replaced");
                multiTranslationRunning = false;
            }
        }

        /// <summary>
        /// sets all unapproved translations to an automatic translation of the template
        /// </summary>
        public void RequestAutomaticTranslationForAllUntranslated()
        {
            if (multiTranslationRunning)
            {
                UI.WarningOk("There is already a multi line translation running, please wait on it to finish");
                return;
            }
            multiTranslationRunning = true;
            abortedAutoTranslation = false;
            int NumberOfUntranslatedLines = TabUI.TranslationsSimilarToTemplate.Count;
            List<LineData> replaced;
            if (NumberOfUntranslatedLines < 0)
                replaced = new();
            else
                replaced = new(NumberOfUntranslatedLines);
            returnedTasks = 0;

            UI.SignalUserWait();
            LogManager.Log($"Starting automatic translation for {NumberOfUntranslatedLines} untranslated lines");
            foreach (var line in TranslationData.Values)
            {
                if (!line.IsTranslated && line.TranslationString == line.TemplateString)
                    AutoTranslation.AutoTranslationAsync(line, Language, (bool successfull, LineData data) =>
                    {
                        if (successfull)
                            replaced.Add(data);
                        addReturned();
                    });
            }
            //seperate updates from ui thread
            Task.Factory.StartNew(() => WaitOnAutomaticTranslationsToFinish(NumberOfUntranslatedLines, replaced));

            //methods only used by this here, so embedded :D
            void addReturned() => returnedTasks++;
            void WaitOnAutomaticTranslationsToFinish(int NumberOfUntranslatedLines, List<LineData> replaced)
            {
                //wait on all translations to end
                while (returnedTasks < NumberOfUntranslatedLines && !abortedAutoTranslation) ;
                if (abortedAutoTranslation) return;

                var oldData = TranslationData;
                foreach (var translated in replaced)
                {
                    if (abortedAutoTranslation) return;
                    if (TranslationData[translated.ID].ShouldBeMarkedSimilarToEnglish)
                    {
                        TranslationData[translated.ID] = translated;
                        UpdateSimilarityMarking(translated.ID);
                    }
                }
                ChangesPending = true;
                History.AddAction(new AllTranslationsChanged(this, oldData, TranslationData));
                TabUI.TranslationsSimilarToTemplate.Clear();
                UI.SignalUserEndWait();
                UI.InfoOk($"{replaced.Count} lines out of {NumberOfUntranslatedLines} untranslated lines were automatically replaced");
                multiTranslationRunning = false;
            }
        }

        public void AbortAllAutomaticTranslations()
        {
            multiTranslationRunning = false;
            abortedAutoTranslation = true;
            AutoTranslation.AbortAllrunningTranslations();
            LogManager.Log("Automatic translatiosn were aborted!");
            UI.SignalUserEndWait();
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
            if (doOnlineUpdate) _ = Task.Run(RemoteUpdate).ContinueWith(RemoteUpdateExceptionHandler(), TaskContinuationOptions.OnlyOnFaulted);

            List<CategorizedLines> CategorizedStrings = SaveAndExportManager.InitializeCategories(StoryName, FileName);

            //sort online line ids into translations but use local values for translations if applicable
            if (DataBase.GetAllLineDataTemplate(FileName, StoryName, out FileData IdsToExport) && DataBase.IsOnline)
                SaveAndExportManager.SortIntoCategories(ref CategorizedStrings, IdsToExport, TranslationData); //export only ids from db
            else
                SaveAndExportManager.SortIntoCategories(ref CategorizedStrings, TranslationData, TranslationData); //eyxport all ids we have

            //save all categorized lines to disk
            SaveAndExportManager.WriteCategorizedLinesToDisk(CategorizedStrings, SourceFilePath);

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
                if (!DataBase.UpdateTranslations(TranslationData, Language)) _ = UI.InfoOk("You seem to be offline, translations are going to be saved locally but not remotely.");
                else LogManager.Log("Successfully saved the file remotely");
                UI.SignalUserEndWait();
            }

            static Action<Task> RemoteUpdateExceptionHandler()
            {
                return faultedTask =>
                {
                    if (faultedTask.Exception is null) return;
                    LogManager.Log(faultedTask.Exception.Message);
                    foreach (Exception exception in faultedTask.Exception.InnerExceptions)
                    {
                        LogManager.Log(exception.Message);
                        LogManager.Log("    " + exception.StackTrace);
                    }
                };
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
        /// Selects the next search result if applicable
        /// </summary>
        /// <returns>True if a new result could be selected</returns>
        public bool SelectNextResultIfApplicable()
        {
            if (IsSearchFocused() && TabUI.Lines.SearchResults.Count > 0)
            {
                if (SelectedResultIndex < 0) SelectedResultIndex = 0;
                //loop back to start
                if (SelectedResultIndex > TabUI.Lines.SearchResults.Count - 1)
                {
                    SelectedResultIndex = 0;
                    //loop over to new tab when in global search
                    if (TabManager.InGlobalSearch)
                    {
                        searchTabIndex = TabManager.SelectedTabIndex;
                        TabManager.SwitchToTab(++searchTabIndex);
                        SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                    }
                    else
                    {
                        //select next index from list of matches
                        if (SelectedResultIndex < TabUI.LineCount)
                        {
                            SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                            ++SelectedResultIndex;
                        }
                    }
                }
                else
                {
                    if (SelectedResultIndex < TabUI.Lines.SearchResults.Count)
                    {
                        SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                        ++SelectedResultIndex;
                    }
                    else if (TabManager.InGlobalSearch)
                    {
                        searchTabIndex = TabManager.SelectedTabIndex;
                        SelectedResultIndex = 1;
                        TabManager.SwitchToTab(++searchTabIndex);
                        SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                        return true;
                    }
                    else
                    {
                        SelectedResultIndex = 1;
                        SelectLine(TabUI.Lines.SearchResults[0]);
                    }
                }

                UI.SelectedSearchResult = SelectedResultIndex;

                if (!SearchQueries.Contains(SearchQuery))
                {
                    SearchQueries.Add(SearchQuery);
                    currentSearchQuery = SearchQueries.Count - 1;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SelectPreviousResultIfApplicable()
        {
            if (!TabUI.IsTranslationBoxFocused && !TabUI.IsCommentBoxFocused && TabUI.Lines.SearchResults.Count > 0)
            {
                if (SelectedResultIndex < 0) SelectedResultIndex = TabUI.Lines.SearchResults.Count - 1;
                //loop back to start
                if (SelectedResultIndex < 1)
                {
                    SelectedResultIndex = TabUI.Lines.SearchResults.Count - 1;
                    //loop over to new tab when in global search
                    if (TabManager.InGlobalSearch)
                    {
                        searchTabIndex = TabManager.SelectedTabIndex;
                        TabManager.SwitchToTab(--searchTabIndex);
                    }
                    SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                }
                else
                {
                    if (SelectedResultIndex > 0)
                    {
                        if (SelectedResultIndex + 2 == UI.SelectedSearchResult)
                            SelectedResultIndex -= 2;
                        else
                            --SelectedResultIndex;
                        SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                    }
                    else if (TabManager.InGlobalSearch)
                    {
                        searchTabIndex = TabManager.SelectedTabIndex;
                        SelectedResultIndex = TabUI.Lines.SearchResults.Count - 1;
                        TabManager.SwitchToTab(--searchTabIndex);
                        SelectLine(TabUI.Lines.SearchResults[SelectedResultIndex]);
                        return true;
                    }
                    else
                    {
                        SelectedResultIndex = TabUI.Lines.SearchResults.Count;
                        SelectLine(TabUI.Lines.SearchResults[^1]);
                    }
                }

                UI.SelectedSearchResult = SelectedResultIndex + 1;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryCycleSearchDown()
        {
            if (!IsSearchFocused()) return false;

            if (currentSearchQuery > 1)
            {
                --currentSearchQuery;
                Search(SearchQueries[currentSearchQuery]);
            }
            else
            {
                currentSearchQuery = 0;
                Search(string.Empty);
            }
            UI.SearchBarText = SearchQueries[currentSearchQuery];
            return true;
        }

        public bool TryCycleSearchUp()
        {
            if (!IsSearchFocused()) return false;

            if (currentSearchQuery < SearchQueries.Count - 1)
            {
                ++currentSearchQuery;
                Search(SearchQueries[currentSearchQuery]);
            }
            else
            {
                currentSearchQuery = 0;
                Search(string.Empty);
            }
            UI.SearchBarText = SearchQueries[currentSearchQuery];
            return true;
        }

        public void UpdateComments()
        {
            //remove pipe to not break saving/export
            SelectedLine.Comments = TabUI.CommentBoxTextArr;
        }

        /// <summary>
        /// Update the currently selected translation string in the TranslationData.
        /// </summary>
        public void UpdateTranslationString()
        {
            //remove pipe to not break saving/export
            TabUI.TranslationBoxText = TabUI.TranslationBoxText.Replace('|', ' ');
            SelectedLine.TranslationString = TabUI.TranslationBoxText.Replace(Environment.NewLine, "\n");
            UpdateCharacterCountLabel();
            ChangesPending = !selectedNew || ChangesPending;
            selectedNew = false;
            if (ChangesPending) _ = TabUI.TranslationsSimilarToTemplate.Remove(SelectedId);
            UpdateSearchForCurrentEditedLine();
        }

        /// <summary>
        /// Loads a file into the program and calls all UI routines
        /// </summary>
        /// <param name="path">The path to the file to translate</param>
        internal void LoadFileIntoProgram(string path)
        {
            if (path == string.Empty) return;
            if (File.Exists(path))
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
                    UI.UpdateRecentFileList();
                    //update search so it makes sense
                    Search();
                }
            }
            else
            {
                UI.ErrorOk("File doesn'indexOfSelectedSearchResult exist");
            }
        }

        /// <summary>
        /// Populates the Editor/Template text boxes and does some basic set/reset logic.
        /// </summary>
        internal void PopulateTextBoxes()
        {
            int currentIndex = TabUI.SelectedLineIndex;

            if (currentIndex >= 0)
            {
                TabUI.TemplateBoxText = Settings.Default.DisplayVoiceActorHints
                    ? SelectedLine.TemplateString.Replace("\n", Environment.NewLine)
                    : SelectedLine.TemplateString.Replace("\n", Environment.NewLine).RemoveVAHints();

                selectedNew = true;

                //display the string in the editable window
                TabUI.TranslationBoxText = SelectedLine.TranslationString.Replace("\n", Environment.NewLine);

                //translate if useful and possible
                ConvenienceAutomaticTranslation();

                TabUI.CommentBoxTextArr = SelectedLine.Comments;

                //sync approvedbox and list
                TabUI.ApprovedButtonChecked = SelectedLine.IsApproved;

                //update label
                UpdateCharacterCountLabel();

                TabUI.SetSelectedTranslationBoxText(SelectedLine.TranslationLength, SelectedLine.TranslationLength);

                UpdateSearchAndSearchHighlight();
            }
            else
            {
                if (TabUI.LineCount > 0) TabUI.SelectLineItem(0);
            }
            UpdateApprovedAndTabName();
        }

        internal void ReloadTranslationTextbox()
        {
            //update textbox
            if (SelectedId != string.Empty)
                TabUI.TranslationBoxText = SelectedLine.TranslationString.Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Replaces a searched string in all applicable lines by the replacement provided using the invariant culture.
        /// </summary>
        /// <param name="replacement">The string to replace all search matches with</param>
        internal void ReplaceAll(string replacement)
        {
            if (TabUI.Lines.SearchResults.Count == 0) return;
            //save old lines for history
            FileData old = new(TranslationData, StoryName, FileName);

            for (int i = 0; i < TabUI.Lines.SearchResults.Count; ++i)
            {
                if (TabUI.Lines.SearchResults[i] < 0) continue;
                TranslationData[TabUI.Lines[TabUI.Lines.SearchResults[i]].Text].TranslationString = Replacer.Replace(TranslationData[TabUI.Lines[TabUI.Lines.SearchResults[i]].Text].TranslationString, replacement, SearchQuery).ToString();
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
        internal void ReplaceSingle(string replacement)
        {
            if (TabUI.Lines.SearchResults.Contains(TabUI.SelectedLineIndex))
            {
                string temp = Replacer.Replace(SelectedLine.TranslationString, replacement, SearchQuery).ToString();
                History.AddAction(new TranslationChanged(this, SelectedId, SelectedLine.TranslationString, temp));
                SelectedLine.TranslationString = temp;

                //update search results
                Search();

                //update textbox
                ReloadTranslationTextbox();
            }
        }

        /// <summary>
        /// Performs a search through all lines currently loaded.
        /// </summary>
        internal void Search()
        {
            SearchQuery = UI.SearchBarText;
            Search(SearchQuery);
        }

        /// <summary>
        /// Performs a search through all lines currently loaded.
        /// </summary>
        /// <param name="query">The search temr to look for</param>
        internal void Search(string query)
        {
            if (query.Length > 0)
            {
                SearchNeedsCleanup = true;
                if (Searcher.Search(query, TranslationData, out List<int>? results, out ReadOnlySpan<char> cleanedSpanQuery))
                {
                    CleanedSearchQuery = cleanedSpanQuery.ToString();

                    TabUI.Lines.SearchResults.Clear();
                    TabUI.Lines.SearchResults.AddRange(results!);
                    UI.SearchResultCount = TabUI.Lines.SearchResults.Count;
                    UpdateSearchAndSearchHighlight();
                    return;
                }
                else
                {
                    UI.SignalUserPing();
                }
            }

            UI.SearchResultCount = 0;
            CleanedSearchQuery = query;
            if (SearchNeedsCleanup)
            {
                TabUI.Lines.SearchResults.Clear();
                UpdateSearchAndSearchHighlight();
                SearchNeedsCleanup = false;
            }
        }

        /// <summary>
        /// Loads the strings and does some work around to ensure smooth sailing.
        /// pupulates the states of the lines read in from the file
        /// </summary>
        private void AddLinesToUIAndIntegrateOnline(bool localTakesPriority = false)
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
                    if (!localTakesPriority
                        && DataBase.IsOnline
                        && tempLine.TranslationLength > 0)
                        TranslationData[key].TranslationString = tempLine.TranslationString;
                    else if (!DataBase.IsOnline) TranslationData[key].TemplateString = tempLine.TemplateString;
                    TranslationData[key].IsApproved = tempLine.IsApproved;
                }

                if (TranslationData[key].TemplateString is null) TranslationData[key].TemplateString = string.Empty;

                TabUI.Lines.Add(key, TranslationData[key].IsApproved);

                //colour string if similar to the english one
                UpdateSimilarityMarking(key);

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

        public void UpdateSimilarityMarking(string id)
        {
            if (!TranslationData.ContainsKey(id)) return;

            if (TranslationData[id].ShouldBeMarkedSimilarToEnglish)
            {
                if (!TabUI.TranslationsSimilarToTemplate.Contains(id))
                    TabUI.TranslationsSimilarToTemplate.Add(id);

                TranslationData[id].IsTranslated = false;
            }
            else
            {
                TranslationData[id].IsTranslated = true;
                _ = TabUI.TranslationsSimilarToTemplate.Remove(id);
            }
        }

        //applies the changes back to our linedata object in use
        private void AutoTranslationCallback(bool successfull, LineData data)
        {
            if (abortedAutoTranslation) return;
            if (successfull)
            {
                History.AddAction(new TranslationChanged(this, data.ID, TranslationData[data.ID].TranslationString, data.TranslationString));
                TranslationData[data.ID] = data;
                if (data.ID == SelectedId)
                    ReloadTranslationTextbox();
                UpdateSimilarityMarking(data.ID);
                LogManager.LogDebug("manual autotranslation for " + data.ID + " succeeded");
            }
            else if (Settings.Default.AutoTranslate)
            {
                if (UI.WarningYesNo("The translator seems to be unavailable. Turn off autotranslation? (needs to be turned back on manually!)", "Turn off autotranslation", PopupResult.YES))
                    Settings.Default.AutoTranslate = false;
            }
        }

        /// <summary>
        /// replaces the template version of the string with a computer translated one to speed up translation, but only if no human input has been given on the new line so far.
        /// </summary>
        private void ConvenienceAutomaticTranslation()
        {
            //todo change this so it shows as a placeholder type of text?
            if (!multiTranslationRunning)
                if (TabUI.TemplateBoxText == TabUI.TranslationBoxText && !SelectedLine.IsTranslated && !SelectedLine.IsApproved && SelectedLine.TemplateLength > 0)
                {
                    LogManager.LogDebug("running convinience autotranslation for " + SelectedId);
                    AutoTranslation.AutoTranslationAsync(new(SelectedLine), Language, ConvenienceTranslationCallback);
                    abortedAutoTranslation = false;
                }
        }

        //applies the changes back to our linedata object in use
        private void ConvenienceTranslationCallback(bool successfull, LineData data)
        {
            if (abortedAutoTranslation) return;
            if (successfull)
            {
                if (TranslationData[data.ID].TranslationString == data.TemplateString || TranslationData[data.ID].TranslationString.Length == 0)
                {
                    History.AddAction(new TranslationChanged(this, data.ID, TranslationData[data.ID].TranslationString, data.TranslationString));
                    TranslationData[data.ID] = data;
                    if (data.ID == SelectedId)
                        ReloadTranslationTextbox();
                    UpdateSimilarityMarking(data.ID);
                    LogManager.LogDebug("convinience autotranslation completed for " + data.ID);
                }
            }
            else
            {
                if (UI.WarningYesNo("The translator seems to be unavailable. Turn off autotranslation? (needs to be turned back on manually!)", "Turn off autotranslation", PopupResult.YES))
                    Settings.Default.AutoTranslate = false;
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
                if (StoryName is not "Hints" and not "UI")
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

        //Creates the actual linedata objects from the file
        private void CreateLineInTranslations(string[] lastLine, StringCategory category, FileData IdsToExport, string translation)
        {
            if (lastLine[0] == string.Empty) return;
            TranslationData[lastLine[0]] = IdsToExport.TryGetValue(lastLine[0], out LineData? templateLine)
                ? new LineData(lastLine[0], StoryName, FileName, category, templateLine.TemplateString, lastLine[1] + translation)
                : new LineData(lastLine[0], StoryName, FileName, category, string.Empty, lastLine[1] + translation);
        }

        private bool CustomStoryTemplateHandle(string story)
        {
            PopupResult result = !Settings.Default.AllowCustomStories
                ? PopupResult.NO
                : Settings.Default.IgnoreCustomStoryWarning
                    ? PopupResult.YES
                    : UI.InfoYesNo($"Detected {story} as the story to use, if this is a custom story you want to translate, you can do so. Choose yes if you want to do that. If not, select no and we will assume this is the Original Story. (You can disable this warning in the settings)", "Custom story?");
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
                                _ = DataBase.UpdateTemplates(templates);

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

        private void DisableHighlights()
        {
            TabUI.Template.ShowHighlight = false;
            TabUI.Translation.ShowHighlight = false;
            TabUI.Comments.ShowHighlight = false;
        }

        private FileData GetTemplatesFromUser()
        {
            return UI.InfoYesNo("Do you have the translation template from Don/Eek available? If so, we can use those if you hit yes, if you hit no we can generate templates from the game's story files.", "Templates available?", PopupResult.YES)
                ? SaveAndExportManager.GetTemplateFromFile(Utils.SelectFileFromSystem(false, $"Choose the template for {StoryName}/{FileName}.", FileName + ".txt"), StoryName, FileName, false)
                : new FileData(StoryName, FileName);
        }

        private bool IsSearchFocused() => !TabUI.IsTranslationBoxFocused && !TabUI.IsCommentBoxFocused;

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
                    AddLinesToUIAndIntegrateOnline(localTakesPriority);
                    //update tab name
                    TabManager.UpdateTabTitle(this, GetTabName());
                    UpdateApprovedAndTabName();

                    if (Settings.Default.HighlightLanguages)
                    {
                        if (DataBase.GetLanguagesForStory(StoryName, out string[] languages))
                            UI.SetLanguageHighlights(languages);
                    }
                }
                else
                {
                    ResetTranslationManager();
                }
            }
        }

        /// <summary>
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        private void ReadStringsTranslationsFromFile()
        {
            FileData templates;
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
                SplitReadTranslations(LinesFromFile, templates);
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
                //and it would take a lot o time to get them all. so just have the user save it once and reload. we might do this automatically, but i don'indexOfSelectedSearchResult know if that is ok to do :)
                {
                    _ = UI.InfoOk(
                    "Some strings are missing from your translation, they will be added with the english version when you first save the file!",
                    "Some strings missing");
                    ChangesPending = true;
                }
            }
        }

        /// <summary>
        /// Resets the translation manager.
        /// </summary>
        private void ResetTranslationManager()
        {
            Settings.Default.RecentIndex = TabUI.SelectedLineIndex;
            TranslationData.Clear();
            TabUI.Lines.Clear();
            TabUI.TranslationsSimilarToTemplate.Clear();
            SelectedResultIndex = 0;
            TabManager.UpdateSelectedTabTitle("Tab");
            TabUI.SetApprovedCount(1, 1, "empty tab");
        }

        private void SaveFileHandler(object? sender, ElapsedEventArgs? e)
        {
            SaveFile();
        }

        private void UpdateSearchForCurrentEditedLine()
        {
            int index = TabUI.SelectedLineIndex;
            if (Searcher.Search(SearchQuery, SelectedLine))
            {
                if (!TabUI.Lines.SearchResults.Contains(index))
                    TabUI.Lines.SearchResults.Add(index);

                UI.SearchResultCount = TabUI.Lines.SearchResults.Count;
                UpdateHighlightPositions(index);
            }
            else if (TabUI.Lines.SearchResults.Remove(index))
            {
                UI.SearchResultCount = TabUI.Lines.SearchResults.Count;
                DisableHighlights();
            }
        }

        private void SelectLine(int i)
        {
            TabUI.SelectLineItem(i);
        }

        private void SplitReadTranslations(List<string> LinesFromFile, FileData IdsToExport)
        {
            string[] lastLine = Array.Empty<string>();
            string multiLineCollector = string.Empty;
            StringCategory category = StringCategory.General;
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
                        multiLineCollector = multiLineCollector.TrimEnd(Extensions.trimmers);
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
                                multiLineCollector = multiLineCollector.TrimEnd(Extensions.trimmers);
                                CreateLineInTranslations(lastLine, category, IdsToExport, multiLineCollector);
                            }
                            else
                            {//write last line with id if no real line of text is afterwards
                                CreateLineInTranslations(lastLine, category, IdsToExport, string.Empty);
                            }
                        }
                        //only set new category after we added the last line of the old one
                        category = tempCategory;
                        //resetting for next iteration
                        lastLine = Array.Empty<string>();
                        multiLineCollector = string.Empty;
                    }
                }
            }

            if (lastLine.Length > 0)
            {
                //add last line (dont care about duplicates because the dict)
                CreateLineInTranslations(lastLine, category, IdsToExport, string.Empty);
            }
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

        private void UpdateApprovedAndTabName()
        {
            TabUI.SetApprovedCount(
                TabUI.Lines.ApprovedCount,
                TabUI.Lines.Count,
                $"Approved: {TabUI.Lines.ApprovedCount} / {TabUI.Lines.Count} {(int)(TabUI.Lines.ApprovedCount / (float)TabUI.Lines.Count * 100)}%");
            TabUI.UpdateTranslationProgressIndicator();
            TabUI.Text = GetTabName();
        }

        private void UpdateCharacterCountLabel()
        {
            if (SelectedLine.TranslationLength <= SelectedLine.TemplateLength * 1.1f)
            {
                TabUI.SetCharacterLabelColor(Color.LawnGreen);
            }//if bigger by no more than 30 percent
            else if (SelectedLine.TranslationLength <= SelectedLine.TemplateLength * 1.3f)
            {
                TabUI.SetCharacterLabelColor(Color.DarkOrange);
            }
            else
            {
                TabUI.SetCharacterLabelColor(Color.Red);
            }
            TabUI.UpdateCharacterCounts(SelectedLine.TemplateLength, SelectedLine.TranslationLength);
        }

        private void UpdateHighlightPositions(int indexOfSelectedSearchResult)
        {
            if (SelectedResultIndex > 0) SelectedResultIndex = indexOfSelectedSearchResult;
            UI.SelectedSearchResult = SelectedResultIndex + 1;

            int TemplateTextQueryLocation, TranslationTextQueryLocation = -1, CommentsTextQueryLocation = -1;
            if (CaseSensitiveSearch)
            {
                TemplateTextQueryLocation = TabUI.TemplateBoxText.IndexOf(CleanedSearchQuery);
                if (Settings.Default.ShowTranslationHighlight)
                    TranslationTextQueryLocation = TabUI.TranslationBoxText.IndexOf(CleanedSearchQuery);
                if (Settings.Default.ShowCommentHighlight)
                    CommentsTextQueryLocation = TabUI.CommentBoxText.IndexOf(CleanedSearchQuery);
            }
            else
            {
                TemplateTextQueryLocation = TabUI.TemplateBoxText.IndexOf(CleanedSearchQuery, StringComparison.InvariantCultureIgnoreCase);
                if (Settings.Default.ShowTranslationHighlight)
                    TranslationTextQueryLocation = TabUI.TranslationBoxText.IndexOf(CleanedSearchQuery, StringComparison.InvariantCultureIgnoreCase);
                if (Settings.Default.ShowCommentHighlight)
                    CommentsTextQueryLocation = TabUI.CommentBoxText.IndexOf(CleanedSearchQuery, StringComparison.InvariantCultureIgnoreCase);
            }
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

            if (TranslationTextQueryLocation >= 0)
            {
                TabUI.Translation.HighlightStart = TranslationTextQueryLocation;
                TabUI.Translation.HighlightEnd = TranslationTextQueryLocation + CleanedSearchQuery.Length;
                TabUI.Translation.ShowHighlight = true;
            }
            else
            {
                TabUI.Translation.ShowHighlight = false;
            }

            if (CommentsTextQueryLocation >= 0)
            {
                TabUI.Comments.HighlightStart = CommentsTextQueryLocation;
                TabUI.Comments.HighlightEnd = CommentsTextQueryLocation + CleanedSearchQuery.Length;
                TabUI.Comments.ShowHighlight = true;
            }
            else
            {
                TabUI.Comments.ShowHighlight = false;
            }
        }

        private void UpdateSearchAndSearchHighlight()
        {
            TabUI.UpdateSearchResultDisplay();
            //renew search result if possible
            int t = TabUI.Lines.SearchResults.IndexOf(TabUI.SelectedLineIndex);
            if (t >= 0)
            {
                UpdateHighlightPositions(t);
            }
            else
            {
                DisableHighlights();
            }
        }
    }
}

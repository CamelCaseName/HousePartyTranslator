using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;
using Timer = System.Timers.Timer;

//TODO add tests

namespace Translator.Core
{
	public delegate bool CreateTemplateFromStoryDelegate(string story, string filename, string path, out FileData data);

	/// <summary>
	/// A class providing functions for loading, approving, and working with strings to be translated. Heavily integrated in all other parts of this application.
	/// </summary>
	public class TranslationManager<TLineItem, TUIHandler, TTabController, TTab> : ITranslationManager<TLineItem>
		where TLineItem : class, ILineItem, new()
		where TUIHandler : class, IUIHandler<TLineItem, TTabController, TTab>, new()
		where TTabController : class, ITabController<TLineItem, TTab>, new()
		where TTab : class, ITab<TLineItem>, new()
	{
		public bool ChangesPending
		{
			get { return _changesPending; }
			set
			{
				if (value)
					TabManager<TLineItem, TUIHandler, TTabController, TTab>.UpdateTabTitle(this, FileName + "*");
				else
					TabManager<TLineItem, TUIHandler, TTabController, TTab>.UpdateTabTitle(this, FileName ?? "");
				_changesPending = value;
			}
		}
		private bool _changesPending = false;
		public static bool IsUpToDate { get; internal set; } = false;
		public List<StringCategory> CategoriesInFile = new();
		public bool isTemplate = false;
		public string SearchQuery { get; private set; } = string.Empty;
		public string CleanedSearchQuery { get; private set; } = string.Empty;
		public bool CaseSensitiveSearch { get; private set;} = false;

		private static readonly Timer AutoSaveTimer = new();

		public int SelectedResultIndex = 0;
		public TLineItem SelectedSearchResultItem => SelectedResultIndex < TabUI.LineCount ? TabUI.Lines[SelectedResultIndex] : new TLineItem();

		//counter so we dont get multiple ids, we dont use the dictionary as ids anyways when uploading templates
		private int templateCounter = 0;

		public FileData TranslationData = new();
		private string fileName = "";
		private bool isSaveAs = false;
		private static string language = Settings.Default.Language;
		private int searchTabIndex = 0;
		private bool selectedNew = false;
		private string sourceFilePath = "";
		private string storyName = "";
#nullable disable
		private static IUIHandler<TLineItem, TTabController, TTab> UI;
#nullable restore
		private static bool StaticUIInitialized = false;
		private readonly ITab<TLineItem> TabUI;
		private bool triedFixingOnce = false;
		private bool triedSavingFixOnce = false;

		public TranslationManager(IUIHandler<TLineItem, TTabController, TTab> ui, ITab<TLineItem> tab)
		{
			if (!StaticUIInitialized) { UI = ui; StaticUIInitialized = true; }
			TabUI = tab;
			AutoSaveTimer.Elapsed += SaveFileHandler;
		}

		static TranslationManager()
		{
			if (Settings.Default.AutoSaveInterval > TimeSpan.FromMinutes(1))
			{
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
						_ = UI.InfoOk($"Not flagged as custom story, can't find \"{value}\", assuming Original Story.");
						storyName = "Original Story";
					}
				}
			}
		}

		public bool CustomStoryTemplateHandle(string story)
		{
			PopupResult result;
			if (Settings.Default.AllowCustomStories)
			{
				result = PopupResult.YES;
			}
			else
			{
				result = UI.InfoYesNo($"Detected {story} as the story to use, if this is a custom story you want to translate, you can do so. Choose yes if you want to do that. If not, select no and we will assume this is the Original Story", "Custom story?");
			}
			if (result == PopupResult.YES || Settings.Default.IgnoreCustomStoryWarning)
			{
				UI.SignalUserWait();
				//check if the template has been added and generated once before
				_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.GetAllLineDataTemplate(fileName, story, out FileData data);
				if (data.Count == 0)
				{//its a custom story but no template so far on the server
				 //use contextprovider to extract the story object and get the lines
					PopupResult typeResult;
					if ((typeResult = UI.InfoYesNoCancel($"You will now be prompted to select the corresponding .story or .character file for the translation you want to do. Is {FileName} a character?", "Custom story?")) != PopupResult.CANCEL)
					{
						if (UI.CreateTemplateFromStory(story, fileName, SourceFilePath, out FileData templates))
							if (templates.Count > 0)
							{
								_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.RemoveOldTemplates(FileName, story);
								_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.UploadTemplates(templates);

								UI.SignalUserEndWait();
								return true;
							}
						_ = UI.ErrorOk("Something broke, please try again.");
					}
				}
				else
				{
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
				if (Index >= 0) TabUI.Lines.SetApprovalState(Index, TabUI.Lines.GetApprovalState(Index));
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
				TabUI.ApprovedButtonChecked = !TabUI.Lines.GetApprovalState(currentIndex);
				//set checkbox state
				SelectedLine.IsApproved = TabUI.ApprovedButtonChecked;

				//move one string down if possible
				if (SelectNewAfter)
				{
					if (currentIndex < TabUI.Lines.Count - 1) TabUI.SelectLineItem(currentIndex + 1);
				}

				UpdateApprovedCountLabel(TabUI.Lines.ApprovedCount, TabUI.Lines.ApprovedCount);
				UI.UpdateTranslationProgressIndicator();
			}
		}

		/// <summary>
		/// Loads a file into the program and calls all UI routines
		/// </summary>
		public void LoadFileIntoProgram()
		{
			LoadFileIntoProgram(Utils<TLineItem, TUIHandler, TTabController, TTab>.SelectFileFromSystem());
		}

		/// <summary>
		/// Loads a file into the program and calls all UI routines
		/// </summary>
		/// <param name="path">The path to the file to translate</param>
		public void LoadFileIntoProgram(string path)
		{
			if (path.Length > 0)
			{
				if (TranslationData.Count > 0) TabManager<TLineItem, TUIHandler, TTabController, TTab>.ShowAutoSaveDialog();
				//clear history if we have a new file, we dont need old one anymore
				if (path != SourceFilePath && FileName != string.Empty && StoryName != string.Empty)
					History.ClearForFile<TLineItem, TUIHandler, TTabController, TTab>(FileName, StoryName);
				ResetTranslationManager();

				if (!IsUpToDate && Settings.Default.AdvancedModeEnabled && DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline)
				{
					LoadTemplates();
				}
				else
				{
					SourceFilePath = path;
					LoadTranslationFile();
				}

				if (TranslationData.Count > 0)
				{
					//log file loading if successfull
					LogManager.Log($"File opened: {StoryName}/{FileName} at {DateTime.Now}");

					//update recents
					RecentsManager.SetMostRecent(SourceFilePath);
					UI.SetFileMenuItems(RecentsManager.GetUpdatedMenuItems<TLineItem, TUIHandler, TTabController, TTab>(UI.FileMenuItems));
				}
			}
		}

		private void LoadTemplates()
		{
			string folderPath = Utils<TLineItem, TUIHandler, TTabController, TTab>.SelectTemplateFolderFromSystem();
			string templateFolderName = folderPath.Split('\\')[^1];
			if (templateFolderName == "TEMPLATE")
			{
				isTemplate = true;
				LoadAndSyncTemplates(folderPath);
			}
			else
			{
				isTemplate = false;
				_ = UI.WarningOk(
					$"Please the template folder named 'TEMPLATE' so we can upload them. " +
					$"Your Current Folder shows as {templateFolderName}.",
					"Updating string DataBase");
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
			UI.SignalUserWait();
			int currentIndex = TabUI.SelectedLineIndex;

			if (currentIndex >= 0)
			{
				TabUI.TemplateBoxText = SelectedLine.TemplateString.Replace("\n", Environment.NewLine);

				if (!isTemplate)
				{
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
					UpdateCharacterCountLabel(SelectedLine.TemplateLength, SelectedLine.TranslationLength);

					TabUI.SetSelectedTranslationBoxText(SelectedLine.TranslationLength, SelectedLine.TranslationLength);

					UpdateSearchAndSearchHighlight();
				}
			}
			else
			{
				if (TabUI.LineCount > 0) TabUI.SelectLineItem(0);
			}
			UpdateApprovedCountLabel(TabUI.Lines.ApprovedCount, TabUI.LineCount);
			UI.SignalUserEndWait();
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
			FileData old = new(TranslationData);

			for (int i = 0; i < TabUI.Lines.SearchResults.Count; ++i)
			{
				if (TabUI.Lines.SearchResults[i].Length == 0) continue;
				TranslationData[TabUI.Lines.SearchResults[i]].TranslationString = TranslationData[TabUI.Lines.SearchResults[i]].TranslationString.ReplaceImpl(replacement, CleanedSearchQuery);
			}

			History.AddAction(new AllTranslationsChanged<TLineItem, TUIHandler, TTabController, TTab>(this, old, TranslationData));

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
				History.AddAction(new TranslationChanged<TLineItem, TUIHandler, TTabController, TTab>(this, SelectedId, SelectedLine.TranslationString, temp));
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
		public void RequestedAutomaticTranslation()
		{
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
				_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.UpdateTranslation(SelectedLine, Language);

				if (TabUI.SimilarStringsToEnglish.Contains(SelectedId)) _ = TabUI.SimilarStringsToEnglish.Remove(SelectedId);

				UI.SignalUserEndWait();
			}
		}

		/// <summary>
		/// Saves all strings to the file we read from.
		/// </summary>
		public void SaveFile()
		{
			UI.SignalUserWait();

			History.ClearForFile<TLineItem, TUIHandler, TTabController, TTab>(FileName, StoryName);

			if (SourceFilePath == "" || Language == "")
			{
				UI.SignalUserEndWait();
				return;
			}
			_ = Task.Run(RemoteUpdate);

			List<CategorizedLines> CategorizedStrings = InitializeCategories();

			//sort online line ids into translations but use local values for translations if applicable
			if (DataBase<TLineItem, TUIHandler, TTabController, TTab>.GetAllLineDataTemplate(FileName, StoryName, out FileData IdsToExport) && DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline)
				SortIntoCategories(CategorizedStrings, IdsToExport);
			else
				SortIntoCategories(CategorizedStrings, TranslationData);

			//save all categorized lines to disk
			WriteCategorizedLinesToDisk(CategorizedStrings);

			//copy file to game rather than writing again
			if (Settings.Default.AlsoSaveToGame)
			{
				CopyToGameModsFolder();
			}
			UI.SignalUserEndWait();

			void RemoteUpdate()
			{
				UI.SignalUserWait();
				if (!DataBase<TLineItem, TUIHandler, TTabController, TTab>.UpdateTranslations(TranslationData, Language) || !DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline) _ = UI.InfoOk("You seem to be offline, translations are going to be saved locally but not remotely.");
				UI.SignalUserEndWait();
			}
		}

		private List<CategorizedLines> InitializeCategories()
		{
			var CategorizedStrings = new List<CategorizedLines>();

			//we need to check whether the file has any strings at all, expecially the categories, if no, add them first or shit breaks.
			if (CategoriesInFile.Count == 0) GenerateCategories();

			foreach (StringCategory category in CategoriesInFile)
			{//add a list for every category we have in the file, so we can then add the strings to these.
				CategorizedStrings.Add((new List<LineData>(), category));
			}

			return CategorizedStrings;
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
						_ = Directory.CreateDirectory(Path.GetDirectoryName(gameFilePath) ?? "");
						File.Copy(SourceFilePath, gameFilePath, true);
					}
				}
			}
		}

		private void WriteCategorizedLinesToDisk(List<CategorizedLines> CategorizedStrings)
		{
			CultureInfo culture = CultureInfo.InvariantCulture;
			using var OutputWriter = new StreamWriter(SourceFilePath, false, new UTF8Encoding(true));
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
					OutputWriter.WriteLine(line.ToString());
				}
				//newline after each category
				OutputWriter.WriteLine();
			}
		}

		private void SortIntoCategories(List<CategorizedLines> CategorizedStrings, FileData IdsToExport)
		{
			foreach (LineData item in IdsToExport.Values)
			{
				//add template to list if no translation is in the file
				if (DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline)
				{
					if (TranslationData.TryGetValue(item.ID, out LineData? TempResult))
					{
						item.TranslationString = TempResult?.TranslationString ?? IdsToExport[item.ID].TemplateString;
					}
					else
					{
						item.TranslationString = IdsToExport[item.ID].TemplateString;
					}
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
				string SaveFile = SelectSaveLocation();
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
		public string SelectSaveLocation()
		{
			if (!UI.SaveFileDialogType.IsAssignableTo(typeof(ISaveFileDialog))) throw new ArgumentException($"{nameof(UI.SaveFileDialogType)} does not inherit {nameof(ISaveFileDialog)}");

			var saveFileDialog = (ISaveFileDialog?)Activator.CreateInstance(UI.SaveFileDialogType, new object?[]
			{
                /*Title*/"Choose a file to save the translations to",
                /*Extension*/ "txt",
                /*CreatePrompt*/ true,
                /*OverwritePrompt*/ true,
                /*FileName*/ FileName,
                /*InitialDirectory*/ SourceFilePath
			});
			if (saveFileDialog == null) return string.Empty;

			if (saveFileDialog.ShowDialog() == PopupResult.OK)
			{
				return saveFileDialog.FileName;
			}
			return string.Empty;
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
					CaseSensitiveSearch= true;
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
					//methodolgy: highlight items which fulfill search and show count
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
		/// Selects the index given in the list of strings
		/// </summary>
		/// <param name="index">The index to select</param>
		public static void SelectLine(int index)
		{
			if (index >= 0 && index < TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTab.LineCount) TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTab.SelectLineItem(index);
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
			else if (Settings.Default.Language != "")
			{
				string languageFromFile = Settings.Default.Language;
				if (languageFromFile != "")
				{
					Language = languageFromFile;
				}
			}
			UI.Language = Language;
		}

		/// <summary>
		/// Update the currently selected translation string in the TranslationData.
		/// </summary>
		public void UpdateTranslationString()
		{
			//remove pipe to not break saving/export
			TabUI.TranslationBoxText = TabUI.TranslationBoxText.Replace('|', ' ');
			SelectedLine.TranslationString = TabUI.TranslationBoxText.Replace(Environment.NewLine, "\n");
			UpdateCharacterCountLabel(SelectedLine.TemplateLength, SelectedLine.TranslationLength);
			ChangesPending = !selectedNew || ChangesPending;
			selectedNew = false;
		}

		public void UpdateComments()
		{
			//remove pipe to not break saving/export
			SelectedLine.Comments = TabUI.CommentBoxTextArr;
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
		/// Reads all files in all subfolders below the given path.
		/// </summary>
		/// <param name="folderPath">The path to the folder to find all files in (iterative).</param>
		private void IterativeReadFiles(string folderPath)
		{
			var templateDir = new DirectoryInfo(folderPath);
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
				LogManager.Log(e.ToString(), LogManager.Level.Warning);
				Utils<TLineItem, TUIHandler, TTabController, TTab>.DisplayExceptionMessage(e.ToString());
			}

			//remove old lines from server
			_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.RemoveOldTemplates(StoryName);

			//add all the new strings
			_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.UploadTemplates(TranslationData);

			PopupResult result = UI.WarningYesNoCancel(
				"This should be done uploading. It should be :)\n" +
				"If you are done uploading, please select YES. ELSE NO. Be wise please!",
				"Updating string DataBase..."
				);

			//update if successfull
			if (result == PopupResult.YES)
			{
				if (DataBase<TLineItem, TUIHandler, TTabController, TTab>.UpdateDBVersion())
				{
					IsUpToDate = true;
					isTemplate = false;
				}
			}
			else if (result == PopupResult.CANCEL)
			{
				UI.SignalAppExit();
			}
		}

		/// <summary>
		/// Prepares the values for reading of the strings, and calls the methods necessary after successfully loading a file.
		/// </summary>
		private void LoadTranslationFile(bool localTakesPriority = false)
		{
			CategoriesInFile.Clear();
			TranslationData.Clear();
			TabUI.Lines.Clear();
			if (SourceFilePath != "")
			{
				string[] paths = SourceFilePath.Split('\\');

				//get parent folder name and check if it is the story, else search around a bit
				//get language text representation
				SetUpStoryName(paths);
				//actually load all strings into the program
				ReadInStringsFromFile();

				if (TranslationData.Count > 0)
				{
					string storyNameToDisplay = StoryName.TrimWithDelim();
					string fileNameToDisplay = FileName.TrimWithDelim();
					TabUI.SetFileInfoText($"File: {storyNameToDisplay}/{fileNameToDisplay}.txt");

					//is up to date, so we can start translation
					LoadTranslations(localTakesPriority);
					UpdateApprovedCountLabel(TabUI.Lines.ApprovedCount, TabUI.Lines.Count);
				}
				//update tab name
				TabManager<TLineItem, TUIHandler, TTabController, TTab>.UpdateTabTitle(FileName);
			}
		}

		private void SetUpStoryName(string[] paths)
		{
			if (paths.Length < 3) throw new ArgumentException("file needs to be at least 2 folders deep from your drive?");

			string tempStoryName = paths[^2];
			bool gotLanguage = LanguageHelper.Languages.TryGetValue(Language, out string? languageAsText);
			if (!gotLanguage) throw new LanguageHelper.LanguageException();
			//compare
			if ((tempStoryName == languageAsText || tempStoryName == (languageAsText + " new")) && gotLanguage)
				//get folder one more up
				tempStoryName = paths[^3];

			if (tempStoryName == "Languages")
			{
				//get folder one more up
				tempStoryName = "UI";
			}

			StoryName = tempStoryName;

		}

		/// <summary>
		/// Loads the strings and does some work around to ensure smooth sailing.
		/// </summary>
		private void LoadTranslations(bool localTakesPriority = false)
		{
			UI.SignalUserWait();
			//todo stop updating the linelist while lines are added, implement interface for that

			int currentIndex = 0;
			FileData onlineLines;

			if (DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline)
				//get template lines from online
				_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.GetAllLineData(FileName, StoryName, out onlineLines, Language);
			else
				//get template lines from user if they want
				onlineLines = GetTemplatesFromUser();

			TabUI.Lines.FreezeLayout();

			foreach (string key in TranslationData.Keys)
			{
				if (onlineLines.TryGetValue(key, out LineData? tempLine))
				{
					TranslationData[key].Category = tempLine.Category;
					if (DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline) TranslationData[key].Comments = tempLine.Comments;
					TranslationData[key].FileName = tempLine.FileName;
					TranslationData[key].ID = key;
					TranslationData[key].IsTemplate = false;
					TranslationData[key].IsTranslated = tempLine.IsTranslated;
					TranslationData[key].Story = tempLine.Story;
					if (!localTakesPriority && DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline) TranslationData[key].TranslationString = tempLine.TranslationString;
					else if (!DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline) TranslationData[key].TemplateString = tempLine.TemplateString;
					TranslationData[key].IsApproved = tempLine.IsApproved;
				}

				if (TranslationData[key].TemplateString == null) TranslationData[key].TemplateString = "";

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
				return GetTemplateFromFile(Utils<TLineItem, TUIHandler, TTabController, TTab>.SelectFileFromSystem(false, $"Choose the template for {StoryName}\\{FileName}.", FileName + ".txt"), false);
			}
			return new FileData();
		}

		/// <summary>
		/// Reads the strings depending on whether its a template or not.
		/// </summary>
		private void ReadInStringsFromFile()
		{
			//read in all strings with IDs
			if (isTemplate && DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline)//read in templates
			{
				TranslationData = GetTemplateFromFile(SourceFilePath);
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
		private FileData GetTemplateFromFile(string path, bool doIterNumbers = true)
		{
			if (Path.GetFileNameWithoutExtension(path) != FileName)
			{
				_ = UI.WarningOk("The template file must have the same name as the file you want to translate!");
				return new FileData();
			}

			var fileData = new FileData();
			StringCategory currentCategory = StringCategory.General;
			string multiLineCollector = "";
			string[] lastLine = Array.Empty<string>();
			//string[] lastLastLine = { };
			//read in lines
			var LinesFromFile = File.ReadAllLines(path).ToList();
			//remove last if empty, breaks line lioading for the last
			while (LinesFromFile.Last() == "") _ = LinesFromFile.Remove(LinesFromFile.Last());
			//load lines and their data and split accordingly
			foreach (string line in LinesFromFile)
			{
				if (line.Contains('|'))
				{
					//if we reach a new id, we can add the old string to the translation manager
					if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : "" + lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true);

					//get current line
					lastLine = line.Split('|');

					//reset multiline collector
					multiLineCollector = "";
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
						if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : "" + lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true);
						lastLine = Array.Empty<string>();
						multiLineCollector = "";
						currentCategory = tempCategory;
					}
				}
			}
			//add last line (dont care about duplicates because sql will get rid of them)
			if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : "" + lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1], true);

			return fileData;
		}

		/// <summary>
		/// loads all the strings from the selected file into a list of LineData elements.
		/// </summary>
		private void ReadStringsTranslationsFromFile()
		{
			StringCategory currentCategory = StringCategory.General;

			_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.GetAllLineDataTemplate(FileName, StoryName, out FileData IdsToExport);
			List<string> LinesFromFile;
			try
			{
				//read in lines
				LinesFromFile = File.ReadAllLines(SourceFilePath).ToList();
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
				SplitReadTranslations(LinesFromFile, currentCategory, IdsToExport);
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
				else if (DataBase<TLineItem, TUIHandler, TTabController, TTab>.IsOnline)
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
			string multiLineCollector = "";
			//remove last if empty, breaks line loading for the last
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
					if (lastLine.Length != 0) CreateLineInTranslations(lastLine, category, IdsToExport, multiLineCollector);
					//get current line
					lastLine = line.Split('|');
					//reset multiline collector
					multiLineCollector = "";
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
							{//write last string with id plus all lines after that minus the last new line char
								CreateLineInTranslations(lastLine, category, IdsToExport, multiLineCollector.Remove(multiLineCollector.Length - 2, 1));
							}
							else
							{//write last line with id if no real line of text is afterwards
								CreateLineInTranslations(lastLine, category, IdsToExport, lastLine[1]);
							}
						}
						//resetting for next iteration
						lastLine = Array.Empty<string>();
						multiLineCollector = "";
						category = tempCategory;
						CategoriesInFile.Add(category);
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
			if (IdsToExport.TryGetValue(lastLine[0], out LineData? templateLine))
				TranslationData[lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, category, templateLine.TemplateString, lastLine[1] + translation);
			else
				TranslationData[lastLine[0]] = new LineData(lastLine[0], StoryName, FileName, category, "", lastLine[1] + translation);
		}

		private void TryFixEmptyFile()
		{
			if (!triedFixingOnce)
			{
				triedFixingOnce = true;
				_ = DataBase<TLineItem, TUIHandler, TTabController, TTab>.GetAllLineDataTemplate(FileName, StoryName, out FileData IdsToExport);
				foreach (LineData item in IdsToExport.Values)
				{
					TranslationData[item.ID] = new LineData(item.ID, StoryName, FileName, item.Category);
				}
				SaveFile();
				TranslationData.Clear();
				ReadStringsTranslationsFromFile();
			}
		}

		/// <summary>
		/// Reloads the file into the program as if it were selected.
		/// </summary>
		public void ReloadFile()
		{
			TabManager<TLineItem, TUIHandler, TTabController, TTab>.ShowAutoSaveDialog();
			LoadTranslationFile();
			if (UI == null) return;
			//select recent index
			if (Settings.Default.RecentIndex > 0 && Settings.Default.RecentIndex < TranslationData.Count) TabUI.SelectLineItem(Settings.Default.RecentIndex);
		}

		/// <summary>
		/// Resets the translation manager.
		/// </summary>
		private void ResetTranslationManager()
		{
			Settings.Default.RecentIndex = TabUI.SelectedLineIndex;
			TranslationData.Clear();
			TabUI.Lines.Clear();
			CategoriesInFile.Clear();
			TabUI.SimilarStringsToEnglish.Clear();
			SelectedResultIndex = 0;
			TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTab.Text = "Tab";
			UpdateApprovedCountLabel(1, 1);
		}

		/// <summary>
		/// Selects the next search result if applicable
		/// </summary>
		/// <returns>True if a new result could be selected</returns>
		public bool SelectNextResultIfApplicable()
		{
			if (!TabUI.IsTranslationBoxFocused && !TabUI.IsCommentBoxFocused && TabUI.Lines.SearchResults.Any())
			{
				//loop back to start
				if (SelectedResultIndex > TabUI.Lines.SearchResults.Count - 1)
				{
					SelectedResultIndex = 0;
					//loop over to new tab when in global search
					if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.InGlobalSearch)
					{
						searchTabIndex = TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTabIndex;
						TabManager<TLineItem, TUIHandler, TTabController, TTab>.SwitchToTab(++searchTabIndex);
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
					else if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.InGlobalSearch && TabManager<TLineItem, TUIHandler, TTabController, TTab>.TabCount > 1)
					{
						searchTabIndex = TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTabIndex;
						TabManager<TLineItem, TUIHandler, TTabController, TTab>.SwitchToTab(++searchTabIndex);
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

		/// <summary>
		/// Does some logic to figure out wether to show or hide the replacing ui
		/// </summary>
		public void ToggleReplaceUI()
		{
			if (!UI.ReplaceBarIsVisible)
			{
				if (TabUI.SelectedTranslationBoxText.Length > 0)
				{
					UI.SearchBarText = TabUI.SelectedTranslationBoxText;
				}
				UI.SetReplaceMenuVisible();

				//set focus to most needed text box, search first
				if (UI.SearchBarText.Length > 0) UI.FocusReplaceBar();
				else UI.FocusSearchBar();
			}
			else
			{
				UI.SetReplaceMenuInVisible();
				TabUI.FocusTranslationBox();
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
			TabUI.SetApprovedLabelText($"Approved: {Approved} / {Total} {(int)(percentage * 100)}%");
			int ProgressValue = (int)(Approved / (float)Total * 100);
			if (ProgressValue != TabUI.ProgressValue)
			{
				if (ProgressValue > 0 && ProgressValue <= 100)
				{
					TabUI.ProgressValue = ProgressValue;
				}
				else
				{
					TabUI.ProgressValue = 0;
				}
				UI.UpdateTranslationProgressIndicator();
			}
		}

		/// <summary>
		/// Updates the label displaying the character count
		/// </summary>
		/// <param name="TemplateCount">The number of chars in the template string.</param>
		/// <param name="TranslationCount">The number of chars in the translated string.</param>
		public void UpdateCharacterCountLabel(int TranslationCount, int TemplateCount)
		{
			if (TranslationCount <= TemplateCount)
			{
				TabUI.SetCharacterLabelColor(Color.LawnGreen);
			}//if bigger by no more than 20 percent
			else if (TranslationCount <= TemplateCount * 1.2f)
			{
				TabUI.SetCharacterLabelColor(Color.DarkOrange);
			}
			else
			{
				TabUI.SetCharacterLabelColor(Color.Red);
			}
			TabUI.SetCharacterCountLabelText($"Template: {TemplateCount} | Translation: {TranslationCount}");
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
	}
}
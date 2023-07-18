using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{
    public static class TabManager
    {
        public static bool InGlobalSearch { get; private set; } = false;
        private static ITabController TabControl => UI?.TabControl!;
        private static int lastIndex = 0;
        private static readonly Dictionary<ITab, TranslationManager> translationManagers = new();

        private static ITab? firstTab;

        /// <summary>
        /// Method returning a Propertyhelper containing all relevant UI elements.
        /// </summary>
        /// <returns>the relevant Propertyhelper</returns>
#nullable disable
        public static IUIHandler UI { get; set; }
#nullable restore

        public static ITab SelectedTab
        {
            get { return TabControl.SelectedTab; }
        }

        /// <summary>
        /// Returns the active translation manager for the currently selected tab
        /// </summary>
        /// <returns>Current translationmanager</returns>
        public static TranslationManager ActiveTranslationManager
        {
            get
            {
                if (translationManagers.TryGetValue(TabControl.SelectedTab, out TranslationManager? translationManager))
                {
                    return translationManager;
                }
                else
                {
                    throw new UnreachableException("There should never be no tab/no translationmanager.");
                }
            }
        }

        public static int SelectedTabIndex
        {
            get { return TabControl.SelectedIndex; }
            set { TabControl.SelectedIndex = value; }
        }
        public static int TabCount
        {
            get { return TabControl.TabCount; }
        }

        public static void CloseTab(int index)
        {
            CloseTab(TabControl.TabPages[index]);
        }

        public static void CloseTab(ITab tab)
        {
            //remove manager for the tab, save first
            if (TabControl.TabPages.Contains(tab) && TabCount > 1)
            {
                translationManagers[tab].SaveFile();
                _ = translationManagers.Remove(tab);
                _ = TabControl.CloseTab(tab);
            }
        }

        /// <summary>
        /// Selects the index given in the list of strings
        /// </summary>
        /// <param name="index">The index to select</param>
        public static void SelectLine(int index)
        {
            if (index >= 0 && index < SelectedTab.LineCount) SelectedTab.SelectLineItem(index);
        }

        /// <summary>
        /// Has to be called on start to set the first tab
        /// </summary>
        /// <param name="tab">The initial tab</param>
        public static void Initialize(IUIHandler ui, Type MenuItem, string appVersion, ITab tab, ISettings settings)
        {
            UI = ui;
            _ = Settings.Initialize(settings);
            DataBase.Initialize(ui, appVersion);
            Utils.Initialize(ui);
            RecentsManager.Initialize(MenuItem);
            firstTab = tab;
        }

        /// <summary>
        /// This has to be called after the form constructor is done, and the form is registered to windows, else the tab adding fails silently
        /// </summary>
        /// <param name="tab">the first tab to be added to the app</param>
        public static void FinalizeInitializer()
        {
            if (UI == null || firstTab == null) throw new InvalidOperationException("You cant finalize the initialization if it hasnt completed");

            if (!UI.TabControl.TabPages.Contains(firstTab))
            {
                UI.TabControl.AddTab(firstTab);
            }
            else
            {
                UI.TabControl.TabPages.Clear();
                UI.TabControl.AddTab(firstTab);
            }

            //create new translationmanager to use with the tab open right now
            translationManagers.Add(firstTab, new TranslationManager(UI, firstTab));
        }

        /// <summary>
        /// Does all the logic to open a file in a new tab
        /// </summary>
        public static void OpenNewTab()
        {
            foreach (var file in Utils.SelectFilesFromSystem())
            {
                OpenInNewTab(file);
            }
        }

        /// <summary>
        /// Opens the given file in a new tab.
        /// </summary>
        /// <param name="path">path to the file to open</param>
        public static void OpenInNewTab(string path)
        {
            if (path.Length > 0)
            {
                //create new support objects
                ITab? newTab = UI.CreateNewTab();
                if (newTab == null) return;
                newTab.Text = $"Tab {translationManagers.Count + 1}";
                //Add tab to form control
                TabControl.AddTab(newTab);
                //create support dict
                var t = new TranslationManager(UI, newTab);
                translationManagers.Add(newTab, t);

                //select new tab
                TabControl.SelectedTab = newTab;

                //call startup for new translationmanager
                t.LoadFileIntoProgram(path);
            }
        }

        /// <summary>
        /// Does all the logic to open all files in the the given folder/path
        /// </summary>
        public static void OpenAllTabs(string basePath)
        {
            if (basePath == string.Empty) return;

            UI.SignalUserWait();

            foreach (string filePath in Directory.GetFiles(basePath))
            {
                if (Path.GetExtension(filePath) == ".txt")
                {
                    OpenInNewTab(filePath);
                }
            }

            UI.SignalUserEndWait();
        }

        /// <summary>
        /// Does all the logic to open all files form a story in tabs
        /// </summary>
        public static void OpenAllTabs()
        {
            OpenAllTabs(Utils.SelectFolderFromSystem("Select the folder named like the Story you want to translate. It has to contain the Translation files, optionally under a folder named after the language"));
        }

        /// <summary>
        /// Updates the corresponding tabs title
        /// </summary>
        /// <param name="manager">The corresponding tab will be updated</param>
        /// <param name="title">The title to set</param>
        public static void UpdateTabTitle(TranslationManager manager, string title)
        {
            foreach (ITab tab in translationManagers.Keys)
            {
                if (translationManagers[tab] == manager)
                {
                    UpdateTabTitle(tab, title);
                    return;
                }
            }
        }

        /// <summary>
        /// Updates the current tabs title
        /// </summary>
        /// <param name="title">The title to set</param>
        public static void UpdateSelectedTabTitle(string title)
        {
            UpdateTabTitle(TabControl.SelectedTab, title);
        }

        /// <summary>
        /// Updates the text of the given TapPage object to the given string.
        /// </summary>
        /// <param name="tab">The tab to set the text of</param>
        /// <param name="title">The string to set the tab text to</param>
        public static void UpdateTabTitle(ITab tab, string title)
        {
            if (title.Length > 0) tab.Text = title;
        }

        /// <summary>
        /// Called when tabs are switched, swaps the search terms
        /// </summary>
        public static void OnSwitchTabs()
        {
            //update history on tab change
            if (lastIndex != TabControl.SelectedIndex)
            {
                History.AddAction(new SelectedTabChanged(lastIndex, TabControl.SelectedIndex) { StoryName = ActiveTranslationManager.StoryName, FileName = ActiveTranslationManager.FileName });
                lastIndex = TabControl.SelectedIndex;
            }

            //set search term to the one from the respective TranslationManager
            if (ActiveTranslationManager != null && UI != null)
            {
                if (InGlobalSearch)
                {
                    ActiveTranslationManager.Search(UI.SearchBarText[1..] ?? string.Empty);
                }
                else
                {
                    UI.SearchBarText = ActiveTranslationManager.SearchQuery;
                }
            }
        }

        /// <summary>
        /// Saves all files in open tabs
        /// </summary>
        /// <returns>True if there are more than one tab and they have been saved</returns>
        public static bool SaveAllTabs()
        {
            if (TabControl.TabCount >= 1)
            {
                UI.SignalUserWait();
                int oldSelection = TabControl.SelectedIndex;
                //save all tabs
                foreach (ITab tab in TabControl.TabPages)
                {
                    if (translationManagers[tab].ChangesPending)
                        TabControl.SelectedTab = tab;
                    translationManagers[tab].SaveFile();
                }
                TabControl.SelectedIndex = oldSelection;
                UI.SignalUserEndWait();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Switches to the Tab specified, does all the other logic behind
        /// </summary>
        /// <param name="i">The tab index to switch to</param>
        public static void SwitchToTab(int i)
        {
            if (i >= 0 && i < TabControl.TabCount)
            {
                TabControl.SelectedIndex = i;
            }
            else
            {
                TabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Call to determine if all tabs should be searched or only the selected one
        /// </summary>
        /// <returns>True if we want to search all, performs the search also. False when single tab search is intended.</returns>
        private static bool SearchAll()
        {
            if (UI == null) return false;

            if (UI.SearchBarText.Length > 0)
            {
                //global search has to start with the ?
                if (UI.SearchBarText[0] == '?')
                {
                    InGlobalSearch = true;
                    return true;
                }
                else
                {
                    InGlobalSearch = false;
                    return false;
                }
            }
            InGlobalSearch = false;
            return false;
        }

        /// <summary>
        /// Call to search tab(s)
        /// </summary>
        public static void Search()
        {
            if (!SearchAll())
            {
                ActiveTranslationManager.Search();
            }
            else
            {
                ActiveTranslationManager.Search(UI.SearchBarText[1..] ?? string.Empty);
            }
        }

        /// <summary>
        /// Copies the Id of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyId()
        {
            UI.ClipboardSetText(ActiveTranslationManager.SelectedId ?? string.Empty);
        }

        /// <summary>
        /// Copies the filename of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyFileName()
        {
            UI.ClipboardSetText(ActiveTranslationManager.FileName ?? string.Empty);
        }

        /// <summary>
        /// Copies the name of the story containing the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyStoryName()
        {
            UI.ClipboardSetText(ActiveTranslationManager.StoryName ?? string.Empty);
        }

        /// <summary>
        /// Copies the all info on the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAll()
        {
            UI.ClipboardSetText(
                ActiveTranslationManager.StoryName +
                "/" + ActiveTranslationManager.FileName +
                " : " +
                ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? string.Empty].ToString());
        }

        /// <summary>
        /// Copies the data as it will be shown in the output files of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAsOutput()
        {
            UI.ClipboardSetText(ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? string.Empty].ToString() ?? string.Empty);
        }

        /// <summary>
        /// Copies the translation of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTranslation()
        {
            UI.ClipboardSetText(ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? string.Empty].TranslationString ?? string.Empty);
        }

        /// <summary>
        /// Copies the template of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTemplate()
        {
            UI.ClipboardSetText(ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? string.Empty].TemplateString ?? string.Empty);
        }

        public static void ReplaceAll()
        {
            if (InGlobalSearch)
            {
                for (int i = 0; i < TabControl.TabCount; i++)
                {
                    //save history
                    if (i != 0) History.AddAction(new SelectedTabChanged(i - 1, i) { StoryName = ActiveTranslationManager.StoryName, FileName = ActiveTranslationManager.FileName });
                    else History.AddAction(new SelectedTabChanged(0, i) { StoryName = ActiveTranslationManager.StoryName, FileName = ActiveTranslationManager.FileName });

                    translationManagers[TabControl.TabPages[i]].ReplaceAll(UI.ReplaceBarText ?? string.Empty);
                }
            }
            else
            {
                ActiveTranslationManager.ReplaceAll(UI.ReplaceBarText ?? string.Empty);
            }
        }

        public static void Replace()
        {
            ActiveTranslationManager.ReplaceSingle(UI.ReplaceBarText ?? string.Empty);
        }

        public static void OpenFile() => OpenFile(Utils.SelectFileFromSystem());

        public static void OpenFile(string path)
        {
            //load new file
            ActiveTranslationManager.LoadFileIntoProgram(path);
        }

        public static void OpenNewFiles()
        {
            var paths = Utils.SelectFilesFromSystem();
            if (paths.Length == 1)
            {
                OpenFile(paths[0]);
            }
            else
            {
                int i = 0;
                foreach (var path in paths)
                {
                    if (i++ == 0)
                        OpenFile(path);
                    else
                        OpenInNewTab(path);
                }
            }
        }

        /// <summary>
        /// Shows a save all changes dialogue (intended to be used before quit) if settings allow.
        /// </summary>
        public static void ShowAutoSaveDialog()
        {
            if (Settings.Default.AskForSaveDialog && translationManagers.Count > 0)
            {
                foreach (var kvp in translationManagers)
                {
                    if (kvp.Value.ChangesPending)
                    {
                        if (UI.WarningYesNo("You may have unsaved changes. Do you want to save all changes?", "Save changes?", PopupResult.YES))
                            _ = SaveAllTabs();
                        return;
                    }
                }
            }
        }
    }
}

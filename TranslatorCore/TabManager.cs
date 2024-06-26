﻿using System;
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
                return translationManagers.TryGetValue(TabControl.SelectedTab, out TranslationManager? translationManager)
                    ? translationManager
                    : throw new UnreachableException("There should never be no tab/no translationmanager.");
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
            if (UI is null || firstTab is null) throw new InvalidOperationException("You cant finalize the initialization if it hasnt completed");

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
            foreach (string file in Utils.SelectFilesFromSystem())
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
            if (path.Length == 0) return;

            //create new support objects
            ITab? newTab = UI.CreateNewTab();
            if (newTab is null) return;
            newTab.Text = $"Tab {translationManagers.Count + 1}";
            //Add tab to form control
            TabControl.AddTab(newTab);
            //create support dict
            var t = new TranslationManager(UI, newTab);
            translationManagers.Add(newTab, t);

            //update search counts and results
            IsSearchAllFiles();
            //select new tab
            TabControl.SelectedTab = newTab;

            //call startup for new translationmanager
            t.LoadFileIntoProgram(path);

            if (InGlobalSearch)
                t.Search(UI.SearchBarText[1..] ?? string.Empty);
            else
                t.Search();

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
                History.AddAction(new SelectedTabChanged(lastIndex, TabControl.SelectedIndex, ActiveTranslationManager.StoryName, ActiveTranslationManager.FileName));
                lastIndex = TabControl.SelectedIndex;
            }

            //set search term to the one from the respective TranslationManager
            if (ActiveTranslationManager is null || UI is null) return;

            if (!InGlobalSearch)
            {
                UI.SearchBarText = ActiveTranslationManager.SearchQuery;
            }
        }

        /// <summary>
        /// Saves all files in open tabs
        /// </summary>
        /// <returns>True if there are more than one tab and they have been saved</returns>
        public static bool SaveAllTabs()
        {
            if (TabControl.TabCount < 1) return false;

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

        /// <summary>
        /// Switches to the Tab specified, does all the other logic behind
        /// </summary>
        /// <param name="i">The tab index to switch to</param>
        public static void SwitchToTab(int i)
        {
            TabControl.SelectedIndex = i >= 0 ? i < TabControl.TabCount ? i : 0 : TabControl.TabCount - 1;
        }

        /// <summary>
        /// Call to determine if all tabs should be searched or only the selected one
        /// </summary>
        /// <returns>True if we want to search all, performs the search also. False when single tab search is intended.</returns>
        private static bool IsSearchAllFiles()
        {
            if (UI is null) return false;
            if (UI.SearchBarText.Length > 0)
            {
                //global search has to start with the ?
                InGlobalSearch = UI.SearchBarText[0] == '?' && UI.SearchBarText.Length > 1;
            }
            else
            {
                InGlobalSearch = false;
            }
            return InGlobalSearch;
        }

        /// <summary>
        /// Call to search tab(s)
        /// </summary>
        public static void Search()
        {
            if (IsSearchAllFiles())
            {
                foreach (var translationManager in translationManagers)
                {
                    translationManager.Value.Search(UI.SearchBarText[1..] ?? string.Empty);
                }
                //turn global search off so we dont switch tabs to none existen once and break counting
                if (TabCount == 1)
                    InGlobalSearch = false;
            }
            else
            {
                ActiveTranslationManager.Search();
            }
            UpdateSearchResultCount();
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
                ActiveTranslationManager.SelectedLine.ToString());
        }

        /// <summary>
        /// Copies the data as it will be shown in the output files of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAsOutput()
        {
            UI.ClipboardSetText(ActiveTranslationManager.SelectedLine.ToString());
        }

        /// <summary>
        /// Copies the translation of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTranslation()
        {
            UI.ClipboardSetText(ActiveTranslationManager.SelectedLine.Translation ?? string.Empty);
        }

        /// <summary>
        /// Copies the template of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTemplate()
        {
            UI.ClipboardSetText(ActiveTranslationManager.SelectedLine.Template ?? string.Empty);
        }

        public static void ReplaceAll()
        {
            if (InGlobalSearch)
            {
                for (int i = 0; i < TabControl.TabCount; i++)
                {
                    //save history
                    if (i != 0) History.AddAction(new SelectedTabChanged(i - 1, i, ActiveTranslationManager.StoryName, ActiveTranslationManager.FileName));
                    else History.AddAction(new SelectedTabChanged(0, i, ActiveTranslationManager.StoryName, ActiveTranslationManager.FileName));

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
            string[] paths = Utils.SelectFilesFromSystem();
            if (paths.Length == 1)
            {
                OpenFile(paths[0]);
            }
            else
            {
                int i = 0;
                foreach (string path in paths)
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
            if (!Settings.Default.AskForSaveDialog || translationManagers.Count <= 0) return;

            foreach (KeyValuePair<ITab, TranslationManager> kvp in translationManagers)
            {
                if (kvp.Value.ChangesPending)
                {
                    if (UI.WarningYesNo("You may have unsaved changes. Do you want to save all changes?", "Save changes?", PopupResult.YES))
                        _ = SaveAllTabs();
                    return;
                }
            }
        }

        public static void UpdateSearchResultCount()
        {
            int t = 0;
            foreach (var tab in translationManagers.Keys)
            {
                t += tab.Lines.SearchResults.Count;
            }
            UI.SearchResultCount = t;
        }

        public static void UpdateSelectedSearchResult()
        {
            int t = 0;
            foreach (var tab in translationManagers.Keys)
            {
                if (tab == SelectedTab)
                {
                    t += translationManagers[tab].SelectedResultIndex + 1;
                    break;
                }
                else
                {
                    t += tab.Lines.SearchResults.Count;
                }
            }
            UI.SelectedSearchResult = t;
        }
    }
}

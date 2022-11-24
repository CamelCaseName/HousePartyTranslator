using System;
using System.Collections.Generic;
using System.IO;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;
using Translator.UICompatibilityLayer.StubImpls;

namespace Translator.Core
{
    public static class TabManager<T> where T : class, ILineItem, new()
    {
        public static bool InGlobalSearch { get; private set; } = false;
        private static ITabController<T> TabControl = (ITabController<T>)new NullTabController();
        private static int lastIndex = 0;
        private static readonly Dictionary<ITab<T>, IUIHandler<T>> handlers = new();
        private static readonly Dictionary<ITab<T>, TranslationManager<T>> translationManagers = new();

        /// <summary>
        /// Method returning a Propertyhelper containing all relevant UI elements.
        /// </summary>
        /// <returns>the relevant Propertyhelper</returns>
        public static IUIHandler<T> ActiveUI
        {
            get
            {
                _ = handlers.TryGetValue(TabControl.SelectedTab, out IUIHandler<T>? property);
                return property
                    ?? (IUIHandler<T>)NullUIHandler.Instance;
            }
        }

        public static ITab<T> SelectedTab
        {
            get { return TabControl.SelectedTab; }
        }

        /// <summary>
        /// Returns the active translation manager for the currently selected tab
        /// </summary>
        /// <returns>Current translationmanager</returns>
        public static TranslationManager<T>? ActiveTranslationManager
        {
            get
            {
                if (translationManagers.TryGetValue(TabControl.SelectedTab, out TranslationManager<T>? translationManager))
                {
                    return translationManager;
                }
                else
                {
                    return null;
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

        public static void CloseTab(ITab<T> tab)
        {
            //remove manager for the tab, save first
            translationManagers[tab].SaveFile();
            _ = translationManagers.Remove(tab);
            _ = handlers.Remove(tab);
            TabControl.CloseTab(tab);
        }

        /// <summary>
        /// Has to be called on start to set the first tab
        /// </summary>
        /// <param name="tab1">The initial tab</param>
        public static TranslationManager<T> Initialize(ITab<T> tab1, IUIHandler<T> ui, Type MenuItem, Type MenuItemSeperator, string password, string appVersion)
        {
            Utils<T>.Initialize(ui);
            DataBase.Initialize(ui, password, appVersion);
            RecentsManager.Initialize(MenuItem, MenuItemSeperator);
            TabControl = ui.TabControl;

            //create new translationmanager to use with the tab open right now
            handlers.Add(tab1, ui);
            translationManagers.Add(tab1, new TranslationManager<T>(ActiveUI, tab1));

            return translationManagers[tab1];
        }

        /// <summary>
        /// Does all the logic to open a file in a new tab
        /// </summary>
        public static void OpenNewTab(IUIHandler<T> ui)
        {
            OpenInNewTab(Utils<T>.SelectFileFromSystem(), ui);
        }

        /// <summary>
        /// Opens the given file in a new tab.
        /// </summary>
        /// <param name="path">path to the file to open</param>
        public static void OpenInNewTab(string path, IUIHandler<T> ui)
        {
            if (path.Length > 0)
            {
                //create new support objects
                ITab<T> newTab = ui.CreateNewTab() ?? (ITab<T>)new NullTab();
                newTab.Text = $"Tab {translationManagers.Count + 1}";
                //Add tab to form control
                TabControl.TabPages.Add(newTab);
                //select new tab
                TabControl.SelectedTab = newTab;
                //create support dict
                handlers.Add(newTab, ui);
                var t = new TranslationManager<T>(ActiveUI, newTab);
                translationManagers.Add(newTab, t);

                //call startup for new translationmanager
                TranslationManager<T>.SetLanguage();
                t.LoadFileIntoProgram(path);
            }
        }

        /// <summary>
        /// Does all the logic to open all files form a story in tabs
        /// </summary>
        public static void OpenAllTabs(IUIHandler<T> ui)
        {
            string basePath = Utils<T>.SelectFolderFromSystem("Select the folder named like the Story you want to translate. It has to contain the Translation files, optionally under a folder named after the language");

            if (basePath.Length > 0)
            {
                foreach (string path in Directory.GetDirectories(basePath))
                {
                    string[] folders = path.Split('\\');

                    //get parent folder name
                    string tempName = folders[^1];
                    //get language text representation
                    bool gotLanguage = LanguageHelper.Languages.TryGetValue(TranslationManager<T>.Language, out string? languageAsText);
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
                        OpenInNewTab(filePath, ui);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the current tabs title
        /// </summary>
        /// <param name="manager">The corresponding tab will be updated</param>
        /// <param name="title">The title to set</param>
        public static void UpdateTabTitle(TranslationManager<T> manager, string title)
        {
            foreach (ITab<T> tab in translationManagers.Keys)
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
        public static void UpdateTabTitle(string title)
        {
            UpdateTabTitle(TabControl.SelectedTab, title);
        }

        /// <summary>
        /// Updates the text of the given TapPage object to the given string.
        /// </summary>
        /// <param name="tab">The tab to set the text of</param>
        /// <param name="title">The string to set the tab text to</param>
        public static void UpdateTabTitle(ITab<T> tab, string title)
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
                History.AddAction(new SelectedTabChanged(lastIndex, TabControl.SelectedIndex));
                lastIndex = TabControl.SelectedIndex;
            }

            //set search term to the one from the respective TranslationManager
            if (ActiveTranslationManager != null && ActiveUI != null)
            {
                if (InGlobalSearch)
                {
                    ActiveTranslationManager.Search(ActiveUI.GetSearchBarText()[1..] ?? "");
                }
                else
                {
                    ActiveUI.SetSearchBarText(ActiveTranslationManager.SearchQuery);
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
                int oldSelection = TabControl.SelectedIndex;
                //save all tabs
                foreach (ITab<T> tab in TabControl.TabPages)
                {
                    if (translationManagers[tab].ChangesPending)
                        TabControl.SelectedTab = tab;
                    translationManagers[tab].SaveFile();
                }
                TabControl.SelectedIndex = oldSelection;
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
            if (ActiveUI == null) return false;

            if (ActiveUI.GetSearchBarText().Length > 0)
            {
                //global search has to start with the ?
                if (ActiveUI.GetSearchBarText()[0] == '?')
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
                ActiveTranslationManager?.Search();
            }
            else
            {
                ActiveTranslationManager?.Search(ActiveUI.GetSearchBarText()[1..] ?? "");
            }
        }

        /// <summary>
        /// Copies the Id of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyId()
        {
            ActiveUI.ClipboardSetText(ActiveTranslationManager?.SelectedId ?? "");
        }

        /// <summary>
        /// Copies the filename of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyFileName()
        {
            ActiveUI.ClipboardSetText(ActiveTranslationManager?.FileName ?? "");
        }

        /// <summary>
        /// Copies the name of the story containing the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyStoryName()
        {
            ActiveUI.ClipboardSetText(ActiveTranslationManager?.StoryName ?? "");
        }

        /// <summary>
        /// Copies the all info on the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAll()
        {
            ActiveUI.ClipboardSetText(
                ActiveTranslationManager?.StoryName +
                "/" + ActiveTranslationManager?.FileName +
                " : " +
                ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].ToString());
        }

        /// <summary>
        /// Copies the data as it will be shown in the output files of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAsOutput()
        {
            ActiveUI.ClipboardSetText(ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].ToString() ?? "");
        }

        /// <summary>
        /// Copies the translation of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTranslation()
        {
            ActiveUI.ClipboardSetText(ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].TranslationString ?? "");
        }

        /// <summary>
        /// Copies the template of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTemplate()
        {
            ActiveUI.ClipboardSetText(ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].TemplateString ?? "");
        }

        public static void ReplaceAll()
        {
            if (InGlobalSearch)
            {
                for (int i = 0; i < TabControl.TabCount; i++)
                {
                    //save history
                    if (i != 0) History.AddAction(new SelectedTabChanged(i - 1, i));
                    else History.AddAction(new SelectedTabChanged(0, i));

                    translationManagers[TabControl.TabPages[i]].ReplaceAll(ActiveUI.GetReplaceBarText() ?? "");
                }
            }
            else
            {
                ActiveTranslationManager?.ReplaceAll(ActiveUI.GetReplaceBarText() ?? "");
            }
        }

        public static void Replace()
        {
            ActiveTranslationManager?.ReplaceSingle(ActiveUI.GetReplaceBarText() ?? "");
        }
    }
}

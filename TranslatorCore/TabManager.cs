using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;

namespace Translator.Core
{
    public static class TabManager<TLineItem, TUIHandler, TTabController, TTab>
        where TLineItem : class, ILineItem, new()
        where TUIHandler : class, IUIHandler<TLineItem, TTabController, TTab>, new()
        where TTabController : class, ITabController<TLineItem, TTab>, new()
        where TTab : class, ITab<TLineItem>, new()
    {
        public static bool InGlobalSearch { get; private set; } = false;
        private static TTabController TabControl => UI?.TabControl ?? new();
        private static int lastIndex = 0;
        private static readonly Dictionary<TTab, TranslationManager<TLineItem, TUIHandler, TTabController, TTab>> translationManagers = new();

        private static TTab? firstTab;

        /// <summary>
        /// Method returning a Propertyhelper containing all relevant UI elements.
        /// </summary>
        /// <returns>the relevant Propertyhelper</returns>
#nullable disable
        public static TUIHandler UI { get; set; }
#nullable restore

        public static TTab SelectedTab
        {
            get { return TabControl.SelectedTab; }
        }

        /// <summary>
        /// Returns the active translation manager for the currently selected tab
        /// </summary>
        /// <returns>Current translationmanager</returns>
        public static TranslationManager<TLineItem, TUIHandler, TTabController, TTab> ActiveTranslationManager
        {
            get
            {
                if (translationManagers.TryGetValue(TabControl.SelectedTab, out TranslationManager<TLineItem, TUIHandler, TTabController, TTab>? translationManager))
                {
                    return translationManager;
                }
                else
                {
                    throw new UnreachableException("There should never be no tab/no translationmanager.");
                    //return new((IUIHandler<TLineItem>)NullUIHandler.Instance, (ITab<TLineItem>)NullTab.Instance);
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

        public static void CloseTab(TTab tab)
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
        /// Has to be called on start to set the first tab
        /// </summary>
        /// <param name="tab">The initial tab</param>
        public static void Initialize(TUIHandler ui, Type MenuItem, Type MenuItemSeperator, string appVersion, TTab tab, ISettings settings)
        {
            UI = ui;
            _ = Settings.Initialize(settings);
            Utils<TLineItem, TUIHandler, TTabController, TTab>.Initialize(ui);
            DataBase<TLineItem, TUIHandler, TTabController, TTab>.Initialize(ui, appVersion);
            RecentsManager.Initialize(MenuItem, MenuItemSeperator);
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
            translationManagers.Add(firstTab, new TranslationManager<TLineItem, TUIHandler, TTabController, TTab>(UI, firstTab));
        }

        /// <summary>
        /// Does all the logic to open a file in a new tab
        /// </summary>
        public static void OpenNewTab()
        {
            OpenInNewTab(Utils<TLineItem, TUIHandler, TTabController, TTab>.SelectFileFromSystem());
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
                TTab? newTab = UI.CreateNewTab();
                if (newTab == null) return;
                newTab.Text = $"Tab {translationManagers.Count + 1}";
                //Add tab to form control
                TabControl.AddTab(newTab);
                //create support dict
                var t = new TranslationManager<TLineItem, TUIHandler, TTabController, TTab>(UI, newTab);
                translationManagers.Add(newTab, t);

                //select new tab
                TabControl.SelectedTab = newTab;

                //call startup for new translationmanager
                t.LoadFileIntoProgram(path);
            }
        }

        /// <summary>
        /// Does all the logic to open all files form a story in tabs
        /// </summary>
        public static void OpenAllTabs()
        {
            string basePath = Utils<TLineItem, TUIHandler, TTabController, TTab>.SelectFolderFromSystem("Select the folder named like the Story you want to translate. It has to contain the Translation files, optionally under a folder named after the language");

            if (basePath.Length > 0)
            {
                UI.SignalUserWait();
                foreach (string path in Directory.GetDirectories(basePath))
                {
                    string[] folders = path.Split('\\');

                    //get parent folder name
                    string tempName = folders[^1];
                    //get language text representation
                    bool gotLanguage = LanguageHelper.Languages.TryGetValue(TranslationManager<TLineItem, TUIHandler, TTabController, TTab>.Language, out string? languageAsText);
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
                        OpenInNewTab(filePath);
                    }
                }
                UI.SignalUserEndWait();
            }
        }

        /// <summary>
        /// Updates the current tabs title
        /// </summary>
        /// <param name="manager">The corresponding tab will be updated</param>
        /// <param name="title">The title to set</param>
        public static void UpdateTabTitle(TranslationManager<TLineItem, TUIHandler, TTabController, TTab> manager, string title)
        {
            foreach (TTab tab in translationManagers.Keys)
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
        public static void UpdateTabTitle(ITab<TLineItem> tab, string title)
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
                History.AddAction(new SelectedTabChanged<TLineItem, TUIHandler, TTabController, TTab>(lastIndex, TabControl.SelectedIndex) { StoryName = ActiveTranslationManager.StoryName, FileName = ActiveTranslationManager.FileName });
                lastIndex = TabControl.SelectedIndex;
            }

            //set search term to the one from the respective TranslationManager
            if (ActiveTranslationManager != null && UI != null)
            {
                if (InGlobalSearch)
                {
                    ActiveTranslationManager.Search(UI.SearchBarText[1..] ?? "");
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
                foreach (TTab tab in TabControl.TabPages)
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
                ActiveTranslationManager.Search(UI.SearchBarText[1..] ?? "");
            }
        }

        /// <summary>
        /// Copies the Id of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyId()
        {
            UI.ClipboardSetText(ActiveTranslationManager.SelectedId ?? "");
        }

        /// <summary>
        /// Copies the filename of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyFileName()
        {
            UI.ClipboardSetText(ActiveTranslationManager.FileName ?? "");
        }

        /// <summary>
        /// Copies the name of the story containing the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyStoryName()
        {
            UI.ClipboardSetText(ActiveTranslationManager.StoryName ?? "");
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
                ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? ""].ToString());
        }

        /// <summary>
        /// Copies the data as it will be shown in the output files of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAsOutput()
        {
            UI.ClipboardSetText(ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? ""].ToString() ?? "");
        }

        /// <summary>
        /// Copies the translation of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTranslation()
        {
            UI.ClipboardSetText(ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? ""].TranslationString ?? "");
        }

        /// <summary>
        /// Copies the template of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTemplate()
        {
            UI.ClipboardSetText(ActiveTranslationManager.TranslationData[ActiveTranslationManager.SelectedId ?? ""].TemplateString ?? "");
        }

        public static void ReplaceAll()
        {
            if (InGlobalSearch)
            {
                for (int i = 0; i < TabControl.TabCount; i++)
                {
                    //save history
                    if (i != 0) History.AddAction(new SelectedTabChanged<TLineItem, TUIHandler, TTabController, TTab>(i - 1, i) { StoryName = ActiveTranslationManager.StoryName, FileName = ActiveTranslationManager.FileName });
                    else History.AddAction(new SelectedTabChanged<TLineItem, TUIHandler, TTabController, TTab>(0, i) { StoryName = ActiveTranslationManager.StoryName, FileName = ActiveTranslationManager.FileName });

                    translationManagers[TabControl.TabPages[i]].ReplaceAll(UI.ReplaceBarText ?? "");
                }
            }
            else
            {
                ActiveTranslationManager.ReplaceAll(UI.ReplaceBarText ?? "");
            }
        }

        public static void Replace()
        {
            ActiveTranslationManager.ReplaceSingle(UI.ReplaceBarText ?? "");
        }

        public static void OpenFile()
        {
            //load new file
            ActiveTranslationManager.LoadFileIntoProgram();
            //update tab name
            SelectedTab.Text = ActiveTranslationManager.FileName;
        }

        public static void OpenNewFiles()
        {
            foreach (var path in Utils<TLineItem, TUIHandler, TTabController, TTab>.SelectFilesFromSystem())
            {
                OpenInNewTab(path);
            }
        }

        /// <summary>
        /// Shows a save all changes dialogue (intended to be used before quit) if settings allow.
        /// </summary>
        public static void ShowAutoSaveDialog()
        {
            if (Settings.Default.AskForSaveDialog && translationManagers.Count > 0)
            {
                foreach (KeyValuePair<TTab, TranslationManager<TLineItem, TUIHandler, TTabController, TTab>> translationManager in translationManagers)
                {
                    if (translationManager.Value.ChangesPending)
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

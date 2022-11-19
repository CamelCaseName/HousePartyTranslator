﻿using Translator.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Translator.Core
{
    public static class TabManager
    {
        /// <summary>
        /// The TabControl in the main form, where all tabs are managed under
        /// </summary>
        public static TabControl TabControl = new();
        public static bool InGlobalSearch = false;
        private static int lastIndex = 0;
        public static Fenster? Main;
        private static readonly Dictionary<TabPage, PropertyHelper> properties = new();
        private static readonly Dictionary<TabPage, TranslationManager> translationManagers = new();

        /// <summary>
        /// Method returning a Propertyhelper containing all relevant UI elements.
        /// </summary>
        /// <returns>the relevant Propertyhelper</returns>
        public static PropertyHelper? ActiveProperties
        {
            get
            {
                if (properties.TryGetValue(TabControl.SelectedTab, out PropertyHelper? property))
                {
                    return property;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the active translation manager for the currently selected tab
        /// </summary>
        /// <returns>Current translationmanager</returns>
        public static TranslationManager? ActiveTranslationManager
        {
            get
            {
                if (translationManagers.TryGetValue(TabControl.SelectedTab, out TranslationManager? translationManager))
                {
                    return translationManager;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Can be called to close a tab
        /// </summary>
        /// <param name="sender">Event initiator</param>
        /// <param name="e">Mouse Parameters for this event, we need the lcoation from that</param>
        
        public static void CloseTab(object? sender, MouseEventArgs? e)
        {
            if (e == null) { LogManager.Log("No eventargs on unhandled exception", LogManager.Level.Error); return; }

            if (e.Button == MouseButtons.Right && TabControl.TabCount > 1)
            {
                for (int ix = 0; ix < TabControl.TabCount; ++ix)
                {
                    if (TabControl.GetTabRect(ix).Contains(e.Location) && ActiveTranslationManager != null)
                    {
                        //remove manager for the tab, save first
                        ActiveTranslationManager?.SaveFile();
                        _ = translationManagers.Remove(TabControl.TabPages[ix]);
                        _ = properties.Remove(TabControl.TabPages[ix]);

                        TabControl.TabPages[ix].Dispose();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Has to be called on start to set the first tab
        /// </summary>
        /// <param name="tabPage1">The initial tab</param>
        /// <param name="presenceManager"></param>
        /// <param name="main"></param>
         
        //todo make this platform agnostic, so encapsulate all ui properties
        
        public static TranslationManager Initialize(TabPage tabPage1, DiscordPresenceManager presenceManager, Fenster main)
        {
            Main = main;
            while (TabControl.Parent == null)
            {
                //get tabcontrol as a statc reference;
                if (Main?.Controls != null) TabControl = (TabControl)Main.Controls.Find("MainTabControl", true)[0];
            }

            //create new translationmanager to use with the tab open right now
            properties.Add(tabPage1, CreateActivePropertyHelper());
            translationManagers.Add(tabPage1, new TranslationManager(ActiveProperties, presenceManager));

            return translationManagers[tabPage1];
        }

        /// <summary>
        /// Does all the logic to open a file in a new tab
        /// </summary>
        
        public static void OpenNewTab()
        {
            OpenInNewTab(Utils.SelectFileFromSystem());
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
                TabPage newTab = Utils.CreateNewTab(translationManagers.Count + 1, Main) ?? new TabPage();
                //Add tab to form control
                TabControl.TabPages.Add(newTab);
                //select new tab
                TabControl.SelectedTab = newTab;
                //create support dict
                properties.Add(newTab, CreateActivePropertyHelper());
                var t = new TranslationManager(ActiveProperties);
                translationManagers.Add(newTab, t);

                //call startup for new translationmanager
                t.SetLanguage();
                t.LoadFileIntoProgram(path);
            }
        }

        /// <summary>
        /// Does all the logic to open all files form a story in tabs
        /// </summary>
        
        public static void OpenAllTabs()
        {
            string basePath = Utils.SelectFolderFromSystem("Select the folder named like the Story you want to translate. It has to contain the Translation files, optionally under a folder named after the language");

            if (basePath.Length > 0)
            {
                foreach (string path in Directory.GetDirectories(basePath))
                {
                    string[] folders = path.Split('\\');

                    //get parent folder name
                    string tempName = folders[^1];
                    //get language text representation
                    bool gotLanguage = LanguageHelper.Languages.TryGetValue(TranslationManager.Language, out string? languageAsText);
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
            }
        }

        /// <summary>
        /// Updates the current tabs title
        /// </summary>
        /// <param name="manager">The corresponding tab will be updated</param>
        /// <param name="title">The title to set</param>
        public static void UpdateTabTitle(TranslationManager manager, string title)
        {
            foreach (TabPage tab in translationManagers.Keys)
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
        public static void UpdateTabTitle(TabPage tab, string title)
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
            if (ActiveTranslationManager != null && ActiveProperties != null)
            {
                if (InGlobalSearch)
                {
                    ActiveTranslationManager.Search(ActiveProperties.SearchBox.Text[1..] ?? "");
                }
                else
                {
                    ActiveProperties.SearchBox.Text = ActiveTranslationManager.SearchQuery;
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
                foreach (TabPage tab in TabControl.TabPages)
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
            if (ActiveProperties == null) return false;

            if (ActiveProperties.SearchBox.TextLength > 0)
            {
                //global search has to start with the ?
                if (ActiveProperties.SearchBox.Text[0] == '?')
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
                ActiveTranslationManager?.Search(ActiveProperties?.SearchBox.Text[1..] ?? "");
            }
        }

        /// <summary>
        /// Copies the Id of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyId()
        {
            Clipboard.SetText(ActiveTranslationManager?.SelectedId ?? "");
        }

        /// <summary>
        /// Copies the filename of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyFileName()
        {
            Clipboard.SetText(ActiveTranslationManager?.FileName ?? "");
        }

        /// <summary>
        /// Copies the name of the story containing the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyStoryName()
        {
            Clipboard.SetText(ActiveTranslationManager?.StoryName ?? "");
        }

        /// <summary>
        /// Copies the all info on the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAll()
        {
            Clipboard.SetText(
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
            Clipboard.SetText(ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].ToString() ?? "");
        }

        /// <summary>
        /// Copies the translation of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTranslation()
        {
            Clipboard.SetText(ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].TranslationString ?? "");
        }

        /// <summary>
        /// Copies the template of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTemplate()
        {
            Clipboard.SetText(ActiveTranslationManager?.TranslationData[ActiveTranslationManager?.SelectedId ?? ""].TemplateString ?? "");
        }

        
        private static PropertyHelper CreateActivePropertyHelper()
        {
            while (!Main?.Visible ?? true && Main == null)
            {

            }
            if (Main == null) throw new NullReferenceException();
            Fenster fenster = Main;
            return new PropertyHelper(
                (CheckBox)TabControl.SelectedTab.Controls.Find("ApprovedBox", true)[0],
                (ColouredCheckedListBox)TabControl.SelectedTab.Controls.Find("CheckListBoxLeft", true)[0],
                fenster.LanguageBox,
                (Label)TabControl.SelectedTab.Controls.Find("WordsTranslated", true)[0],
                (Label)TabControl.SelectedTab.Controls.Find("CharacterCountLabel", true)[0],
                (Label)TabControl.SelectedTab.Controls.Find("SelectedFile", true)[0],
                (NoAnimationBar)TabControl.SelectedTab.Controls.Find("ProgressbarTranslated", true)[0],
                (TextBox)TabControl.SelectedTab.Controls.Find("CommentTextBox", true)[0],
                fenster.SearchBox,
                fenster.ReplaceBox,
                fenster.ReplaceAllButton,
                (TextBox)TabControl.SelectedTab.Controls.Find("TemplateTextBox", true)[0],
                (TextBox)TabControl.SelectedTab.Controls.Find("TranslatedTextBox", true)[0],
                fenster.ReplaceButton
                );
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

                    translationManagers[TabControl.TabPages[i]].ReplaceAll(ActiveProperties?.ReplaceBox.Text ?? "");
                }
            }
            else
            {
                ActiveTranslationManager?.ReplaceAll(ActiveProperties?.ReplaceBox.Text ?? "");
            }
        }

        public static void Replace()
        {
            ActiveTranslationManager?.ReplaceSingle(ActiveProperties?.ReplaceBox.Text ?? "");
        }
    }
}
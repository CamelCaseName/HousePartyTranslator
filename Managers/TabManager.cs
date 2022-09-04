using HousePartyTranslator.Helpers;
using System.Collections.Generic;
using System.Windows.Forms;


namespace HousePartyTranslator.Managers
{
    static class TabManager
    {
        /// <summary>
        /// The TabControl in the main form, where all tabs are managed under
        /// </summary>
        public static TabControl TabControl = null;
        public static bool InGlobalSearch = false;
        private static readonly Dictionary<TabPage, PropertyHelper> properties = new Dictionary<TabPage, PropertyHelper>();
        private static readonly Dictionary<TabPage, TranslationManager> translationManagers = new Dictionary<TabPage, TranslationManager>();
        private static DiscordPresenceManager presenceManager;

        /// <summary>
        /// Method returning a Propertyhelper containing all relevant UI elements.
        /// </summary>
        /// <returns>the relevant Propertyhelper</returns>
        public static PropertyHelper ActiveProperties
        {
            get
            {
                if (properties.TryGetValue(TabControl.SelectedTab, out PropertyHelper property))
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
        public static TranslationManager ActiveTranslationManager
        {
            get
            {
                if (translationManagers.TryGetValue(TabControl.SelectedTab, out TranslationManager translationManager))
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
        public static void CloseTab(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && TabControl.TabCount > 1)
            {
                for (int ix = 0; ix < TabControl.TabCount; ++ix)
                {
                    if (TabControl.GetTabRect(ix).Contains(e.Location) && ActiveTranslationManager != null)
                    {
                        //remove manager for the tab, save first
                        ActiveTranslationManager.SaveFile();
                        translationManagers.Remove(TabControl.TabPages[ix]);
                        properties.Remove(TabControl.TabPages[ix]);

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
        public static TranslationManager Initialize(TabPage tabPage1, DiscordPresenceManager presenceManager)
        {
            while (TabControl == null)
            {
                //get tabcontrol as a statc reference;
                if (Form.ActiveForm?.Controls != null) TabControl = (TabControl)Form.ActiveForm.Controls.Find("MainTabControl", true)[0];
            }

            //create new translationmanager to use with the tab open right now
            properties.Add(tabPage1, CreateActivePropertyHelper());
            translationManagers.Add(tabPage1, new TranslationManager(ActiveProperties));

            TabManager.presenceManager = presenceManager;

            return translationManagers[tabPage1];
        }

        /// <summary>
        /// Does all the logic to open a file in a new tab
        /// </summary>
        public static void OpenNewTab()
        {
            OpenInNewTab(TranslationManager.SelectFileFromSystem());
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
                TabPage newTab = Utils.CreateNewTab(translationManagers.Count + 1, (Fenster)Form.ActiveForm);
                //Add tab to form control
                TabControl.TabPages.Add(newTab);
                //select new tab
                TabControl.SelectedTab = newTab;
                //create support dict
                properties.Add(newTab, CreateActivePropertyHelper());
                TranslationManager t = new TranslationManager(ActiveProperties);
                translationManagers.Add(newTab, t);

                //call startup for new translationmanager
                t.SetLanguage();
                t.LoadFileIntoProgram(path, presenceManager);
            }
        }

        /// <summary>
        /// Does all the logic to open all files form a story in tabs
        /// </summary>
        public static void OpenAllTabs()
        {
            TranslationManager.LoadAllFilesIntoProgram();
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
            //set search term to the one from the respective TranslationManager
            if (ActiveTranslationManager != null)
            {
                if (InGlobalSearch)
                {
                    ActiveTranslationManager.Search(ActiveProperties.SearchBox.Text.Substring(1));
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
                //save all tabs
                foreach (TabPage tab in TabControl.TabPages)
                {
                    translationManagers[tab].SaveFile();
                }
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
                ActiveTranslationManager.Search();
            }
            else
            {
                ActiveTranslationManager.Search(ActiveProperties.SearchBox.Text.Substring(1));
            }
        }

        /// <summary>
        /// Copies the Id of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyId()
        {
            Clipboard.SetText(ActiveTranslationManager.SelectedId);
        }

        /// <summary>
        /// Copies the filename of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyFileName()
        {
            Clipboard.SetText(ActiveTranslationManager.FileName);
        }

        /// <summary>
        /// Copies the name of the story containing the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyStoryName()
        {
            Clipboard.SetText(ActiveTranslationManager.StoryName);
        }

        /// <summary>
        /// Copies the all info on the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAll()
        {
            Clipboard.SetText(
                ActiveTranslationManager.StoryName +
                "/" + ActiveTranslationManager.FileName +
                " : " +
                ActiveTranslationManager.TranslationData.Find(p => p.ID == ActiveTranslationManager.SelectedId).ToString());
        }

        /// <summary>
        /// Copies the data as it will be shown in the output files of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyAsOutput()
        {
            Clipboard.SetText(ActiveTranslationManager.TranslationData.Find(p => p.ID == ActiveTranslationManager.SelectedId).ToString());
        }

        /// <summary>
        /// Copies the translation of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTranslation()
        {
            Clipboard.SetText(ActiveTranslationManager.TranslationData.Find(p => p.ID == ActiveTranslationManager.SelectedId).TranslationString);
        }

        /// <summary>
        /// Copies the template of the selected, or most recently selected line into the clipboard
        /// </summary>
        public static void CopyTemplate()
        {
            DataBaseManager.GetStringTemplate(ActiveTranslationManager.SelectedId, ActiveTranslationManager.FileName, ActiveTranslationManager.StoryName, out string templateString);
            Clipboard.SetText(templateString);
        }

        private static PropertyHelper CreateActivePropertyHelper()
        {
            while (!Form.ActiveForm.Visible && Form.ActiveForm == null)
            {

            }
            Fenster fenster = (Fenster)Form.ActiveForm;
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
                fenster.ReplaceButton,
                (TextBox)TabControl.SelectedTab.Controls.Find("TemplateTextBox", true)[0],
                (TextBox)TabControl.SelectedTab.Controls.Find("TranslatedTextBox", true)[0]
                );
        }

        public static void Replace()
        {
            if (InGlobalSearch)
            {
                for (int i = 0; i < TabControl.TabCount; i++)
                {
                    translationManagers[TabControl.TabPages[i]].Replace(ActiveProperties.ReplaceBox.Text);
                }
            }
            else
            {
                ActiveTranslationManager.Replace(ActiveProperties.ReplaceBox.Text);
            }
        }
    }
}

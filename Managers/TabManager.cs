using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HousePartyTranslator.Helpers;


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
                    if (TabControl.GetTabRect(ix).Contains(e.Location))
                    {
                        //remove manager for the tab, save first
                        ActiveTranslationManager.SaveFile(ActiveProperties);
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
        public static TranslationManager Initialize(TabPage tabPage1)
        {
            while (TabControl == null)
            {
                //get tabcontrol as a statc reference;
                TabControl = (TabControl)Form.ActiveForm.Controls.Find("MainTabControl", true)[0];
            }

            //create new translationmanager to use with the tab open right now
            translationManagers.Add(tabPage1, new TranslationManager());
            properties.Add(tabPage1, CreateActivePropertyHelper());

            return translationManagers[tabPage1];
        }

        /// <summary>
        /// Does all the logic to open a file in a new tab
        /// </summary>
        public static void OpenNewTab()
        {
            //create new support objects
            TranslationManager t = new TranslationManager();
            TabPage newTab = Utils.CreateNewTab(translationManagers.Count + 1);
            //Add tab to form control
            TabControl.TabPages.Add(newTab);
            //select new tab
            TabControl.SelectedTab = newTab;
            //create support dict
            translationManagers.Add(newTab, t);
            properties.Add(newTab, CreateActivePropertyHelper());

            //call startup for new translationmanager
            t.SetLanguage(ActiveProperties);
            t.LoadFileIntoProgram(ActiveProperties);

            //update tab name
            if (t.FileName.Length > 0) newTab.Text = t.FileName;
        }

        /// <summary>
        /// Called when tabs are switched, swaps the search terms
        /// </summary>
        public static void SwitchTabs()
        {
            //set search term to the one from the respective TranslationManager
            if (ActiveTranslationManager != null)
            {
                if (InGlobalSearch)
                {
                    ActiveTranslationManager.Search(ActiveProperties, ActiveProperties.SearchBox.Text.Substring(1));
                }
                else
                {
                    ActiveProperties.SearchBox.Text = ActiveTranslationManager.SearchQuery;
                }
            }
        }

        /// <summary>
        /// Switches to the Tab specified, does all the other logic behind
        /// </summary>
        /// <param name="i">The tab index to switch to</param>
        public static void SwitchTabs(int i)
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
        public static bool SearchAll()
        {
            //global search has to start with the ?
            if (ActiveProperties.SearchBox.TextLength > 0)
            {
                if (ActiveProperties.SearchBox.Text[0] == '?')
                {
                    InGlobalSearch = true;
                    ActiveTranslationManager.Search(ActiveProperties, ActiveProperties.SearchBox.Text.Substring(1));
                    return true;
                }
                else
                {
                    InGlobalSearch = false;
                    return false;
                }
            }
            else
            {
                InGlobalSearch = false;
                return false;
            }
        }

        private static PropertyHelper CreateActivePropertyHelper()
        {
            return new PropertyHelper(
                (CheckBox)TabControl.SelectedTab.Controls.Find("ApprovedBox", true)[0],
                (ColouredCheckedListBox)TabControl.SelectedTab.Controls.Find("CheckListBoxLeft", true)[0],
                ((Fenster)Form.ActiveForm).LanguageBox,
                (Label)TabControl.SelectedTab.Controls.Find("WordsTranslated", true)[0],
                (Label)TabControl.SelectedTab.Controls.Find("CharacterCountLabel", true)[0],
                (Label)TabControl.SelectedTab.Controls.Find("SelectedFile", true)[0],
                (NoAnimationBar)TabControl.SelectedTab.Controls.Find("ProgressbarTranslated", true)[0],
                (TextBox)TabControl.SelectedTab.Controls.Find("CommentTextBox", true)[0],
                ((Fenster)Form.ActiveForm).SearchBox,
                (TextBox)TabControl.SelectedTab.Controls.Find("EnglishTextBox", true)[0],
                (TextBox)TabControl.SelectedTab.Controls.Find("TranslatedTextBox", true)[0]
                );
        }
    }
}

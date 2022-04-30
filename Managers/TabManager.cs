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
        public static TabControl TabControl;
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

        public static void Initialize(TabPage tabPage1)
        {
            //get tabcontrol as a statc reference;
            TabControl = (TabControl)Form.ActiveForm.Controls.Find("MainTabControl", true)[0];

            //create new translationmanager to use with the tab open right now
            translationManagers.Add(tabPage1, new TranslationManager());
            properties.Add(tabPage1, CreateActivePropertyHelper());
        }

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

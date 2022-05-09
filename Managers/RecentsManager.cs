using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    //todo also resume to last string
    static class RecentsManager
    {
        static readonly List<string> recents = new List<string>(5);
        private static Helpers.PropertyHelper _helper;

        public static void Initialize(Helpers.PropertyHelper helper)
        {
            _helper = helper;
            //add all saved recents
            recents.Add(Properties.Settings.Default.recents_0);
            recents.Add(Properties.Settings.Default.recents_1);
            recents.Add(Properties.Settings.Default.recents_2);
            recents.Add(Properties.Settings.Default.recents_3);
            recents.Add(Properties.Settings.Default.recents_4);
        }

        public static void SetMostRecent(string filepath)
        {
            if (filepath.Length > 0) recents.Insert(0, filepath);
        }

        public static ToolStripItem[] GetRecents()
        {
            int count = 0;
            for (int i = 0; i < recents.Count; i++)
            {
                if (recents[i].Length > 10)
                {
                    count++;
                }
            }
            count = count < 5 ? count : 5;
            ToolStripItem[] items = new ToolStripItem[count];
            int k = 0;
            for (int i = 0; i < recents.Count; i++)
            {
                if (recents[i].Length > 10)
                {
                    items[k] = new ToolStripMenuItem(recents[i]);
                    items[k].Click += RecentsManager_Click;
                    if (k < 4)
                    {
                        k++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return items;
        }

        private static void RecentsManager_Click(object sender, EventArgs e)
        {
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.LoadFileIntoProgram(_helper, ((ToolStripMenuItem)sender).Text);             
        }

        public static void UpdateMenuItems(ToolStripItemCollection collection)
        {
            ToolStripItem[] items = GetRecents();
            if (items.Length > 0)
            {
                if (items.Length == 5)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (collection[3].GetType() != typeof(ToolStripSeparator)) collection.RemoveAt(3);
                        else break;
                    }
                }
                else
                {
                    for (int i = 0; i < items.Length - 1; i++)
                    {
                        if (collection[3].GetType() != typeof(ToolStripSeparator)) collection.RemoveAt(3);
                        else break;
                    }
                }
            }
            for (int i = 0; i < items.Length; i++)
            {
                collection.Insert(3, items[items.Length - i - 1]);
            }

            SaveRecents();
            //save settings
            Properties.Settings.Default.Save();
        }

        public static void SaveRecents()
        {
            Properties.Settings.Default.recents_0 = recents[0];
            Properties.Settings.Default.recents_1 = recents[1];
            Properties.Settings.Default.recents_2 = recents[2];
            Properties.Settings.Default.recents_3 = recents[3];
            Properties.Settings.Default.recents_4 = recents[4];
        }
    }
}

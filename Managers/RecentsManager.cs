using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    internal static class RecentsManager
    {
        private static readonly List<string> recents = new List<string>(5);
        /// <summary>
        /// Set to the number of recents set you want to ignore, used for the first one on stasrtup here
        /// </summary>
        public static int ignorenextRecents = 0;
        private static int recentIndex = -1;

        /// <summary>
        /// Get the most recently opened files as a collection of ToolStriItems
        /// </summary>
        /// <returns>A Collection of ToolStripItems with a length between 0 and 5</returns>
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
            var items = new ToolStripItem[count];
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

        /// <summary>
        /// Initializes the most recently opened files from the saved settings
        /// </summary>
        public static void Initialize()
        {
            //add all saved recents
            recents.Add(Properties.Settings.Default.recents_0);
            recents.Add(Properties.Settings.Default.recents_1);
            recents.Add(Properties.Settings.Default.recents_2);
            recents.Add(Properties.Settings.Default.recents_3);
            recents.Add(Properties.Settings.Default.recents_4);
            recentIndex = Properties.Settings.Default.recent_index;
        }

        /// <summary>
        /// Save the recently opened file paths to the settings
        /// </summary>
        public static void SaveRecents()
        {
            //set most recent to the last file open in the selected tab so the index is correct
            Properties.Settings.Default.recents_0 = TabManager.ActiveTranslationManager?.SourceFilePath ?? "";
            Properties.Settings.Default.recents_1 = recents.Count > 1 ? recents[1] : "";
            Properties.Settings.Default.recents_2 = recents.Count > 2 ? recents[2] : "";
            Properties.Settings.Default.recents_3 = recents.Count > 3 ? recents[3] : "";
            Properties.Settings.Default.recents_4 = recents.Count > 4 ? recents[4] : "";
            Properties.Settings.Default.recent_index = TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex;

            //save settings
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Sets the given path as the most recently opened file
        /// </summary>
        /// <param name="filepath">The path to set as most recent</param>
        public static void SetMostRecent(string filepath)
        {//if we dont ignore recents, keep number near 0 so we dont underflow
            if (ignorenextRecents-- <= 0)
            {
                if (filepath.Length > 0) recents.Insert(0, filepath);
                ignorenextRecents = 0;
            }
        }

        /// <summary>
        /// Opens the most recent file
        /// </summary>
        public static void OpenMostRecent()
        {
            if (Properties.Settings.Default.autoLoadRecent)
            {
                if (recents.Count > 0)
                {
                    ignorenextRecents = 1;
                    TranslationManager t = TabManager.ActiveTranslationManager;
                    t.LoadFileIntoProgram(recents[0]);
                    if (Properties.Settings.Default.autoLoadRecentIndex) t.SelectLine(recentIndex);
                    else t.SelectLine(0);
                }
            }
        }

        /// <summary>
        /// Updates the recent menuitems in the given collection
        /// </summary>
        /// <param name="collection"></param>
        public static void UpdateMenuItems(ToolStripItemCollection collection)
        {
            ToolStripItem[] items = GetRecents();
            if (items.Length > 0)
            {
                int recentsStart;
                //find start of recents
                for (recentsStart = 0; recentsStart < collection.Count; recentsStart++)
                {
                    if (collection[recentsStart].GetType() == typeof(ToolStripSeparator)) break;
                }
                recentsStart += 2;
                for (int i = 0; i < items.Length; i++)
                {
                    collection.Insert(recentsStart, items[items.Length - i - 1]);
                }

                collection.Insert(recentsStart + items.Length, new ToolStripSeparator());
                SaveRecents();
            }
        }

        private static void RecentsManager_Click(object sender, EventArgs e)
        {
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.LoadFileIntoProgram(((ToolStripMenuItem)sender).Text);
            if (Properties.Settings.Default.autoLoadRecentIndex) translationManager.SelectLine(recentIndex);
        }
    }
}
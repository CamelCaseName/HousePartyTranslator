using System;
using System.Collections.Generic;

namespace Translator.Core
{
    public static class RecentsManager
    {
        private static readonly List<string> recents = new(5);
        /// <summary>
        /// Set to the number of recents set you want to ignore, used for the first one on startup here
        /// </summary>
        public static int ignorenextRecents = 0;
        private static int maxNumberOfMenuItems = 0;
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
            recents.Add(Settings.Default.recents_0);
            recents.Add(Settings.Default.recents_1);
            recents.Add(Settings.Default.recents_2);
            recents.Add(Settings.Default.recents_3);
            recents.Add(Settings.Default.recents_4);
            recentIndex = Settings.Default.recent_index;
        }

        /// <summary>
        /// Save the recently opened file paths to the settings
        /// </summary>
        public static void SaveRecents()
        {
            //set most recent to the last file open in the selected tab so the index is correct
            Settings.Default.recents_0 = TabManager.ActiveTranslationManager?.SourceFilePath ?? "";
            Settings.Default.recents_1 = recents.Count > 1 ? recents[1] : "";
            Settings.Default.recents_2 = recents.Count > 2 ? recents[2] : "";
            Settings.Default.recents_3 = recents.Count > 3 ? recents[3] : "";
            Settings.Default.recents_4 = recents.Count > 4 ? recents[4] : "";
            Settings.Default.recent_index = TabManager.ActiveProperties?.CheckListBoxLeft.SelectedIndex ?? 0;

            //save settings
            Settings.Default.Save();
        }

        /// <summary>
        /// Sets the given path as the most recently opened file
        /// </summary>
        /// <param name="filepath">The path to set as most recent</param>
        public static void SetMostRecent(string filepath)
        {//if we dont ignore recents, keep number near 0 so we dont underflow
            if (--ignorenextRecents <= 0 && filepath.Length > 0)
            {
                if (filepath.Length > 0) recents.Insert(0, filepath);
                if (recents.Count > 5) recents.RemoveRange(5, recents.Count - 5);
                ignorenextRecents = 0;
            }
        }

        /// <summary>
        /// Opens the most recent file
        /// </summary>
        
        public static void OpenMostRecent()
        {
            if (Settings.Default.autoLoadRecent)
            {
                bool recentsAvailable = false;
                for (int i = 0; i < 5; i++)
                {
                    if (recents[i].Length > 0)
                    {
                        recentsAvailable = true;
                        break;
                    }
                }
                if (recentsAvailable)
                {
                    ignorenextRecents = 1;
                    TabManager.ActiveTranslationManager?.LoadFileIntoProgram(recents[0]);
                    if (Settings.Default.autoLoadRecentIndex) TranslationManager.SelectLine(recentIndex);
                    else TranslationManager.SelectLine(0);
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
            int recentsStart;

            if (maxNumberOfMenuItems == 0) maxNumberOfMenuItems = collection.Count + 6;

            //find start of recents
            for (recentsStart = 0; recentsStart < collection.Count; recentsStart++)
            {
                if (collection[recentsStart].GetType() == typeof(ToolStripSeparator)) break;
            }
            //update menu
            if (items.Length > 0 && collection.Count < maxNumberOfMenuItems)
            {
                recentsStart += 2;
                for (int i = 0; i < items.Length; i++)
                {
                    //we replace until we hit seperator, then we insert
                    if (collection[recentsStart + i].GetType() != typeof(ToolStripSeparator))
                    {
                        collection.RemoveAt(recentsStart + i);
                    }

                    collection.Insert(recentsStart, items[items.Length - i - 1]);
                }

                if (collection[recentsStart + items.Length].GetType() != typeof(ToolStripSeparator))
                    collection.Insert(recentsStart + items.Length, new ToolStripSeparator());
                SaveRecents();
                //for the name update stuff
                recentsStart -= 2;
            }

            if (items.Length == 0) collection[recentsStart + 1].Text = "No Recents";
            else collection[recentsStart + 1].Text = "Recents:";
        }

        
        private static void RecentsManager_Click(object? sender, EventArgs? e)
        {
            if (sender == null) return;
            TranslationManager? t = TabManager.ActiveTranslationManager;
            t?.ShowAutoSaveDialog();
            t?.LoadFileIntoProgram(((ToolStripMenuItem)sender).Text);
            if (Settings.Default.autoLoadRecentIndex) TranslationManager.SelectLine(recentIndex);
        }
    }
}
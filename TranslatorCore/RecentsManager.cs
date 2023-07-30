using System;
using System.Collections.Generic;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{
    public static class RecentsManager
    {
        private static readonly List<string> recents = new(5);
        private static int IgnoreNextRecents = 0;
        private static int recentIndex = -1;
        private static Type MenuItem = typeof(IMenuItem);

        /// <summary>
        /// Get the most recently opened files as a collection of ToolStriItems
        /// </summary>
        /// <returns>A Collection of ToolStripItems with a length between 0 and 5</returns>
        public static IMenuItem[] GetRecents()
        {
            if (!MenuItem.IsAssignableTo(typeof(IMenuItem))) return Array.Empty<IMenuItem>();

            int count = 0;
            for (int i = 0; i < recents.Count; i++)
            {
                if (recents[i].Length > 10)
                {
                    count++;
                }
            }
            count = count < 5 ? count : 5;
            var items = new IMenuItem[count];
            int k = 0;
            for (int i = 0; i < recents.Count; i++)
            {
                if (recents[i].Length > 10)
                {
                    items[k] = (IMenuItem)(Activator.CreateInstance(MenuItem, new object?[]
                    {
                        recents[i],
                        (EventHandler)RecentsManager_Click
                    }) ?? new object());
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
        public static void Initialize(Type MenuItem)
        {
            RecentsManager.MenuItem = MenuItem;
            //add all saved recents
            recents.Add(Settings.Default.Recents0);
            recents.Add(Settings.Default.Recents1);
            recents.Add(Settings.Default.Recents2);
            recents.Add(Settings.Default.Recents3);
            recents.Add(Settings.Default.Recents4);
            recentIndex = Settings.Default.RecentIndex;
        }

        /// <summary>
        /// Save the recently opened file paths to the settings
        /// </summary>
        public static void SaveRecents()
        {
            //set most recent to the last file open in the selected tab so the index is correct
            Settings.Default.Recents0 = TabManager.ActiveTranslationManager.SourceFilePath ?? string.Empty;
            Settings.Default.Recents1 = recents.Count > 1 ? recents[1] : string.Empty;
            Settings.Default.Recents2 = recents.Count > 2 ? recents[2] : string.Empty;
            Settings.Default.Recents3 = recents.Count > 3 ? recents[3] : string.Empty;
            Settings.Default.Recents4 = recents.Count > 4 ? recents[4] : string.Empty;
            Settings.Default.RecentIndex = TabManager.SelectedTab.SelectedLineIndex;

            //save settings
            Settings.Default.Save();
        }

        /// <summary>
        /// Sets the given path as the most recently opened file
        /// </summary>
        /// <param name="filepath">The path to set as most recent</param>
        public static void SetMostRecent(string filepath)
        {//if we dont ignore recents, keep number near 0 so we dont underflow
            if (--IgnoreNextRecents <= 0 && filepath.Length > 0)
            {
                if (filepath.Length > 0) recents.Insert(0, filepath);
                if (recents.Count > 5) recents.RemoveRange(5, recents.Count - 5);
                IgnoreNextRecents = 0;
            }
        }

        /// <summary>
        /// Opens the most recent file
        /// </summary>
        public static void OpenMostRecent()
        {
            if (Settings.Default.AutoLoadRecent)
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
                    IgnoreNextRecents = 1;
                    TabManager.ActiveTranslationManager.LoadFileIntoProgram(recents[0]);
                    if (Settings.Default.AutoLoadRecentIndex) TabManager.SelectLine(recentIndex);
                    else TabManager.SelectLine(0);
                }
            }
        }

        private static void RecentsManager_Click(object? sender, EventArgs? e)
        {
            if (sender is null || TabManager.ActiveTranslationManager is null) return;
            TabManager.ShowAutoSaveDialog();
            TabManager.ActiveTranslationManager.LoadFileIntoProgram(((IMenuItem)sender).Text);
            if (Settings.Default.AutoLoadRecentIndex) TabManager.SelectLine(recentIndex);
        }
    }
}
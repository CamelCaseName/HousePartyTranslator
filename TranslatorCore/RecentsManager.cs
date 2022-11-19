﻿using System;
using System.Collections.Generic;
using Translator.UICompatibilityLayer;

namespace Translator.Core
{
    public static class RecentsManager
    {
        private static readonly List<string> recents = new(5);
        /// <summary>
        /// Set to the number of recents set you want to ignore, used for the first one on startup here
        /// </summary>
        public static int IgnoreNextRecents = 0;
        private static int maxNumberOfMenuItems = 0;
        private static int recentIndex = -1;

        /// <summary>
        /// Get the most recently opened files as a collection of ToolStriItems
        /// </summary>
        /// <returns>A Collection of ToolStripItems with a length between 0 and 5</returns>
        public static IMenuItem[] GetRecents<MenuItemType>() where MenuItemType : class, IMenuItem, new()
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
            var items = new IMenuItem[count];
            int k = 0;
            for (int i = 0; i < recents.Count; i++)
            {
                if (recents[i].Length > 10)
                {
                    items[k] = new MenuItemType()
                    {
                        Text = recents[i],
                        OnClick = RecentsManager_Click
                    };
                }
                if (k < 4)
                {
                    k++;
                }
                else
                {
                    break;
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
            Settings.Default.Recents0 = TabManager.ActiveTranslationManager?.SourceFilePath ?? "";
            Settings.Default.Recents1 = recents.Count > 1 ? recents[1] : "";
            Settings.Default.Recents2 = recents.Count > 2 ? recents[2] : "";
            Settings.Default.Recents3 = recents.Count > 3 ? recents[3] : "";
            Settings.Default.Recents4 = recents.Count > 4 ? recents[4] : "";
            Settings.Default.RecentIndex = TabManager.ActiveProperties?.CheckListBoxLeft.SelectedIndex ?? 0;

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
                    TabManager.ActiveTranslationManager?.LoadFileIntoProgram(recents[0]);
                    if (Settings.Default.AutoLoadRecentIndex) TranslationManager.SelectLine(recentIndex);
                    else TranslationManager.SelectLine(0);
                }
            }
        }

        /// <summary>
        /// Updates the recent menuitems in the given collection
        /// </summary>
        /// <param name="collection"></param>

        public static void UpdateMenuItems<MenuItemType, MenuItemSeperatorType>(MenuItems collection) where MenuItemType : class, IMenuItem, new() where MenuItemSeperatorType : class, IMenuItem, new()
        {
            IMenuItem[] items = GetRecents<MenuItemType>();
            int recentsStart;

            if (maxNumberOfMenuItems == 0) maxNumberOfMenuItems = collection.Count + 6;

            //find start of recents
            for (recentsStart = 0; recentsStart < collection.Count; recentsStart++)
            {
                if (collection[recentsStart].GetType() == typeof(MenuItemSeperatorType)) break;
            }
            //update menu
            if (items.Length > 0 && collection.Count < maxNumberOfMenuItems)
            {
                recentsStart += 2;
                for (int i = 0; i < items.Length; i++)
                {
                    //we replace until we hit seperator, then we insert
                    if (collection[recentsStart + i].GetType() != typeof(MenuItemSeperatorType))
                    {
                        collection.RemoveAt(recentsStart + i);
                    }

                    collection.Insert(recentsStart, items[items.Length - i - 1]);
                }

                if (collection[recentsStart + items.Length].GetType() != typeof(MenuItemSeperatorType))
                    collection.Insert(recentsStart + items.Length, new MenuItemSeperatorType());
                SaveRecents();
                //for the name update stuff
                recentsStart -= 2;
            }

            if (items.Length == 0) collection[recentsStart + 1].Text = "No Recents";
            else collection[recentsStart + 1].Text = "Recents:";
        }

        private static void RecentsManager_Click(object? sender, EventArgs? e)
        {
            if (sender == null || TabManager.ActiveTranslationManager == null) return;
            TabManager.ActiveTranslationManager?.ShowAutoSaveDialog();
            TabManager.ActiveTranslationManager?.LoadFileIntoProgram(((IMenuItem)sender).Text);
            if (Settings.Default.AutoLoadRecentIndex) TranslationManager.SelectLine(recentIndex);
        }
    }
}
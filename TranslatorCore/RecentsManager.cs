﻿using System;
using System.Collections.Generic;
using Translator.UICompatibilityLayer;
using Translator.UICompatibilityLayer.StubImpls;

namespace Translator.Core
{
    public static class RecentsManager
    {
        private static readonly List<string> recents = new(5);
        private static int IgnoreNextRecents = 0;
        private static int maxNumberOfMenuItems = 0;
        private static int recentIndex = -1;
        private static Type MenuItem = (Type)Type.Missing;
        private static Type MenuItemSeperator= (Type)Type.Missing;

        /// <summary>
        /// Get the most recently opened files as a collection of ToolStriItems
        /// </summary>
        /// <returns>A Collection of ToolStripItems with a length between 0 and 5</returns>
        public static IMenuItem[] GetRecents()
        {
            if (MenuItem.IsAssignableTo(typeof(IMenuItem))) return Array.Empty<IMenuItem>();

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
                        (object)RecentsManager_Click
                    }) ?? NullMenuItem.Instance);
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
        public static void Initialize(Type MenuItem, Type MenuItemSeperator)
        {
            RecentsManager.MenuItem = MenuItem;
            RecentsManager.MenuItemSeperator = MenuItemSeperator;
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
            Settings.Default.Recents0 = TabManager<NullLineItem>.ActiveTranslationManager?.SourceFilePath ?? "";
            Settings.Default.Recents1 = recents.Count > 1 ? recents[1] : "";
            Settings.Default.Recents2 = recents.Count > 2 ? recents[2] : "";
            Settings.Default.Recents3 = recents.Count > 3 ? recents[3] : "";
            Settings.Default.Recents4 = recents.Count > 4 ? recents[4] : "";
            Settings.Default.RecentIndex = TabManager<NullLineItem>.SelectedTab.SelectedLineIndex;

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
        public static void OpenMostRecent<T>() where T : class, ILineItem, new()
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
                    TabManager<T>.ActiveTranslationManager?.LoadFileIntoProgram(recents[0]);
                    if (Settings.Default.AutoLoadRecentIndex) TranslationManager<T>.SelectLine(recentIndex);
                    else TranslationManager<T>.SelectLine(0);
                }
            }
        }

        /// <summary>
        /// Updates the recent menuitems in the given collection
        /// </summary>
        /// <param name="collection"></param>
        public static void UpdateMenuItems(MenuItems collection)
        {
            IMenuItem[] items = GetRecents();
            int recentsStart;

            if (maxNumberOfMenuItems == 0) maxNumberOfMenuItems = collection.Count + 6;

            //find start of recents
            for (recentsStart = 0; recentsStart < collection.Count; recentsStart++)
            {
                if (collection[recentsStart].GetType() == MenuItemSeperator) break;
            }
            //update menu
            if (items.Length > 0 && collection.Count < maxNumberOfMenuItems)
            {
                recentsStart += 2;
                for (int i = 0; i < items.Length; i++)
                {
                    //we replace until we hit seperator, then we insert
                    if (collection[recentsStart + i].GetType() != MenuItemSeperator)
                    {
                        collection.RemoveAt(recentsStart + i);
                    }

                    collection.Insert(recentsStart, items[items.Length - i - 1]);
                }

                if (collection[recentsStart + items.Length].GetType() != MenuItemSeperator)
                    collection.Insert(recentsStart + items.Length, (IMenuItem)(Activator.CreateInstance(MenuItemSeperator) ?? NullMenuItem.Instance));
                SaveRecents();
                //for the name update stuff
                recentsStart -= 2;
            }

            if (items.Length == 0) collection[recentsStart + 1].Text = "No Recents";
            else collection[recentsStart + 1].Text = "Recents:";
        }

        private static void RecentsManager_Click(object? sender, EventArgs? e)
        {
            if (sender == null || TabManager<NullLineItem>.ActiveTranslationManager == null) return;
            TabManager<NullLineItem>.ActiveTranslationManager?.ShowAutoSaveDialog();
            TabManager<NullLineItem>.ActiveTranslationManager?.LoadFileIntoProgram(((IMenuItem)sender).Text);
            if (Settings.Default.AutoLoadRecentIndex) TranslationManager<NullLineItem>.SelectLine(recentIndex);
        }
    }
}
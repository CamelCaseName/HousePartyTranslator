﻿using Google.Protobuf;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.Helpers
{
    /// <summary>
    /// Provides some generic utility methods.
    /// </summary>

    public static partial class Utils
    {
        public const int MaxTextLength = 100;
        public const int MaxWordLength = 15;
        public static readonly Color foreground = SystemColors.Window;
        public static readonly Color background = SystemColors.ControlDarkDark;
        public static readonly Color backgroundDarker = SystemColors.MenuText;
        public static readonly Color brightText = SystemColors.HighlightText;
        public static readonly Color darkText = SystemColors.WindowText;
        public static readonly Color menu = SystemColors.ScrollBar;
        public static readonly Color frame = SystemColors.WindowFrame;
        public static readonly Color highlight = SystemColors.Info;
        public static readonly Color menuHighlight = SystemColors.MenuHighlight;
        private static HashSet<string> fileNames = new();
        private static HashSet<string> storyNames = new();
        private static DateTime namesAcquired = DateTime.MinValue;

        private static IUIHandler? MainUI { get; set; }

        internal static void Initialize(IUIHandler ui)
        {
            MainUI = ui;
            DataBase.GetAllFilesAndStories(out storyNames, out fileNames);
            namesAcquired = DateTime.Now;
        }

        /// <summary>
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.Log("Exception message shown: " + message);
            _ = MainUI?.ErrorOk(
                $"The application encountered a Problem. Probably the database can not be reached :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Something happened");

            MainUI?.SignalUserEndWait();
        }

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string SelectFileFromSystem(bool isTranslation = true, string Title = "", string preselectedFile = "", string filter = "Text files (*.txt)|*.txt", bool checkFileExists = false)
        {
            if (!MainUI?.FileDialogType.IsAssignableTo(typeof(IFileDialog)) ?? true) throw new ArgumentException($"{nameof(MainUI.FileDialogType)} does not inherit {nameof(IFileDialog)}");

            var selectFileDialog = (IFileDialog?)Activator.CreateInstance(MainUI?.FileDialogType ?? typeof(IFileDialog), new object?[]
            {
                /*title*/Title?.Length > 0 ? Title : "Choose a file for translation",
                /*filter*/filter,
                /*initialDirectory*/Settings.Default.TranslationPath.Length > 0 && isTranslation ?
                    Settings.Default.TranslationPath :
                    Settings.Default.TemplatePath.Length > 0 && !isTranslation ?
                        Settings.Default.TemplatePath :
                        @"C:\Users\%USER%\Documents",
                /*filename*/preselectedFile ?? string.Empty,
            });
            if (selectFileDialog == null) { return string.Empty; }

            selectFileDialog.MultiSelect = false;
            selectFileDialog.CheckFileExists = checkFileExists;
            if (selectFileDialog.ShowDialog() == PopupResult.OK)
            {
                if (isTranslation)
                    Settings.Default.TranslationPath = Path.GetDirectoryName(selectFileDialog.SelectedPath) ?? Settings.Default.TranslationPath;
                else
                    Settings.Default.TemplatePath = Path.GetDirectoryName(selectFileDialog.SelectedPath) ?? Settings.Default.TemplatePath;

                Settings.Default.Save();

                return selectFileDialog.SelectedPath;
            }
            return string.Empty;
        }

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string[] SelectFilesFromSystem(bool isTranslation = true, string Title = "", string preselectedFile = "")
        {
            if (!MainUI?.FileDialogType.IsAssignableTo(typeof(IFileDialog)) ?? true) throw new ArgumentException($"{nameof(MainUI.FileDialogType)} does not inherit {nameof(IFileDialog)}");

            var selectFileDialog = (IFileDialog?)Activator.CreateInstance(MainUI?.FileDialogType ?? typeof(IFileDialog), new object?[]
            {
                /*title*/Title?.Length > 0 ? Title : "Choose files for translation",
                /*filter*/"Text files (*.txt)|*.txt",
                /*initialDirectory*/Settings.Default.TranslationPath.Length > 0 && isTranslation ?
                    Settings.Default.TranslationPath :
                    Settings.Default.TemplatePath.Length > 0 && !isTranslation ?
                        Settings.Default.TemplatePath :
                        @"C:\Users\%USER%\Documents",
                /*filename*/preselectedFile ?? string.Empty
            });
            if (selectFileDialog == null) { return Array.Empty<string>(); }

            if (selectFileDialog.ShowDialog() == PopupResult.OK)
            {
                if (isTranslation)
                    Settings.Default.TranslationPath = Path.GetDirectoryName(selectFileDialog.SelectedPath) ?? Settings.Default.TranslationPath;
                else
                    Settings.Default.TemplatePath = Path.GetDirectoryName(selectFileDialog.SelectedPath) ?? Settings.Default.TemplatePath;

                Settings.Default.Save();

                return selectFileDialog.SelectedPaths;
            }
            return Array.Empty<string>();
        }

        /// <summary>
        /// Opens a select folder dialogue to find the template folder and returns the selected folder as a path.
        /// </summary>
        /// <returns>The folder path selected.</returns>
        public static string SelectTemplateFolderFromSystem()
        {
            return SelectFolderFromSystem("Please select the 'TEMPLATE' folder, for Help see the discord");
        }

        /// <summary>
        /// Opens a select folder dialogue and returns the selected folder as a path.
        /// </summary>
        /// <param name="message">The description of the dialogue to display</param>
        /// <returns>The folder path selected.</returns>
        public static string SelectFolderFromSystem(string message)
        {
            if (!MainUI?.FolderDialogType.IsAssignableTo(typeof(IFolderDialog)) ?? true) throw new ArgumentException($"{nameof(MainUI.FolderDialogType)} does not inherit {nameof(IFolderDialog)}");

            var selectFolderDialog = (IFolderDialog?)Activator.CreateInstance(MainUI?.FolderDialogType ?? typeof(IFileDialog), new object?[]
            {
                /*title*/message,
                /*selectedPath*/Settings.Default.TemplatePath == string.Empty ? Environment.SpecialFolder.UserProfile.ToString() : Settings.Default.TemplatePath,
            });
            if (selectFolderDialog == null) { return string.Empty; }

            if (selectFolderDialog.ShowDialog() == PopupResult.OK)
            {
                string t = selectFolderDialog.SelectedFolderPath;
                if (t != null)
                {
                    Settings.Default.TemplatePath = t;
                    Settings.Default.Save();
                    return t;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Opens a save file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the file to save to.</returns>
        public static string SelectSaveLocation(string message = "", string path = "", string file = "", string extension = "txt", bool checkFileExists = true, bool checkPathExists = true, bool createPrompt = false)
        {
            if (!MainUI?.SaveFileDialogType.IsAssignableTo(typeof(ISaveFileDialog)) ?? true) throw new ArgumentException($"{nameof(MainUI.SaveFileDialogType)} does not inherit {nameof(ISaveFileDialog)}");

            var saveFileDialog = (ISaveFileDialog?)Activator.CreateInstance(MainUI?.SaveFileDialogType ?? typeof(ISaveFileDialog), new object?[]
            {
                /*Title*/message,
                /*Extension*/ extension,
                /*CreatePrompt*/ createPrompt,
                /*OverwritePrompt*/ true,
                /*FileName*/ file,
                /*InitialDirectory*/ path
            });
            if (saveFileDialog == null) return string.Empty;

            saveFileDialog.CheckFileExists = checkFileExists;
            saveFileDialog.CheckPathExists = checkPathExists;

            if (saveFileDialog.ShowDialog() == PopupResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return string.Empty;
        }

        public static string ExtractStoryName(string path)
        {
            if ((DateTime.Now - namesAcquired).Hours > 1)
                DataBase.GetAllFilesAndStories(out storyNames, out fileNames);

            if (!LanguageHelper.Languages.TryGetValue(TranslationManager.Language, out string? languageAsText))
                throw new LanguageHelper.LanguageException();

            var paths = path.Split('\\');
            if (paths.Length < 3) throw new ArgumentException("the file needs to be at least 2 folders deep from your drive?");

            //check if we have a similar name to the cloud, return that if we have
            for (int i = paths.Length - 1; i >= 0; i--)
            {
                var enumerator = storyNames.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (string.Compare(paths[i], enumerator.Current, true, CultureInfo.InvariantCulture) == 0)
                        return enumerator.Current;
                }
            }

            string maybeStoryName = paths[^2];
            if (!paths[^1].Contains('.')) //if we already got a folder in path we can use the last element
                return paths[^1];

            //check if we are in the games documents
            if (string.Compare(maybeStoryName, languageAsText, true, CultureInfo.InvariantCulture) == 0 ||
                string.Compare(maybeStoryName, string.Concat(languageAsText, " new"), true, CultureInfo.InvariantCulture) == 0)
                //get folder one more up
                maybeStoryName = paths[^3];

            //also can return instantly, we are in the folder which has the languages in it
            if (string.Compare(maybeStoryName, "Languages", true, CultureInfo.InvariantCulture) == 0) //get folder one more up
                return "UI";

            //check if we have a similar name to the cloud, return that if we have
            for (int i = paths.Length - 1; i >= 0; i--)
            {
                //check again more lenient
                var enumerator = storyNames.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (paths[i].Contains(enumerator.Current, StringComparison.InvariantCultureIgnoreCase))
                        if (MainUI!.InfoYesNo($"Is this the correct story, as it appeared in the templates: {enumerator.Current}?", "Correct story?", PopupResult.YES))
                            return enumerator.Current;
                    if (enumerator.Current.Contains(paths[i], StringComparison.InvariantCultureIgnoreCase))
                        if (MainUI!.InfoYesNo($"Is this the correct story, as it appeared in the templates: {enumerator.Current}?", "Correct story?", PopupResult.YES))
                            return enumerator.Current;
                }
            }

            //we didnt find it, probably new custom story
            return maybeStoryName;
        }

        public static string ExtractFileName(string path)
        {
            if ((DateTime.Now - namesAcquired).Hours > 1)
                DataBase.GetAllFilesAndStories(out storyNames, out fileNames);

            string maybeFileName = Path.GetFileNameWithoutExtension(path);

            var enumerator = fileNames.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (string.Compare(maybeFileName, enumerator.Current, true, CultureInfo.InvariantCulture) == 0)
                    return enumerator.Current;
            }
            //search again more lenient
            enumerator = fileNames.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (maybeFileName.Contains(enumerator.Current, StringComparison.InvariantCultureIgnoreCase))
                    if (MainUI!.InfoYesNo($"Is this the correct filename, as it appeared in the templates: {enumerator.Current}?", "Correct file?", PopupResult.YES))
                        return enumerator.Current;
                if (enumerator.Current.Contains(maybeFileName, StringComparison.InvariantCultureIgnoreCase))
                    if (MainUI!.InfoYesNo($"Is this the correct filename, as it appeared in the templates: {enumerator.Current}?", "Correct file?", PopupResult.YES))
                        return enumerator.Current;
            }

            //we have no known filename, we can just continue and ask if its a custom story
            return maybeFileName;
        }

        public static (string story, string file) ExtractFileAndStoryName(string path) => (ExtractStoryName(path), ExtractFileName(path));

        /// <summary>
        /// Gets the current assembly version as a string.
        /// </summary>
        /// <returns>The current assembly version</returns>
        public static string GetAssemblyFileVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersion?.FileVersion ?? "0.0.0.0";
        }

        /// <summary>
        /// Tries to parse a line into the category it indicates.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The category representing the string, or none.</returns>
        public static StringCategory GetCategoryFromString(string line)
        {
            if (line.Contains('['))
            {
                switch (line)
                {
                    case "[General]":
                        return StringCategory.General;
                    case "[Dialogues]":
                        return StringCategory.Dialogue;
                    case "[Responses]":
                        return StringCategory.Response;
                    case "[Events]":
                        return StringCategory.Event;
                    case "[Background Chatter]":
                        return StringCategory.BGC;
                    case "[Quests]":
                        return StringCategory.Quest;
                    case "[Item Names]":
                        return StringCategory.ItemName;
                    case "[Item Action]":
                        return StringCategory.ItemAction;
                    case "[Item Group Action]":
                        return StringCategory.ItemGroupAction;
                    case "[Achievements]":
                        return StringCategory.Achievement;
                    default:
                        break;
                }
            }
            return StringCategory.Neither;
        }

        /// <summary>
        /// Returns the string representatio of a category.
        /// </summary>
        /// <param name="category">The Category to parse.</param>
        /// <returns>The string representing the category.</returns>
        public static string GetStringFromCategory(StringCategory category)
        {
            return category switch
            {
                StringCategory.General => "[General]",
                StringCategory.Dialogue => "[Dialogues]",
                StringCategory.Response => "[Responses]",
                StringCategory.Quest => "[Quests]",
                StringCategory.Event => "[Events]",
                StringCategory.BGC => "[Background Chatter]",
                StringCategory.ItemName => "[Item Names]",
                StringCategory.ItemAction => "[Item Actions]",
                StringCategory.ItemGroupAction => "[Item Group Actions]",
                StringCategory.Achievement => "[Achievements]",
                StringCategory.Neither => string.Empty,//do nothing hehehehe
                _ => string.Empty,//do nothing hehehehe
            };
        }
    }
}

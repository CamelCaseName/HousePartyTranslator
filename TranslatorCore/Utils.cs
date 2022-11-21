using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using Translator.UICompatibilityLayer;
using Translator.UICompatibilityLayer.StubImpls;

namespace Translator.Core.Helpers
{
    /// <summary>
    /// Provides some generic utility methods.
    /// </summary>
    public static class Utils
    {
        public const int MaxTextLength = 100;
        public const int MaxWordLength = 15;

        private static int ExceptionCount = 0;
        private static IUIHandler MainUI = new NullUIHandler();

        public static readonly Color foreground = SystemColors.Window;
        public static readonly Color background = SystemColors.ControlDarkDark;
        public static readonly Color backgroundDarker = SystemColors.MenuText;
        public static readonly Color brightText = SystemColors.HighlightText;
        public static readonly Color darkText = SystemColors.WindowText;
        public static readonly Color menu = SystemColors.ScrollBar;
        public static readonly Color frame = SystemColors.WindowFrame;

        internal static void Initialize(IUIHandler ui)
        {
            MainUI = ui;
        }

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
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.Log("Exception message shown: " + message);
            LogManager.Log("Current exception count: " + ExceptionCount++);
            _ = MainUI.ErrorOk(
                $"The application encountered a Problem. Probably the database can not be reached, or you did something too quickly :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Some Error found (Nr. {ExceptionCount})");

            MainUI.SignalUserEndWait();

            if (ExceptionCount > 3)
            {
                LogManager.Log("Too many exceptions encountered, aborting", LogManager.Level.Crash);
                MainUI.SignalAppExit();
            }
        }

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string SelectFileFromSystem(bool isTranslation = true, string Title = "", string preselectedFile = "")
        {
            IFileDialog? selectFileDialog = (IFileDialog?)Activator.CreateInstance(MainUI.FileDialogType,new object?[]
            {
                Title?.Length > 0 ? Title : "Choose a file for translation",
                "Text files (*.txt)|*.txt",
                Settings.Default.TranslationPath.Length > 0 && isTranslation ?
                    Settings.Default.TranslationPath :
                    Settings.Default.TemplatePath.Length > 0 && !isTranslation ?
                        Settings.Default.TemplatePath :
                        @"C:\Users\%USER%\Documents",
                preselectedFile ?? ""
            });
            if (selectFileDialog == null) { return string.Empty; }

            if (selectFileDialog.ShowDialog() == PopupResult.OK)
            {
                if (isTranslation)
                    Settings.Default.TranslationPath = Path.GetDirectoryName(selectFileDialog.FileName) ?? Settings.Default.TranslationPath;
                else
                    Settings.Default.TemplatePath = Path.GetDirectoryName(selectFileDialog.FileName) ?? Settings.Default.TemplatePath;

                Settings.Default.Save();

                return selectFileDialog.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Opens a select folder dialogue to find the template folder and returns the selected folder as a path.
        /// </summary>
        /// <returns>The folder path selected.</returns>
        public static string SelectTemplateFolderFromSystem()
        {
            return SelectFolderFromSystem("Please select the 'TEMPLATE' folder like in the repo");
        }

        /// <summary>
        /// Opens a select folder dialogue and returns the selected folder as a path.
        /// </summary>
        /// <param name="message">The description of the dialogue to display</param>
        /// <returns>The folder path selected.</returns>
        public static string SelectFolderFromSystem(string message)
        {
            IFileDialog? selectFolderDialog = (IFileDialog?)Activator.CreateInstance(MainUI.FileDialogType, new object?[]
            {
                message,
                Settings.Default.TemplatePath == string.Empty ? Environment.SpecialFolder.UserProfile.ToString() : Settings.Default.TemplatePath,
            });
            if(selectFolderDialog == null) { return string.Empty; }

            if (selectFolderDialog.ShowDialog() == PopupResult.OK)
            {
                string t = selectFolderDialog.SelectedPath;
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
        /// Tries to parse a line into the category it indicates.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The category representing the string, or none.</returns>
        public static StringCategory GetCategoryFromString(string line)
        {
            if (line.Contains('['))
            {
                if (line == "[General]")
                {
                    return StringCategory.General;
                }
                else if (line == "[Dialogues]")
                {
                    return StringCategory.Dialogue;
                }
                else if (line == "[Responses]")
                {
                    return StringCategory.Response;
                }
                else if (line == "[Quests]")
                {
                    return StringCategory.Quest;
                }
                else if (line == "[Events]")
                {
                    return StringCategory.Event;
                }
                else if (line == "[Background Chatter]")
                {
                    return StringCategory.BGC;
                }
                else if (line == "[Item Names]")
                {
                    return StringCategory.ItemName;
                }
                else if (line == "[Item Actions]")
                {
                    return StringCategory.ItemAction;
                }
                else if (line == "[Item Group Actions]")
                {
                    return StringCategory.ItemGroupAction;
                }
                else if (line == "[Achievements]")
                {
                    return StringCategory.Achievement;
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
#pragma warning disable IDE0066
            switch (category)
            {
                case StringCategory.General:
                    return "[General]";

                case StringCategory.Dialogue:
                    return "[Dialogues]";

                case StringCategory.Response:
                    return "[Responses]";

                case StringCategory.Quest:
                    return "[Quests]";

                case StringCategory.Event:
                    return "[Events]";

                case StringCategory.BGC:
                    return "[Background Chatter]";

                case StringCategory.ItemName:
                    return "[Item Names]";

                case StringCategory.ItemAction:
                    return "[Item Actions]";

                case StringCategory.ItemGroupAction:
                    return "[Item Group Actions]";

                case StringCategory.Achievement:
                    return "[Achievements]";

                case StringCategory.Neither:
                    //do nothing hehehehe
                    return "";

                default:
                    //do nothing hehehehe
                    return "";
            }
#pragma warning restore IDE0066
        }
    }

    public struct CategorizedLines
    {
        public List<LineData> lines;
        public StringCategory category;

        public CategorizedLines(List<LineData> lines, StringCategory category)
        {
            this.lines = lines;
            this.category = category;
        }

        public override bool Equals(object? obj) => obj is CategorizedLines other && EqualityComparer<List<LineData>>.Default.Equals(lines, other.lines) && category == other.category;

        public override int GetHashCode()
        {
            return HashCode.Combine(lines, category);
        }

        public void Deconstruct(out List<LineData> lines, out StringCategory category)
        {
            lines = this.lines;
            category = this.category;
        }

        public static implicit operator (List<LineData> lines, StringCategory category)(CategorizedLines value) => (value.lines, value.category);
        public static implicit operator CategorizedLines((List<LineData> lines, StringCategory category) value) => new(value.lines, value.category);

        public static bool operator ==(CategorizedLines left, CategorizedLines right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CategorizedLines left, CategorizedLines right)
        {
            return !(left == right);
        }
    }

    public sealed class FileData : Dictionary<string, LineData>
    {
        public FileData(Dictionary<string, LineData> data)
        {
            foreach (KeyValuePair<string, LineData> item in data)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public FileData()
        {
            this.Clear();
        }
    }
}

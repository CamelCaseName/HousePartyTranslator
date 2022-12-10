using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using Translator.UICompatibilityLayer;

namespace Translator.Core.Helpers
{
    /// <summary>
    /// Provides some generic utility methods.
    /// </summary>

    public static partial class Utils<T, V, X, W> 
		where T : class, ILineItem, new() 
		where V : class, IUIHandler<T, X, W>, new() 
		where X : class, ITabController<T, W>, new() 
		where W : class, ITab<T>, new()
    {
        private static int ExceptionCount = 0;
        private static V? MainUI { get; set; }

        internal static void Initialize(V ui)
        {
            MainUI = ui;
        }

        /// <summary>
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.Log("Exception message shown: " + message);
            LogManager.Log("Current exception count: " + ExceptionCount++);
            _ = MainUI?.ErrorOk(
                $"The application encountered a Problem. Probably the database can not be reached, or you did something too quickly :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Some Error found (Nr. {ExceptionCount})");

            MainUI?.SignalUserEndWait();

            if (ExceptionCount > 3)
            {
                LogManager.Log("Too many exceptions encountered, aborting", LogManager.Level.Crash);
                MainUI?.SignalAppExit();
            }
        }

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string SelectFileFromSystem(bool isTranslation = true, string Title = "", string preselectedFile = "")
        {
            if (!MainUI?.FileDialogType.IsAssignableTo(typeof(IFileDialog)) ?? true) throw new ArgumentException($"{nameof(MainUI.FileDialogType)} does not inherit {nameof(IFileDialog)}");

            IFileDialog? selectFileDialog = (IFileDialog?)Activator.CreateInstance(MainUI?.FileDialogType ?? typeof(IFileDialog), new object?[]
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
            if (!MainUI?.FolderDialogType.IsAssignableTo(typeof(IFolderDialog)) ?? true) throw new ArgumentException($"{nameof(MainUI.FolderDialogType)} does not inherit {nameof(IFolderDialog)}");

            IFolderDialog? selectFolderDialog = (IFolderDialog?)Activator.CreateInstance(MainUI?.FileDialogType ?? typeof(IFileDialog), new object?[]
            {
                message,
                Settings.Default.TemplatePath == string.Empty ? Environment.SpecialFolder.UserProfile.ToString() : Settings.Default.TemplatePath,
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
    }

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
                StringCategory.Neither => "",//do nothing hehehehe
                _ => "",//do nothing hehehehe
            };
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

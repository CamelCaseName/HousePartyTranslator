using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{
    public static class SaveAndExportManager
    {
        //counter so we dont get multiple ids, we dont use the dictionary as ids anyways when uploading templates
        private static int templateCounter = 0;

        public static void ExportTemplate(string path, string story = "", string file = "", bool warnOnOverwrite = false, bool confirmSuccess = true)
        {
            if (path == string.Empty) return;
            if (!File.Exists(path)) File.OpenWrite(path).Close();
            else if (warnOnOverwrite)
                if (TabManager.UI.WarningYesNo("You are about to overwrite " + path + "\n Are you sure?", "Warning!", PopupResult.NO)) return;

            if (story == string.Empty) story = Utils.ExtractStoryName(path);
            if (file == string.Empty) file = Utils.ExtractFileName(path);

            if (story == "Hints")
                file = "Hints";

            LogManager.Log("Exporting template for " + story + "/" + file + " to " + path);
            if (!DataBase.GetAllLineDataTemplate(file, story, out FileData templates))
            {
                TabManager.UI.WarningOk("No template found for that story/file, nothing exported.");
                LogManager.Log("    No template found for that file");
                return;
            }

            List<CategorizedLines> sortedLines = InitializeCategories(story, file);
            SortIntoCategories(ref sortedLines, templates, templates);

            WriteCategorizedLinesToDisk(sortedLines, path);

            if (confirmSuccess) TabManager.UI.InfoOk("Template exported to " + path);
            LogManager.Log("    Sucessfully exported the template");
        }

        public static void ExportTemplatesForStory(string path, string story = "")
        {
            if (path == string.Empty) return;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (story == string.Empty) story = Utils.ExtractStoryName(path);
            LogManager.Log("Exporting all templates for " + story + " to " + path);

            //export templates as hints.txt if we have the hints, no need to get filenames
            if (story == "Hints")
                ExportTemplate(Path.Combine(path, "Hints.txt"), story, story, confirmSuccess: false);
            else if (Directory.GetFiles(path).Length > 0)
                foreach (string file in Directory.GetFiles(path))
                {
                    ExportTemplate(file, story, warnOnOverwrite: true, confirmSuccess: false);
                }
            else if (DataBase.GetFilesForStory(story, out string[] names))
                foreach (string item in names)
                {
                    ExportTemplate(Path.Combine(path, item + ".txt"), story, item, confirmSuccess: false);
                }
            else
            {
                TabManager.UI.WarningOk("No templates found for that story, nothing exported.");
                LogManager.Log("\tNo templates found for that story");
                return;
            }

            TabManager.UI.InfoOk("Templates exported to " + path);
            LogManager.Log("\tSucessfully exported the templates");
        }

        public static void ExportTemplatesForStoryOrFile()
        {
            string path = Utils.SelectSaveLocation("Select a file or folder to export the templates to", checkFileExists: false, checkPathExists: false, extension: string.Empty);
            if (Path.GetExtension(path) != string.Empty) ExportTemplate(path);
            else ExportTemplatesForStory(path);
        }

        public static void ExportAllMissinglinesForStoryIntoFolder(string path, string story = "")
        {
            if (path == string.Empty)
                return;
            if (Path.GetExtension(path) == string.Empty)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else if (Directory.GetFiles(path).Length > 0)
                {
                    if (TabManager.UI.WarningYesNo("You are about to overwrite " + path + "\n Are you sure?", "Warning!", PopupResult.NO))
                        return;
                }
            }
            else
            {
                if (!File.Exists(path))
                    path = Path.GetFileNameWithoutExtension(path);
                else if (TabManager.UI.WarningYesNo("You are about to overwrite " + path + "\n Are you sure?", "Warning!", PopupResult.NO))
                    return;
            }

            if (story == string.Empty)
                story = Utils.ExtractStoryName(path);
            LogManager.Log("Exporting all missing lines for " + story + " to " + path);

            if (!DataBase.GetAllLinesAndTemplateForStory(story, TranslationManager.Language, out Dictionary<string, FileData> lines, out Dictionary<string, FileData> templates))
            {
                TabManager.UI.WarningOk("No lines found for that story/file, nothing exported.");
                LogManager.Log("    No lines found for that file");
                return;
            }

            SortAndWriteMissingLinesToDiskMultipleFiles(path, story, lines, templates);

            TabManager.UI.InfoOk("Missing lines exported to " + path);
            LogManager.Log("    Sucessfully exported missing lines");
        }

        public static void ExportAllMissinglinesForStoryIntoFile(string path, string story = "")
        {
            if (path == string.Empty)
                return;
            if (Path.GetExtension(path) == string.Empty)
            {
                TabManager.UI.WarningOk("Please provide a valid file, " + path + " was not a valid file", "Warning!");
                return;
            }
            else
            {
                if (!File.Exists(path))
                    File.OpenWrite(path).Close();
            }

            if (story == string.Empty)
                story = Utils.ExtractStoryName(path);

            LogManager.Log("Exporting all missing lines for " + story + " to " + path);

            if (!DataBase.GetAllLinesAndTemplateForStory(story, TranslationManager.Language, out Dictionary<string, FileData> lines, out Dictionary<string, FileData> templates))
            {
                TabManager.UI.WarningOk("No lines found for that story/file, nothing exported.");
                LogManager.Log("    No lines found for that file");
                return;
            }

            SortAndWriteMissingLinesToDiskSingleFile(path, story, lines, templates);

            TabManager.UI.InfoOk("Missing lines exported to " + path);
            LogManager.Log("    Sucessfully exported missing lines");
        }

        public static void ExportMissingLinesForFile(string path, string story = "", string file = "")
        {
            if (path == string.Empty)
                return;
            if (Path.GetExtension(path) == string.Empty)
            {
                TabManager.UI.WarningOk("Please provide a valid file, " + path + " was not a valid file", "Warning!");
                return;
            }
            else
            {
                if (!File.Exists(path))
                    File.OpenWrite(path).Close();
            }

            if (story == string.Empty)
                story = Utils.ExtractStoryName(path);
            if (file == string.Empty)
                file = Utils.ExtractFileName(path);

            LogManager.Log("Exporting all missing lines for " + story + "/" + file + " to " + path);

            if (!DataBase.GetAllLinesAndTemplateForFile(story, file, TranslationManager.Language, out FileData lines, out FileData templates))
            {
                TabManager.UI.WarningOk("No lines found for that story/file, nothing exported.");
                LogManager.Log("    No lines found for that file");
                return;
            }

            SortAndWriteMissingLinesToDisk(path, story, file, lines, templates);

            TabManager.UI.InfoOk("Missing lines exported to " + path);
            LogManager.Log("    Sucessfully exported missing lines");
        }

        public static void SaveTemplateDiff(FileData diff)
        {
            if (Settings.Default.ExportTemplateDiff)
            {
                string path = Utils.SelectSaveLocation("Select where you want to save the template diff to", file: diff.FileName + "_diff", checkFileExists: false);
                if (path == string.Empty)
                    return;
                if (!File.Exists(path))
                    File.OpenWrite(path).Close();
                else if (TabManager.UI.WarningYesNo("You are about to overwrite " + path + "\n Are you sure?", "Warning!", PopupResult.NO))
                    return;

                List<CategorizedLines> categories = InitializeCategories(diff.StoryName, diff.FileName);
                SortIntoCategories(ref categories, diff, diff);

                WriteCategorizedLinesToDisk(categories, path, true);
            }
        }

        private static void SortAndWriteMissingLinesToDiskMultipleFiles(string path, string story, Dictionary<string, FileData> lines, Dictionary<string, FileData> templates)
        {
            foreach (FileData fileData in templates.Values)
            {
                FileData results = new(story, fileData.FileName);
                //export templates as hints.txt if we have the hints, no need to get filenames
                string file = story == "Hints" ? "Hints" : fileData.FileName;
                CompareAndAggregateTranslationAndTemplate(lines[file], templates[file], ref results);

                //sort and save
                List<CategorizedLines> sortedLines = InitializeCategories(story, file);
                SortIntoCategories(ref sortedLines, results, results);

                WriteCategorizedLinesToDisk(sortedLines, Path.Combine(path, file + "_missing.txt"));
            }
        }

        private static void SortAndWriteMissingLinesToDiskSingleFile(string path, string story, Dictionary<string, FileData> lines, Dictionary<string, FileData> templates)
        {
            StreamWriter writer = new(path);
            foreach (FileData fileData in templates.Values)
            {
                FileData results = new(story, fileData.FileName);

                CompareAndAggregateTranslationAndTemplate(lines[fileData.FileName], templates[fileData.FileName], ref results);

                //sort and save
                List<CategorizedLines> sortedLines = InitializeCategories(story, fileData.FileName);
                SortIntoCategories(ref sortedLines, results, results);

                writer.WriteLine("FILE: " + fileData.FileName + ".txt starts here:\n\r");
                WriteCategorizedLinesToDisk(sortedLines, path, OutputWriter: writer);
            }
            writer.Dispose();
        }

        private static void SortAndWriteMissingLinesToDisk(string path, string story, string file, FileData lines, FileData templates)
        {
            FileData results = new(story, templates.FileName);
            CompareAndAggregateTranslationAndTemplate(lines, templates, ref results);

            //sort and save
            List<CategorizedLines> sortedLines = InitializeCategories(story, file);
            SortIntoCategories(ref sortedLines, results, results);

            WriteCategorizedLinesToDisk(sortedLines, path);
        }

        private static void CompareAndAggregateTranslationAndTemplate(FileData lines, FileData templates, ref FileData results)
        {
            foreach (LineData lineData in templates.Values)
            {
                lines.TryGetValue(lineData.ID, out LineData? translatedLineData);

                if (translatedLineData == null)
                {
                    results.Add(lineData.ID, lineData);
                    continue;
                }
                //skip line if its approved
                if (translatedLineData.IsApproved)
                {
                    continue;
                }
                //either not translated at all, or translated but its the same and its not approved.
                //this could happen when just clicking through lines in older versions. 
                else if (!translatedLineData.IsTranslated)
                {
                    results.Add(lineData.ID, lineData);
                }
                else if (translatedLineData.IsTranslated)
                {
                    if (translatedLineData.TranslationString == lineData.TemplateString)
                    {
                        results.Add(lineData.ID, lineData);
                    }
                    else if (Settings.Default.ExportTranslatedWithMissingLines)
                    {
                        lineData.TemplateString += " @@@TN: " + translatedLineData.TranslationString;
                        results.Add(lineData.ID, lineData);
                    }
                }
            }
        }

        public static void GenerateTemplateForAllFiles(bool SaveOnline = false)
        {
            PopupResult typeResult;
            if ((typeResult = TabManager.UI.InfoYesNoCancel($"You will now be prompted to select any file in the story folder you want to create the templates for. After that you must select the folder in which the translations will be placed.", "Create templates for a complete story")) != PopupResult.CANCEL)
            {
                //set up 
                string path = Utils.SelectFileFromSystem(false, "Select a file in the folder you want to create templates for", filter: "Character/Story files (*.character;*.story)|*.character;*.story");
                if (path.Length == 0) return;

                TabManager.UI.SignalUserWait();
                string story = Utils.ExtractStoryName(path);

                if (story.IsOfficialStory() && !Settings.Default.AdvancedModeEnabled) SaveOnline = false;
                LogManager.Log("creating templates for " + story);

                //create translation and open it
                string newFiles_dir = Directory.GetParent(Utils.SelectSaveLocation("Select the folder where you want the generated templates to go", path, "template export", string.Empty, false, false))?.FullName
                    ?? SpecialDirectories.MyDocuments;
                foreach (string file_path in Directory.GetFiles(Directory.GetParent(path)?.FullName ?? string.Empty))
                {
                    string file = Utils.ExtractFileName(file_path);
                    if (Path.GetExtension(file_path) is not ".character" and not ".story") continue;

                    //create and upload templates
                    if (TabManager.UI.CreateTemplateFromStory(story, file, file_path, out FileData templates))
                    {
                        if (templates.Count > 0 && SaveOnline)
                        {
                            _ = DataBase.UpdateTemplates(templates);
                        }
                        else if (SaveOnline)
                        {
                            _ = TabManager.UI.ErrorOk("No templates resulted from the generation, please try again.");
                            TabManager.UI.SignalUserEndWait();
                            return;
                        }
                    }
                    else
                    {
                        _ = TabManager.UI.ErrorOk("Templates were not created, please try again.");
                        TabManager.UI.SignalUserEndWait();
                        return;
                    }

                    if (newFiles_dir != string.Empty)
                    {
                        List<CategorizedLines> sortedLines = InitializeCategories(story, file);
                        SortIntoCategories(ref sortedLines, templates, templates);
                        WriteCategorizedLinesToDisk(sortedLines, Path.Combine(newFiles_dir, file + ".txt"));
                    }
                }

                LogManager.Log("successfully created templates");
                TabManager.UI.SignalUserEndWait();
            }
        }

        public static void GenerateTemplateForSingleFile(bool SaveOnline = false)
        {
            PopupResult typeResult;
            if ((typeResult = TabManager.UI.InfoYesNoCancel($"You will now be prompted to select the corresponding .story or .character file you want to create the template for. (Note: templates of official stories can only be created for local use)", "Create a template")) != PopupResult.CANCEL)
            {
                //set up 
                string path = Utils.SelectFileFromSystem(false, "Select the file to create the template for", filter: "Character/Story files (*.character;*.story)|*.character;*.story");
                if (path.Length == 0) return;

                TabManager.UI.SignalUserWait();
                (string story, string file) = Utils.ExtractFileAndStoryName(path);

                if (story.IsOfficialStory() && !Settings.Default.AdvancedModeEnabled) SaveOnline = false;

                LogManager.Log("creating template for " + story + "/" + file);
                //create and upload templates
                if (TabManager.UI.CreateTemplateFromStory(story, file, path, out FileData templates))
                {
                    if (templates.Count > 0 && SaveOnline)
                    {
                        _ = DataBase.UpdateTemplates(templates);
                    }
                    else if (SaveOnline)
                    {
                        _ = TabManager.UI.ErrorOk("No template resulted from the generation, please try again.");
                        TabManager.UI.SignalUserEndWait();
                        return;
                    }
                }
                else
                {
                    _ = TabManager.UI.ErrorOk("Template was not created, please try again.");
                    TabManager.UI.SignalUserEndWait();
                    return;
                }

                LogManager.Log("successfully created template");

                //create translation and open it
                string newFile = Utils.SelectSaveLocation("Select a file to save the generated templates to", path, file, "txt", false, false);
                if (newFile != string.Empty)
                {
                    List<CategorizedLines> sortedLines = InitializeCategories(story, file);
                    SortIntoCategories(ref sortedLines, templates, templates);

                    WriteCategorizedLinesToDisk(sortedLines, newFile);
                }
                TabManager.UI.SignalUserEndWait();
            }
        }

        /// <summary>
        /// Generates a list of all string categories depending on the filename.
        /// </summary>
        public static List<StringCategory> GetCategories(string story, string file)
        {
            return file == "all"
                ? new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.Dialogue,
                            StringCategory.Response,
                            StringCategory.Quest,
                            StringCategory.Event,
                            StringCategory.BGC,
                            StringCategory.ItemName,
                            StringCategory.ItemAction,
                            StringCategory.ItemGroupAction,
                            StringCategory.Achievement,
                            StringCategory.Neither }
                : story is "UI" or "Hints"
                    ? new List<StringCategory>() { StringCategory.General }
                    : file == story
                                    ? new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.ItemName,
                            StringCategory.ItemAction,
                            StringCategory.ItemGroupAction,
                            StringCategory.Event,
                            StringCategory.Achievement }
                                    : new List<StringCategory>() {
                            StringCategory.General,
                            StringCategory.Dialogue,
                            StringCategory.Response,
                            StringCategory.Quest,
                            StringCategory.Event,
                            StringCategory.BGC};
        }

        internal static List<CategorizedLines> InitializeCategories(string story, string file)
        {
            var CategorizedStrings = new List<CategorizedLines>();

            foreach (StringCategory category in GetCategories(story, file))
            {//add a list for every category we have in the file, so we can then add the strings to these.
                CategorizedStrings.Add((new List<LineData>(), category));
            }

            return CategorizedStrings;
        }

        internal static void SortIntoCategories(ref List<CategorizedLines> CategorizedStrings, FileData IdsToExport, FileData translationData)
        {
            foreach (LineData item in IdsToExport.Values)
            {
                if (item.ID == string.Empty) continue;
                if (translationData.TryGetValue(item.ID, out LineData? TempResult))
                {
                    if (TempResult != null)
                    {
                        item.TranslationString = TempResult.TranslationLength > 0 ? TempResult.TranslationString : item.TemplateString.RemoveVAHints();
                    }
                }
                else
                {
                    item.TranslationString = item.TemplateString.RemoveVAHints();
                }

                int intCategory = CategorizedStrings.FindIndex(predicateCategory => predicateCategory.category == item.Category);

                if (intCategory < CategorizedStrings.Count && intCategory >= 0)
                    CategorizedStrings[intCategory].lines.Add(item);
                else
                {
                    CategorizedStrings.Add((new List<LineData>(), item.Category));
                    CategorizedStrings[^1].lines.Add(item);
                }
            }
        }

        internal static void WriteCategorizedLinesToDisk(List<CategorizedLines> CategorizedStrings, string path, bool warnOnOverwrite = false, bool append = false, StreamWriter? OutputWriter = null)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool needDispose = false;

            if (OutputWriter == null)
            {
                if (warnOnOverwrite)
                    if (File.OpenRead(path).Length > 0)
                        if (TabManager.UI.WarningYesNo("You are about to overwrite " + path + " \nAre you sure?", "Warning", PopupResult.NO))
                            return;
                OutputWriter = new StreamWriter(path, append, new UTF8Encoding(true));
                needDispose = true;
            }
            foreach (CategorizedLines CategorizedLines in CategorizedStrings)
            {
                //write category if it has any lines, else we skip the category
                if (CategorizedLines.lines.Count > 0) OutputWriter.WriteLine(CategorizedLines.category.AsString());
                else continue;

                //sort strings depending on category
                if (CategorizedLines.category == StringCategory.Dialogue)
                {
                    CategorizedLines.lines.Sort((line1, line2) => decimal.Parse(line1.ID, culture).CompareTo(decimal.Parse(line2.ID, culture)));
                }
                else if (CategorizedLines.category == StringCategory.BGC)
                {
                    //sort using custom IComparer
                    CategorizedLines.lines.Sort(new BGCComparer());
                }
                else if (CategorizedLines.category == StringCategory.General)
                {
                    //hints have to be sortet a bit different because the numbers can contain a u
                    CategorizedLines.lines.Sort(new GeneralComparer());
                }
                else if (CategorizedLines.category is StringCategory.Quest or StringCategory.Achievement)
                {
                    CategorizedLines.lines.Sort((line1, line2) => line2.ID.CompareTo(line1.ID));
                }

                //iterate through each and print them
                foreach (LineData line in CategorizedLines.lines)
                {
                    OutputWriter.WriteLine(line.ToString().RemoveVAHints());
                }
                //newline after each category
                OutputWriter.WriteLine();
            }
            if (needDispose)
                OutputWriter.Dispose();
        }

        internal static void GenerateOfficialTemplates()
        {
            if (TabManager.UI.InfoYesNoCancel($"You will now be prompted to select any folder in the folder which contains all Official Stories and UI/Hints.", "Create templates for a official stories") != PopupResult.YES)
                return;

            //set up 
            string path = Utils.SelectTemplateFolderFromSystem();
            if (path.Length == 0) return;

            TabManager.UI.SignalUserWait();

            LogManager.Log("Creating official templates for version " + Settings.Default.FileVersion);

            //create translation and open it
            FileData templates;
            foreach (string folder_path in Directory.GetDirectories(Directory.GetParent(path)?.FullName ?? string.Empty))
            {
                string story = Utils.ExtractStoryName(folder_path);
                foreach (string file_path in Directory.GetFiles(folder_path))
                {
                    string file = Utils.ExtractFileName(file_path);
                    if (Path.GetExtension(file_path) != ".txt") continue;

                    //create and upload templates
                    templates = GetTemplateFromFile(file_path, story, file, false);
                    if (templates.Count > 0)
                    {
                        if (!DataBase.UpdateTemplates(templates))
                        {
                            _ = TabManager.UI.ErrorOk("New official templates were not uploaded, please try again.");
                            TabManager.UI.SignalUserEndWait();
                            return;
                        }
                        LogManager.Log($"Successfully read and uploaded templates for {story}/{file}");
                    }
                    else
                    {
                        _ = TabManager.UI.ErrorOk($"{story}/{file} contained no templates, skipping");
                    }
                }
            }
            DataBase.UpdateDBVersion();
            TabManager.UI.InfoOk("Successfully created and uploaded offical templates");
            LogManager.Log("Successfully created and uploaded offical templates");
            TabManager.UI.SignalUserEndWait();
        }

        /// <summary>
        /// tldr: magic
        ///
        /// loads all the strings from the selected file into a list of LineData elements.
        /// </summary>
        internal static FileData GetTemplateFromFile(string path, string story = "", string fileName = "", bool doIterNumbers = true)
        {
            if (Utils.ExtractFileName(path) != fileName)
            {
                _ = TabManager.UI.WarningOk("The template file must have the same name as the file you want to translate!");
                return new FileData(story, fileName);
            }
            if (story == string.Empty) story = Utils.ExtractStoryName(path);
            if (fileName == string.Empty) fileName = Utils.ExtractFileName(path);

            var fileData = new FileData(story, fileName);
            StringCategory currentCategory = StringCategory.General;
            string multiLineCollector = string.Empty;
            string[] lastLine = Array.Empty<string>();
            //string[] lastLastLine = { };
            //read in lines
            var LinesFromFile = new List<string>(File.ReadAllLines(path));
            //remove last if empty, breaks line loading for the last
            while (LinesFromFile[^1] == string.Empty) _ = LinesFromFile.Remove(LinesFromFile[^1]);
            //load lines and their data and split accordingly
            foreach (string line in LinesFromFile)
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : string.Empty + lastLine[0]] = new LineData(lastLine[0], story, fileName, currentCategory, lastLine[1] + multiLineCollector, true);

                    //get current line
                    lastLine = line.Split('|');

                    //reset multiline collector
                    multiLineCollector = string.Empty;
                }
                else
                {
                    StringCategory tempCategory = line.AsCategory();
                    if (tempCategory == StringCategory.Neither)
                    {
                        //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                        multiLineCollector += "\n" + line;
                    }
                    else
                    {
                        //if we reach a category, we can add the old string to the translation manager
                        if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : string.Empty + lastLine[0]] = new LineData(lastLine[0], story, fileName, currentCategory, lastLine[1] + multiLineCollector, true);
                        lastLine = Array.Empty<string>();
                        multiLineCollector = string.Empty;
                        currentCategory = tempCategory;
                    }
                }
            }
            //add last line (dont care about duplicates because sql will get rid of them)
            if (lastLine.Length != 0) fileData[doIterNumbers ? (++templateCounter).ToString() : string.Empty + lastLine[0]] = new LineData(lastLine[0], story, fileName, currentCategory, lastLine[1], true);

            return fileData;
        }

        public static string CreateNewFile(INewFileSelector dialog)
        {
            PopupResult result = dialog.ShowDialog();
            if (result != PopupResult.OK || dialog.StoryName == string.Empty) return string.Empty;

            string? path = Utils.SelectSaveLocation("Select a folder to place the file into, missing folders will be created.", file: dialog.StoryName, checkFileExists: false, checkPathExists: false, extension: string.Empty);
            if (path == string.Empty || path == null) return string.Empty;

            if (dialog.StoryName == path.Split('\\')[^2])
                path = Path.GetDirectoryName(path);
            else
                _ = Directory.CreateDirectory(path);

            if (dialog.FileName != string.Empty)
            {
                path = Path.Combine(path!, dialog.FileName + ".txt");
                File.OpenWrite(path!).Close();
            }
            return path!;
        }
    }
}

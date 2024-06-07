using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{

    /// <summary>
    /// loads all the strings from the selected file into a list of LineData elements.
    /// </summary>
    internal static class FileReader
    {
        private static bool triedFixingOnce = false;
        public static FileData GetTranslationsFromFile(TranslationManager manager)
        {
            FileData templates;
            if (DataBase.IsOnline)
            {
                _ = DataBase.GetAllLineDataTemplate(manager.FileName, manager.StoryName, out templates);
            }
            else
            {
                templates = GetTemplatesFromUser(manager);
            }
            List<string> LinesFromFile;
            try
            {
                //read in lines
                LinesFromFile = new List<string>(File.ReadAllLines(manager.SourceFilePath));
            }
            catch (Exception e)
            {
                LogManager.Log($"File not found under {manager.SourceFilePath}.\n{e}", LogManager.Level.Warning);
                _ = TranslationManager.UI.InfoOk($"File not found under {manager.SourceFilePath}. Please reopen.", "Invalid path");
                manager.Reset();
                return new(manager.StoryName, manager.FileName);
            }
            FileData translations = new(manager.StoryName, manager.FileName);
            //if we got lines at all
            if (LinesFromFile.Count > 0)
            {
                translations = SplitReadTranslations(LinesFromFile, templates);
            }
            else
            {
                TryFixEmptyFile(manager, ref translations);
            }

            if (templates.Count != translations.Count)
            {
                if (translations.Count == 0)
                {
                    TryFixEmptyFile(manager, ref translations);
                }
                else if (DataBase.IsOnline && !Settings.Default.IgnoreMissingLinesWarning)
                //inform user the issing translations will be added after export. i see no viable way to add them before having them all read in,
                //and it would take a lot o time to get them all. so just have the user save it once and reload. we might do this automatically, but i don'indexOfSelectedSearchResult know if that is ok to do :)
                {
                    _ = TranslationManager.UI.InfoOk(
                    "Some strings are missing from your translation, they will be added with the english version when you first save the file!",
                    "Some strings missing");
                    manager.ChangesPending = true;
                }
            }

            return translations;
        }

        private static FileData SplitReadTranslations(List<string> LinesFromFile, FileData IdsToImport)
        {
            //todo insert setting/toggle for what type of line we have
            var t = true;
            if (t)
                return ReadEekLines(LinesFromFile, IdsToImport);
            else if (t)
                return ReadCSVLines();
            else
                return ReadPythonStringLines();

        }

        //todo implement
        private static FileData ReadCSVLines() => throw new NotImplementedException();

        //todo maybe build into a generic one where you can just specify the file format
        // also find a way for custom dynamic categories
        private static FileData ReadPythonStringLines() => throw new NotImplementedException();

        private static FileData ReadEekLines(List<string> LinesFromFile, FileData IdsToImport)
        {
            string[] lastLine = Array.Empty<string>();
            string multiLineCollector = string.Empty;
            StringCategory category = StringCategory.General;
            FileData translations = new(IdsToImport.StoryName, IdsToImport.FileName);
            //remove last if empty, breaks line loading for the last
            while (LinesFromFile.Count > 0)
            {
                if (LinesFromFile[^1] == string.Empty)
                    LinesFromFile.RemoveAt(LinesFromFile.Count - 1);
                else break;
            }
            //load lines and their data and split accordingly
            foreach (string line in LinesFromFile)
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0)
                    {
                        multiLineCollector = multiLineCollector.TrimEnd(Extensions.trimmers);
                        CreateLineInTranslations(lastLine, category, IdsToImport, multiLineCollector, ref translations);
                    }
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
                        if (lastLine.Length != 0)
                        {

                            if (multiLineCollector.Length > 2)
                            {//write last string with id plus all lines after that minus the last new line char(s)
                                multiLineCollector = multiLineCollector.TrimEnd(Extensions.trimmers);
                                CreateLineInTranslations(lastLine, category, IdsToImport, multiLineCollector, ref translations);
                            }
                            else
                            {//write last line with id if no real line of text is afterwards
                                CreateLineInTranslations(lastLine, category, IdsToImport, string.Empty, ref translations);
                            }
                        }
                        //only set new category after we added the last line of the old one
                        category = tempCategory;
                        //resetting for next iteration
                        lastLine = Array.Empty<string>();
                        multiLineCollector = string.Empty;
                    }
                }
            }

            if (lastLine.Length > 0)
            {
                //add last line (dont care about duplicates because the dict)
                CreateLineInTranslations(lastLine, category, IdsToImport, string.Empty, ref translations);
            }

            return translations;
        }

        private static void TryFixEmptyFile(TranslationManager manager, ref FileData translations)
        {
            if (!triedFixingOnce)
            {
                triedFixingOnce = true;
                _ = DataBase.GetAllLineDataTemplate(translations.FileName, translations.StoryName, out FileData IdsToExport);
                foreach (LineData item in IdsToExport.Values)
                {
                    translations[item.EekID] = new LineData(
                        item.ID,
                        translations.StoryName,
                        translations.FileName,
                        item.Category,
                        translations.TryGetValue(item.EekID, out LineData? tempLineData) ?
                        (tempLineData?.TranslationLength > 0 ?
                        tempLineData?.Translation ?? item.Template.RemoveVAHints()
                        : item.Template.RemoveVAHints())
                        : item.Template.RemoveVAHints()
                        );
                }
                manager.SaveFile(false);
                translations.Clear();
                GetTranslationsFromFile(manager);
            }
        }

        private static FileData GetTemplatesFromUser(TranslationManager manager)
        {
            return TranslationManager.UI.InfoYesNo("Do you have the translation template from Don/Eek available? If so, we can use those if you hit yes, if you hit no we can generate templates from the game's story files.", "Templates available?", PopupResult.YES)
                ? SaveAndExportManager.GetTemplateFromFile(Utils.SelectFileFromSystem(false, $"Choose the template for {manager.StoryName}/{manager.FileName}.", manager.FileName + ".txt"), manager.StoryName, manager.FileName, false)
                : new FileData(manager.StoryName, manager.FileName);
        }

        //Creates the actual linedata objects from the file
        private static void CreateLineInTranslations(string[] lastLine, StringCategory category, FileData IdsToExport, string translation, ref FileData TranslationData)
        {
            if (lastLine[0] == string.Empty) return;
            var eekId = new EekStringID(lastLine[0], category);
            if (IdsToExport.TryGetValue(eekId, out LineData? templateLine))
                TranslationData[eekId] = new LineData(lastLine[0], IdsToExport.StoryName, IdsToExport.FileName, category, templateLine.Template, lastLine[1] + translation);
            else
            {
                TranslationData[eekId] = new LineData(lastLine[0], IdsToExport.StoryName, IdsToExport.FileName, category, string.Empty, lastLine[1] + translation);
            }
        }

    }
}

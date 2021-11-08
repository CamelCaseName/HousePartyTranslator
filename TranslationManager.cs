using System;
using System.Collections.Generic;
using System.IO;

public class TranslationManager
{
    public static TranslationManager main;
    public List<LineData> TranslationData = new List<LineData>();
    public bool IsUpToDate = false;

    public string SourceFilePath
    {
        get
        {
            return sourceFilePath;
        }
        set
        {
            if (sourceFilePath != "")
            {
                //TODO close opened file here
            }
            sourceFilePath = value;
            LoadSourceFile(value);
        }
    }
    private string sourceFilePath = "";
    public string TemplateFileString
    {
        get
        {
            return templateFileString;
        }
        set
        {
            templateFileString = value;
        }
    }
    private string templateFileString = "";
    public string TranslationFileString
    {
        get
        {
            return translationFileString;
        }
        set
        {
            translationFileString = value;
        }
    }
    private string translationFileString = "";

    public string FileName
    {
        get
        {
            return fileName;
        }
        set
        {
            fileName = value;
        }
    }
    private string fileName = "";

    public string StoryName
    {
        get
        {
            return storyName;
        }
        set
        {
            storyName = value;
        }
    }
    private string storyName = "";


    public TranslationManager()
    {
        if (main != null)
        {
            return;
        }
        main = this;
    }

    private void LoadSourceFile(string path)
    {
        string folderPath = Path.GetDirectoryName(path);
        FileName = Path.GetFileNameWithoutExtension(path);
    }
}

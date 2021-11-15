using HousePartyTranslator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class TranslationManager
{
    public static TranslationManager main;
    public List<LineData> TranslationData = new List<LineData>();
    public bool IsUpToDate = false;
    public bool isTemplate = false;

    public string Language
    {
        get
        {
            if (language.Length == 0)
            {
                MessageBox.Show("Please enter a valid language or select one.", "Enter valid language");
            }
            return language;
        }
        set
        {
            language = value;
        }
    }
    private string language = "";

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

    public void UpdateTranslationString(TextBox EditorTextBox, CheckedListBox CheckedListBoxLeft)
    {
        int internalIndex = CheckedListBoxLeft.SelectedIndex;
        if (internalIndex >= 0)
        {
            TranslationData[internalIndex].TranslationString = EditorTextBox.Text;
        }
    }

    public void LoadFileIntoProgram(CheckedListBox CheckedListBoxLeft, Label SelectedFile)
    {
        TranslationData.Clear();
        CheckedListBoxLeft.Items.Clear();

        CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
        if (IsUpToDate)
        {
            SourceFilePath = SelectFileFromSystem();
            if (SourceFilePath != "")
            {
                
                string[] paths = SourceFilePath.Split('\\');
                //get parent folder name
                StoryName = paths[paths.Length - 2];
                ReadStringsFromFile();

                SelectedFile.Text = "File: " + FileName + ".txt";

                //is up to date, so we can start translation
                HandleTranslationLoading(CheckedListBoxLeft);
            }
        }
        else
        {
            string folderPath = SelectFolderFromSystem();
            string templateFolderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];
            if (templateFolderName == "TEMPLATE")
            {
                isTemplate = true;
                HandleTemplateLoading(folderPath);
            }
            else
            {
                isTemplate = false;
                MessageBox.Show(
                    $"Please the template folder named 'TEMPLATE' so we can upload them. " +
                    $"Your Current Folder shows as {templateFolderName}.",
                    "Updating string database"
                    );
            }
        }

        CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
    }

    public void PopulateTextBoxes(CheckedListBox CheckedListBoxLeft, TextBox TextBoxReadOnly, TextBox TextBoxEditable)
    {
        int currentIndex = CheckedListBoxLeft.SelectedIndex;
        if (currentIndex >= 0)
        {
            if (isTemplate)
            {
                TextBoxReadOnly.Text = TranslationData[currentIndex].EnglishString.Replace("\n", Environment.NewLine);
            }
            else
            {
                TextBoxEditable.Text = TranslationData[currentIndex].TranslationString.Replace("\n", Environment.NewLine); ;
                if (ProofreadDB.GetStringTemplate(TranslationData[currentIndex].ID, FileName, StoryName, out string templateString))
                {
                    TextBoxReadOnly.Text = templateString.Replace("\n", Environment.NewLine);
                }
            }
        }
    }

    public void SetLanguage(ComboBox LanguageBox)
    {
        if (LanguageBox.FindStringExact(LanguageBox.Text) > -1)
        {
            Language = LanguageBox.Text;
        }
        else if (LanguageBox.SelectedIndex > -1)
        {
            Language = LanguageBox.GetItemText(LanguageBox.SelectedItem);
        }
        _ = Language;
    }

    public void ApproveIfPossible(CheckedListBox CheckedListBoxLeft)
    {
        int currentIndex = CheckedListBoxLeft.SelectedIndex;
        if (currentIndex >= 0)
        {
            string ID = TranslationData[currentIndex].ID;
            ProofreadDB.SetStringTranslation(ID, FileName, StoryName, TranslationData[currentIndex].Category, TranslationData[currentIndex].TranslationString, main.Language);
            if (!ProofreadDB.SetStringApprovedState(ID, FileName, StoryName, TranslationData[currentIndex].Category, !CheckedListBoxLeft.GetItemChecked(currentIndex), main.Language))
            {
                MessageBox.Show("Could not set approved state of string " + ID);
            }
        }
    }

    public void SaveFile(CheckedListBox CheckedListBoxLeft)
    {
        //IMPORTANT remove all \r so only \n!!
        CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;



        CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
    }

    private void ReadStringsFromFile()
    {
        //read in all strings with IDs
        StringCategory internalCategory = StringCategory.General;
        if (isTemplate)
        {
            string multiLineCollector = "";
            string[] lastLine = { };
            foreach (string line in File.ReadAllLines(SourceFilePath))
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, internalCategory, lastLine[1] + multiLineCollector, true));

                    //get current line
                    lastLine = line.Split('|');

                    //reset multiline collector
                    multiLineCollector = "";
                }
                else
                {
                    StringCategory tempCategory = getStringCategory(line);
                    if (tempCategory == StringCategory.Neither)
                    {
                        //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                        multiLineCollector += "\n" + line;
                    }
                    else
                    {
                        //if we reach a category, we can add the old string to the translation manager
                        if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, internalCategory, lastLine[1] + multiLineCollector, true));

                        multiLineCollector = "";
                        internalCategory = tempCategory;
                    }
                }
            }
            //add last line (if it does not exist)
            if (!TranslationData.Exists(predicateLine => predicateLine.ID == lastLine[0]))
            {
                if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, internalCategory, lastLine[1], true));
            }
        }
        else //read in translations
        {
            string multiLineCollector = "";
            string[] lastLine = { };
            foreach (string line in File.ReadAllLines(SourceFilePath))
            {
                if (line.Contains('|'))
                {
                    //if we reach a new id, we can add the old string to the translation manager
                    if(lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, internalCategory, lastLine[1] + multiLineCollector));

                    //get current line
                    lastLine = line.Split('|');

                    //reset multiline collector
                    multiLineCollector = "";
                }
                else
                {
                    StringCategory tempCategory = getStringCategory(line);
                    if (tempCategory == StringCategory.Neither)
                    {
                        //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                        multiLineCollector += "\n" + line;
                    }
                    else
                    {
                        //if we reach a category, we can add the old string to the translation manager
                        if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, internalCategory, lastLine[1] + multiLineCollector));

                        multiLineCollector = "";
                        internalCategory = tempCategory;
                    }
                }
            }
            //add last line (if it does not exist)
            if (!TranslationData.Exists(predicateLine => predicateLine.ID == lastLine[0]))
            {
                if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, internalCategory, lastLine[1]));
            }
            TranslationData.RemoveAt(0);
        }
    }

    private void HandleTranslationLoading(CheckedListBox CheckedListBoxLeft)
    {
        CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
        HandleTranslationApprovalLoading(CheckedListBoxLeft);
        CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
    }

    private void HandleTemplateLoading(string folderPath)
    {
        //upload all new strings
        try
        {
            DirectoryInfo templateDir = new DirectoryInfo(folderPath);
            if (templateDir != null)
            {
                DirectoryInfo[] storyDirs = templateDir.GetDirectories();
                if (storyDirs != null)
                {
                    foreach (DirectoryInfo storyDir in storyDirs)
                    {
                        if (storyDir != null)
                        {
                            StoryName = storyDir.Name;
                            FileInfo[] templateFiles = storyDir.GetFiles();
                            if (templateFiles != null)
                            {
                                foreach (FileInfo templateFile in templateFiles)
                                {
                                    if (templateFile != null)
                                    {
                                        SourceFilePath = templateFile.FullName;
                                        FileName = Path.GetFileNameWithoutExtension(SourceFilePath);
                                        ReadStringsFromFile();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (UnauthorizedAccessException e)
        {
            MessageBox.Show("Encountered " + e.Message);
        }

        MessageBox.Show(
            "This is going to take some time(15+ minutes), get a tea or sth.\n" +
            "A Message will appear once it is done.\n" +
            "Click OK if you want to start.",
            "Updating string database...",
            MessageBoxButtons.OK,
            MessageBoxIcon.Asterisk
            );

        //clear db of all old strings
        //if (ProofreadDB.ClearDBofAllStrings())
        //{
        //    Console.WriteLine("Cleared old strings");
        //}

        //add all the new strings
        foreach (LineData lineD in TranslationData)
        {
            ProofreadDB.SetStringTemplate(lineD.ID, lineD.FileName, lineD.Story, lineD.Category, lineD.EnglishString);
        }

        DialogResult result = MessageBox.Show(
            "This should be done uploading. It should be :)\n" +
            "If you are done uploading, please select YES. ELSE NO. Be wise please!",
            "Updating string database...",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2
            );

        //update if successfull
        if (result == DialogResult.Yes)
        {
            if (ProofreadDB.UpdateDBVersion())
            {
                IsUpToDate = true;
                isTemplate = false;
            }
        }
        else if (result == DialogResult.Cancel)
        {
            Application.Exit();
        }
    }

    private StringCategory getStringCategory(string line)
    {
        StringCategory internalCategory = StringCategory.Neither;
        if (line.Contains("["))
        {
            if (line == "[General]")
            {
                internalCategory = StringCategory.General;
            }
            else if (line == "[Dialogues]")
            {
                internalCategory = StringCategory.Dialogue;
            }
            else if (line == "[Responses]")
            {
                internalCategory = StringCategory.Response;
            }
            else if (line == "[Quests]")
            {
                internalCategory = StringCategory.Quest;
            }
            else if (line == "[Events]")
            {
                internalCategory = StringCategory.Event;
            }
            else if (line == "[Background Chatter]")
            {
                internalCategory = StringCategory.BGC;
            }
            else if (line == "[Item Names]")
            {
                internalCategory = StringCategory.ItemName;
            }
            else if (line == "[Item Actions]")
            {
                internalCategory = StringCategory.ItemAction;
            }
            else if (line == "[Item Group Actions]")
            {
                internalCategory = StringCategory.ItemGroupAction;
            }
            else if (line == "[Achievements]")
            {
                internalCategory = StringCategory.Achievement;
            }
        }
        return internalCategory;
    }

    private void LoadSourceFile(string path)
    {
        FileName = Path.GetFileNameWithoutExtension(path);
    }

    private static void HandleTranslationApprovalLoading(CheckedListBox CheckedListBoxLeft)
    {
        bool lineIsApproved = false;
        bool gotApprovedStates = ProofreadDB.GetAllApprovalStatesForFile(main.FileName, main.StoryName, out List<LineData> internalLines, main.Language);

        foreach (LineData lineD in main.TranslationData)
        {
            if (gotApprovedStates)
            {
                lineIsApproved = internalLines.Exists(predicateLine => predicateLine.ID == lineD.ID);
            }

            CheckedListBoxLeft.Items.Add(lineD.ID, lineIsApproved);
            lineD.IsApproved = lineIsApproved;
        }
    }

    public static string SelectFileFromSystem()
    {
        OpenFileDialog selectFileDialog = new OpenFileDialog
        {
            Title = "Choose a file for translation",
            Filter = "Text files (*.txt)|*.txt",
            InitialDirectory = @"C:\Users\%USER%\Documents"
        };

        if (selectFileDialog.ShowDialog() == DialogResult.OK)
        {
            return selectFileDialog.FileName;
        }
        return "";
    }

    public static string SelectFolderFromSystem()
    {
        FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog
        {
            Description = "Please select the 'TEMPLATE' folder like in the repo",
            RootFolder = Environment.SpecialFolder.MyComputer
        };

        if (selectFolderDialog.ShowDialog() == DialogResult.OK)
        {
            return selectFolderDialog.SelectedPath;
        }
        return "";
    }
}

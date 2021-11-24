using HousePartyTranslator;
using HousePartyTranslator.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class TranslationManager
{
    public static TranslationManager main;
    public List<LineData> TranslationData = new List<LineData>();
    public List<StringCategory> CategoriesInFile = new List<StringCategory>();
    public bool IsUpToDate = false;
    public bool isTemplate = false;
    public bool AutoSave = true;
    private int LastIndex = -1;
    private bool isSaveAs = false;

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
            ((StringSetting)SettingsManager.main.Settings.Find(pS => pS.GetKey() == "language")).UpdateValue(language);
            SettingsManager.main.UpdateSettings();
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
            if (!isSaveAs) FileName = Path.GetFileNameWithoutExtension(value);
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

    public void UpdateTranslationString(TextBox EditorTextBox, TextBox TemplateTextBox, CheckedListBox CheckedListBoxLeft, Label CharacterCountLabel)
    {
        int internalIndex = CheckedListBoxLeft.SelectedIndex;
        if (internalIndex >= 0)
        {
            TranslationData[internalIndex].TranslationString = EditorTextBox.Text.Replace(Environment.NewLine, "\n");
            UpdateCharacterCountLabel(TemplateTextBox.Text.Count(), EditorTextBox.Text.Count(), CharacterCountLabel);
        }
    }

    public void LoadFileIntoProgram(CheckedListBox checkedListBoxLeft, Label SelectedFile)
    {
        TranslationData.Clear();
        checkedListBoxLeft.Items.Clear();
        CategoriesInFile.Clear();
        LastIndex = -1;

        checkedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
        if (IsUpToDate)
        {
            SourceFilePath = SelectFileFromSystem();
            if (SourceFilePath != "")
            {

                string[] paths = SourceFilePath.Split('\\');
                //get parent folder name
                StoryName = paths[paths.Length - 2];
                HandleStringReadingFromFile();

                SelectedFile.Text = "File: " + FileName + ".txt";

                //is up to date, so we can start translation
                HandleTranslationLoading(checkedListBoxLeft);
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

        checkedListBoxLeft.FindForm().Cursor = Cursors.Default;
    }

    public void PopulateTextBoxes(CheckedListBox CheckedListBoxLeft, TextBox TextBoxReadOnly, TextBox TextBoxEditable, TextBox CommentBox, Label CharacterCountLabel, Label ApprovedStringLabel)
    {
        TextBoxReadOnly.FindForm().Cursor = Cursors.WaitCursor;
        int currentIndex = CheckedListBoxLeft.SelectedIndex;
        if (LastIndex < 0)
        {
            //sets index the first time/when we click elsewhere
            LastIndex = currentIndex;
        }
        else
        {
            //if we changed the eselction and have autsave enabled
            if (LastIndex != currentIndex && AutoSave)
            {

                //update translation in the database
                DataBaseManager.SetStringTranslation(
                    TranslationData[LastIndex].ID,
                    FileName,
                    StoryName,
                    TranslationData[LastIndex].Category,
                    TranslationData[LastIndex].TranslationString,
                    Language);

                //upload comment on change
                DataBaseManager.SetTranslationComments(
                    TranslationData[LastIndex].ID,
                    FileName,
                    StoryName,
                    CommentBox.Lines,
                    Language
                    );

                LastIndex = currentIndex;

                //clear comment after save
                CommentBox.Lines = new string[0];
            }
        }

        if (currentIndex >= 0)
        {
            if (isTemplate)
            {
                TextBoxReadOnly.Text = TranslationData[currentIndex].EnglishString.Replace("\n", Environment.NewLine);
            }
            else
            {
                string id = TranslationData[currentIndex].ID;
                //read latest version from the database
                if (DataBaseManager.GetStringTranslation(id, FileName, StoryName, out string translation, Language))
                {
                    //replace older one in file by new one from database
                    TranslationData[currentIndex].TranslationString = translation;
                }
                //display the string in the editable window
                TextBoxEditable.Text = TranslationData[currentIndex].TranslationString.Replace("\n", Environment.NewLine); ;
                if (DataBaseManager.GetStringTemplate(id, FileName, StoryName, out string templateString))
                {
                    //read the template form the db and display it if it exists
                    TextBoxReadOnly.Text = templateString.Replace("\n", Environment.NewLine);
                }
                if (DataBaseManager.GetTranslationComments(id, FileName, StoryName, out string[] comments, Language))
                {
                    CommentBox.Lines = comments;
                }

                UpdateCharacterCountLabel(TextBoxReadOnly.Text.Count(), TextBoxEditable.Text.Count(), CharacterCountLabel);
                UpdateApprovedCountLabel(CheckedListBoxLeft.CheckedIndices.Count, CheckedListBoxLeft.Items.Count, ApprovedStringLabel);
            }
        }
        TextBoxReadOnly.FindForm().Cursor = Cursors.Default;
    }

    public void SaveCurrentString(CheckedListBox CheckedListBoxLeft)
    {
        int currentIndex = CheckedListBoxLeft.SelectedIndex;

        //if we changed the eselction and have autsave enabled
        if (currentIndex >= 0)
        {
            CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

            //update translation in the database
            DataBaseManager.SetStringTranslation(
                TranslationData[LastIndex].ID,
                FileName,
                StoryName,
                TranslationData[LastIndex].Category,
                TranslationData[LastIndex].TranslationString,
                Language);

            CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
        }
    }

    public void SaveCurrentComment(CheckedListBox CheckedListBoxLeft, TextBox CommentBox)
    {
        int currentIndex = CheckedListBoxLeft.SelectedIndex;

        //if we changed the eselction and have autsave enabled
        if (currentIndex >= 0)
        {
            CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

            //upload comment
            DataBaseManager.SetTranslationComments(
                TranslationData[currentIndex].ID,
                FileName,
                StoryName,
                CommentBox.Lines,
                Language
                );

            CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
        }
    }

    public void SetLanguage(ComboBox LanguageBox)
    {
        if (LanguageBox.SelectedIndex > -1)
        {
            Language = LanguageBox.GetItemText(LanguageBox.SelectedItem);
        }
        else if (SettingsManager.main.Settings.Exists(pS => pS.GetKey() == "language"))
        {
            string languageFromFile = ((StringSetting)SettingsManager.main.Settings.Find(pS => pS.GetKey() == "language")).GetValue();
            if (languageFromFile != "")
            {
                Language = languageFromFile;
                LanguageBox.Text = Language;
            }
        }
        LanguageBox.Text = Language;
    }

    public void ApproveIfPossible(CheckedListBox CheckedListBoxLeft, Label ApprovedCountLabel)
    {
        int currentIndex = CheckedListBoxLeft.SelectedIndex;
        if (currentIndex >= 0)
        {
            string ID = TranslationData[currentIndex].ID;
            DataBaseManager.SetStringTranslation(ID, FileName, StoryName, TranslationData[currentIndex].Category, TranslationData[currentIndex].TranslationString, main.Language);
            if (!DataBaseManager.SetStringApprovedState(ID, FileName, StoryName, TranslationData[currentIndex].Category, !CheckedListBoxLeft.GetItemChecked(currentIndex), main.Language))
            {
                MessageBox.Show("Could not set approved state of string " + ID);
            }
            if (!CheckedListBoxLeft.GetItemChecked(currentIndex))
            {
                if (currentIndex < CheckedListBoxLeft.Items.Count - 1) CheckedListBoxLeft.SelectedIndex = currentIndex + 1;
            }

            UpdateApprovedCountLabel(CheckedListBoxLeft.CheckedIndices.Count, CheckedListBoxLeft.Items.Count, ApprovedCountLabel);
        }
    }

    public void SaveFile(CheckedListBox CheckedListBoxLeft)
    {
        if (SourceFilePath != "")
        {
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
            CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
            List<Tuple<List<LineData>, StringCategory>> CategorizedStrings = new List<Tuple<List<LineData>, StringCategory>>();

            foreach (StringCategory category in CategoriesInFile)
            {//add a list for every category we have in the file, so we can then add the strings to these.
                CategorizedStrings.Add(new Tuple<List<LineData>, StringCategory>(new List<LineData>(), category));
            }

            StreamWriter OutputWriter = new StreamWriter(SourceFilePath);

            DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);

            foreach (LineData item in IdsToExport)
            {
                LineData lineDataResult = TranslationData.Find(predicateLine => predicateLine.ID == item.ID);
                if (lineDataResult != null)
                {
                    //add translation to the list in the correct category if present
                    int intCategory = CategoriesInFile.FindIndex(predicateCategory => predicateCategory == item.Category);
                    CategorizedStrings[intCategory].Item1.Add(lineDataResult);
                }
                else// if id is not found
                {
                    //add template to list if no translation is in the file
                    DataBaseManager.GetStringTemplate(item.ID, FileName, StoryName, out string templateString);
                    item.TranslationString = templateString;
                    int intCategory = CategoriesInFile.FindIndex(predicateCategory => predicateCategory == item.Category);
                    CategorizedStrings[intCategory].Item1.Add(item);
                }
            }

            foreach (Tuple<List<LineData>, StringCategory> CategorizedLines in CategorizedStrings)
            {
                //write category 
                OutputWriter.WriteLine(GetStringFromCategory(CategorizedLines.Item2));

                //sort strings depending on category
                if (CategorizedLines.Item2 == StringCategory.Dialogue)
                {
                    CategorizedLines.Item1.Sort((line1, line2) => decimal.Parse(line1.ID, culture).CompareTo(decimal.Parse(line2.ID, culture)));
                }
                else if (CategorizedLines.Item2 == StringCategory.BGC)
                {
                    //sort using custom IComparer
                    CategorizedLines.Item1.Sort(new BGCComparer());
                }
                else if (CategorizedLines.Item2 == StringCategory.General)
                {
                    //hints have to be sortet a bit different because the numbers can contain a u
                    CategorizedLines.Item1.Sort(new GeneralComparer());
                }
                else if (CategorizedLines.Item2 == StringCategory.Quest || CategorizedLines.Item2 == StringCategory.Achievement)
                {
                    CategorizedLines.Item1.Sort((line1, line2) => line2.ID.CompareTo(line1.ID));
                }

                //iterate through each and print them
                foreach (LineData line in CategorizedLines.Item1)
                {
                    OutputWriter.WriteLine(line.ToString());
                }
                //newline after each category
                OutputWriter.WriteLine();
            }

            OutputWriter.Close();

            CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;

        }
    }

    public void SaveFileAs(CheckedListBox CheckedListBoxLeft)
    {
        if (SourceFilePath != "")
        {
            isSaveAs = true;
            string oldFile = main.SourceFilePath;
            string SaveFile = SaveFileOnSystem();
            main.SourceFilePath = SaveFile;
            main.SaveFile(CheckedListBoxLeft);
            main.SourceFilePath = oldFile;
            isSaveAs = false;
        }
    }
    private void UpdateCharacterCountLabel(int TemplateCount, int TranslationCount, Label CharacterCountLabel)
    {
        if (TemplateCount >= TranslationCount)
        {
            CharacterCountLabel.ForeColor = System.Drawing.Color.LawnGreen;
        }//if bigger by no more than 20 percent
        else if ((TranslationCount - TemplateCount) < (TemplateCount / 20))
        {
            CharacterCountLabel.ForeColor = System.Drawing.Color.DarkOrange;
        }
        else
        {
            CharacterCountLabel.ForeColor = System.Drawing.Color.Red;
        }
        CharacterCountLabel.Text = $"Template: {TemplateCount} | Translation: {TranslationCount}";
    }

    private void UpdateApprovedCountLabel(int Approved, int Total, Label ApprovedCountLabel)
    {
        ApprovedCountLabel.Text = $"Approved: {Approved} / {Total}";
    }

    private void HandleStringReadingFromFile()
    {
        //read in all strings with IDs
        if (isTemplate)//read in templates
        {
            ReadStringsTemplateFromFile();
        }
        else //read in translations
        {
            ReadStringsTranslationsFromFile();
        }
    }

    private void ReadStringsTemplateFromFile()
    {
        StringCategory currentCategory = StringCategory.General;
        string multiLineCollector = "";
        string[] lastLine = { };
        foreach (string line in File.ReadAllLines(SourceFilePath))
        {
            if (line.Contains('|'))
            {
                //if we reach a new id, we can add the old string to the translation manager
                if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true));

                //get current line
                lastLine = line.Split('|');

                //reset multiline collector
                multiLineCollector = "";
            }
            else
            {
                StringCategory tempCategory = GetCategoryFromString(line);
                if (tempCategory == StringCategory.Neither)
                {
                    //line is part of a multiline, add to collector (we need newline because they get removed by ReadAllLines)
                    multiLineCollector += "\n" + line;
                }
                else
                {
                    //if we reach a category, we can add the old string to the translation manager
                    if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector, true));

                    multiLineCollector = "";
                    currentCategory = tempCategory;
                }
            }
        }
        //add last line (if it does not exist)
        if (!TranslationData.Exists(predicateLine => predicateLine.ID == lastLine[0]))
        {
            if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1], true));
        }
    }

    private void ReadStringsTranslationsFromFile()
    {
        StringCategory currentCategory = StringCategory.General;
        string multiLineCollector = "";
        string[] lastLine = { };

        DataBaseManager.GetAllLineDataBasicForFile(FileName, StoryName, out List<LineData> IdsToExport);

        foreach (string line in File.ReadAllLines(SourceFilePath))
        {
            if (line.Contains('|'))
            {
                //if we reach a new id, we can add the old string to the translation manager
                if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1] + multiLineCollector));

                //get current line
                lastLine = line.Split('|');

                //reset multiline collector
                multiLineCollector = "";
            }
            else
            {
                StringCategory tempCategory = GetCategoryFromString(line);
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
                        {//write last string with id plus all lines after that minus the last new line char
                            TranslationData.Add(
                              new LineData(
                                  lastLine[0],
                                  StoryName,
                                  FileName,
                                  currentCategory,
                                  lastLine[1] + multiLineCollector.Remove(multiLineCollector.Length - 2, 1)));
                        }
                        else
                        {//write last line with id if no real line of text is afterwards
                            TranslationData.Add(
                              new LineData(
                                  lastLine[0],
                                  StoryName,
                                  FileName,
                                  currentCategory,
                                  lastLine[1]));
                        }
                    }
                    lastLine = new string[0];
                    multiLineCollector = "";
                    currentCategory = tempCategory;
                    CategoriesInFile.Add(currentCategory);
                }
            }
        }

        if (lastLine.Length > 0)
        {
            //add last line (if it does not exist)
            if (!TranslationData.Exists(predicateLine => predicateLine.ID == lastLine[0]))
            {
                if (lastLine.Length != 0) TranslationData.Add(new LineData(lastLine[0], StoryName, FileName, currentCategory, lastLine[1]));
            }
        }

        //set categories if file is a hint file
        if (StoryName == "Hints") CategoriesInFile = new List<StringCategory>() { StringCategory.General };

        if (IdsToExport.Count != TranslationData.Count)
        {//inform user the issing translations will be added after export. i see no viable way to add them before having them all read in,
         //and it would take a lot o time to get them all. so just have the user save it once and reload. we might do this automatically, but i don't know if that is ok to do :)
            MessageBox.Show(
                @"Some strings are missing from your translation, they will be added with the english version when you first save the file!",
                "Some strings missing",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk);
            /*DialogResult ReloadResult = MessageBox.Show(
                @"Some strings are missing from your translation, they will be added with the english version when you first save the file!

                If you select YES, the application will autosave the file and reload it automatically.
                If you select NO, it will do nothing right now, but add them when you save manually.", 
                "Some strings missing", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Asterisk);

            if(ReloadResult == DialogResult.Yes)
            {
                SaveFile(checkedListBoxLeft);
                ReadStringsTranslationsFromFile(checkedListBoxLeft);
            }*/
        }
    }

    private void HandleTranslationLoading(CheckedListBox CheckedListBoxLeft)
    {
        CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;

        bool lineIsApproved = false;
        bool gotApprovedStates = DataBaseManager.GetAllApprovalStatesForFile(main.FileName, main.StoryName, out List<LineData> internalLines, main.Language);

        foreach (LineData lineD in main.TranslationData)
        {
            if (gotApprovedStates)
            {
                LineData tempLine = internalLines.Find(predicateLine => predicateLine.ID == lineD.ID);
                if (tempLine != null) lineIsApproved = tempLine.IsApproved;
            }

            CheckedListBoxLeft.Items.Add(lineD.ID, lineIsApproved);
            lineD.IsApproved = lineIsApproved;
        }
        CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
    }

    private void HandleTemplateLoading(string folderPath)
    {
        //upload all new strings
        try
        {
            IterativeReadFiles(folderPath);
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

        //add all the new strings
        foreach (LineData lineD in TranslationData)
        {
            DataBaseManager.SetStringTemplate(lineD.ID, lineD.FileName, lineD.Story, lineD.Category, lineD.EnglishString);
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
            if (DataBaseManager.UpdateDBVersion())
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

    private void IterativeReadFiles(string folderPath)
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
                                    HandleStringReadingFromFile();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private StringCategory GetCategoryFromString(string line)
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

    private string GetStringFromCategory(StringCategory category)
    {
        string returnedString = "";
        switch (category)
        {
            case StringCategory.General:
                returnedString = "[General]";
                break;
            case StringCategory.Dialogue:
                returnedString = "[Dialogues]";
                break;
            case StringCategory.Response:
                returnedString = "[Responses]";
                break;
            case StringCategory.Quest:
                returnedString = "[Quests]";
                break;
            case StringCategory.Event:
                returnedString = "[Events]";
                break;
            case StringCategory.BGC:
                returnedString = "[Background Chatter]";
                break;
            case StringCategory.ItemName:
                returnedString = "[Item Names]";
                break;
            case StringCategory.ItemAction:
                returnedString = "[Item Actions]";
                break;
            case StringCategory.ItemGroupAction:
                returnedString = "[Item Group Actions]";
                break;
            case StringCategory.Achievement:
                returnedString = "[Achievements]";
                break;
            case StringCategory.Neither:
                //do nothing hehehehe
                break;
            default:
                //do nothing hehehehe
                break;
        }
        return returnedString;
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

    public static string SaveFileOnSystem()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Choose a file to save the translations to",
            AddExtension = true,
            DefaultExt = "txt",
            CheckPathExists = true,
            CreatePrompt = true,
            OverwritePrompt = true,
            FileName = main.FileName,
            InitialDirectory = main.SourceFilePath
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            return saveFileDialog.FileName;
        }
        return "";
    }
}

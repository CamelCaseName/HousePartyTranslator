using HousePartyTranslator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

public class TranslationManager
{
    public static TranslationManager main;
    public List<LineData> TranslationData = new List<LineData>();
    public bool IsUpToDate = false;
    public bool isTemplate = false;

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

    public void LoadFileIntoProgram(CheckedListBox CheckedListBoxLeft)
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
                TextBoxReadOnly.Text = TranslationData[currentIndex].EnglishString;
            }
            else
            {
                TextBoxEditable.Text = TranslationData[currentIndex].TranslationString;
                if (ProofreadDB.GetStringTemplate(TranslationData[currentIndex].ID, FileName, StoryName, out string templateString))
                {
                    TextBoxReadOnly.Text = templateString;
                }
            }
        }
    }

    public void ApproveIfPossible(CheckedListBox CheckedListBoxLeft)
    {
        int currentIndex = CheckedListBoxLeft.SelectedIndex;
        if (currentIndex >= 0)
        {
            string ID = TranslationData[currentIndex].ID;
            ProofreadDB.SetStringTranslation(ID, FileName, StoryName, TranslationData[currentIndex].TranslationString, "de");
            if (!ProofreadDB.SetStringApprovedState(ID, FileName, StoryName, !CheckedListBoxLeft.GetItemChecked(currentIndex), "de"))
            {
                MessageBox.Show("Could not set approved state of string " + ID);
            }
        }
    }

    public void SaveFile(CheckedListBox CheckedListBoxLeft)
    {
        CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;




        CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
    }

    private void ReadStringsFromFile()
    {
        //read in all strings with IDs
        if (isTemplate)
        {
            foreach (string line in File.ReadAllLines(SourceFilePath))
            {
                if (line.Contains('|'))
                {
                    string[] Splitted = line.Split('|');
                    TranslationData.Add(new LineData(Splitted[0], StoryName, FileName, Splitted[1], true));
                }
            }
        }
        else //read in translations
        {
            foreach (string line in File.ReadAllLines(SourceFilePath))
            {
                if (line.Contains('|'))
                {
                    string[] Splitted = line.Split('|');
                    //add new translation
                    TranslationData.Add(new LineData(Splitted[0], StoryName, FileName, Splitted[1]));
                }
            }
        }
    }

    private void HandleTranslationLoading(CheckedListBox CheckedListBoxLeft)
    {
        CheckedListBoxLeft.FindForm().Cursor = Cursors.WaitCursor;
        HandleTranslationApprovalLoading(CheckedListBoxLeft);
        CheckedListBoxLeft.FindForm().Cursor = Cursors.Default;
        //foreach (LineData lineD in TranslationData)
        //{
        //    CheckedListBoxLeft.Items.Add(lineD.ID, false);
        //}
        //dont want to do cross thread ui calls :puke:
        //Thread loadApprovalThread = new Thread(() => HandleTranslationApprovalLoading(CheckedListBoxLeft)) { Name = "ApprovalLoadingThread" };
        //loadApprovalThread.Start();

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

        foreach (LineData lineD in TranslationData)
        {
            ProofreadDB.SetStringTemplate(lineD.ID, lineD.Story, lineD.FileName, lineD.EnglishString);
        }

        DialogResult result = MessageBox.Show(
            "This should be done uploading. It should be :)\n" +
            "If you are done uploading, please select YES. ELSE NO. Be wise please!",
            "Updating string database...",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2
            );

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

    private void LoadSourceFile(string path)
    {
        FileName = Path.GetFileNameWithoutExtension(path);
    }

    private static void HandleTranslationApprovalLoading(CheckedListBox CheckedListBoxLeft)
    {
        bool lineIsApproved = false;
        bool gotApprovedStates = ProofreadDB.GetAllApprovalStatesForFile(main.FileName, main.StoryName, out List<LineData> internalLines, "de");

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

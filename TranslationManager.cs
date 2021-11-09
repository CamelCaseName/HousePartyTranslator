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

    public void UpdateTranslationString(TextBox t)
    {
    }

    //Open
    public void LoadFileIntoProgram(string filePath, CheckedListBox CheckedListBoxLeft)
    {
        SourceFilePath = filePath;
        TemplateFileString = File.ReadAllText(filePath);
        TranslationData.Clear();
        CheckedListBoxLeft.Items.Clear();
        StoryName = SourceFilePath.Split('\\')[SourceFilePath.Split('\\').Length - 2];
        string ParentFolder = SourceFilePath.Split('\\')[SourceFilePath.Split('\\').Length - 3];

        //set whether we have a template or translation
        isTemplate = ParentFolder == "TEMPLATE";

        //read in all strings with IDs
        if (isTemplate)
        {
            foreach (string line in File.ReadAllLines(filePath))
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
            foreach (string line in File.ReadAllLines(filePath))
            {
                if (line.Contains('|'))
                {
                    string[] Splitted = line.Split('|');
                    //add new translation
                    TranslationData.Add(new LineData(Splitted[0], StoryName, FileName, Splitted[1]));
                }
            }
        }

        //is up to date, so we can start translation
        if (IsUpToDate)
        {
            //add all strings to translate to the checklistbox
            foreach (LineData lineD in TranslationData)
            {
                CheckedListBoxLeft.Items.Add(lineD.ID, false);
            }
        }
        else if (isTemplate) // not up to date, so we need to add all strings if they come from the template folder
        {

            //upload all new strings
            Application.UseWaitCursor = true;
            foreach (LineData lineD in TranslationData)
            {
                ProofreadDB.SetStringTemplate(lineD.ID, lineD.Story, lineD.FileName, lineD.EnglishString);
            }
            Application.UseWaitCursor = false;

            MessageBox.Show("Operation complete, you may now select the next file.", "Updating string database");
            DialogResult result = MessageBox.Show("Was this the last file? If you are unsure, select cancel and contact us on discord. " +
                "If it was the last file, please select YES. ELSE NO. BE VERY CAREFUL!",
                "Updating string database",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                if (ProofreadDB.UpdateDBVersion())
                {
                    IsUpToDate = true;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                Application.Exit();
            }
        }
        else
        {
            MessageBox.Show($"Please select a template file with its second parent folder as 'TEMPLATE' so we can upload them. " +
                $"Your Current Folder shows as {ParentFolder}.", "Updating string database");
        }
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
            if (CheckedListBoxLeft.GetItemChecked(currentIndex))
            {
                string ID = TranslationData[currentIndex].ID;
                string Story = SourceFilePath.Split('\\')[SourceFilePath.Split('\\').Length - 2];
                ProofreadDB.SetStringApprovedState(ID, FileName, Story, true);
            }
        }
    }

    private void LoadSourceFile(string path)
    {
        string folderPath = Path.GetDirectoryName(path);
        FileName = Path.GetFileNameWithoutExtension(path);
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
}

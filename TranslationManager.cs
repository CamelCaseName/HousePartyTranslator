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
        int selectionStart = t.GetFirstCharIndexOfCurrentLine();
        int CurrentLine = t.GetLineFromCharIndex(selectionStart);
        Console.WriteLine(CurrentLine.ToString());

        int selectionEnd = t.GetFirstCharIndexFromLine(CurrentLine + 1) - 1;
        string approvedString = "";
        if (selectionEnd - selectionStart > 0) { approvedString = t.Text.Substring(selectionStart, selectionEnd); }
        else { approvedString = t.Text.Substring(selectionStart, t.Text.Length); }

        if (approvedString.Contains('|'))
        {
            string ID = approvedString.Split('|')[0];
        }
    }

    //Open
    public void LoadFileIntoProgram(string filePath, CheckedListBox CheckedListBoxLeft)
    {
        SourceFilePath = filePath;
        TemplateFileString = File.ReadAllText(filePath);
        TranslationData.Clear();
        string Story = SourceFilePath.Split('\\')[SourceFilePath.Split('\\').Length - 2];

        //read in all strings with IDs
        foreach (string line in File.ReadAllLines(filePath))
        {
            if (line.Contains('|'))
            {
                string[] Splitted = line.Split('|');
                TranslationData.Add(new LineData(Splitted[0], Splitted[1], Story, FileName));
            }
        }

        CheckedListBoxLeft.Items.Clear();

        //is up to date, so we can start translation
        if (IsUpToDate)
        {
            //add all strings to translate to the checklistbox
            foreach (LineData lineD in TranslationData)
            {
                CheckedListBoxLeft.Items.Add(lineD.ID, false);
            }
        }
        else // not up to date, so we need to add all strings if they come from the template folder
        {
            string ParentFolder = SourceFilePath.Split('\\')[SourceFilePath.Split('\\').Length - 3];
            if (ParentFolder == "TEMPLATE")
            {
                //upload all new strings
                Application.UseWaitCursor = true;
                foreach (LineData lineD in TranslationData)
                {
                    ProofreadDB.AddTemplateString(lineD.ID, lineD.Story, lineD.FileName, lineD.EnglishString, "de");
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
    }

    public void ApproveIfPossible(CheckedListBox CheckedListBoxLeft)
    {
        if (CheckedListBoxLeft.GetItemChecked(CheckedListBoxLeft.SelectedIndex))
        {
            string ID = TranslationData[CheckedListBoxLeft.SelectedIndex].ID;
            string Story = SourceFilePath.Split('\\')[SourceFilePath.Split('\\').Length - 2];
            ProofreadDB.SetStringAccepted(ID, FileName, Story);
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

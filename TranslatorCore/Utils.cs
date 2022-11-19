using Translator.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

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
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.Log("Exception message shown: " + message);
            LogManager.Log("Current exception count: " + ExceptionCount++);
            _ = Msg.ErrorOk(
                $"The application encountered a Problem. Probably the database can not be reached, or you did something too quickly :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Some Error found (Nr. {ExceptionCount})");

            Application.UseWaitCursor = false;

            if (ExceptionCount > 3)
            {
                LogManager.Log("Too many exceptions encountered, aborting", LogManager.Level.Crash);
                UIHandler.SignalAppExit();
            }
        }

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string SelectFileFromSystem(bool isTranslation = true, string Title = "", string preselectedFile = "")
        {
            var selectFileDialog = new OpenFileDialog
            {
                Title = Title?.Length > 0 ? Title : "Choose a file for translation",
                Filter = "Text files (*.txt)|*.txt",
                InitialDirectory = Settings.Default.translation_path.Length > 0 && isTranslation ?
                    Settings.Default.translation_path :
                    Settings.Default.template_path.Length > 0 && !isTranslation ?
                        Settings.Default.template_path :
                        @"C:\Users\%USER%\Documents",
                RestoreDirectory = false,
                FileName = preselectedFile ?? ""
            };

            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (isTranslation)
                    Settings.Default.translation_path = Path.GetDirectoryName(selectFileDialog.FileName);
                else
                    Settings.Default.template_path = Path.GetDirectoryName(selectFileDialog.FileName);

                Settings.Default.Save();

                return selectFileDialog.FileName;
            }
            return "";
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
            string templatePath = Settings.Default.template_path;
            var selectFolderDialog = new FolderBrowserDialog
            {
                Description = message,
                SelectedPath = templatePath == "" ? Environment.SpecialFolder.UserProfile.ToString() : templatePath,
            };

            if (selectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                string t = selectFolderDialog.SelectedPath;
                if (templatePath != null)
                {
                    Settings.Default.template_path = t;
                    Settings.Default.Save();
                }
                return t;
            }
            return "";
        }

        /// <summary>
        /// Gets the category of a node from a node type
        /// </summary>
        /// <param name="type">the type to get the stringcategory form</param>
        /// <returns>the corresponding stringcategory</returns>
        public static StringCategory CategoryFromNode(NodeType type)
        {
#pragma warning disable IDE0066
            switch (type)
            {
                case NodeType.Null:
                    return StringCategory.Neither;
                case NodeType.Item:
                    return StringCategory.ItemName;
                case NodeType.ItemGroup:
                    return StringCategory.ItemGroupAction;
                case NodeType.Action:
                    return StringCategory.ItemAction;
                case NodeType.Event:
                    return StringCategory.Event;
                case NodeType.Criterion:
                    return StringCategory.Neither;
                case NodeType.Response:
                    return StringCategory.Response;
                case NodeType.Dialogue:
                    return StringCategory.Dialogue;
                case NodeType.Quest:
                    return StringCategory.Quest;
                case NodeType.Achievement:
                    return StringCategory.Achievement;
                case NodeType.Reaction:
                    return StringCategory.Response;
                case NodeType.BGC:
                    return StringCategory.BGC;
                default:
                    return StringCategory.Neither;
            }
#pragma warning restore IDE0066
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

        /// <summary>
        /// Creates a new tab with all default controls
        /// </summary>
        /// <param name="number">the number of the tab starting at 1, is only used for name and text</param>
        /// <param name="form"></param>
        /// <returns>a TabPage with all controls as child controls</returns>
        
        public static TabPage? CreateNewTab(int number, Fenster? form)
        {
            if (form == null) return null;
            var newTab = new TabPage()
            {
                BackColor = Color.Black,
                ForeColor = SystemColors.ScrollBar,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(4, 22),
                Name = $"TabPage{number}",
                Padding = new Padding(3),
                TabIndex = 0,
                Text = $"Tab{number}",
            };

            var TranslateThis = new Button();
            var ApprovedBox = new CheckBox();
            var CheckListBoxLeft = new ColouredCheckedListBox();
            var ListContextMenu = new ContextMenuStrip();
            var CommentGroup = new GroupBox();
            var CharacterCountLabel = new Label();
            var SelectedFile = new Label();
            var WordsTranslated = new Label();
            var ProgressbarTranslated = new NoAnimationBar();
            var panel1 = new Panel();
            var panel2 = new Panel();
            var mainTableLayoutPanel = new TableLayoutPanel();
            var CommentTextBox = new TextBox();
            var TemplateTextBox = new TextBox();
            var TranslatedTextBox = new TextBox();
            var CopyAllContextMenuButton = new ToolStripMenuItem();
            var CopyAsOutputContextMenuButton = new ToolStripMenuItem();
            var CopyFileNameContextMenuButton = new ToolStripMenuItem();
            var CopyIdContextMenuButton = new ToolStripMenuItem();
            var CopyStoryNameContextMenuButton = new ToolStripMenuItem();
            var CopyTemplateContextMenuButton = new ToolStripMenuItem();
            var CopyTranslationContextMenuButton = new ToolStripMenuItem();
            mainTableLayoutPanel.SuspendLayout();
            CommentGroup.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            newTab.SuspendLayout();
            // 
            // TranslatedTextBox
            // 
            TranslatedTextBox.AcceptsReturn = true;
            TranslatedTextBox.AllowDrop = true;
            TranslatedTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            TranslatedTextBox.BackColor = background;
            TranslatedTextBox.Dock = DockStyle.Fill;
            TranslatedTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TranslatedTextBox.ForeColor = brightText;
            TranslatedTextBox.ImeMode = ImeMode.On;
            TranslatedTextBox.Location = new Point(689, 294);
            TranslatedTextBox.Multiline = true;
            TranslatedTextBox.Name = "TranslatedTextBox";
            TranslatedTextBox.Size = new Size(678, 275);
            TranslatedTextBox.TabIndex = 0;
            TranslatedTextBox.Text = "edit here";
            TranslatedTextBox.TextChanged += new EventHandler(form.TextBoxRight_TextChanged);
            TranslatedTextBox.MouseUp += new MouseEventHandler(form.TextContextOpened);
            TranslatedTextBox.MouseEnter += new EventHandler(form.TextContextOpened);
            // 
            // AutoTranslateThis
            // 
            TranslateThis.AutoSize = true;
            TranslateThis.BackColor = menu;
            TranslateThis.ForeColor = darkText;
            TranslateThis.Location = new Point(80, 1);
            TranslateThis.Name = "AutoTranslateThis";
            TranslateThis.Size = new Size(60, 20);
            TranslateThis.TabIndex = 13;
            TranslateThis.Text = "Automatic Translation";
            TranslateThis.UseVisualStyleBackColor = true;
            TranslateThis.Click += new EventHandler(form.TranslateThis_Click);
            // 
            // TemplateTextBox
            // 
            TemplateTextBox.BackColor = background;
            TemplateTextBox.Dock = DockStyle.Fill;
            TemplateTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TemplateTextBox.ForeColor = brightText;
            TemplateTextBox.Location = new Point(689, 33);
            TemplateTextBox.Multiline = true;
            TemplateTextBox.Name = "TemplateTextBox";
            TemplateTextBox.ReadOnly = true;
            TemplateTextBox.Size = new Size(678, 255);
            TemplateTextBox.TabIndex = 9;
            TemplateTextBox.Text = "Lorem ipsum dolor sit amed";
            // 
            // CommentTextBox
            // 
            CommentTextBox.BackColor = background;
            CommentTextBox.Dock = DockStyle.Fill;
            CommentTextBox.Font = new Font("Consolas", 11F);
            CommentTextBox.ForeColor = brightText;
            CommentTextBox.Location = new Point(3, 16);
            CommentTextBox.Multiline = true;
            CommentTextBox.Name = "CommentTextBox";
            CommentTextBox.Size = new Size(672, 105);
            CommentTextBox.TabIndex = 13;
            CommentTextBox.TextChanged += new EventHandler(form.Comments_TextChanged);
            CommentTextBox.MouseUp += new MouseEventHandler(form.TextContextOpened);
            CommentTextBox.MouseEnter += new EventHandler(form.TextContextOpened);
            // 
            // CharacterCountLabel
            // 
            CharacterCountLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CharacterCountLabel.AutoSize = true;
            CharacterCountLabel.ForeColor = brightText;
            CharacterCountLabel.Location = new Point(23, 5);
            CharacterCountLabel.Name = "CharacterCountLabel";
            CharacterCountLabel.Size = new Size(143, 13);
            CharacterCountLabel.TabIndex = 20;
            CharacterCountLabel.Text = "Template: xx | Translation: xx";
            // 
            // SelectedFile
            // 
            SelectedFile.AutoSize = true;
            SelectedFile.ForeColor = brightText;
            SelectedFile.Location = new Point(0, 6);
            SelectedFile.Name = "SelectedFile";
            SelectedFile.Size = new Size(98, 13);
            SelectedFile.TabIndex = 7;
            SelectedFile.Text = "Selected File: none";
            // 
            // WordsTranslated
            // 
            WordsTranslated.Anchor = AnchorStyles.Top;
            WordsTranslated.Parent = panel1;
            WordsTranslated.AutoSize = true;
            WordsTranslated.BackColor = Color.Transparent;
            WordsTranslated.ForeColor = brightText;
            WordsTranslated.Location = new Point(60, 6);
            WordsTranslated.Name = "WordsTranslated";
            WordsTranslated.Size = new Size(47, 13);
            WordsTranslated.TabIndex = 15;
            WordsTranslated.Text = "progress";
            // 
            // ApprovedBox
            // 
            ApprovedBox.AutoSize = true;
            ApprovedBox.ForeColor = brightText;
            ApprovedBox.Location = new Point(3, 5);
            ApprovedBox.Name = "ApprovedBox";
            ApprovedBox.Size = new Size(72, 17);
            ApprovedBox.TabIndex = 13;
            ApprovedBox.Text = Resources.Approved;
            ApprovedBox.UseVisualStyleBackColor = true;
            ApprovedBox.CheckedChanged += new EventHandler(form.ApprovedBox_CheckedChanged);
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 2;
            _ = mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.07924F));
            _ = mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.92076F));
            mainTableLayoutPanel.Controls.Add(CommentGroup, 1, 3);
            mainTableLayoutPanel.Controls.Add(TranslatedTextBox, 1, 2);
            mainTableLayoutPanel.Controls.Add(TemplateTextBox, 1, 1);
            mainTableLayoutPanel.Controls.Add(CheckListBoxLeft, 0, 1);
            mainTableLayoutPanel.Controls.Add(panel1, 0, 0);
            mainTableLayoutPanel.Controls.Add(panel2, 1, 0);
            mainTableLayoutPanel.Parent = newTab;
            mainTableLayoutPanel.Dock = DockStyle.Fill;
            mainTableLayoutPanel.Location = new Point(3, 3);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowCount = 4;
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 38.94275F));
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 41.86569F));
            _ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 19.19156F));
            mainTableLayoutPanel.Size = new Size(1370, 702);
            mainTableLayoutPanel.TabIndex = 18;
            // 
            // CommentGroup
            // 
            CommentGroup.Controls.Add(CommentTextBox);
            CommentGroup.Dock = DockStyle.Fill;
            CommentGroup.ForeColor = brightText;
            CommentGroup.Location = new Point(689, 575);
            CommentGroup.Name = "CommentGroup";
            CommentGroup.Size = new Size(678, 124);
            CommentGroup.TabIndex = 11;
            CommentGroup.TabStop = false;
            CommentGroup.Text = "Comments";
            // 
            // panel1
            // 
            panel1.Controls.Add(SelectedFile);
            panel1.Controls.Add(WordsTranslated);
            panel1.Controls.Add(ProgressbarTranslated);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(680, 24);
            panel1.TabIndex = 12;
            // 
            // panel2
            // 
            panel2.Controls.Add(ApprovedBox);
            panel2.Controls.Add(TranslateThis);
            panel2.Controls.Add(CharacterCountLabel);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(689, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(678, 24);
            panel2.TabIndex = 13;
            // 
            // CheckListBoxLeft
            // 
            CheckListBoxLeft.BackColor = frame;
            CheckListBoxLeft.Dock = DockStyle.Fill;
            CheckListBoxLeft.ForeColor = brightText;
            CheckListBoxLeft.FormattingEnabled = true;
            CheckListBoxLeft.Location = new Point(3, 33);
            CheckListBoxLeft.Name = "CheckListBoxLeft";
            mainTableLayoutPanel.SetRowSpan(CheckListBoxLeft, 3);
            CheckListBoxLeft.Size = new Size(680, 666);
            CheckListBoxLeft.TabIndex = 10;
            CheckListBoxLeft.ThreeDCheckBoxes = true;
            CheckListBoxLeft.ItemCheck += new ItemCheckEventHandler(form.CheckListBoxLeft_ItemCheck);
            CheckListBoxLeft.SelectedIndexChanged += new EventHandler(form.CheckListBoxLeft_SelectedIndexChanged);
            CheckListBoxLeft.ContextMenuStrip = ListContextMenu;
            CheckListBoxLeft.MouseDown += new MouseEventHandler(form.OpeningContextMenu);
            // 
            // ProgressbarTranslated
            // 
            ProgressbarTranslated.BackColor = background;
            ProgressbarTranslated.Cursor = Cursors.Default;
            ProgressbarTranslated.Dock = DockStyle.Fill;
            ProgressbarTranslated.ForeColor = foreground;
            ProgressbarTranslated.Location = new Point(0, 0);
            ProgressbarTranslated.Name = "ProgressbarTranslated";
            ProgressbarTranslated.Size = new Size(680, 24);
            ProgressbarTranslated.Step = 1;
            ProgressbarTranslated.Style = ProgressBarStyle.Continuous;
            ProgressbarTranslated.TabIndex = 8;
            ProgressbarTranslated.Value = 50;
            // 
            // CopyIdContextMenuButton
            // 
            CopyIdContextMenuButton.Name = "CopyIdContextMenuButton";
            CopyIdContextMenuButton.Size = new Size(236, 22);
            CopyIdContextMenuButton.Text = "Copy Id";
            CopyIdContextMenuButton.Click += new EventHandler(form.CopyIdContextMenuButton_Click);
            // 
            // CopyFileNameContextMenuButton
            // 
            CopyFileNameContextMenuButton.Name = "CopyFileNameContextMenuButton";
            CopyFileNameContextMenuButton.Size = new Size(236, 22);
            CopyFileNameContextMenuButton.Text = "Copy file name";
            CopyFileNameContextMenuButton.Click += new EventHandler(form.CopyFileNameContextMenuButton_Click);
            // 
            // CopyStoryNameContextMenuButton
            // 
            CopyStoryNameContextMenuButton.Name = "CopyStoryNameContextMenuButton";
            CopyStoryNameContextMenuButton.Size = new Size(236, 22);
            CopyStoryNameContextMenuButton.Text = "Copy story name";
            CopyStoryNameContextMenuButton.Click += new EventHandler(form.CopyStoryNameContextMenuButton_Click);
            // 
            // CopyAllContextMenuButton
            // 
            CopyAllContextMenuButton.Name = "CopyAllContextMenuButton";
            CopyAllContextMenuButton.Size = new Size(236, 22);
            CopyAllContextMenuButton.Text = "Copy everything";
            CopyAllContextMenuButton.Click += new EventHandler(form.CopyAllContextMenuButton_Click);
            // 
            // CopyAsOutputContextMenuButton
            // 
            CopyAsOutputContextMenuButton.Name = "CopyAsOutputContextMenuButton";
            CopyAsOutputContextMenuButton.Size = new Size(236, 22);
            CopyAsOutputContextMenuButton.Text = "Copy as output";
            CopyAsOutputContextMenuButton.Click += new EventHandler(form.CopyAsOutputContextMenuButton_Click);
            // 
            // CopyTemplateContextMenuButton
            // 
            CopyTemplateContextMenuButton.Name = "CopyTemplateContextMenuButton";
            CopyTemplateContextMenuButton.Size = new Size(236, 22);
            CopyTemplateContextMenuButton.Text = "Copy template";
            CopyTemplateContextMenuButton.Click += new EventHandler(form.CopyTemplateContextMenuButton_Click);
            // 
            // CopyTranslationContextMenuButton
            // 
            CopyTranslationContextMenuButton.Name = "CopyTranslationContextMenuButton";
            CopyTranslationContextMenuButton.Size = new Size(236, 22);
            CopyTranslationContextMenuButton.Text = "Copy translation";
            CopyTranslationContextMenuButton.Click += new EventHandler(form.CopyTranslationContextMenuButton_Click);
            //
            // ListContextMenu
            //
            ListContextMenu.Name = "ListContextMenu";
            ListContextMenu.BackColor = SystemColors.ScrollBar;
            ListContextMenu.ShowCheckMargin = false;
            ListContextMenu.ShowImageMargin = false;
            ListContextMenu.ForeColor = SystemColors.MenuText;
            ListContextMenu.Size = new Size(236, 160);
            ListContextMenu.Items.Clear();
            ListContextMenu.Items.AddRange(new ToolStripItem[]{
            CopyIdContextMenuButton,
            CopyFileNameContextMenuButton,
            CopyStoryNameContextMenuButton,
            CopyTemplateContextMenuButton,
            CopyTranslationContextMenuButton,
            CopyAsOutputContextMenuButton,
            CopyAllContextMenuButton});

            newTab.Controls.Add(mainTableLayoutPanel);
            mainTableLayoutPanel.ResumeLayout();
            mainTableLayoutPanel.PerformLayout();
            CommentGroup.ResumeLayout();
            CommentGroup.PerformLayout();
            panel1.ResumeLayout();
            panel1.PerformLayout();
            panel2.ResumeLayout();
            panel2.PerformLayout();
            newTab.ResumeLayout();

            return newTab;
        }

        /// <summary>
        /// Tries to delete the word in let or right ofthe cursor in the currently selected TextBox.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="toLeft">true if deleting to the left</param>
        /// <returns>true if successfull</returns>
        public static bool DeleteCharactersInText(Form form, bool toLeft)
        {
            if (form.ContainsFocus)
            {
                Control? focused_control = form.ActiveControl;
                if (focused_control == null) return false;
                if(!focused_control.GetType().IsAssignableTo(typeof(TextBox))) { return false; }

                var textBox = (TextBox)focused_control;
                if (toLeft)
                {
                    return textBox.DeleteCharactersInTextLeft();
                }
                else
                {
                    return textBox.DeleteCharactersInTextRight();
                }
            }
            return false;
        }

        /// <summary>
        /// Moves the cursor to the beginning/end of the next word in the specified direction
        /// </summary>
        /// <param name="form"></param>
        /// <param name="toLeft">true if to scan to the left</param>
        /// <returns>true if succeeded</returns>
        public static bool MoveCursorInText(Form form, bool toLeft)
        {
            if (form.ContainsFocus && form.ActiveControl != null)
            {
                Control focused_control = form.ActiveControl;
                try
                {
                    var _ = (TextBox)focused_control;
                }
                //ignore exception, really intended
                catch { return false; }
                var textBox = (TextBox)focused_control;
                if (toLeft)
                {
                    textBox.MoveCursorWordLeft();
                    return true;
                }
                else
                {
                    textBox.MoveCursorWordRight();
                    return true;
                }
            }
            return false;
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

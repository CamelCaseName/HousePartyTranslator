using HousePartyTranslator.Managers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HousePartyTranslator.Helpers
{
    /// <summary>
    /// Provides some generic utility methods.
    /// </summary>
    public static class Utils
    {
        public static readonly int MaxTextLength = 100;
        public static readonly int maxWordLength = 15;

        private static int ExceptionCount = 0;

        public static readonly System.Drawing.Color foreground = System.Drawing.SystemColors.Window;
        public static readonly System.Drawing.Color background = System.Drawing.SystemColors.ControlDarkDark;
        public static readonly System.Drawing.Color backgroundDarker = System.Drawing.SystemColors.MenuText;
        public static readonly System.Drawing.Color brightText = System.Drawing.SystemColors.HighlightText;
        public static readonly System.Drawing.Color darkText = System.Drawing.SystemColors.WindowText;
        public static readonly System.Drawing.Color menu = System.Drawing.SystemColors.ScrollBar;
        public static readonly System.Drawing.Color frame = System.Drawing.SystemColors.WindowFrame;

        /// <summary>
        /// Returns whether a story is official or not
        /// </summary>
        /// <param name="storyName">the name to check</param>
        /// <returns></returns>
        public static bool IsOfficialStory(string storyName)
        {
            string[] stories = { "UI", "Hints", "Original Story", "A Vickie Vixen Valentine", "Combat Training", "Date Night with Brittney", "Date Night With Brittney" };
            return Array.IndexOf(stories, storyName) >= 0;
        }

        /// <summary>
        /// Replaces all matches in the given string and returns it
        /// </summary>
        /// <param name="input">The string to work on</param>
        /// <param name="replacement">The replacement for all matches</param>
        /// <param name="search">the pattern to search for</param>
        /// <returns>the replaced string</returns>
        public static string Replace(string input, string replacement, string search)
        {
            return ReplaceRegex(input, replacement, Regex.Escape(search));
        }

        /// <summary>
        /// Replaces all regex rule matches inte given string and returns it
        /// </summary>
        /// <param name="input">The string to work on</param>
        /// <param name="replacement">The replacement for all matches</param>
        /// <param name="regexRules">The regex to match</param>
        /// <returns></returns>
        public static string ReplaceRegex(string input, string replacement, string regexRules)
        {
            return Regex.Replace(input, regexRules, replacement, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline, new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// Removes the voice actor infos from the given string. Info has to be encapsulated in []. 
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string RemoveVAHints(string input)
        {
            bool inVAHint = false;
            string output = "";
            foreach (char character in input)
            {
                if (character == '[' && !inVAHint)
                {
                    inVAHint = true;
                }
                else if (character == ']' && inVAHint)
                {
                    inVAHint = false;
                }
                else if (!inVAHint)
                {
                    output += character;
                }
            }

            return output;
        }

        /// <summary>
        /// Constrains the lenght of a single string to a multi line version of said string, where every MaxTextLength characters a newline is inserted.
        /// </summary>
        /// <param name="input">The string to format</param>
        /// <returns>The formatted, blockified string</returns>
        public static string ConstrainLength(string input)
        {
            string output = "";
            bool inWord;
            int currentWordLength = 0, currentLength = 0;

            foreach (char c in input)
            {
                if (c != ' ' && c != '\n' && c != '\r')
                {
                    inWord = true;
                    currentWordLength++;
                }
                else
                {
                    inWord = false;
                    currentWordLength = 0;
                }

                currentLength++;

                //if line is short still
                if (currentLength <= MaxTextLength)
                {
                    output += c;
                }
                else
                {
                    if (inWord && currentWordLength < maxWordLength)
                    {
                        //line is too long but we in a word
                        output += c;
                    }
                    else
                    {
                        output += c + "\n";
                        currentLength = 0;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Deletes the characters to the left of the char until the first seperator or the start of the text.
        /// </summary>
        /// <param name="textBox">The textbox to work on</param>
        /// <returns>true if successful</returns>
        public static bool DeleteCharactersInTextLeft(TextBox textBox)
        {
            int oldPos = textBox.SelectionStart;
            MoveCursorWordLeft(textBox);
            int newPos = textBox.SelectionStart;
            if (oldPos - newPos > 0) textBox.Text = textBox.Text.Remove(newPos, oldPos - newPos);
            textBox.SelectionStart = newPos;
            //to stop winforms adding the weird backspace character to the text
            return true;
        }

        /// <summary>
        /// Deletes the characters to the right of the char until the first seperator or the end of the text.
        /// </summary>
        /// <param name="textBox">The textbox to work on</param>
        /// <returns>true if successful</returns>
        public static bool DeleteCharactersInTextRight(TextBox textBox)
        {
            int oldPos = textBox.SelectionStart;
            MoveCursorWordRight(textBox);
            int newPos = textBox.SelectionStart;
            if (newPos - oldPos > 0) textBox.Text = textBox.Text.Remove(oldPos, newPos - oldPos);
            textBox.SelectionStart = oldPos;
            return true;
        }

        /// <summary>
        /// Moves the cursor in the given textbox to the next beginning/end of a word to the left
        /// </summary>
        /// <param name="textBox">The box to work on</param>
        public static void MoveCursorWordLeft(TextBox textBox)
        {
            bool broken = false;
            int oldPos = textBox.SelectionStart;
            if (textBox.SelectionStart > 0)
            {
                if (textBox.Text[textBox.SelectionStart - 1] == ' ')
                {
                    --textBox.SelectionStart;
                }
            }
            for (int i = textBox.SelectionStart; i > 0; i--)
            {
                switch (textBox.Text.Substring(i - 1, 1))
                {    //set up any stopping points you want
                    case " ":
                    case ";":
                    case ",":
                    case ".":
                    case "-":
                    case "'":
                    case "/":
                    case "\\":
                    case "\n":
                        textBox.SelectionStart = i;
                        broken = true;
                        break;
                }
                if (broken) break;
            }
            if (oldPos - textBox.SelectionStart < 1 && textBox.SelectionStart > 0)
            {
                --textBox.SelectionStart;
            }
            if (!broken)
            {
                textBox.SelectionStart = 0;
            }
        }

        /// <summary>
        /// Moves the cursor in the given textbox to the next beginning/end of a word to the right
        /// </summary>
        /// <param name="textBox">The box to work on</param>
        public static void MoveCursorWordRight(TextBox textBox)
        {
            bool broken = false;
            int oldPos = textBox.SelectionStart;
            if (textBox.SelectionStart < textBox.Text.Length)
            {
                if (textBox.Text[textBox.SelectionStart] == ' ' && textBox.SelectionStart < textBox.Text.Length - 1)
                {
                    ++textBox.SelectionStart;
                }
            }
            for (int i = textBox.SelectionStart; i < textBox.TextLength; i++)
            {
                switch (textBox.Text[i].ToString())
                {    //set up any stopping points you want
                    case " ":
                    case ";":
                    case ",":
                    case ".":
                    case "-":
                    case "_":
                    case "'":
                    case "/":
                    case "\\":
                    case "\n":
                        textBox.SelectionStart = i;
                        broken = true;
                        break;
                }
                if (broken) break;
            }
            if (textBox.SelectionStart - oldPos < 1)
            {
                textBox.SelectionStart++;
            }
            if (!broken)
            {
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        /// <summary>
        /// Gets the current assembly version as a string.
        /// </summary>
        /// <returns>The current assembly version</returns>
        public static string GetAssemblyFileVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersion.FileVersion;
        }

        /// <summary>
        /// Displays a window with the necessary info about the exception.
        /// </summary>
        /// <param name="message">The error message to display</param>
        public static void DisplayExceptionMessage(string message)
        {
            LogManager.LogEvent("Exception message shown: " + message);
            LogManager.LogEvent("Current exception count: " + ExceptionCount++);
            MessageBox.Show(
                $"The application encountered a Problem. Probably the database can not be reached, or you did something too quickly :). " +
                $"Anyways, here is what happened: \n\n{message}\n\n " +
                $"Oh, and if you click OK the application will try to resume. On the 4th exception it will close :(",
                $"Some Error found (Nr. {ExceptionCount})",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            Application.OpenForms[0].Cursor = Cursors.Default;

            if (ExceptionCount > 3)
            {
                LogManager.LogEvent("Too many exceptions encountered, aborting", LogManager.Level.Crash);
                Application.Exit();
            }
        }

        /// <summary>
        /// Opens a select file dialogue and returns the selected file as a path.
        /// </summary>
        /// <returns>The path to the selected file.</returns>
        public static string SelectFileFromSystem()
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog
            {
                Title = "Choose a file for translation",
                Filter = "Text files (*.txt)|*.txt",
                InitialDirectory = Properties.Settings.Default.translation_path.Length > 0 ? System.IO.Path.GetDirectoryName(Properties.Settings.Default.translation_path) : @"C:\Users\%USER%\Documents",
                RestoreDirectory = false
            };

            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.translation_path = selectFileDialog.FileName;
                Properties.Settings.Default.Save();
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
            string templatePath = Properties.Settings.Default.template_path;
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog
            {
                Description = message,
                SelectedPath = templatePath == "" ? Environment.SpecialFolder.UserProfile.ToString() : templatePath,
            };

            if (selectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                string t = selectFolderDialog.SelectedPath;
                if (templatePath != null)
                {
                    Properties.Settings.Default.template_path = t;
                    Properties.Settings.Default.Save();
                }
                return t;
            }
            return "";
        }

        /// <summary>
        /// cuts a string to a maximum length and adds a delimiter
        /// </summary>
        /// <param name="toTrim">the string to cut</param>
        /// <param name="delimiter">the delimiter to add afterwards</param>
        /// <param name="maxLength">the length of the resulting string including the delimiter</param>
        /// <returns></returns>
        public static string TrimWithDelim(string toTrim, string delimiter = "...", int maxLength = 18)
        {
            int length = toTrim.Length, delimLength = delimiter.Length;
            if (length > maxLength - delimLength)
            {
                length = maxLength - delimLength;
            }
            return toTrim.Length > length ? toTrim.Substring(0, length).Trim() + delimiter : toTrim;
        }

        public static StringCategory CategoryFromNode(NodeType type)
        {
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
        }

        /// <summary>
        /// Creates a new tab with all default controls
        /// </summary>
        /// <param name="number">the number of the tab starting at 1, is only used for name and text</param>
        /// <returns>a TabPage with all controls as child controls</returns>
        public static TabPage CreateNewTab(int number, Fenster form)
        {
            TabPage newTab = new TabPage()
            {
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.SystemColors.ScrollBar,
                Location = new System.Drawing.Point(4, 22),
                Name = $"TabPage{number}",
                Padding = new Padding(3),
                TabIndex = 0,
                Text = $"Tab{number}",
            };

            Button TranslateThis = new Button();
            CheckBox ApprovedBox = new CheckBox();
            ColouredCheckedListBox CheckListBoxLeft = new ColouredCheckedListBox();
            ContextMenuStrip ListContextMenu = new ContextMenuStrip();
            GroupBox CommentGroup = new GroupBox();
            Label CharacterCountLabel = new Label();
            Label SelectedFile = new Label();
            Label WordsTranslated = new Label();
            NoAnimationBar ProgressbarTranslated = new NoAnimationBar();
            Panel panel1 = new Panel();
            Panel panel2 = new Panel();
            TableLayoutPanel mainTableLayoutPanel = new TableLayoutPanel();
            TextBox CommentTextBox = new TextBox();
            TextBox TemplateTextBox = new TextBox();
            TextBox TranslatedTextBox = new TextBox();
            ToolStripMenuItem CopyAllContextMenuButton = new ToolStripMenuItem();
            ToolStripMenuItem CopyAsOutputContextMenuButton = new ToolStripMenuItem();
            ToolStripMenuItem CopyFileNameContextMenuButton = new ToolStripMenuItem();
            ToolStripMenuItem CopyIdContextMenuButton = new ToolStripMenuItem();
            ToolStripMenuItem CopyStoryNameContextMenuButton = new ToolStripMenuItem();
            ToolStripMenuItem CopyTemplateContextMenuButton = new ToolStripMenuItem();
            ToolStripMenuItem CopyTranslationContextMenuButton = new ToolStripMenuItem();
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
            TranslatedTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TranslatedTextBox.ForeColor = brightText;
            TranslatedTextBox.ImeMode = ImeMode.On;
            TranslatedTextBox.Location = new System.Drawing.Point(689, 294);
            TranslatedTextBox.Multiline = true;
            TranslatedTextBox.Name = "TranslatedTextBox";
            TranslatedTextBox.Size = new System.Drawing.Size(678, 275);
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
            TranslateThis.Location = new System.Drawing.Point(80, 1);
            TranslateThis.Name = "AutoTranslateThis";
            TranslateThis.Size = new System.Drawing.Size(60, 20);
            TranslateThis.TabIndex = 13;
            TranslateThis.Text = "Automatic Translation";
            TranslateThis.UseVisualStyleBackColor = true;
            TranslateThis.Click += new EventHandler(form.TranslateThis_Click);
            // 
            // TemplateTextBox
            // 
            TemplateTextBox.BackColor = background;
            TemplateTextBox.Dock = DockStyle.Fill;
            TemplateTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TemplateTextBox.ForeColor = brightText;
            TemplateTextBox.Location = new System.Drawing.Point(689, 33);
            TemplateTextBox.Multiline = true;
            TemplateTextBox.Name = "TemplateTextBox";
            TemplateTextBox.ReadOnly = true;
            TemplateTextBox.Size = new System.Drawing.Size(678, 255);
            TemplateTextBox.TabIndex = 9;
            TemplateTextBox.Text = "Lorem ipsum dolor sit amed";
            // 
            // CommentTextBox
            // 
            CommentTextBox.BackColor = background;
            CommentTextBox.Dock = DockStyle.Fill;
            CommentTextBox.Font = new System.Drawing.Font("Consolas", 11F);
            CommentTextBox.ForeColor = brightText;
            CommentTextBox.Location = new System.Drawing.Point(3, 16);
            CommentTextBox.Multiline = true;
            CommentTextBox.Name = "CommentTextBox";
            CommentTextBox.Size = new System.Drawing.Size(672, 105);
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
            CharacterCountLabel.Location = new System.Drawing.Point(23, 5);
            CharacterCountLabel.Name = "CharacterCountLabel";
            CharacterCountLabel.Size = new System.Drawing.Size(143, 13);
            CharacterCountLabel.TabIndex = 20;
            CharacterCountLabel.Text = "Template: xx | Translation: xx";
            // 
            // SelectedFile
            // 
            SelectedFile.AutoSize = true;
            SelectedFile.ForeColor = brightText;
            SelectedFile.Location = new System.Drawing.Point(0, 6);
            SelectedFile.Name = "SelectedFile";
            SelectedFile.Size = new System.Drawing.Size(98, 13);
            SelectedFile.TabIndex = 7;
            SelectedFile.Text = "Selected File: none";
            // 
            // WordsTranslated
            // 
            WordsTranslated.Anchor = AnchorStyles.Top;
            WordsTranslated.Parent = panel1;
            WordsTranslated.AutoSize = true;
            WordsTranslated.BackColor = System.Drawing.Color.Transparent;
            WordsTranslated.ForeColor = brightText;
            WordsTranslated.Location = new System.Drawing.Point(60, 6);
            WordsTranslated.Name = "WordsTranslated";
            WordsTranslated.Size = new System.Drawing.Size(47, 13);
            WordsTranslated.TabIndex = 15;
            WordsTranslated.Text = "progress";
            // 
            // ApprovedBox
            // 
            ApprovedBox.AutoSize = true;
            ApprovedBox.ForeColor = brightText;
            ApprovedBox.Location = new System.Drawing.Point(3, 5);
            ApprovedBox.Name = "ApprovedBox";
            ApprovedBox.Size = new System.Drawing.Size(72, 17);
            ApprovedBox.TabIndex = 13;
            ApprovedBox.Text = Properties.Resources.Approved;
            ApprovedBox.UseVisualStyleBackColor = true;
            ApprovedBox.CheckedChanged += new EventHandler(form.ApprovedBox_CheckedChanged);
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 2;
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.07924F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.92076F));
            mainTableLayoutPanel.Controls.Add(CommentGroup, 1, 3);
            mainTableLayoutPanel.Controls.Add(TranslatedTextBox, 1, 2);
            mainTableLayoutPanel.Controls.Add(TemplateTextBox, 1, 1);
            mainTableLayoutPanel.Controls.Add(CheckListBoxLeft, 0, 1);
            mainTableLayoutPanel.Controls.Add(panel1, 0, 0);
            mainTableLayoutPanel.Controls.Add(panel2, 1, 0);
            mainTableLayoutPanel.Parent = newTab;
            mainTableLayoutPanel.Dock = DockStyle.Fill;
            mainTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowCount = 4;
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 38.94275F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 41.86569F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 19.19156F));
            mainTableLayoutPanel.Size = new System.Drawing.Size(1370, 702);
            mainTableLayoutPanel.TabIndex = 18;
            // 
            // CommentGroup
            // 
            CommentGroup.Controls.Add(CommentTextBox);
            CommentGroup.Dock = DockStyle.Fill;
            CommentGroup.ForeColor = brightText;
            CommentGroup.Location = new System.Drawing.Point(689, 575);
            CommentGroup.Name = "CommentGroup";
            CommentGroup.Size = new System.Drawing.Size(678, 124);
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
            panel1.Location = new System.Drawing.Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(680, 24);
            panel1.TabIndex = 12;
            // 
            // panel2
            // 
            panel2.Controls.Add(ApprovedBox);
            panel2.Controls.Add(TranslateThis);
            panel2.Controls.Add(CharacterCountLabel);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(689, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(678, 24);
            panel2.TabIndex = 13;
            // 
            // CheckListBoxLeft
            // 
            CheckListBoxLeft.BackColor = frame;
            CheckListBoxLeft.Dock = DockStyle.Fill;
            CheckListBoxLeft.ForeColor = brightText;
            CheckListBoxLeft.FormattingEnabled = true;
            CheckListBoxLeft.Location = new System.Drawing.Point(3, 33);
            CheckListBoxLeft.Name = "CheckListBoxLeft";
            mainTableLayoutPanel.SetRowSpan(CheckListBoxLeft, 3);
            CheckListBoxLeft.Size = new System.Drawing.Size(680, 666);
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
            ProgressbarTranslated.Location = new System.Drawing.Point(0, 0);
            ProgressbarTranslated.Name = "ProgressbarTranslated";
            ProgressbarTranslated.Size = new System.Drawing.Size(680, 24);
            ProgressbarTranslated.Step = 1;
            ProgressbarTranslated.Style = ProgressBarStyle.Continuous;
            ProgressbarTranslated.TabIndex = 8;
            ProgressbarTranslated.Value = 50;
            // 
            // CopyIdContextMenuButton
            // 
            CopyIdContextMenuButton.Name = "CopyIdContextMenuButton";
            CopyIdContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyIdContextMenuButton.Text = "Copy Id";
            CopyIdContextMenuButton.Click += new System.EventHandler(form.CopyIdContextMenuButton_Click);
            // 
            // CopyFileNameContextMenuButton
            // 
            CopyFileNameContextMenuButton.Name = "CopyFileNameContextMenuButton";
            CopyFileNameContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyFileNameContextMenuButton.Text = "Copy file name";
            CopyFileNameContextMenuButton.Click += new System.EventHandler(form.CopyFileNameContextMenuButton_Click);
            // 
            // CopyStoryNameContextMenuButton
            // 
            CopyStoryNameContextMenuButton.Name = "CopyStoryNameContextMenuButton";
            CopyStoryNameContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyStoryNameContextMenuButton.Text = "Copy story name";
            CopyStoryNameContextMenuButton.Click += new System.EventHandler(form.CopyStoryNameContextMenuButton_Click);
            // 
            // CopyAllContextMenuButton
            // 
            CopyAllContextMenuButton.Name = "CopyAllContextMenuButton";
            CopyAllContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyAllContextMenuButton.Text = "Copy everything";
            CopyAllContextMenuButton.Click += new System.EventHandler(form.CopyAllContextMenuButton_Click);
            // 
            // CopyAsOutputContextMenuButton
            // 
            CopyAsOutputContextMenuButton.Name = "CopyAsOutputContextMenuButton";
            CopyAsOutputContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyAsOutputContextMenuButton.Text = "Copy as output";
            CopyAsOutputContextMenuButton.Click += new System.EventHandler(form.CopyAsOutputContextMenuButton_Click);
            // 
            // CopyTemplateContextMenuButton
            // 
            CopyTemplateContextMenuButton.Name = "CopyTemplateContextMenuButton";
            CopyTemplateContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyTemplateContextMenuButton.Text = "Copy template";
            CopyTemplateContextMenuButton.Click += new System.EventHandler(form.CopyTemplateContextMenuButton_Click);
            // 
            // CopyTranslationContextMenuButton
            // 
            CopyTranslationContextMenuButton.Name = "CopyTranslationContextMenuButton";
            CopyTranslationContextMenuButton.Size = new System.Drawing.Size(236, 22);
            CopyTranslationContextMenuButton.Text = "Copy translation";
            CopyTranslationContextMenuButton.Click += new System.EventHandler(form.CopyTranslationContextMenuButton_Click);
            //
            // ListContextMenu
            //
            ListContextMenu.Name = "ListContextMenu";
            ListContextMenu.BackColor = System.Drawing.SystemColors.ScrollBar;
            ListContextMenu.ShowCheckMargin = false;
            ListContextMenu.ShowImageMargin = false;
            ListContextMenu.ForeColor = System.Drawing.SystemColors.MenuText;
            ListContextMenu.Size = new System.Drawing.Size(236, 160);
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
    }
}

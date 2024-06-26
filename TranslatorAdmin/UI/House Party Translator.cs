﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Translator.Core;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.InterfaceImpls;
using Translator.Desktop.Managers;
using Translator.Desktop.UI.Components;
using Translator.Explorer.Graph;
using Translator.Explorer.Story;
using Translator.Explorer.Window;
using Translator.Helpers;

namespace Translator.Desktop.UI
{
    /// <summary>
    /// The main class which handles the UI for the House Party Translator Window
    /// </summary>
    [DesignerCategory("")]
    [SupportedOSPlatform("windows")]
    public sealed class Fenster : Form
    {
        private StoryExplorer? SExplorer;
        private readonly ContextMenuStrip? ListContextMenu;
#nullable disable
        public MenuStrip MainMenu;
        public readonly WinTabController TabControl = new()
        {
            Dock = DockStyle.Fill,
            Location = new Point(0, 27),
            Name = nameof(TabControl),
            SelectedIndex = 0,
            SizeMode = TabSizeMode.Normal,
            TabIndex = 9
        };

        public WinUIHandler UI;
        private static readonly Color background = Utils.background;
        private static readonly Color backgroundDarker = Utils.backgroundDarker;
        private static readonly Color brightText = Utils.brightText;
        private static readonly Color darkText = Utils.darkText;
        private static readonly Color foreground = Utils.foreground;
        private static readonly Color frame = Utils.frame;
        private static readonly Color menu = Utils.menu;
        private readonly CancellationTokenSource CancelTokens = new();
        private readonly LineList CheckListBoxLeft;
        private readonly System.Timers.Timer PresenceTimer = new(2000);
        private WinMenuItem customOpenStoryExplorer;
        private WinMenuItem undoMenuButton;
        private WinMenuItem redoMenuButton;
        private WinMenuItem uploadTemplate;
        private WinMenuItem uploadTemplates;
        private WinMenuItem uploadAllTemplates;
        private WinMenuItem generateTemplateForFile;
        private WinMenuItem generateTemplateForCompleteStory;
        private WinMenuItem createTemplateForFile;
        private WinMenuItem createTemplateForCompleteStory;
        private WinMenuItem exportTemplates;
        private WinMenuItem editToolStripMenuItem;
        private WinMenuItem exitToolStripMenuItem;
        private WinMenuItem fileToolStripMenuItem;
        private WinMenuItem templateToolStripMenuItem;
        private ColoredToolStripDropDown languageToolStripComboBox;
        private WinMenuItem openAllToolStripMenuItem;
        private WinMenuItem openInNewTabToolStripMenuItem;
        private WinMenuItem openToolStripMenuItem;
        private WinMenuItem newFileToolStripMenuItem;
        private WinMenuItem newFileInNewTabToolStripMenuItem;
        private WinMenuItem newFilesForStoryToolStripMenuItem;
        private WinMenuItem overrideCloudSaveToolStripMenuItem;
        private WinMenuItem Recents;
        private WinMenuItem ReloadFileMenuItem;
        private WinMenuItem AutoTranslateUnapproved;
        private WinMenuItem AutoTranslateUntranslated;
        private WinMenuItem AutoTranslateAbort;
        private WinMenuItem ExportAllMissingLinesFolder;
        private WinMenuItem ExportAllMissingLinesFile;
        private WinMenuItem ExportMissingLinesCurrentFile;
        private WinMenuItem replaceToolStripMenuItem;
        private WinMenuItem saveAllToolStripMenuItem;
        private WinMenuItem saveAsToolStripMenuItem;
        private WinMenuItem saveCurrentStringToolStripMenuItem;
        private WinMenuItem saveToolStripMenuItem;
        private WinMenuItem searchAllToolStripMenuItem;
        private WinMenuItem searchToolStripMenuItem;
        private SearchToolStripTextBox searchToolStripTextBox;
        private WinMenuItem settingsToolStripMenuItem;
        private WinMenuItem storyExplorerStripMenuItem;
        private WinToolStripTextBox ToolStripMenuReplaceBox;
        private WinMenuItem toolStripReplaceAllButton;
        private WinMenuItem toolStripReplaceButton;
#nullable restore

        /// <summary>
        /// static constructor for static fields
        /// </summary>
        static Fenster()
        {
            ProgressbarWindow = new ProgressWindow();
            ProgressbarWindow.Status.Text = "Creating UI";
            ProgressbarWindow.Text = "Startup";
            ProgressbarWindow.Show();
            while (!ProgressbarWindow.IsInitialized) ;
            ProgressbarWindow.PerformStep();
        }

        /// <summary>
        /// Constructor for the main window of the translator, starts all other components in the correct order
        /// </summary>
        public Fenster()
        {
            //custom exception handlers to handle mysql exceptions
#if RELEASE || USER_RELEASE
            AppDomain.CurrentDomain.UnhandledException += FensterUnhandledExceptionHandler;
            Application.ThreadException += ThreadExceptionHandler;
#endif
            UI = new(TabControl);

            //initialize ui, controllers, database and so on
            var tab = new WinTab(this);
            CheckForPassword();

            TabManager.Initialize(UI, typeof(WinMenuItem), SoftwareVersionManager.LocalVersion, tab, new WinSettings());
            CheckListBoxLeft = tab.Lines;
            ListContextMenu = CheckListBoxLeft.ContextMenuStrip;

            //init all form components
            InitializeComponent();

            History.HistoryChanged += (s, e) => UpdateHistoryItems();
        }

        public static ProgressWindow ProgressbarWindow { get; private set; }

        /// <summary>
        /// Instance of the Story Explorer, but the owner is checked so only the Storyexplorer class itself can instantiate it.
        /// </summary>
        public StoryExplorer? Explorer
        {
            get
            {
                return SExplorer;
            }
            set
            {
                if (value is not null)
                {
                    SExplorer = value.ParentName == Name
                        ? value
                        : throw new UnauthorizedAccessException("You must only write to this object from within the Explorer class");
                }
            }
        }

        public WinMenuItem FileToolStripMenuItem => fileToolStripMenuItem;
        public ColoredToolStripDropDown LanguageBox => languageToolStripComboBox;
        public WinMenuItem ReplaceAllButton => toolStripReplaceAllButton;
        public ToolStripTextBox ReplaceBox => ToolStripMenuReplaceBox;
        public WinMenuItem ReplaceButton => toolStripReplaceButton;
        public SearchToolStripTextBox SearchBox => searchToolStripTextBox;

        public void CheckListBoxLeft_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            InputHandler.SelectedItemChanged(CheckListBoxLeft);
            if (Explorer is not null
                && Explorer.IsHandleCreated
                && Explorer.StoryName == TabManager.ActiveTranslationManager.StoryName
                && Explorer.FileName == TabManager.ActiveTranslationManager.FileName)
                Explorer.Invoke(TabManager.ActiveTranslationManager.SetHighlightedNode);
        }

        public void Comments_TextChanged(object? sender, EventArgs? e)
        {
            InputHandler.TextChangedCallback(TabControl.SelectedTab.Comments, CheckListBoxLeft.SelectedIndex);
            TabManager.ActiveTranslationManager.UpdateComments();
        }

        /// <summary>
        /// Moves the cursor to the beginning/end of the next word in the specified direction
        /// </summary>
        /// <param name="form"></param>
        /// <param name="toLeft">true if to scan to the left</param>
        /// <returns>true if succeeded</returns>
        public bool MoveCursorInText(bool toLeft)
        {
            Control? focused_control = ActiveControl;
            try
            {
                _ = (WinTextBox?)(TextBox?)focused_control;
            }
            //ignore exception, really intended
            catch { return false; }
            if (focused_control is null) return false;
            var textBox = (TextBox)focused_control;
            if (toLeft)
            {
                ((WinTextBox)textBox).MoveCursorWordLeft();
                return true;
            }
            else
            {
                ((WinTextBox)textBox).MoveCursorWordRight();
                return true;
            }
        }

        public void OpeningContextMenu(object? sender, MouseEventArgs? e)
        {
            if (e is null || ListContextMenu is null)
                return;
            WindowsKeypressManager.OpenContextMenu(ListContextMenu, e);
        }

        public void TextBoxRight_TextChanged(object? sender, EventArgs? e)
        {
            InputHandler.TextChangedCallback(TabControl.SelectedTab.Translation, CheckListBoxLeft.SelectedIndex);
            TabManager.ActiveTranslationManager.UpdateTranslationString();
        }

        public void TextContextOpened(object? sender, EventArgs? e)
        {
            if (sender is null) return;
            if (sender is ITextBox textBox)
            {
                InputHandler.PrepareTextChanged(textBox);
            }
        }

        /// <summary>
        /// Override to intercept the Keystrokes windows sends us.
        /// </summary>
        /// <param name="msg">The message containing relevant info</param>
        /// <param name="keyData">List of all Keys that were pressed in this event</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return WindowsKeypressManager.MainKeyPressHandler(ref msg, keyData, CancelTokens) || base.ProcessCmdKey(ref msg, keyData);
        }

        private void CloseTab_Click(object? sender, MouseEventArgs? e)
        {
            if (sender is WinTabController tabController)
            {
                if (e?.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < TabControl.TabCount; i++)
                    {
                        if (TabControl.GetTabRect(i).Contains(e.Location))
                        {
                            TabManager.CloseTab(tabController.TabPages[i]);
                            return;
                        }
                    }
                }
            }
        }

        public void CreateStoryExplorer(bool autoOpen, CancellationTokenSource tokenSource)
        {
            if (TabManager.ActiveTranslationManager is null) return;

            //get currently active translation manager
            TranslationManager manager = TabManager.ActiveTranslationManager;

            bool isStory = manager.StoryName.ToLowerInvariant() == manager.FileName.ToLowerInvariant();
            try
            {
                DialogResult openAll = Msg.InfoYesNoCancel(
                        $"Do you want to explore all files for the selected story{(autoOpen ? " (" + manager.StoryName + ")" : string.Empty)} or only the selected file{(autoOpen ? " (" + manager.FileName + ")" : string.Empty)}?\nNote: more files means slower layout, but viewing performance is about the same.",
                        "All files?"
                        );

                if (openAll == DialogResult.Cancel)
                {
                    return;
                }
                var explorer = new StoryExplorer(isStory, autoOpen, manager.FileName, manager.StoryName, tokenSource.Token, App.MainForm);

                //todo keep track of all instances and threads/tasks created in it so we can close it
                Task.Run(() =>
                {
                    //def answer set to no because true for opening a single one is needed
                    explorer.Initialize(openAll == DialogResult.No);

                    manager.SetHighlightedNode();
                }).ContinueWith((result) =>
                {
                    if (result.Exception is not null)
                    {
                        if (result.Exception.InnerException is JsonReaderException ex)
                        {
                            Msg.ErrorOk("Story file corrupt: " + ex.Message + "\n\n Please select the correct file manually!");
                        }
                        else
                        {
                            Msg.ErrorOk("Story file corrupt: " + result.Exception.Message + "\n\n Please select the correct file manually!");
                        }
                        explorer.Close();
                    }
                });
                if (!explorer.IsDisposed)
                {
                    explorer.Show();
                    Explorer = explorer;
                }
            }
            catch (OperationCanceledException)
            {
                LogManager.Log("Explorer closed during creation", LogManager.Level.Warning);
                return;
            }
        }

        private void FensterUnhandledExceptionHandler(object? sender, UnhandledExceptionEventArgs? e)
        {
            if (e is null) { LogManager.Log("No eventargs on unhandled exception", LogManager.Level.Error); }
            else
            {
                LogManager.Log(e.ExceptionObject?.ToString() ?? "ExceptionObject is null", LogManager.Level.Error);

                if (e.ExceptionObject is null) return;
                if (e.ExceptionObject is LanguageHelper.LanguageException)
                {
                    Msg.WarningOk("Please select a language first! (Dropdown in the top menu bar)");
                    LogManager.Log("Aborted, no language selected.");
                }
                else if (e.ExceptionObject is Exception exception)
                {
                    Utils.DisplayExceptionMessage(exception.Message);
                }
                else
                {
                    Utils.DisplayExceptionMessage(e.ExceptionObject.ToString() ?? "ExceptionObject is null");
                }
            }
            UI.ClearUserWaitCount();
        }

        private void ThreadExceptionHandler(object? sender, ThreadExceptionEventArgs? e)
        {
            if (e is null) { LogManager.Log("No eventargs on unhandled exception", LogManager.Level.Error); return; }
            LogManager.Log(e.Exception.ToString(), LogManager.Level.Error);
            if (e.Exception is LanguageHelper.LanguageException)
            {
                Msg.WarningOk("Please select a language first! (Dropdown in the top menu bar)");
                LogManager.Log("Aborted, no language selected.");
            }
            else
            {
                Utils.DisplayExceptionMessage(e.Exception.Message);
            }
            UI.ClearUserWaitCount();
        }

        private void CheckForPassword()
        {
            if (Settings.Default.DbPassword.Length > 0) return;

            var Passwordbox = new Password();
            DialogResult passwordResult = Passwordbox.ShowDialog(this);
            if (passwordResult == DialogResult.OK)
            {
                Settings.Default.DbPassword = Passwordbox.GetPassword();
                Settings.Default.Save();
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(Fenster));
            SuspendLayout();

            // generateTemplateForFile
            generateTemplateForFile = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(generateTemplateForFile),
                Size = new Size(236, 22),
                Text = "Generate o&ne template file",
                ToolTipText = "Generates and uploads the template for a single file"
            };
            generateTemplateForFile.Click += (object? sender, EventArgs e) => SaveAndExportManager.GenerateTemplateForSingleFile(true);

            // ExportAllMissingLinesFolder
            ExportAllMissingLinesFolder = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(ExportAllMissingLinesFolder),
                Size = new Size(236, 22),
                Text = "Export all &missing lines ->folder",
                ToolTipText = "Exports all missing lines for the current story into a folder, one filer per character"
            };
            ExportAllMissingLinesFolder.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.ExportMissingLinesForCurrentStory(true);

            // ExportAllMissingLinesFile
            ExportAllMissingLinesFile = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(ExportAllMissingLinesFile),
                Size = new Size(236, 22),
                Text = "Export all missing lines ->&one file",
                ToolTipText = "Exports all missing lines for the current story into one big file"
            };
            ExportAllMissingLinesFile.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.ExportMissingLinesForCurrentStory(false);

            // ExportMissingLinesCurrentFile
            ExportMissingLinesCurrentFile = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(ExportMissingLinesCurrentFile),
                Size = new Size(236, 22),
                Text = "E&xport missing lines from the current file",
                ToolTipText = "Exports all missing lines for the currently opened and selected file into a new file"
            };
            ExportMissingLinesCurrentFile.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.ExportMissinglinesForCurrentFile();

            // uploadTemplate
            uploadTemplate = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(uploadTemplate),
                Size = new Size(236, 22),
                Text = "&Upload a template file",
                ToolTipText = "Upload a single template file (no official stories)"
            };
            uploadTemplate.Click += (object? sender, EventArgs e) => SaveAndExportManager.UploadTemplate();

            // uploadTemplates
            uploadTemplates = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(uploadTemplates),
                Size = new Size(236, 22),
                Text = "U&pload all template files for a story",
                ToolTipText = "Upload all template files for a story (no official stories)"
            };
            uploadTemplates.Click += (object? sender, EventArgs e) => SaveAndExportManager.UploadTemplates();

            // uploadAllTemplates
            uploadAllTemplates = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(uploadAllTemplates),
                Size = new Size(236, 22),
                Text = "Up&load all official template files",
                ToolTipText = "Upload all template files for all official stories and UI (only in admin mode)"
            };
            uploadAllTemplates.Click += (object? sender, EventArgs e) => SaveAndExportManager.UploadOfficialTemplates();

            // generateTemplateForCompleteStory
            generateTemplateForCompleteStory = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(generateTemplateForCompleteStory),
                Size = new Size(236, 22),
                Text = "&Generate all template files",
                ToolTipText = "Generates and uploads templates for a complete story"
            };
            generateTemplateForCompleteStory.Click += (object? sender, EventArgs e) => SaveAndExportManager.GenerateTemplateForAllFiles(true);

            // createTemplateForFile
            createTemplateForFile = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(createTemplateForFile),
                Size = new Size(236, 22),
                Text = "Create &one template file",
                ToolTipText = "Creates the template locally for a single file"
            };
            createTemplateForFile.Click += (object? sender, EventArgs e) => SaveAndExportManager.GenerateTemplateForSingleFile(false);

            // createTemplateForCompleteStory
            createTemplateForCompleteStory = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(createTemplateForCompleteStory),
                Size = new Size(236, 22),
                Text = "&Create all template files",
                ToolTipText = "Creates templates locally for a complete story"
            };
            createTemplateForCompleteStory.Click += (object? sender, EventArgs e) => SaveAndExportManager.GenerateTemplateForAllFiles(false);

            // undoMenuButton
            undoMenuButton = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(undoMenuButton),
                Size = new Size(236, 22),
                Text = "&Undo",
                ToolTipText = "Undoes the latest action",
                Enabled = false
            };
            undoMenuButton.Click += (object? sender, EventArgs e) => History.Undo();

            // redoMenuButton
            redoMenuButton = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(redoMenuButton),
                Size = new Size(236, 22),
                Text = "&Redo",
                ToolTipText = "Redoes the latest undone action",
                Enabled = false
            };
            redoMenuButton.Click += (object? sender, EventArgs e) => History.Redo();

            // exportTemplates
            exportTemplates = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(exportTemplates),
                Size = new Size(236, 22),
                Text = "Export &template files",
                ToolTipText = "Exports templates for a complete story or single file"
            };
            exportTemplates.Click += (object? sender, EventArgs e) => SaveAndExportManager.ExportTemplatesForStoryOrFile();

            // searchToolStripMenuItem
            searchToolStripMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(searchToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Search",
                ToolTipText = "Selects the search bar"
            };
            searchToolStripMenuItem.Click += new EventHandler(SearchToolStripMenuItem_click);

            // searchAllToolStripMenuItem
            searchAllToolStripMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(searchAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Search &all",
                ToolTipText = "Selects the search bar with the search all open files mode"
            };
            searchAllToolStripMenuItem.Click += new EventHandler(SearchAllToolStripMenuItem_click);

            // ReloadFileMenuItem
            ReloadFileMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(ReloadFileMenuItem),
                Size = new Size(236, 22),
                Text = "Reload selecte&d file",
                ToolTipText = "Reloads the currently selected file"
            };
            ReloadFileMenuItem.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.ReloadFile();

            // AutoTranslateUnapproved
            AutoTranslateUnapproved = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(AutoTranslateUnapproved),
                Size = new Size(236, 22),
                Text = "Automatically translate all una&pproved lines",
                ToolTipText = "Translates all unapproved lines with Libretranslate"
            };
            AutoTranslateUnapproved.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.RequestAutomaticTranslationForAllUnapproved();

            // AutoTranslateUntranslated
            AutoTranslateUntranslated = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(AutoTranslateUntranslated),
                Size = new Size(236, 22),
                Text = "Automatically &translate all untranslated lines",
                ToolTipText = "Translates all untranslated lines with Libretranslate"
            };
            AutoTranslateUntranslated.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.RequestAutomaticTranslationForAllUntranslated();

            // AutoTranslateAbort
            AutoTranslateAbort = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(AutoTranslateAbort),
                Size = new Size(236, 22),
                Text = "Abort all automati&c translations",
                ToolTipText = "Aborts all running automatic translations"
            };
            AutoTranslateAbort.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.AbortAllAutomaticTranslations();

            // replaceToolStripMenuItem
            replaceToolStripMenuItem = new WinMenuItem()
            {
                ImageTransparentColor = Color.Magenta,
                Name = nameof(replaceToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "R&eplace",
                ToolTipText = "Opens the searchbar in replace mode"
            };
            replaceToolStripMenuItem.Click += (object? sender, EventArgs e) => UI.ToggleReplaceBar();

            // openToolStripMenuItem
            openToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Open",
                ToolTipText = "Opens a dialog to select a file"
            };
            openToolStripMenuItem.Click += (object? sender, EventArgs e) =>
            {
                TabManager.OpenNewFiles();
                DiscordPresenceManager.Update();
            };

            // newFileToolStripMenuItem
            newFileToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(newFileToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&New file",
                ToolTipText = "Opens a dialog to create a new file"
            };
            newFileToolStripMenuItem.Click += (object? sender, EventArgs e) => CreateNewFile();

            // newFileInNewTabToolStripMenuItem
            newFileInNewTabToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(newFileInNewTabToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "New &file in new tab",
                ToolTipText = "Opens a dialog to create a new file and open it in a new tab"
            };
            newFileInNewTabToolStripMenuItem.Click += (object? sender, EventArgs e) => CreateNewFileInNewTab();

            // newFilesForStoryToolStripMenuItem
            newFilesForStoryToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(newFilesForStoryToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "New files for &a story",
                ToolTipText = "Opens a dialog to create a all files for a story and opens them in tabs"
            };
            newFilesForStoryToolStripMenuItem.Click += (object? sender, EventArgs e) => CreateNewFilesForStory();

            // openAllToolStripMenuItem
            openAllToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "O&pen all",
                ToolTipText = "Opens a dialog to select a file, all others will be discovered automatically. Usually."
            };
            openAllToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.OpenAllTabs();

            // openInNewTabToolStripMenuItem
            openInNewTabToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("openToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(openInNewTabToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Open in new &tab",
                ToolTipText = "Opens a dialog to select a file"
            };
            openInNewTabToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.OpenNewTab();

            // Recents
            Recents = new WinMenuItem()
            {
                Enabled = false,
                Name = nameof(Recents),
                ShowShortcutKeys = false,
                Size = new Size(236, 22),
                Text = "Recents:",
                ToolTipText = "The 5 most recently opened files"
            };

            // saveToolStripMenuItem
            saveToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("saveToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Save",
                ToolTipText = "Saves the currently selected file to disk and online."
            };
            saveToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.SaveFile();

            // saveAllToolStripMenuItem
            saveAllToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("saveToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAllToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Sa&ve all",
                ToolTipText = "Saves all currently opened files to disk and online."
            };
            saveAllToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.SaveAllTabs();

            // saveAsToolStripMenuItem
            saveAsToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("saveAsToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(saveAsToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Save &as",
                ToolTipText = "Saves the currently selected file online and to disk with a new name."
            };
            saveAsToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.SaveFileAs();

            // overrideCloudSaveToolStripMenuItem
            overrideCloudSaveToolStripMenuItem = new WinMenuItem()
            {
                Image = (Image?)resources.GetObject("saveAsToolStripMenuItem.Image"),
                ImageTransparentColor = Color.Magenta,
                Name = nameof(overrideCloudSaveToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "Override &cloud save",
                ToolTipText = "Overrides the online state with the actual contents of the currently opened file. Only available in the Admin version."
            };
            overrideCloudSaveToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.OverrideCloudSave();

            // exitToolStripMenuItem
            exitToolStripMenuItem = new WinMenuItem()
            {
                Name = nameof(exitToolStripMenuItem),
                Size = new Size(236, 22),
                Text = "&Exit",
                ToolTipText = "Exits the application gracefully."
            };
            exitToolStripMenuItem.Click += (object? sender, EventArgs e) => UI.SignalAppExit();

            // saveCurrentStringToolStripMenuItem
            saveCurrentStringToolStripMenuItem = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(saveCurrentStringToolStripMenuItem),
                Size = new Size(122, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Save selected string",
                ToolTipText = "Saves the currently selected string/line to the online database."
            };
            saveCurrentStringToolStripMenuItem.Click += (object? sender, EventArgs e) => TabManager.ActiveTranslationManager.SaveCurrentString();

            // searchToolStripTextBox
            searchToolStripTextBox = new SearchToolStripTextBox()
            {
                BackColor = menu,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Name = nameof(searchToolStripTextBox),
                Size = new Size(150, 23),
                MinimumSize = new Size(150, 23),
                MaximumSize = new Size(150, 23),
                Margin = new Padding(1),
                ToolTipText = "Enter your searchterm here.",
                PlaceHoldeText = "search here"
            };
            searchToolStripTextBox.TextChanged += new EventHandler(SearchToolStripTextBox_TextChanged);

            // ToolStripMenuReplaceBox
            ToolStripMenuReplaceBox = new WinToolStripTextBox()
            {
                BackColor = menu,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Name = nameof(ToolStripMenuReplaceBox),
                Size = new Size(100, 23),
                AutoSize = true,
                Margin = new Padding(1),
                ToolTipText = "Enter the text to replace with here.",
                Visible = false
            };

            // toolStripReplaceAllButton
            toolStripReplaceAllButton = new WinMenuItem()
            {
                BackColor = menu,
                ForeColor = darkText,
                Name = nameof(toolStripReplaceAllButton),
                Size = new Size(63, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Replace all",
                ToolTipText = "Replaces all occurances of the searchterm with the text from the right text field.",
                Visible = false
            };
            toolStripReplaceAllButton.Click += (object? sender, EventArgs e) => TabManager.ReplaceAll();

            // toolStripReplaceButton
            toolStripReplaceButton = new WinMenuItem()
            {
                BackColor = menu,
                ForeColor = darkText,
                Name = nameof(toolStripReplaceButton),
                Size = new Size(63, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Replace",
                ToolTipText = "Replaces the occurance of the search term in the currently selected line with the text from the replace box, if applicable.",
                Visible = false
            };
            toolStripReplaceButton.Click += (object? sender, EventArgs e) => TabManager.Replace();

            // languageToolStripComboBox
            languageToolStripComboBox = new ColoredToolStripDropDown()
            {
                BackColor = menu,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Name = nameof(languageToolStripComboBox),
                Size = new Size(50, 23),
                MaxLength = 4,
                AutoSize = true,
                Margin = new Padding(1),
                ToolTipText = "Select your language here"
            };
            languageToolStripComboBox.Items.AddRange(LanguageHelper.ShortLanguages);
            languageToolStripComboBox.SelectedItem = Settings.Default.Language;
            languageToolStripComboBox.SelectedIndexChanged += (object? sender, EventArgs e) => InputHandler.SelectedLanguageChanged();

            // storyExplorerStripMenuItem
            storyExplorerStripMenuItem = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(storyExplorerStripMenuItem),
                Size = new Size(118, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Auto StoryExplorer",
                ToolTipText = "Starts the StoryExplorer with the currently selected character/story loaded."
            };
            storyExplorerStripMenuItem.Click += (object? sender, EventArgs e) => CreateStoryExplorer(true, CancelTokens);

            // customOpenStoryExplorer
            customOpenStoryExplorer = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(customOpenStoryExplorer),
                Size = new Size(121, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Open StoryE&xplorer",
                ToolTipText = "Opens the StoryExplorer and gives you the choice which file to open."
            };
            customOpenStoryExplorer.Click += (object? sender, EventArgs e) => CreateStoryExplorer(false, CancelTokens);

            // settingsToolStripMenuItem
            settingsToolStripMenuItem = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(settingsToolStripMenuItem),
                Size = new Size(61, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "Settin&gs",
                ToolTipText = "Opens a list of all settings for the translator, see the github for reference."
            };
            settingsToolStripMenuItem.Click += (object? sender, EventArgs e) => WindowsKeypressManager.ShowSettings();

            // editToolStripMenuItem
            editToolStripMenuItem = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(editToolStripMenuItem),
                Size = new Size(35, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Edit",
                ToolTipText = "All relevant controls for editing a file, plus special controls"
            };
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                undoMenuButton,
                redoMenuButton,
                new WinMenuSeperator(),
                searchToolStripMenuItem,
                searchAllToolStripMenuItem,
                new WinMenuSeperator(),
                replaceToolStripMenuItem,
                AutoTranslateUnapproved,
                AutoTranslateUntranslated,
                new WinMenuSeperator(),
                ReloadFileMenuItem,
                AutoTranslateAbort,
                new WinMenuSeperator(),
                overrideCloudSaveToolStripMenuItem
            });

            // fileToolStripMenuItem
            fileToolStripMenuItem = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(fileToolStripMenuItem),
                Size = new Size(35, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&File",
                ToolTipText = "All relevant controls for opening and saving a file"
            };
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                newFileToolStripMenuItem,
                newFileInNewTabToolStripMenuItem,
                newFilesForStoryToolStripMenuItem,
                new WinMenuSeperator(),
                openToolStripMenuItem,
                openAllToolStripMenuItem,
                openInNewTabToolStripMenuItem,
                new WinMenuSeperator(),
                Recents,
                new WinMenuSeperator(),
                saveToolStripMenuItem,
                saveAllToolStripMenuItem,
                saveAsToolStripMenuItem,
                new WinMenuSeperator(),
                exitToolStripMenuItem
            });

            // templateToolStripMenuItem
            templateToolStripMenuItem = new WinMenuItem()
            {
                BackColor = menu,
                Name = nameof(templateToolStripMenuItem),
                Size = new Size(60, 23),
                AutoSize = false,
                Margin = new Padding(1),
                Text = "&Template",
                ToolTipText = "All relevant controls for generating or exporting templates"
            };
            templateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                uploadTemplate,
                uploadTemplates,
                uploadAllTemplates,
                new WinMenuSeperator(),
                generateTemplateForFile,
                generateTemplateForCompleteStory,
                createTemplateForFile,
                createTemplateForCompleteStory,
                new WinMenuSeperator(),
                exportTemplates,
                new WinMenuSeperator(),
                ExportAllMissingLinesFolder,
                ExportAllMissingLinesFile,
                ExportMissingLinesCurrentFile,
            });

            // MainMenu
            MainMenu = new MenuStrip()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSize = true,
                BackColor = backgroundDarker,
                Dock = DockStyle.Top,
                ForeColor = backgroundDarker,
                LayoutStyle = ToolStripLayoutStyle.Flow,
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Name = nameof(MainMenu),
                TabIndex = 17
            };
            MainMenu.SuspendLayout();
            MainMenu.Items.AddRange(new ToolStripItem[] {
                fileToolStripMenuItem,
                editToolStripMenuItem,
                templateToolStripMenuItem,
                saveCurrentStringToolStripMenuItem,
                searchToolStripTextBox,
                ToolStripMenuReplaceBox,
                toolStripReplaceButton,
                toolStripReplaceAllButton,
                languageToolStripComboBox,
                storyExplorerStripMenuItem,
                customOpenStoryExplorer,
                settingsToolStripMenuItem
            });

            TabControl.SuspendLayout();
            TabControl.SelectedIndexChanged += new EventHandler(MainTabControl_SelectedIndexChanged);
            TabControl.MouseClick += new MouseEventHandler(CloseTab_Click);

            // Fenster
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = backgroundDarker;
            ClientSize = new Size(1400, 760);
            Controls.Add(TabControl);
            Controls.Add(MainMenu);
            MinimumSize = new Size(640, 470);
            Name = nameof(Fenster);
            ShowIcon = false;
            Icon = Translator.Explorer.Properties.Resources.wumpus_smoll;
#if DEBUG || DEBUG_ADMIN
            Text = "Translator (DEBUG)";
#else
            Text = "Translator";
#endif
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += new FormClosingEventHandler(OnFormClosing);
            MouseUp += new MouseEventHandler(TextContextOpened);
            Shown += new EventHandler(OnFormShown);
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            TabControl.ResumeLayout(false);
            ResumeLayout();
            PerformLayout();
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data is null) return;
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]?)drgevent.Data.GetData(DataFormats.FileDrop) ?? Array.Empty<string>();
                foreach (string filePath in files)
                {
                    TabManager.OpenInNewTab(filePath);
                }
            }
            else if (drgevent.Data.GetDataPresent(DataFormats.OemText)
                || drgevent.Data.GetDataPresent(DataFormats.Text)
                || drgevent.Data.GetDataPresent(DataFormats.UnicodeText))

            {
                string? str = (string?)drgevent.Data.GetData(DataFormats.UnicodeText, true);
                if (string.IsNullOrEmpty(str)) return;

                TabControl.SelectedTab.Translation.Text += str;
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data is null) return;
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                || e.Data.GetDataPresent(DataFormats.UnicodeText)
                || e.Data.GetDataPresent(DataFormats.OemText)
                || e.Data.GetDataPresent(DataFormats.Text)
                ? DragDropEffects.All
                : DragDropEffects.None;
        }

        private void MainTabControl_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            TabManager.OnSwitchTabs();
            WindowsKeypressManager.SelectedTabChanged();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs? e)
        {
            Settings.Default.Save();
            LogManager.SaveLogFile();

            //prevent discord from getting angry
            DiscordPresenceManager.DeInitialize();

            RecentsManager.SaveRecents();

            try
            {
                CancelTokens.Cancel();
                CancelTokens.Dispose();
            }
            catch { }

            //show save unsaved changes dialog
            TabManager.ShowAutoSaveDialog();

        }

        private void OnFormShown(object? sender, EventArgs? e)
        {
            //check for update and replace if we want one
            ProgressbarWindow.Status.Text = "Checking for an update";
            _ = Task.Run(SoftwareVersionManager.ReplaceFileIfNew);
            ProgressbarWindow.PerformStep();

            ProgressbarWindow.Status.Text = "Finishing startup";
            TabManager.FinalizeInitializer();

            Text = DataBase.AppTitle;
            ProgressbarWindow.PerformStep();

            LogManager.Log("Application initializing...");
            ProgressbarWindow.Status.Text = "Starting discord worker";

            ProgressbarWindow.PerformStep();

            ProgressbarWindow.Status.Text = "Loading recents";
            //open most recent after db is initialized
            UpdaterecentFileMenu();
            RecentsManager.OpenMostRecent();

            //start timer to update presence
            PresenceTimer.Elapsed += (sender_, args) =>
            {
                Invoke(DiscordPresenceManager.Update);
            };
            PresenceTimer.Start();

            DiscordPresenceManager.Update();
            //done
            ProgressbarWindow.PerformStep();
            LogManager.Log($"Application initialized with app version:{SoftwareVersionManager.LocalVersion} db version:{(DataBase.IsOnline ? DataBase.DBVersion : "*offline*")} story version:{Settings.Default.FileVersion}");

            //hide override button if not in advanced mode
            if (!Settings.Default.AdvancedModeEnabled)
            {
                overrideCloudSaveToolStripMenuItem.Enabled = false;
                uploadAllTemplates.Enabled = false;
            }

            ProgressbarWindow.Hide();
        }

        private void SearchAllToolStripMenuItem_click(object? sender, EventArgs? e)
        {
            searchToolStripTextBox.Focus();
            if (searchToolStripTextBox.Text.Length == 0)
            {
                searchToolStripTextBox.Text = "?search here";
                searchToolStripTextBox.SelectionStart = 1;
            }
        }

        private void SearchToolStripMenuItem_click(object? sender, EventArgs? e)
        {
            InputHandler.FocusSearch();
        }

        private void SearchToolStripTextBox_TextChanged(object? sender, EventArgs? e)
        {
            if (sender is null) return;
            TabManager.Search();
        }

        private static void CreateNewFile()
        {
            var dialog = new NewFileSelector();
            string path = SaveAndExportManager.CreateNewFile(dialog);
            if (path == string.Empty) return;

            if (dialog.FileName == string.Empty)
            {
                foreach (string file in dialog.files[Utils.ExtractStoryName(path!)])
                {
                    File.OpenWrite(Path.Combine(path!, file + ".txt")).Close();
                }
                TabManager.OpenAllTabs(path!);
            }
            else
            {
                TabManager.OpenFile(path);
                DiscordPresenceManager.Update();
            }
        }

        private static void CreateNewFileInNewTab()
        {
            var dialog = new NewFileSelector();
            string path = SaveAndExportManager.CreateNewFile(dialog);
            if (path == string.Empty) return;

            if (dialog.FileName == string.Empty)
            {
                foreach (string file in dialog.files[Utils.ExtractStoryName(path!)])
                {
                    File.OpenWrite(Path.Combine(path!, file + ".txt")).Close();
                }
                TabManager.OpenAllTabs(path!);
            }
            else
            {
                TabManager.OpenInNewTab(path);
            }
        }

        private static void CreateNewFilesForStory()
        {
            var dialog = new NewFileSelector(true);
            PopupResult result = dialog.ShowDialog();
            if (result != PopupResult.OK) return;

            string? path = Utils.SelectSaveLocation("Select a folder to place the file into, missing folders will be created.", file: dialog.StoryName, checkFileExists: false, checkPathExists: false, extension: string.Empty);
            if (path == string.Empty || path is null) return;

            if (dialog.StoryName == path.Split('\\')[^2] && Path.HasExtension(path))
                path = Path.GetDirectoryName(path);
            else if (dialog.StoryName == path.Split('\\')[^1])
                _ = Directory.CreateDirectory(path);

            if (path == string.Empty) return;

            foreach (string file in dialog.files[dialog.StoryName])
            {
                File.OpenWrite(Path.Combine(path!, file + ".txt")).Close();
            }
            TabManager.OpenAllTabs(path!);
        }

        public void UpdaterecentFileMenu()
        {
            IMenuItem[] items = RecentsManager.GetRecents();
            int recentsStart;

            //find start of recents
            for (recentsStart = 4; recentsStart < FileToolStripMenuItem.DropDownItems.Count; recentsStart++)
            {
                if (FileToolStripMenuItem.DropDownItems[recentsStart] is WinMenuSeperator) break;
            }
            //update menu
            if (items.Length > 0 && FileToolStripMenuItem.DropDownItems.Count < FileToolStripMenuItem.DropDownItems.Count + 6)
            {
                recentsStart += 2;
                for (int i = 0; i < items.Length; i++)
                {
                    //we replace until we hit seperator, then we insert
                    if (FileToolStripMenuItem.DropDownItems[recentsStart + i] is not WinMenuSeperator)
                    {
                        FileToolStripMenuItem.DropDownItems.RemoveAt(recentsStart + i);
                    }

                    FileToolStripMenuItem.DropDownItems.Insert(recentsStart, (WinMenuItem)items[items.Length - i - 1]);
                }

                if (FileToolStripMenuItem.DropDownItems[recentsStart + items.Length] is not WinMenuSeperator)
                    FileToolStripMenuItem.DropDownItems.Insert(recentsStart + items.Length, new WinMenuSeperator());
                RecentsManager.SaveRecents();
                //for the name update stuff
                recentsStart -= 2;
            }

            FileToolStripMenuItem.DropDownItems[recentsStart + 1].Text = items.Length == 0 ? "No Recents" : "Recents:";
        }

        public void UpdateHistoryItems()
        {
            undoMenuButton.DropDownItems.Clear();
            redoMenuButton.DropDownItems.Clear();

            var commands = History.GetLastFiveActions();
            undoMenuButton.Enabled = commands.Count > 0;
            int i = 0;
            foreach (var cmd in commands)
            {
                WinMenuItem cmdItem = new()
                {
                    Name = cmd.GetType().Name + i,
                    Size = new Size(236, 22),
                    Text = "(" + i + ") " + cmd.Description,
                    ToolTipText = cmd.DetailedDescription,
                };
                cmdItem.Click += (s, e) => History.Undo(i + 1);
                undoMenuButton.DropDownItems.Add(cmdItem);
                i++;
            }

            commands = History.GetNextFiveActions();
            redoMenuButton.Enabled = commands.Count > 0;
            i = 0;
            foreach (var cmd in commands)
            {
                WinMenuItem cmdItem = new()
                {
                    Name = cmd.GetType().Name + i,
                    Size = new Size(236, 22),
                    Text = "(" + i + ") " + cmd.Description,
                    ToolTipText = cmd.DetailedDescription,
                };
                cmdItem.Click += (s, e) => History.Redo(i + 1);
                redoMenuButton.DropDownItems.Add(cmdItem);
                i++;
            }

        }

        public static void CreateContextExplorer()
        {
            string story = TabManager.ActiveTranslationManager.StoryName;
            string file = TabManager.ActiveTranslationManager.FileName;
            bool isStory = story == file;
            var context = new ContextExplorer(story, file);

            string storyPathMinusStory = Directory.GetParent(WinSettings.StoryPath)?.FullName ?? string.Empty;
            string filepath = isStory
                ? Path.Combine(storyPathMinusStory, story, $"{file}.story")
                : Path.Combine(storyPathMinusStory, story, $"{file}.character");
            var contextProvider = new ContextProvider(
                new(),
                isStory,
                false,
                file,
                story,
                filepath);

            NodeList nodes = contextProvider.GetTemplateNodes();
            var currentNode = nodes.Find((Node n) => n.ID == TabManager.ActiveTranslationManager.SelectedId);
            if (currentNode is null) return;
            context.SetLines(currentNode);
            context.Show();
        }
    }
}
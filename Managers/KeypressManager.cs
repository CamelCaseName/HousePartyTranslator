using HousePartyTranslator.Helpers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// Class that handles all keyboard presses and calls the appropiate methods if a hotkey was detected
    /// </summary>
    internal static class KeypressManager
    {
        private static TextBox lastChangedTextBox;
        private static string lastText;
        private static int lastIndex = Properties.Settings.Default.recent_index;

        public static void ApprovedButtonChanged()
        {
            TabManager.ActiveTranslationManager.ApprovedButtonHandler();
        }

        public static void AutoTranslate()
        {
            TabManager.ActiveTranslationManager.RequestedAutomaticTranslation();
        }

        public static void CheckItemChanged()
        {
            TabManager.ActiveTranslationManager.ApproveIfPossible(false);
        }

        public static async Task<StoryExplorerForm.StoryExplorer> CreateStoryExplorer(bool autoOpen, Form explorerParent, CancellationTokenSource tokenSource)
        {
            explorerParent.UseWaitCursor = true;

            // Use ParallelOptions instance to store the CancellationToken
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = tokenSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount - 1
            };

            //get currently active translation manager
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            bool isStory = translationManager.StoryName.ToLowerInvariant() == translationManager.FileName.ToLowerInvariant();
            try
            {
                //create an id to differentiate between the different calculated layouts later
                string FileId = translationManager.StoryName + translationManager.FileName + DataBase.DBVersion;
                string savedNodesPath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{FileId}.json");
                DialogResult result = DialogResult.OK;
                if (!File.Exists(savedNodesPath))
                {
                    result = Msg.WarningOkCancel("If you never opened this character in the explorer before, " +
                       "be aware that this can take some time (5+ minutes) and is really cpu intensive. " +
                       "This only has to be done once for each character!\n" +
                       "You may notice stutters and stuff hanging. ",
                       "Warning!");
                }
                //inform user this is going to take some time
                if (result == DialogResult.OK)
                {
                    var explorer = new StoryExplorerForm.StoryExplorer(isStory, autoOpen, translationManager.FileName, translationManager.StoryName, explorerParent, parallelOptions)
                    {
                        UseWaitCursor = true
                    };

                    //task to offload initialization workload
                    var explorerTask = Task.Run(() =>
                    {
                        explorer.Initialize();
                        return true;
                    });

                    if (await explorerTask)
                    {
                        if (!explorer.IsDisposed)
                        {//reset cursor and display window
                            explorer.UseWaitCursor = false;
                            explorer.Invalidate();
                            explorer.Show();
                        }

                        translationManager.SetHighlightedNode();
                        explorerParent.UseWaitCursor = false;
                        return explorer;
                    }
                    else
                    {
                        explorerParent.UseWaitCursor = false;
                        return null;
                    }
                }
                else
                {
                    explorerParent.UseWaitCursor = false;
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                explorerParent.UseWaitCursor = false;
                LogManager.Log("Explorer closed during creation");
                return null;
            }
        }

        /// <summary>
        /// Detects for hotkeys used, if they are consumed we return true, else false is returned. Call TextChangedCallback if this returned false and the base.WndProc has finished to call back on text changes.
        /// </summary>
        /// <param name="msg">Windows message contaning the info on the event.</param>
        /// <param name="keyData">Keydata containing all currently pressed keys.</param>
        /// <param name="presence"></param>
        /// <param name="parent"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool MainKeyPressHandler(ref Message msg, Keys keyData, DiscordPresenceManager presence, Form parent, CancellationTokenSource tokenSource)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            switch (keyData)
            {
                //handle enter as jumping to first search result if searched something, and focus is not on text editor.
                case (Keys.Enter):
                    return TabManager.ActiveTranslationManager.SelectNextResultIfApplicable();

                //set selected string as search string and place cursor in search box
                case (Keys.Control | Keys.F):
                    if (TabManager.ActiveProperties.TranslationTextBox.SelectedText.Length > 0)
                    {
                        TabManager.ActiveProperties.SearchBox.Text = TabManager.ActiveProperties.TranslationTextBox.SelectedText;
                    }
                    TabManager.ActiveProperties.SearchBox.Focus();
                    return true;

                //search, but also with replacing
                case (Keys.Control | Keys.Shift | Keys.F):
                    ToggleReplaceUI();
                    return true;

                //save current file
                case (Keys.Control | Keys.S):
                    TabManager.ActiveTranslationManager.SaveFile();
                    return true;

                //save current string
                case (Keys.Control | Keys.Shift | Keys.S):
                    TabManager.ActiveTranslationManager.SaveCurrentString();
                    return true;

                //saves all open tabs
                case (Keys.Alt | Keys.Shift | Keys.S):
                    _ = TabManager.SaveAllTabs();
                    return true;

                //reload currently loaded file
                case (Keys.Control | Keys.R):
                    TabManager.ActiveTranslationManager.ReloadFile();
                    return true;

                //select string above current selection
                case (Keys.Control | Keys.Up):
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex > 0) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex--;
                    return true;

                //select string below current selection
                case (Keys.Control | Keys.Down):
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count - 1) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //switch tab to the left
                case (Keys.Alt | Keys.Left):
                    if (TabManager.TabControl.TabCount > 1) TabManager.SwitchToTab(TabManager.TabControl.SelectedIndex - 1);
                    return true;

                //switch tab to the right
                case (Keys.Alt | Keys.Right):
                    if (TabManager.TabControl.TabCount > 1) TabManager.SwitchToTab(TabManager.TabControl.SelectedIndex + 1);
                    return true;

                //save translation and move down one
                case (Keys.Control | Keys.Enter):
                    TabManager.ActiveTranslationManager.SaveCurrentString();
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count - 1) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //save translation, approve and move down one
                case (Keys.Control | Keys.Shift | Keys.Enter):
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex >= 0) TabManager.ActiveProperties.CheckListBoxLeft.SetItemChecked(TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex, true);
                    else TabManager.ActiveProperties.CheckListBoxLeft.SetItemChecked(0, true);
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count - 1) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //ripple delete all chars to the right of the cursor to the next nonalphanumerical char
                case (Keys.Control | Keys.Delete):
                    return Utils.DeleteCharactersInText(parent, false);

                //ripple delete all alphanumerical chars to the left of the cursor
                case (Keys.Control | Keys.Back):
                    return Utils.DeleteCharactersInText(parent, true);

                //move cursor to the left, clinging to words
                case (Keys.Control | Keys.Left):
                    return Utils.MoveCursorInText(parent, true);

                //move cursor to the right, clinging to words
                case (Keys.Control | Keys.Right):
                    return Utils.MoveCursorInText(parent, false);

                case Keys.Control | Keys.O:
                    OpenNew(presence);
                    return true;

                case Keys.Control | Keys.Z:
                    History.Undo();
                    return true;

                case Keys.Control | Keys.U:
                    History.Redo();
                    return true;

                case Keys.Control | Keys.Shift | Keys.O:
                    OpenNewTab(presence);
                    return true;

                case Keys.Alt | Keys.Shift | Keys.O:
                    OpenAll(presence);
                    return true;

                case Keys.Control | Keys.E:
                    _ = CreateStoryExplorer(true, parent, tokenSource);
                    return true;

                case Keys.Control | Keys.T:
                    _ = CreateStoryExplorer(false, parent, tokenSource);
                    return true;

                case Keys.Control | Keys.P:
                    ShowSettings();
                    return true;

                default:
                    PrepareTextChanged(parent.ActiveControl);
                    //return false, we dont consume the keypresses, only save a state to monitor for change
                    return false;
            }
        }

        public static void PrepareTextChanged(object textBox)
        {
            //if we have any kind of text box selected, we save keypresses for undo, else not
            if (textBox.GetType().IsAssignableFrom(typeof(TextBox)))
            {
                lastChangedTextBox = (TextBox)textBox;
                lastText = lastChangedTextBox.Text;
            }
        }

        /// <summary>
        /// Call this after performing base.WndProc, but before returning in the overriden form WndProc
        /// </summary>
        public static void TextChangedCallback(Form parent, int selectedIndex)
        {
            if (parent.ActiveControl.GetType().IsAssignableFrom(typeof(TextBox)))
            {
                if (((TextBox)parent.ActiveControl) == lastChangedTextBox
                    && lastText != lastChangedTextBox?.Text
                    && !History.CausedByHistory
                    && lastIndex == selectedIndex)
                {
                    History.AddAction(new TextChanged(lastChangedTextBox, lastText, lastChangedTextBox.Text));
                }
            }
        }

        public static void OpenAll(DiscordPresenceManager presenceManager)
        {
            //opne the story in tabs
            TabManager.OpenAllTabs();

            //update presence and recents
            presenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        public static void OpenContextMenu(ContextMenuStrip context, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = TabManager.ActiveProperties.CheckListBoxLeft.IndexFromPoint(e.Location);
                if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex <= 0) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = 0;
                context.Show();
            }
        }

        public static void OpenNew(DiscordPresenceManager presenceManager)
        {
            //get currently active translationmanager
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.LoadFileIntoProgram(presenceManager);
            translationManager.SetLanguage();
            //update tab name
            if (translationManager.FileName.Length > 0) TabManager.TabControl.SelectedTab.Text = translationManager.FileName;
        }

        public static void OpenNewTab(DiscordPresenceManager presenceManager)
        {
            TabManager.OpenNewTab();

            //update presence and recents
            presenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        public static void SelectedItemChanged(Helpers.ColouredCheckedListBox listBox)
        {
            if (!History.CausedByHistory)
                History.AddAction(new SelectedLineChanged(listBox, lastIndex, listBox.SelectedIndex));
            lastIndex = listBox.SelectedIndex;
            TabManager.ActiveTranslationManager.PopulateTextBoxes();
        }

        public static void SelectedLanguageChanged()
        {
            TabManager.ActiveTranslationManager.SetLanguage();
        }

        public static void SelectedTabChanged(DiscordPresenceManager presenceManager)
        {
            TabManager.OnSwitchTabs();
            //update tabs
            if (TabManager.ActiveTranslationManager != null) presenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        public static void ShowSettings()
        {
            var settings = new SettingsForm.SettingsForm();
            if (!settings.IsDisposed) settings.Show();
        }

        public static void ToggleReplaceUI()
        {
            TabManager.ActiveTranslationManager.ToggleReplaceUI();
        }

        public static void TranslationTextChanged()
        {
            TabManager.ActiveTranslationManager.UpdateTranslationString();
        }
    }
}

using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{
    public class InputHandler
    {
        private static ITextBox? lastChangedTextBox;
        private static string? lastText;
        private static int lastIndex = Settings.Default.RecentIndex;
        private static int lastIndexForTextChanges = Settings.Default.RecentIndex;

        public static void SaveAndApproveAndSelectNewLine()
        {
            if (TabManager.UI.SelectedTab.Lines.SelectedIndex >= 0)
                TabManager.UI.SelectedTab.Lines.ApproveItem(TabManager.UI.SelectedTab.Lines.SelectedIndex);
            else
                TabManager.UI.SelectedTab.Lines.ApproveItem(0);
            if (TabManager.UI.SelectedTab.Lines.SelectedIndex < TabManager.UI.SelectedTab.LineCount - 1)
                TabManager.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void SaveAndSelectNewLine()
        {
            TabManager.ActiveTranslationManager.SaveCurrentString();
            if (TabManager.UI.SelectedTab.Lines.SelectedIndex < TabManager.UI.SelectedTab.Lines.Count - 1)
                TabManager.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void SelectTabRight()
        {
            if (TabManager.TabCount > 1)
                TabManager.SwitchToTab(TabManager.SelectedTabIndex + 1);
        }

        public static void SelectTabLeft()
        {
            if (TabManager.TabCount > 1)
                TabManager.SwitchToTab(TabManager.SelectedTabIndex - 1);
        }

        public static void MoveLineSelectionDown()
        {
            if (TabManager.UI.SelectedTab.Lines.SelectedIndex < TabManager.UI.SelectedTab.LineCount - 1)
                TabManager.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void MoveLineSelectionUp()
        {
            if (TabManager.UI.SelectedTab.Lines.SelectedIndex > 0)
                TabManager.UI.SelectedTab.Lines.SelectedIndex--;
        }

        public static void FocusSearch()
        {
            //we have something selected we want to search for
            if (TabManager.UI.TabControl.SelectedTab.SelectedTranslationBoxText.Length > 0)
            {
                TabManager.UI.SearchBarText = TabManager.UI.TranslationBoxText;
            }
            TabManager.UI.FocusSearchBar();
        }

        /// <summary>
        /// call this before the text changed
        /// </summary>
        /// <param name="textBox"></param>
        public static void PrepareTextChanged(ITextBox textBox)
        {
            lastChangedTextBox = textBox;
            lastText = lastChangedTextBox.Text;
        }

        /// <summary>
        /// Call this after the text has changed
        /// </summary>
        public static void TextChangedCallback(ITextBox text, int selectedIndex)
        {
            if (text == lastChangedTextBox
                && lastText != lastChangedTextBox?.Text
                && lastIndexForTextChanges == selectedIndex
                && lastChangedTextBox is not null
                && lastText is not null)
            {
                History.AddAction(new TextChanged(
                    lastChangedTextBox,
                    lastText,
                    lastChangedTextBox.Text,
                    TabManager.ActiveTranslationManager.FileName,
                    TabManager.ActiveTranslationManager.StoryName));
            }
            else
            {
                //we selected a new line and dont need to add a history item
                lastIndexForTextChanges = selectedIndex;
            }
        }

        public static void SelectedItemChanged(ILineList listBox)
        {
            if (lastIndex >= 0)
            {
                TabManager.ActiveTranslationManager.UpdateSimilarityMarking(listBox[lastIndex].ToString()!);
                if (listBox.SelectedIndex >= 0)
                {
                    if (History.Peek().FileName == TabManager.ActiveTranslationManager.FileName && History.Peek().StoryName == TabManager.ActiveTranslationManager.StoryName)
                        History.AddAction(new SelectedLineChanged(listBox, lastIndex, listBox.SelectedIndex, TabManager.ActiveTranslationManager.FileName, TabManager.ActiveTranslationManager.StoryName));
                    else
                        History.AddAction(new SelectedLineChanged(listBox, 0, listBox.SelectedIndex, TabManager.ActiveTranslationManager.FileName, TabManager.ActiveTranslationManager.StoryName));
                }
            }
            lastIndex = listBox.SelectedIndex;
            TabManager.ActiveTranslationManager.PopulateTextBoxes();
        }

        public static void SelectedLanguageChanged()
        {
            SetLanguage();
            TabManager.ActiveTranslationManager.ReloadFile();
        }

        public static void SaveAndApproveLine()
        {
            if (TabManager.UI.SelectedTab.Lines.SelectedIndex >= 0)
                TabManager.UI.SelectedTab.Lines.ApproveItem(TabManager.UI.SelectedTab.Lines.SelectedIndex);
        }

        /// <summary>
        /// Sets the language the translation is associated with
        /// </summary>
        public static void SetLanguage()
        {
            if (TabManager.UI.Language.Length >= 0)
            {
                TranslationManager.Language = TabManager.UI.Language;
            }
            else if (Settings.Default.Language != string.Empty)
            {
                if (Settings.Default.Language != string.Empty)
                {
                    TranslationManager.Language = Settings.Default.Language;
                }
            }
            TabManager.UI.Language = TranslationManager.Language;
        }
    }
}

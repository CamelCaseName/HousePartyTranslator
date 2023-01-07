using Translator.UICompatibilityLayer;

namespace Translator.Core
{
    public class InputHandler<T, V, X, W>
        where T : class, ILineItem, new()
        where V : class, IUIHandler<T, X, W>, new()
        where X : class, ITabController<T, W>, new()
        where W : class, ITab<T>, new()
    {
        private static ITextBox? lastChangedTextBox;
        private static string? lastText;
        private static int lastIndex = Settings.Default.RecentIndex;

        public static void ApprovedButtonChanged()
        {
            TabManager<T, V, X, W>.ActiveTranslationManager.ApprovedButtonHandler();
        }

        public static void AutoTranslate()
        {
            TabManager<T, V, X, W>.ActiveTranslationManager.RequestedAutomaticTranslation();
        }

        public static void CheckItemChanged()
        {
            TabManager<T, V, X, W>.ActiveTranslationManager.ApproveIfPossible(false);
        }

        public static void Redo() => History.Redo();

        public static void Undo() => History.Undo();

        public static void OnSwitchTabs() => TabManager<T,V,X,W>.OnSwitchTabs();

        public static void SaveAndApproveAndSelectNewLine()
        {
            if (TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex >= 0) 
                TabManager<T, V, X, W>.UI.SelectedTab.Lines.ApproveItem(TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex);
            else 
                TabManager<T, V, X, W>.UI.SelectedTab.Lines.ApproveItem(0);
            if (TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex < TabManager<T, V, X, W>.UI.SelectedTab.LineCount - 1) 
                TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void SaveAndSelectNewLine()
        {
            TabManager<T, V, X, W>.ActiveTranslationManager.SaveCurrentString();
            if (TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex < TabManager<T, V, X, W>.UI.SelectedTab.Lines.Count - 1) 
                TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void SelectTabRight()
        {
            if (TabManager<T, V, X, W>.TabCount > 1) 
                TabManager<T, V, X, W>.SwitchToTab(TabManager<T, V, X, W>.SelectedTabIndex + 1);
        }

        public static void SelectTabLeft()
        {
            if (TabManager<T, V, X, W>.TabCount > 1) 
                TabManager<T, V, X, W>.SwitchToTab(TabManager<T, V, X, W>.SelectedTabIndex - 1);
        }

        public static void MoveLineSelectionDown()
        {
            if (TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex < TabManager<T, V, X, W>.UI.SelectedTab.LineCount - 1) 
                TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void MoveLineSelectionUp()
        {
            if (TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex > 0) 
                TabManager<T, V, X, W>.UI.SelectedTab.Lines.SelectedIndex--;
        }

        public static void ReloadFile() =>
                            TabManager<T, V, X, W>.ActiveTranslationManager.ReloadFile();

        public static void SaveAllTabs() =>
                            _ = TabManager<T, V, X, W>.SaveAllTabs();

        public static void SaveSelectedString() =>
                            TabManager<T, V, X, W>.ActiveTranslationManager.SaveCurrentString();

        public static void SaveFile() => TabManager<T, V, X, W>.ActiveTranslationManager.SaveFile();

        public static void FocusSearch()
        {
            if (TabManager<T, V, X, W>.UI.TabControl.SelectedTab.SelectedTranslationBoxText.Length > 0)
            {
                TabManager<T, V, X, W>.UI.SearchBarText = TabManager<T, V, X, W>.UI.TranslationBoxText;
            }
            TabManager<T, V, X, W>.UI.SelectedTab.FocusSearchBox();
        }

        public static bool AdvanceSearchResultSelection() => TabManager<T, V, X, W>.ActiveTranslationManager.SelectNextResultIfApplicable();

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
                && lastIndex == selectedIndex
                && lastChangedTextBox != null
                && lastText != null)
            {
                History.AddAction(new TextChanged(
                    lastChangedTextBox,
                    lastText,
                    lastChangedTextBox.Text,
                    TabManager<T, V, X, W>.ActiveTranslationManager.FileName,
                    TabManager<T, V, X, W>.ActiveTranslationManager.StoryName));
            }
        }

        public static void OpenAll()
        {
            //opne the story in tabs
            TabManager<T, V, X, W>.OpenAllTabs();
        }
        
        public static void OpenNew()
        {
            //get currently active translationmanager
            TabManager<T, V, X, W>.OpenNewFile();
        }

        public static void OpenNewTab()
        {
            TabManager<T, V, X, W>.OpenNewTab();
        }

        public static void SelectedItemChanged(ILineList<T> listBox)
        {
            if (lastIndex >= 0 && listBox.SelectedIndex >= 0)
            {
                if (History.Peek().FileName == TabManager<T, V, X, W>.ActiveTranslationManager.FileName && History.Peek().StoryName == TabManager<T, V, X, W>.ActiveTranslationManager.StoryName)
                    History.AddAction(new SelectedLineChanged<T>(listBox, lastIndex, listBox.SelectedIndex, TabManager<T, V, X, W>.ActiveTranslationManager.FileName, TabManager<T, V, X, W>.ActiveTranslationManager.StoryName));
                else
                    History.AddAction(new SelectedLineChanged<T>(listBox, 0, listBox.SelectedIndex, TabManager<T, V, X, W>.ActiveTranslationManager.FileName, TabManager<T, V, X, W>.ActiveTranslationManager.StoryName));
            }
            lastIndex = listBox.SelectedIndex;
            TabManager<T, V, X, W>.ActiveTranslationManager.PopulateTextBoxes();
        }

        public static void SelectedLanguageChanged()
        {
            TranslationManager<T, V, X, W>.SetLanguage();
            TabManager<T, V, X, W>.ActiveTranslationManager.ReloadFile();
        }

        public static void ToggleReplaceUI()
        {
            TabManager<T, V, X, W>.ActiveTranslationManager.ToggleReplaceUI();
        }

        public static void TranslationTextChanged()
        {
            TabManager<T, V, X, W>.ActiveTranslationManager.UpdateTranslationString();
        }
    }
}

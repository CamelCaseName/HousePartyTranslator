using System.Windows.Forms;

namespace Translator.Helpers
{
    /// <summary>
    /// A Property to replace all properties which would have to be passed from the Fenster to the TranslationManager.
    /// </summary>
    internal sealed class PropertyHelper
    {
        public readonly CheckBox ApprovedBox;
        public readonly ColouredCheckedListBox CheckListBoxLeft;
        public readonly ToolStripComboBox LanguageBox;
        public readonly Label ApprovedCountLabel;
        public readonly Label CharacterCountLabel;
        public readonly Label SelectedFileLabel;
        public readonly NoAnimationBar NoProgressbar;
        public readonly TextBox CommentBox;
        public readonly ToolStripTextBox SearchBox;
        public readonly ToolStripTextBox ReplaceBox;
        public readonly ToolStripMenuItem ReplaceAllButton;
        public readonly ToolStripMenuItem ReplaceButton;
        public readonly TextBox TemplateTextBox;
        public readonly TextBox TranslationTextBox;

        /// <summary>
        /// A Property to replace all properties which would have to be passed from the Fenster to the TranslationManager.
        /// </summary>
        /// <param name="ApprovedBox">A checkbox denoting the approval state of a string.</param>
        /// <param name="ApprovedCountLabel">A label to show the amount of approves strings.</param>
        /// <param name="CharacterCountLabel">A label displaying the current count of characters for template and translation.</param>
        /// <param name="CheckListBoxLeft">The list of all strings.</param>
        /// <param name="CommentBox">The textbox for comments.</param>
        /// <param name="LanguageBox">The drop down box to choose the language with.</param>
        /// <param name="NoProgressbar">The progress bar.</param>
        /// <param name="SearchBox">The textbox to perform searches with.</param>
        /// <param name="ReplaceBox">The textbox to replace search with.</param>
        /// <param name="ReplaceAllButton">button to replace all occurencas of the search</param>
        /// <param name="SelectedFileLabel">The currently selected file will be displayed there.</param>
        /// <param name="TemplateTextBox">The textbox containing the template.</param>
        /// <param name="TranslationTextBox">The textbox containing the translation</param>
        /// <param name="ReplaceButton"></param>
        public PropertyHelper(
            CheckBox ApprovedBox,
            ColouredCheckedListBox CheckListBoxLeft,
            ToolStripComboBox LanguageBox,
            Label ApprovedCountLabel,
            Label CharacterCountLabel,
            Label SelectedFileLabel,
            NoAnimationBar NoProgressbar,
            TextBox CommentBox,
            ToolStripTextBox SearchBox,
            ToolStripTextBox ReplaceBox,
            ToolStripMenuItem ReplaceAllButton,
            TextBox TemplateTextBox,
            TextBox TranslationTextBox,
            ToolStripMenuItem ReplaceButton
            )
        {
            this.ApprovedBox = ApprovedBox;
            this.ApprovedCountLabel = ApprovedCountLabel;
            this.CharacterCountLabel = CharacterCountLabel;
            this.CheckListBoxLeft = CheckListBoxLeft;
            this.CommentBox = CommentBox;
            this.LanguageBox = LanguageBox;
            this.NoProgressbar = NoProgressbar;
            this.SearchBox = SearchBox;
            this.ReplaceBox = ReplaceBox;
            this.ReplaceAllButton = ReplaceAllButton;
            this.SelectedFileLabel = SelectedFileLabel;
            this.TemplateTextBox = TemplateTextBox;
            this.TranslationTextBox = TranslationTextBox;
            this.ReplaceButton = ReplaceButton;
        }
    }
}

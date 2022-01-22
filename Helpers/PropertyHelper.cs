using System.Windows.Forms;

namespace HousePartyTranslator.Helpers
{
    /// <summary>
    /// A Property to replace all properties which would have to be passed from the Fenster to the TranslationManager
    /// </summary>
    public class PropertyHelper
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
        public readonly TextBox TemplateTextBox;
        public readonly TextBox TranslationTextBox;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApprovedBox"></param>
        /// <param name="ApprovedCountLabel"></param>
        /// <param name="CharacterCountLabel"></param>
        /// <param name="CheckListBoxLeft"></param>
        /// <param name="CommentBox"></param>
        /// <param name="LanguageBox"></param>
        /// <param name="NoProgressbar"></param>
        /// <param name="SearchBox"></param>
        /// <param name="SelectedFileLabel"></param>
        /// <param name="TemplateTextBox"></param>
        /// <param name="TranslationTextBox"></param>
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
            TextBox TemplateTextBox,
            TextBox TranslationTextBox
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
            this.SelectedFileLabel = SelectedFileLabel;
            this.TemplateTextBox = TemplateTextBox;
            this.TranslationTextBox = TranslationTextBox;
        }
    }
}

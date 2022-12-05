using Translator.Core.Helpers;
using Translator.Helpers;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal static partial class WinExtensions
    {
        internal static PopupResult ToPopupResult(this DialogResult result)
        {
            return result switch
            {
                DialogResult.None => PopupResult.NONE,
                DialogResult.OK => PopupResult.OK,
                DialogResult.Cancel => PopupResult.CANCEL,
                DialogResult.Abort => PopupResult.ABORT,
                DialogResult.Retry => PopupResult.NONE,
                DialogResult.Ignore => PopupResult.IGNORE,
                DialogResult.Yes => PopupResult.YES,
                DialogResult.No => PopupResult.NO,
                DialogResult.TryAgain => PopupResult.NONE,
                DialogResult.Continue => PopupResult.CONTINUE,
                _ => PopupResult.NONE,
            };
        }

        /// <summary>
        /// Gets the category of a node from a node type
        /// </summary>
        /// <param name="type">the type to get the stringcategory form</param>
        /// <returns>the corresponding stringcategory</returns>
        internal static StringCategory CategoryFromNode(this NodeType type)
        {
            return type switch
            {
                NodeType.Null => StringCategory.Neither,
                NodeType.Item => StringCategory.ItemName,
                NodeType.ItemGroup => StringCategory.ItemGroupAction,
                NodeType.Action => StringCategory.ItemAction,
                NodeType.Event => StringCategory.Event,
                NodeType.Criterion => StringCategory.Neither,
                NodeType.Response => StringCategory.Response,
                NodeType.Dialogue => StringCategory.Dialogue,
                NodeType.Quest => StringCategory.Quest,
                NodeType.Achievement => StringCategory.Achievement,
                NodeType.Reaction => StringCategory.Response,
                NodeType.BGC => StringCategory.BGC,
                _ => StringCategory.Neither,
            };
        }
    }
}

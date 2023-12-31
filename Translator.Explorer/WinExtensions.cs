using System.Runtime.Versioning;
using Translator.Core.Data;
using Translator.Explorer.Graph;

namespace Translator.Desktop.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    public static class WinExtensions
    {
       

        /// <summary>
        /// Gets the category of a node from a node type
        /// </summary>
        /// <param name="type">the type to get the stringcategory form</param>
        /// <returns>the corresponding stringcategory</returns>
        public static StringCategory CategoryFromNode(this NodeType type)
        {
            return type switch
            {
                NodeType.Null => StringCategory.Neither,
                NodeType.Item => StringCategory.ItemName,
                NodeType.ItemGroup => StringCategory.ItemGroupAction,
                NodeType.ItemAction => StringCategory.ItemAction,
                NodeType.Event => StringCategory.Event,
                NodeType.Criterion => StringCategory.Neither,
                NodeType.Response => StringCategory.Response,
                NodeType.Dialogue => StringCategory.Dialogue,
                NodeType.Quest => StringCategory.Quest,
                NodeType.Achievement => StringCategory.Achievement,
                NodeType.EventTrigger => StringCategory.Response,
                NodeType.BGC => StringCategory.BGC,
                _ => StringCategory.Neither,
            };
        }        
    }
}

using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface INewFileSelector
    {
        string CombinedStoryFile { get; }
        string FileName { get; }
        string StoryName { get; }

        public PopupResult ShowDialog();
    }
}
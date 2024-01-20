using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ILineItem
    {
        public bool IsApproved { get; set; }
        public bool IsSearchResult { get; set; }
        public bool IsTranslated { get; set; }
        public string Text { get; init; }
        public EekStringID ID { get; init; }
        void Approve();
        void Unapprove();
    }
}
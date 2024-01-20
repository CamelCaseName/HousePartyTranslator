using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.DefaultImpls
{
    public class DefaultLineItem : ILineItem

    {
        public bool IsApproved { get => false; set { } }
        public bool IsSearchResult { get => false; set { } }
        public bool IsTranslated { get => false; set { } }
        public string Text { get => string.Empty; init { } }

        public EekStringID ID { get => new(); init { } }

        public void Approve() { }
        public void Unapprove() { }
    }
}

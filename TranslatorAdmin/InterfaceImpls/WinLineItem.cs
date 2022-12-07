using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinLineItem : object, ILineItem
    {
        public bool IsApproved { get; set; } = false;
        public bool IsSearchResult { get; set; } = false;
        public bool IsTranslated { get; set; } = false;
        public string Text { get; init; } = string.Empty;

        public void Approve() => IsApproved = IsTranslated = true;
        public void Unapprove() => IsApproved = false;
        public override string ToString() => Text;
    }
}

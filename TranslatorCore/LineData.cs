using System;

namespace Translator.Core.Helpers
{
    public sealed class LineData
    {
        public string ID = string.Empty;
        public string Story = string.Empty;
        public string FileName = string.Empty;
        public StringCategory Category = StringCategory.Neither;
        public bool IsTranslated = false;
        public bool IsApproved = false;
        public bool IsTemplate = false;
        public string TemplateString = string.Empty;
        public string TranslationString { get => _translationString; set => _translationString = value.RemoveVAHints(); }
        private string _translationString = string.Empty;
        public string[] Comments = Array.Empty<string>();

        public int TemplateLength => TemplateString.RemoveVAHints().Length;
        public int TranslationLength => TranslationString.Length;

        public LineData() { }

        public LineData(string id, string story, string filename, StringCategory category)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
        }

        public LineData(string id, string story, string filename, StringCategory category, string english, bool isTemplate)
        {
            ID = id.Trim();
            TemplateString = english.Trim();
            Story = story.Trim();
            IsTranslated = true;
            FileName = filename.Trim();
            Category = category;
            IsTemplate = isTemplate;
        }

        public LineData(string id, string story, string filename, StringCategory category, string english, string translation)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
            TemplateString = english.Trim();
            TranslationString = translation.Trim();
            IsTranslated = translation.Length > 1;
        }

        public LineData(string id, string story, string filename, StringCategory category, string translation)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
            TranslationString = translation.Trim();
        }

        public override string ToString()
        {
            string value;
            if (TranslationString != null && TranslationString != string.Empty)
            {
                value = ID + "|" + TranslationString;
            }
            else
            {
                value = ID + "|" + TemplateString;
            }
            return value;
        }
    }
}
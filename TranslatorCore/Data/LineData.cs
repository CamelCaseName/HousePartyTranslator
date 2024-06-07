using System;
using System.Linq;
using Translator.Core.Helpers;

namespace Translator.Core.Data
{
    public sealed class LineData
    {
        public readonly string ID = string.Empty;
        public readonly EekStringID EekID;
        public readonly string Story = string.Empty;
        public readonly string FileName = string.Empty;
        public readonly StringCategory Category = StringCategory.Neither;
        public bool IsTranslated = false;
        public bool IsApproved = false;
        public bool IsTemplate = false;
        public string TemplateString = string.Empty;
        public bool WasChanged
        {
            get;
            set;
        }
        public string TranslationString { get => _translationString; set => _translationString = value.RemoveVAHints(false); }
        private string _translationString = string.Empty;
        public string[] Comments = Array.Empty<string>();

        //returns the length of the template WITHOUT the voice actor hints
        public int TemplateLength => TemplateString.RemoveVAHints().Length;
        public int TranslationLength => TranslationString.Length;

        public LineData() { }

        public LineData(string id, string story, string filename, StringCategory category)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
            EekID = new(ID, Category);
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
            EekID = new(ID, Category);
        }

        public LineData(string id, string story, string filename, StringCategory category, string english, string translation)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
            TemplateString = english.Trim();
            TranslationString = translation.RemoveVAHints(true);
            IsTranslated = translation.Length > 1;
            EekID = new(ID, Category);
        }

        public LineData(string id, string story, string filename, StringCategory category, string translation)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
            TranslationString = translation.RemoveVAHints(true);
            EekID = new(ID, Category);
        }

        public LineData(LineData line)
        {
            Category = line.Category;
            Comments = line.Comments;
            FileName = line.FileName;
            ID = line.ID;
            IsApproved = line.IsApproved;
            IsTemplate = line.IsTemplate;
            IsTranslated = line.IsTranslated;
            Story = line.Story;
            TemplateString = line.TemplateString;
            _translationString = line.TranslationString;
            EekID = new(ID, Category);
        }

        public override string ToString()
        {
            string value = TranslationString is not null && TranslationString != string.Empty ? ID + "|" + TranslationString : ID + "|" + TemplateString;
            return value;
        }

        public bool ShouldBeMarkedSimilarToEnglish =>
            !IsTranslated && !IsApproved
            || (TranslationString == TemplateString && !Utils.OfficialFileNames.Contains(TemplateString));
    }
}
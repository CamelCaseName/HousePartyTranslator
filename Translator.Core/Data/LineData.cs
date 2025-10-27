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
        public string Template = string.Empty;
        public bool WasChanged
        {
            get;
            set;
        }
        public string Translation { get => _translation; set => _translation = value.RemoveVAHints(false); }
        private string _translation = string.Empty;
        public string[] Comments = Array.Empty<string>();

        //returns the length of the template WITHOUT the voice actor hints
        public int TemplateLength => Template.RemoveVAHints().Length;
        public int TranslationLength => Translation.Length;

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
            Template = english.Trim();
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
            Template = english.Trim();
            Translation = translation.RemoveVAHints(true);
            IsTranslated = translation.Length > 1;
            EekID = new(ID, Category);
        }

        public LineData(string id, string story, string filename, StringCategory category, string translation)
        {
            ID = id.Trim();
            Story = story.Trim();
            FileName = filename.Trim();
            Category = category;
            Translation = translation.RemoveVAHints(true);
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
            Template = line.Template;
            _translation = line.Translation;
            EekID = new(ID, Category);
        }

        public LineData(LineData line, string template)
        {
            Category = line.Category;
            Comments = line.Comments;
            FileName = line.FileName;
            ID = line.ID;
            IsApproved = line.IsApproved;
            IsTemplate = line.IsTemplate;
            IsTranslated = line.IsTranslated;
            Story = line.Story;
            Template = template;
            _translation = line.Translation;
            EekID = new(ID, Category);
        }

        public LineData(LineData line, EekStringID id)
        {
            Category = id.Category;
            Comments = line.Comments;
            FileName = line.FileName;
            ID = id.ID;
            IsApproved = line.IsApproved;
            IsTemplate = line.IsTemplate;
            IsTranslated = line.IsTranslated;
            Story = line.Story;
            Template = line.Template;
            _translation = line.Translation;
            EekID = id;
        }

        public LineData(LineData line, string translation, bool isTranslated)
        {
            Category = line.Category;
            Comments = line.Comments;
            FileName = line.FileName;
            ID = line.ID;
            IsApproved = line.IsApproved;
            IsTemplate = line.IsTemplate;
            IsTranslated = isTranslated;
            Story = line.Story;
            Template = line.Template;
            _translation = translation;
            EekID = new(ID, Category);
        }

        public override string ToString()
        {
            string value = Translation is not null && Translation != string.Empty ? ID + "|" + Translation : ID + "|" + Template;
            return value;
        }

        public bool ShouldBeMarkedSimilarToEnglish =>
            !IsTranslated && !IsApproved
            || (Translation == Template && !Utils.OfficialFileNames.Contains(Template));
    }
}
public class LineData
{
    public string ID;
    public string Story;
    public string FileName;
    public bool IsTranslated;
    public bool IsApproved;
    public bool IsTemplate;
    public string EnglishString;
    public string TranslationString;
    public string[] Comments;

    public LineData(string id, string story, string filename, bool isApproved)
    {
        ID = id;
        Story = story;
        FileName = filename;
        IsApproved = isApproved;
    }

    public LineData(string id, string story, string filename, string english, bool isTemplate)
    {
        ID = id;
        EnglishString = english;
        Story = story;
        FileName = filename;
        IsTemplate = isTemplate;
    }

    public LineData(string id, string story, string filename, string english, string translation)
    {
        ID = id;
        EnglishString = english;
        TranslationString = translation;
        Story = story;
        FileName = filename;
    }

    public LineData(string id, string story, string filename, string translation)
    {
        ID = id;
        TranslationString = translation;
        Story = story;
        FileName = filename;
    }

    public LineData(string id, string story, string filename, string english, string translation, bool isTranslated, bool isApproved, string[] comments)
    {
        ID = id;
        EnglishString = english;
        TranslationString = translation;
        IsTranslated = isTranslated;
        IsApproved = isApproved;
        Comments = comments;
        Story = story;
        FileName = filename;
    }

    public void UpdateTranslated(string translation)
    {
        TranslationString = translation;
        if (translation != "")
        {
            IsTranslated = true;
        }
    }

    public void UpdateApproval(bool isApproved)
    {
        IsApproved = isApproved;
    }

    public void AddComment(string Comment)
    {
        Comments[Comments.Length] = Comment;
    }

    public override string ToString()
    {
        string value = "";
        if (TranslationString != "")
        {
            value = ID + "|" + TranslationString;

        }
        else
        {
            value = ID + "|" + EnglishString;
        }
        return value;
    }
}

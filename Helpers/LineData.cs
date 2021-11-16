using HousePartyTranslator;

public class LineData
{
    public string ID;
    public string Story;
    public string FileName;
    public StringCategory Category;
    public bool IsTranslated;
    public bool IsApproved;
    public bool IsTemplate;
    public string EnglishString;
    public string TranslationString;
    public string[] Comments;

    public LineData(string id, string story, string filename, StringCategory category, bool isApproved)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        IsApproved = isApproved;
    }

    public LineData(string id, string story, string filename, StringCategory category)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
    }

    public LineData(string id, string story, string filename, StringCategory category, string english, bool isTemplate)
    {
        ID = id;
        EnglishString = english;
        Story = story;
        FileName = filename;
        Category = category;
        IsTemplate = isTemplate;
    }

    public LineData(string id, string story, string filename, StringCategory category, string english, string translation)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        EnglishString = english;
        TranslationString = translation;
    }

    public LineData(string id, string story, string filename, StringCategory category, string translation)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        TranslationString = translation;
    }

    public LineData(string id, string story, string filename, StringCategory category, string english, string translation, bool isTranslated, bool isApproved, string[] comments)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        EnglishString = english;
        TranslationString = translation;
        IsTranslated = isTranslated;
        IsApproved = isApproved;
        Comments = comments;
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

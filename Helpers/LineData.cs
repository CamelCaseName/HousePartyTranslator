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

    public LineData(string id, string story, string filename, StringCategory category, bool isApproved, bool isTranslated)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        IsApproved = isApproved;
        IsTranslated = isTranslated;
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
        IsTranslated = translation.Length > 1;
    }

    public LineData(string id, string story, string filename, StringCategory category, bool isApproved, string english, string translation)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        IsApproved = isApproved;
        EnglishString = english;
        TranslationString = translation;
        IsTranslated = translation.Length > 1;
    }

    public LineData(string id, string story, string filename, StringCategory category, string translation)
    {
        ID = id;
        Story = story;
        FileName = filename;
        Category = category;
        TranslationString = translation;
    }

    public LineData(string id, string story, string filename, StringCategory category, string english, string translation, bool isApproved, bool isTranslated, string[] comments)
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
        if (Comments != null)
        {
            string[] tempArray = Comments;
            Comments = new string[tempArray.Length + 1];
            for (int i = 0; i < tempArray.Length; i++)
            {
                Comments[i] = tempArray[i];
            }
            Comments[tempArray.Length] = Comment;
        }
        else
        {
            Comments = new string[1] { Comment };
        }
    }

    public override string ToString()
    {
        string value;
        if (TranslationString != "" && TranslationString != null)
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
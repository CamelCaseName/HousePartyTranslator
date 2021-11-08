public class LineData
{
    public string ID;
    public string Story;
    public string FileName;
    public bool IsTranslated;
    public bool IsApproved;
    public string EnglishString;
    public string TranslationString;
    public string[] Comments;

    public LineData(string id, string english, string story, string filename)
    {
        this.ID = id;
        this.EnglishString = english;
        this.Story = story;
        this.FileName = filename;
    }

    public LineData(string id, string english, string story, string filename, string translation)
    {
        this.ID = id;
        this.EnglishString = english;
        this.TranslationString = translation;
        this.Story = story;
        this.FileName = filename;
    }

    public LineData(string id, string english, string story, string filename, string translation, bool isTranslated, bool isApproved, string[] comments)
    {
        this.ID = id;
        this.EnglishString = english;
        this.TranslationString = translation;
        this.IsTranslated = isTranslated;
        this.IsApproved = isApproved;
        this.Comments = comments;
        this.Story = story;
        this.FileName = filename;
    }

    public void UpdateTranslated(string translation)
    {
        this.TranslationString = translation;
        if (translation != "")
        {
            IsTranslated = true;
        }
    }

    public void UpdateApproval(bool isApproved)
    {
        this.IsApproved = isApproved;
    }

    public void AddComment(string Comment)
    {
        Comments[Comments.Length] = Comment;
    }

    public override string ToString()
    {
        string value = ID + "|" + IsTranslated + "|" + IsApproved + "|";
        for (int i = 0; i < Comments.Length; i++)
        {
            value += Comments[i];
            if (i < Comments.Length - 1)
            {
                value += ":";
            }
        }
        return value;
    }
}

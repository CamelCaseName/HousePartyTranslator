using HousePartyTranslator;

internal sealed class LineData
{
    public string ID;
    public string Story;
    public string FileName;
    public StringCategory Category;
    public bool IsTranslated;
    public bool IsApproved;
    public bool IsTemplate;
    public string TemplateString;
    public string TranslationString;
    public string[] Comments;

    public LineData() { }

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
        TemplateString = english;
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
        TemplateString = english;
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

    public override string ToString()
    {
        string value;
        if (TranslationString != null && TranslationString != "")
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
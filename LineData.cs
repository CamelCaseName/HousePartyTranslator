using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LineData
{
    public string key;
    public bool isTranslated;
    public bool isApproved;
    public string[] comments;

    public LineData(string key)
    {
        this.key = key;
    }

    public LineData(string key, bool isTranslated, bool isApproved, string[] comments)
    {
        this.key = key;
        this.isTranslated = isTranslated;
        this.isApproved = isApproved;
        this.comments = comments;
    }

    public override string ToString()
    {
        string value = key + "|" + isTranslated + "|" + isApproved + "|";
        for (int i = 0; i < comments.Length; i++)
        {
            value += comments[i];
            if (i < comments.Length - 1)
            {
                value += ":";
            }
        }
        return value;
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LineData
{
    public string key;
    public bool isTranslated;
    public bool isApproved;
    public string english;
    public string translation;
    public string[] comments;

    public LineData(string key, string english)
    {
        this.key = key;
        this.english = english;
    }

    public LineData(string key, string english, string translation)
    {
        this.key = key;
        this.english = english;
        this.translation = translation;
    }

    public LineData(string key, string english, string translation, bool isTranslated, bool isApproved, string[] comments)
    {
        this.key = key;
        this.english = english;
        this.translation = translation;
        this.isTranslated = isTranslated;
        this.isApproved = isApproved;
        this.comments = comments;
    }

    public void UpdateTranslated(string translation)
    {
        this.translation = translation;
        if(translation != "")
        {
            isTranslated = true;
        }
    }

    public void UpdateApproval(bool isApproved)
    {
        this.isApproved = isApproved;
    }

    public void AddComment(string Comment)
    {
        comments[comments.Length] = Comment;
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

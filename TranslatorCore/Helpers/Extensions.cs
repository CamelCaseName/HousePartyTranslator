using System;
using System.Text;
using System.Text.RegularExpressions;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.Helpers
{
    public static class Extensions
    {
        public static readonly char[] trimmers = { '\0', ' ', '\t', '\n', '\r', (char)160 };

        /// <summary>
        /// Returns whether a story is official or not
        /// </summary>
        /// <param name="storyName">the name to check</param>
        /// <returns></returns>
        public static bool IsOfficialStory(this string storyName)
        {
            string[] stories = { "UI", "Hints", "Original Story", "A Vickie Vixen Valentine", "Combat Training", "Date Night with Brittney", "Date Night With Brittney" };
            return Array.IndexOf(stories, storyName) >= 0;
        }

        /// <summary>
        /// Replaces all matches in the given string and returns it
        /// </summary>
        /// <param name="input">The string to work on</param>
        /// <param name="replacement">The replacement for all matches</param>
        /// <param name="search">the pattern to search for</param>
        /// <returns>the replaced string</returns>
        public static string ReplaceImpl(this string input, string replacement, string search)
        {
            return ReplaceRegex(input, replacement, Regex.Escape(search));
        }

        /// <summary>
        /// Replaces all regex rule matches inte given string and returns it
        /// </summary>
        /// <param name="input">The string to work on</param>
        /// <param name="replacement">The replacement for all matches</param>
        /// <param name="regexRules">The regex to match</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string input, string replacement, string regexRules)
        {
            return Regex.Replace(input, regexRules, replacement, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline, new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// Removes the voice actor infos from the given string. Info has to be encapsulated in []. 
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string RemoveVAHints(this string input)
        {
            return input.AsSpan().RemoveVAHints().ToString();
        }

        public static ReadOnlySpan<char> RemoveVAHints(this ReadOnlySpan<char> span)
        {
            bool inVAHint = false;
            Span<char> output = new Span<char>(new char[span.Length]);
            int iterator = 0;
            foreach (char character in span)
            {
                if (character == '[' && !inVAHint)
                {
                    inVAHint = true;
                }
                else if (character == ']' && inVAHint)
                {
                    inVAHint = false;
                }
                else if (!inVAHint)
                {
                    output[iterator++] = character;
                }
            }

            output = output.Trim(trimmers);

            return (ReadOnlySpan<char>)output;
        }

        /// <summary>
        /// Constrains the lenght of a single string to a multi line version of said string, 
        /// where every MaxTextLength characters a newline is inserted.
        /// </summary>
        /// <param name="input">The string to format</param>
        /// <returns>The formatted, blockified string</returns>
        /// 
        public static string ConstrainLength(this string input) => input.ConstrainLength(Utils.MaxTextLength);
        public static string ConstrainLength(this string input, int maxlineLength)
        {
            int currentWordLength = 0, currentLength = 0, lastWordStart = 0, totalCount = 0;
            bool inWord = false;
            var builder = new StringBuilder(input.Length);

            foreach (char c in input.AsSpan())
            {
                if (c != ' ' && c != '\n' && c != '\r')
                {
                    if (!inWord) lastWordStart = totalCount;
                    inWord = true;
                    currentWordLength++;
                }
                else
                {
                    inWord = false;
                    currentWordLength = 0;
                }

                currentLength++;

                //if line is short still
                if (currentLength <= maxlineLength)
                {
                    _ = builder.Append(c);
                }
                else
                {
                    if (inWord && currentWordLength < maxlineLength)
                    {
                        //line is too long but we in a word, so we move the word to the next line if it fits, else we just split it
                        _ = builder.Insert(lastWordStart, '\n');
                        _ = builder.Append(c);
                    }
                    else
                    {
                        _ = builder.Append('\n');
                        _ = builder.Append(c);
                    }
                    currentLength = 0;
                }

                ++totalCount;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Moves the cursor in the given textbox to the next beginning/end of a word to the left
        /// </summary>
        /// <param name="textBox">The box to work on</param>
        public static void MoveCursorWordLeft(this ITextBox textBox)
        {
            bool broken = false;
            int oldPos = textBox.SelectionStart;
            if (textBox.SelectionStart > 0)
            {
                if (textBox.Text[textBox.SelectionStart - 1] == ' ')
                {
                    --textBox.SelectionStart;
                }
            }
            for (int i = textBox.SelectionStart; i > 0; i--)
            {
                switch (textBox.Text.Substring(i - 1, 1))
                {    //set up any stopping points you want
                    case " ":
                    case ";":
                    case ",":
                    case ".":
                    case "-":
                    case "'":
                    case "/":
                    case "\\":
                    case "\n":
                        textBox.SelectionStart = i;
                        broken = true;
                        break;
                }
                if (broken) break;
            }
            if (oldPos - textBox.SelectionStart < 1 && textBox.SelectionStart > 0)
            {
                --textBox.SelectionStart;
            }
            if (!broken)
            {
                textBox.SelectionStart = 0;
            }
        }

        /// <summary>
        /// Moves the cursor in the given textbox to the next beginning/end of a word to the right
        /// </summary>
        /// <param name="textBox">The box to work on</param>
        public static void MoveCursorWordRight(this ITextBox textBox)
        {
            bool broken = false;
            int oldPos = textBox.SelectionStart;
            if (textBox.SelectionStart < textBox.Text.Length)
            {
                if (textBox.Text[textBox.SelectionStart] == ' ' && textBox.SelectionStart < textBox.Text.Length - 1)
                {
                    ++textBox.SelectionStart;
                }
            }
            for (int i = textBox.SelectionStart; i < textBox.Text.Length; i++)
            {
                switch (textBox.Text[i].ToString())
                {    //set up any stopping points you want
                    case " ":
                    case ";":
                    case ",":
                    case ".":
                    case "-":
                    case "_":
                    case "'":
                    case "/":
                    case "\\":
                    case "\n":
                        textBox.SelectionStart = i;
                        broken = true;
                        break;
                }
                if (broken) break;
            }
            if (textBox.SelectionStart - oldPos < 1)
            {
                textBox.SelectionStart++;
            }
            if (!broken)
            {
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        /// <summary>
        /// cuts a string to a maximum length and adds a delimiter
        /// </summary>
        /// <param name="toTrim">the string to cut</param>
        /// <param name="delimiter">the delimiter to add afterwards</param>
        /// <param name="maxLength">the length of the resulting string including the delimiter</param>
        /// <returns></returns>
        public static string TrimWithDelim(this string toTrim, string delimiter = "...", int maxLength = 18)
        {
            return toTrim.AsSpan().TrimWithDelim(delimiter, maxLength).ToString();
        }

        public static ReadOnlySpan<char> TrimWithDelim(this ReadOnlySpan<char> toTrim, string delimiter = "...", int maxLength = 18)
        {
            int length = toTrim.Length, delimLength = delimiter.Length;
            if (length > maxLength - delimLength)
            {
                length = maxLength - delimLength;
            }
            return toTrim.Length > length ? string.Concat(toTrim[..length], delimiter.AsSpan()).AsSpan() : toTrim;
        }

        /// <summary>
        /// Tries to parse a line into the category it indicates.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The category representing the string, or none.</returns>
        public static StringCategory AsCategory(this string input)
        {
            return Utils.GetCategoryFromString(input);
        }

        /// <summary>
        /// Returns the string representation of a category.
        /// </summary>
        /// <param name="category">The Category to parse.</param>
        /// <returns>The string representing the category.</returns>
        public static string AsString(this StringCategory category)
        {
            return Utils.GetStringFromCategory(category);
        }
    }
}

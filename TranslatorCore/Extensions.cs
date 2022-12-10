using System;
using System.Text;
using System.Text.RegularExpressions;
using Translator.UICompatibilityLayer;

namespace Translator.Core.Helpers
{
    public static class Extensions
    {
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
            bool inVAHint = false;
            string output = "";
            foreach (char character in input)
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
                    output += character;
                }
            }

            return output;
        }

        /// <summary>
        /// Constrains the lenght of a single string to a multi line version of said string, 
		/// where every MaxTextLength characters a newline is inserted.
        /// </summary>
        /// <param name="input">The string to format</param>
        /// <returns>The formatted, blockified string</returns>
        public static string ConstrainLength(this string input)
        {
            int currentWordLength = 0, currentLength = 0;
            bool inWord;
            StringBuilder builder= new StringBuilder(input.Length);

            foreach (char c in input.AsSpan())
            {
                if (c != ' ' && c != '\n' && c != '\r')
                {
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
                if (currentLength <= Utils.MaxTextLength)
                {
                    builder.Append(c);
                }
                else
                {
                    if (inWord && currentWordLength < Utils.MaxWordLength)
                    {
                        //line is too long but we in a word
                        builder.Append(c);
                    }
                    else
                    {
                        builder.Append(c + "\n");
                        currentLength = 0;
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Deletes the characters to the left of the char until the first seperator or the start of the text.
        /// </summary>
        /// <param name="textBox">The textbox to work on</param>
        /// <returns>true if successful</returns>
        public static bool DeleteCharactersInTextLeft(this ITextBox textBox)
        {
            int oldPos = textBox.SelectionStart;
            MoveCursorWordLeft(textBox);
            int newPos = textBox.SelectionStart;
            if (oldPos - newPos > 0) textBox.Text = textBox.Text.Remove(newPos, oldPos - newPos);
            textBox.SelectionStart = newPos;
            //to stop winforms adding the weird backspace character to the text
            return true;
        }

        /// <summary>
        /// Deletes the characters to the right of the char until the first seperator or the end of the text.
        /// </summary>
        /// <param name="textBox">The textbox to work on</param>
        /// <returns>true if successful</returns>
        public static bool DeleteCharactersInTextRight(this ITextBox textBox)
        {
            int oldPos = textBox.SelectionStart;
            MoveCursorWordRight(textBox);
            int newPos = textBox.SelectionStart;
            if (newPos - oldPos > 0) textBox.Text = textBox.Text.Remove(oldPos, newPos - oldPos);
            textBox.SelectionStart = oldPos;
            return true;
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
            int length = toTrim.Length, delimLength = delimiter.Length;
            if (length > maxLength - delimLength)
            {
                length = maxLength - delimLength;
            }
            return toTrim.Length > length ? toTrim[..length].Trim() + delimiter : toTrim;
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

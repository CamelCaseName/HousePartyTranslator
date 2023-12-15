using System;
using System.Runtime.InteropServices;
using System.Text;
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
            return Array.IndexOf(Utils.OfficialStories, Utils.ExtractStoryName(storyName)) >= 0;
        }

        /// <summary>
        /// Removes the voice actor infos from the given string. Info has to be encapsulated in []. 
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string RemoveVAHints(this string input, bool removeDoubleSpace = true)
        {
            return input.AsSpan().RemoveVAHints(removeDoubleSpace).ToString();
        }

        public static ReadOnlySpan<char> RemoveVAHints(this ReadOnlySpan<char> span, bool removeDoubleSpace = true)
        {
            bool inVAHint = false;
            var output = new Span<char>(new char[span.Length]);
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

            if (removeDoubleSpace && output.Length > 1)
            {
                for (int i = 0; i < output.Length - 1; i++)
                {
                    if (output[i] == ' ' && output[i + 1] == ' ')
                    {
                        int old_i = i;
                        while (output[++i] == ' ') ;
                        output = output.RemoveAt(old_i, i - old_i - 1);
                    }
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
                if (c is not ' ' and not '\n' and not '\r')
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
                    case "#":
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
                    case "#":
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

        public static int DigitCount(this int num)
        {
            int numToWorkOn = num;
            int count = 0;
            while ((numToWorkOn /= 10) > 10)
                ++count;
            return count;
        }

        public static ReadOnlySpan<char> RemoveAt(this ReadOnlySpan<char> span, int index, int count)
        {
            return span.RemoveAt(index, count);
        }

        public static Span<char> RemoveAt(this Span<char> span, int index, int count)
        {
            if (span.IsEmpty) return span;
            return index < 0
                ? throw new ArgumentOutOfRangeException(nameof(index), "The index cannot be negative")
                : count < 0
                ? throw new ArgumentOutOfRangeException(nameof(count), "The count cannot be negative")
                : index >= span.Length
                ? throw new ArgumentOutOfRangeException(nameof(index), "The index has to be less than the length of the span")
                : index + count >= span.Length
                ? throw new ArgumentOutOfRangeException(nameof(count), "The count added to the index has to be less than the length of the span")
                : MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference((ReadOnlySpan<char>)string.Concat(span[..index], span[(index + count)..])), span.Length - count);
        }
    }
}

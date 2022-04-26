using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace HousePartyTranslator.Helpers
{
    /// <summary>
    /// Provides some generic utility methods.
    /// </summary>
    static class Utils
    {
        public static readonly int MaxTextLength = 100;

        /// <summary>
        /// Removes the voice actor infos from the given string. Info has to be encapsulated in []. 
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string RemoveVAHints(string input)
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
        /// Constrains the lenght of a single string to a multi line version of said string, where every MaxTextLength characters a newline is inserted.
        /// </summary>
        /// <param name="input">The string to format</param>
        /// <returns>The formatted, blockified string</returns>
        public static string ConstrainLength(string input)
        {
            string output = "";
            int inputLength = input.Length;

            //TODO add lookahead and lookback for detecting "words", or change up how we split
            for (int i = 0; i <= (inputLength / MaxTextLength); i++)
            {
                int possibleEnd = Math.Min(MaxTextLength, input.Length);
                output += input.Substring(0, possibleEnd);
                if (possibleEnd == MaxTextLength) output += " \n";
                input = input.Remove(0, possibleEnd);
            }

            return output;
        }

        /// <summary>
        /// Deletes the characters to the left of the char until the first seperator or the start of the text.
        /// </summary>
        /// <param name="textBox">The textbox to work on</param>
        /// <returns>true if successful</returns>
        public static bool DeleteCharactersInTextLeft(TextBox textBox)
        {
            bool success = false;
            for (int i = textBox.SelectionStart - 1; i > 0; i--)
            {
                if (!success)
                {
                    switch (textBox.Text.Substring(i, 1))
                    {    //set up any stopping points you want
                        case " ":
                        case ";":
                        case ",":
                        case ".":
                        case "-":
                        case "'":
                        case "/":
                        case "\\":
                            textBox.Text = textBox.Text.Remove(i, textBox.SelectionStart - i);
                            textBox.SelectionStart = i;
                            success = true;
                            break;
                        case "\n":
                            textBox.Text = textBox.Text.Remove(i - 1, textBox.SelectionStart - i);
                            textBox.SelectionStart = i;
                            success = true;
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (!success)
            {
                textBox.Clear();
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Deletes the characters to the right of the char until the first seperator or the end of the text.
        /// </summary>
        /// <param name="textBox">The textbox to work on</param>
        /// <returns>true if successful</returns>
        public static bool DeleteCharactersInTextRight(TextBox textBox)
        {
            bool success = false;
            int sel = textBox.SelectionStart;
            for (int i = textBox.SelectionStart; i < textBox.TextLength - 1; i++)
            {
                if (!success)
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
                            textBox.Text = textBox.Text.Remove(textBox.SelectionStart, i - textBox.SelectionStart);
                            textBox.SelectionStart = sel;
                            success = true;
                            break;
                        case "\n":
                            textBox.Text = textBox.Text.Remove(textBox.SelectionStart, i - textBox.SelectionStart);
                            textBox.SelectionStart = sel;
                            success = true;
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (!success && textBox.SelectionStart < textBox.Text.Length - 1)
            {
                textBox.Text = textBox.Text.Remove(textBox.SelectionStart, textBox.Text.Length - textBox.SelectionStart);
                textBox.SelectionStart = sel;
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Gets the current assembly version as a string.
        /// </summary>
        /// <returns>The current assembly version</returns>
        public static string GetAssemblyFileVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersion.FileVersion;
        }
    }

}

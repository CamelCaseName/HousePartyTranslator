using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousePartyTranslator.Helpers
{
    static class Utils
    {
        public static readonly int MaxTextLength = 100;

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
    }

}

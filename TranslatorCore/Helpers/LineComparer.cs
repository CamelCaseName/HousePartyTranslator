﻿using System.Collections.Generic;
using Translator.Core.Data;

namespace Translator.Core.Helpers
{
    /// <summary>
    /// This class provides a Compare() method to compare LineData objects with a BGC ID
    /// </summary>
    public class BGCComparer : IComparer<LineData>
    {
        /// <summary>
        /// Compares the id of line1 and line2 and returns an indication
        /// </summary>
        /// <param name="line1"> The line to compare with line2</param>
        /// <param name="line2"> line1 is compare to this one</param>
        /// <returns>
        /// A signed number indicating the relative values of line1 and line2. Return
        /// Value Description Less than zero: line1 is before line2. Zero: line1
        /// is equal to line2. Greater than zero: line1 is after than line2.
        /// </returns>
        public int Compare(LineData? line1, LineData? line2)
        {
            if (line1 is null || line2 is null)
            {
                return CompareNullables(line1, line2);
            }
            //this comparer strips the id of the preceding BGC and then compares the resulting numbers
            return int.Parse(line1.ID[3..]).CompareTo(int.Parse(line2.ID[3..]));
        }

        /// <summary>
        /// Compares the id of line1 and line2 and returns an indication
        /// </summary>
        /// <param name="line1"> The line to compare with line2</param>
        /// <param name="line2"> line1 is compare to this one</param>
        /// <returns>
        /// A signed number indicating the relative values of line1 and line2. Return
        /// Value Description Less than zero: line1 is before line2. Zero: line1
        /// is equal to line2. Greater than zero: line1 is after than line2.
        /// </returns>
        public static int CompareNullables(LineData? line1, LineData? line2)
        {
            //this comparer strips the id of the preceding BGC and then compares the resulting numbers
            return int.Parse(line1?.ID[3..] ?? "-1").CompareTo(int.Parse(line2?.ID[3..] ?? "0"));
        }
    }

    /// <summary>
    /// This class provides a Compare() methiod for LineData objects with a seminumeric ID, especially hints.
    /// Intended for hint sorting, everythin else will be sorted albhabetically.
    /// </summary>
    public class GeneralComparer : IComparer<LineData>
    {
        /// <summary>
        /// Compares the id of line1 and line2 and returns an indication
        /// </summary>
        /// <param name="line1"> The line to compare with line2</param>
        /// <param name="line2"> line1 is compare to this one</param>
        /// <returns>
        /// A signed number indicating the relative values of line1 and line2. Return
        /// Value Description Less than zero: line1 is before line2. Zero: line1
        /// is equal to line2. Greater than zero: line1 is after than line2.
        /// </returns>
        public int Compare(LineData? line1, LineData? line2)
        {
            if (line1 is null || line2 is null)
            {
                return CompareNullables(line1, line2);
            }
            //this comparer checks if the id can be converted to a number, if so remove the u and comapre.
            int parsed2 = 0;
            bool isNumber = int.TryParse(line1.ID.Replace("u", string.Empty), out int parsed1) && int.TryParse(line2.ID.Replace("u", string.Empty), out parsed2);
            return isNumber ? parsed1.CompareTo(parsed2) : line1.ID.CompareTo(line2.ID);
        }

        /// <summary>
        /// Compares the id of line1 and line2 and returns an indication
        /// </summary>
        /// <param name="line1"> The line to compare with line2</param>
        /// <param name="line2"> line1 is compare to this one</param>
        /// <returns>
        /// A signed number indicating the relative values of line1 and line2. Return
        /// Value Description Less than zero: line1 is before line2. Zero: line1
        /// is equal to line2. Greater than zero: line1 is after than line2.
        /// </returns>
        public static int CompareNullables(LineData? line1, LineData? line2)
        {
            //this comparer checks if the id can be converted to a number, if so remove the u and comapre.
            int parsed2 = 0;
            bool isNumber = int.TryParse(line1?.ID?.Replace("u", string.Empty) ?? " -1", out int parsed1) && int.TryParse(line2?.ID?.Replace("u", string.Empty) ?? " 0", out parsed2);
            return isNumber ? parsed1.CompareTo(parsed2) : line1?.ID?.CompareTo(line2?.ID) ?? 0;
        }
    }
}
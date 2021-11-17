using System.Collections.Generic;

namespace HousePartyTranslator.Helpers
{
    class BGCComparer : IComparer<LineData>
    {
        public int Compare(LineData line1, LineData line2)
        {
            //this comparer strips the id of the preceding BGC and then compares the resulting numbers
            return int.Parse(line1.ID.Substring(3)).CompareTo(int.Parse(line2.ID.Substring(3)));
        }
    }

    class GeneralComparer : IComparer<LineData>
    {
        public int Compare(LineData line1, LineData line2)
        {
            //this comparer checks if the id can be converted to a number, if so remove the u and comapre.
            int parsed2 = 0;
            bool isNumber = int.TryParse(line1.ID.Replace("u", ""), out int parsed1) && int.TryParse(line2.ID.Replace("u", ""), out parsed2);
            return isNumber ? parsed1.CompareTo(parsed2) : line1.ID.CompareTo(line2.ID);
        }
    }
}

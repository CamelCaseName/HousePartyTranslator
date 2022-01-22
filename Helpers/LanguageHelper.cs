using System.Collections.Generic;

namespace HousePartyTranslator.Helpers
{
    static class LanguageHelper
    {
        public readonly static Dictionary<string, string> Languages = new Dictionary<string, string>() {
            {"cs","Czech" },
            {"da","Danish" },
            {"de","German" },
            {"fi","Finnish" },
            {"fr","French" },
            {"hu","Hungarian" },
            {"it","Italian" },
            {"ja","Japanese" },
            {"ko","Korean" },
            {"pl","Polish" },
            {"pt","Purtuguese" },
            {"ptbr","Portuguese, Brazilian" },
            {"es","Spanish" },
            {"esmx","Spanish, Mexico" },
            {"tr","Turkish" },
        };
    }
}

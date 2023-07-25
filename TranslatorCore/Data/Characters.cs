using System.Collections.Generic;

namespace Translator.Helpers
{
    public static class Characters
    {
        public static readonly Dictionary<string, CharacterEnum> CharacterDict = new(){
            {"Amy", CharacterEnum.amy},
            {"Arin",CharacterEnum.arin },
            {"Ashley", CharacterEnum.ashley},
            {"Brittney", CharacterEnum.brittney},
            {"Compubrah", CharacterEnum.compubrah},
            {"Dan", CharacterEnum.dan},
            {"Derek", CharacterEnum.derek},
            {"Frank", CharacterEnum.frank},
            {"Katherine", CharacterEnum.katherine},
            {"Leah", CharacterEnum.leah},
            {"Lety", CharacterEnum.lety},
            {"Madison", CharacterEnum.madison},
            {"Patrick", CharacterEnum.patrick},
            {"Phone Call", CharacterEnum.phonecall},
            {"Rachael", CharacterEnum.rachael},
            {"Stephanie", CharacterEnum.stephanie},
            {"Vickie", CharacterEnum.vickie}
};

        public enum CharacterEnum
        {
            amala,
            amy,
            arin,
            ashley,
            brittney,
            compubrah,
            dan,
            derek,
            dojacat,
            frank,
            katherine,
            leah,
            lety,
            lizkatz,
            madison,
            patrick,
            phonecall,
            rachael,
            rule34,
            stephanie,
            vickie
        }
    }
}

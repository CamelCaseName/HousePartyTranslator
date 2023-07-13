using System;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.DefaultImpls
{
    public class DefaultSettings : ISettings
    {
        public bool AdvancedModeEnabled { get => false; set { } }
        public bool AllowCustomStories { get => false; set { } }
        public bool AlsoSaveToGame { get => false; set { } }
        public bool AskForSaveDialog { get => false; set { } }
        public bool AutoLoadRecent { get => false; set { } }
        public bool AutoLoadRecentIndex { get => false; set { } }
        public bool AutoSave { get => false; set { } }
        public bool AutoTranslate { get => false; set { } }
        public bool DisplayVoiceActorHints { get => false; set { } }
        public bool DoDiscordRichPresence { get => false; set { } }
        public bool UseFalseFolder { get => false; set { } }
        public int RecentIndex { get => -1; set { } }
        public string DbPassword { get => string.Empty; set { } }
        public string DbUserName { get => string.Empty; set { } }
        public string FileVersion { get => string.Empty; set { } }
        public string Language { get => string.Empty; set { } }
        public string Recents0 { get => string.Empty; set { } }
        public string Recents1 { get => string.Empty; set { } }
        public string Recents2 { get => string.Empty; set { } }
        public string Recents3 { get => string.Empty; set { } }
        public string Recents4 { get => string.Empty; set { } }
        public string TemplatePath { get => string.Empty; set { } }
        public string TranslationPath { get => string.Empty; set { } }
        public TimeSpan AutoSaveInterval { get => new(); set { } }

        public bool IgnoreCustomStoryWarning { get => false; set { } }

        public bool IgnoreMissingLinesWarning { get => false; set { } }

        public bool ShowTranslationHighlight { get => false; set { } }
        public bool ShowCommentHighlight { get => false; set { } }

        public bool HighlightLanguages { get => false; set { } }

        public bool ExportTranslatedWithMissingLines { get => false; set { } }

        public bool ExportTemplateDiff { get => false; set { } }

        public void Save() { }
    }
}

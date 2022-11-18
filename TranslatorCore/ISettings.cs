using System;

namespace TranslatorCore
{
    internal interface ISettings
    {
        bool AdvancedModeEnabled { get; set; }
        bool AllowCustomStories { get; set; }
        bool AlsoSaveToGame { get; set; }
        bool AskForSaveDialog { get; set; }
        bool AutoLoadRecent { get; set; }
        bool AutoLoadRecentIndex { get; set; }
        bool AutoSave { get; set; }
        bool AutoTranslate { get; set; }
        bool DoDiscordRichPresence { get; set; }
        bool UseFalseFolder { get; set; }
        int RecentIndex { get; set; }
        string DbPassword { get; set; }
        string DbUserName { get; set; }
        string FileVersion { get; set; }
        string Language { get; set; }
        string Recents0 { get; set; }
        string Recents1 { get; set; }
        string Recents2 { get; set; }
        string Recents3 { get; set; }
        string Recents4 { get; set; }
        string TemplatePath { get; set; }
        string TranslationPath { get; set; }
        TimeSpan AutoSaveInterval { get; set; }
    }
}

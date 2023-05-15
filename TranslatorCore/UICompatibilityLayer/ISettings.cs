using System;
using Translator.Core.DefaultImpls;

namespace Translator.Core.UICompatibilityLayer
{
    public static class Settings
    {
        public static ISettings Default { get; private set; } = new DefaultSettings();
        public static bool IsInitialized { get; private set; }
        internal static bool Initialize(ISettings settings)
        {
            if (!IsInitialized)
            {
                Default = settings;
                IsInitialized = true;
            }
            return IsInitialized;
        }
    }

    public interface ISettings
    {
        public static ISettings Default { get { return Default ?? throw new NullReferenceException("Default settings were not initialized."); } }
        public bool AdvancedModeEnabled { get; set; }
        public bool AllowCustomStories { get; set; }
        public bool AlsoSaveToGame { get; set; }
        public bool AskForSaveDialog { get; set; }
        public bool AutoLoadRecent { get; set; }
        public bool AutoLoadRecentIndex { get; set; }
        public bool AutoSave { get; set; }
        public bool AutoTranslate { get; set; }
        public bool DisplayVoiceActorHints { get; set; }
        public bool DoDiscordRichPresence { get; set; }
        public bool UseFalseFolder { get; set; }
        public int RecentIndex { get; set; }
        public string DbPassword { get; set; }
        public string DbUserName { get; set; }
        public string FileVersion { get; set; }
        public string Language { get; set; }
        public string Recents0 { get; set; }
        public string Recents1 { get; set; }
        public string Recents2 { get; set; }
        public string Recents3 { get; set; }
        public string Recents4 { get; set; }
        public string TemplatePath { get; set; }
        public string TranslationPath { get; set; }
        public TimeSpan AutoSaveInterval { get; set; }
        public bool IgnoreCustomStoryWarning { get; set; }
        public bool IgnoreMissingLinesWarning { get; set; }

        public void Save();
    }
}

﻿using System;

namespace Translator.Core
{
    public class Settings
    {
        public static ISettings Default { get; private set; } = new DefaultSettings();
        public static bool IsInitialized { get; private set; }
        public Settings(ISettings settings)
        {
            Default = settings;
            IsInitialized = true;
        }
    }

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
        public void Save() { }
    }

    public interface ISettings
    {
        public bool AdvancedModeEnabled { get; set; }
        public bool AllowCustomStories { get; set; }
        public bool AlsoSaveToGame { get; set; }
        public bool AskForSaveDialog { get; set; }
        public bool AutoLoadRecent { get; set; }
        public bool AutoLoadRecentIndex { get; set; }
        public bool AutoSave { get; set; }
        public bool AutoTranslate { get; set; }
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
        public void Save();
    }
}
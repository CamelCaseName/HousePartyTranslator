using Translator.Core;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinAdminSettings : ISettings
    {
        public bool AdvancedModeEnabled { get => true; set { } }
        public bool AllowCustomStories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AlsoSaveToGame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AskForSaveDialog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoLoadRecent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoLoadRecentIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoSave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoTranslate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DoDiscordRichPresence { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UseFalseFolder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int RecentIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DbPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DbUserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string FileVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Language { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Recents0 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Recents1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Recents2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Recents3 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Recents4 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TemplatePath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TranslationPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan AutoSaveInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Save() => throw new NotImplementedException();
    }
}

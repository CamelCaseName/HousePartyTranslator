using Translator.Core;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinAdminSettings : ISettings
    {
        //todo
        public bool AdvancedModeEnabled
        {
            get
            {
#if DEBUG || RELEASE
                return true;
#elif DEBUG_USER || RELEASE_USER
                return false;
#endif
            }
            set { }
        }
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
        public string DbPassword { get => Properties.Settings.Default.dbPassword; set => Properties.Settings.Default.dbPassword = value; }
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

        public void Save() => Properties.Settings.Default.Save();
    }
}

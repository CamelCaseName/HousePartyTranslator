using Translator.Core;
namespace TranslatorApp.InterfaceImpls
{
	internal class WinSettings : ISettings
	{
		public static readonly ISettings Default = new WinSettings();
		public bool AdvancedModeEnabled
		{
			get
			{
#if DEBUG_ADMIN || RELEASE_ADMIN
				return true;
#elif DEBUG || RELEASE
                return false;
#endif
			}
			set { }
		}
		public bool EnableDiscordRP { get => Properties.Settings.Default.enableDiscordRP; set => Properties.Settings.Default.enableDiscordRP = value; }
		public bool AllowCustomStories { get => Properties.Settings.Default.EnableCustomStories; set => Properties.Settings.Default.EnableCustomStories = value; }
		public bool AlsoSaveToGame { get => Properties.Settings.Default.alsoSaveToGame; set => Properties.Settings.Default.alsoSaveToGame = value; }
		public bool AskForSaveDialog { get => Properties.Settings.Default.askForSaveDialog; set => Properties.Settings.Default.askForSaveDialog = value; }
		public bool AutoLoadRecent { get => Properties.Settings.Default.autoLoadRecent; set => Properties.Settings.Default.autoLoadRecent = value; }
		public bool AutoLoadRecentIndex { get => Properties.Settings.Default.autoLoadRecentIndex; set => Properties.Settings.Default.autoLoadRecentIndex = value; }
		public bool AutoSave { get => Properties.Settings.Default.autoSave; set => Properties.Settings.Default.autoSave = value; }
		public bool AutoTranslate { get => Properties.Settings.Default.autoTranslate; set => Properties.Settings.Default.autoTranslate = value; }
		public bool DoDiscordRichPresence { get => Properties.Settings.Default.enableDiscordRP; set => Properties.Settings.Default.enableDiscordRP = value; }
		public bool UseFalseFolder { get => Properties.Settings.Default.useFalseFolder; set => Properties.Settings.Default.useFalseFolder = value; }
		public int RecentIndex { get => Properties.Settings.Default.recent_index; set => Properties.Settings.Default.recent_index = value; }
		public string DbPassword { get => Properties.Settings.Default.dbPassword; set => Properties.Settings.Default.dbPassword = value; }
		public string DbUserName { get => Properties.Settings.Default.dbUserName; set => Properties.Settings.Default.dbUserName = value; }
		public string FileVersion { get => Properties.Settings.Default.version; set => Properties.Settings.Default.version = value; }
		public string Language { get => Properties.Settings.Default.language; set => Properties.Settings.Default.language = value; }
		public string Recents0 { get => Properties.Settings.Default.recents_0; set => Properties.Settings.Default.recents_0 = value; }
		public string Recents1 { get => Properties.Settings.Default.recents_1; set => Properties.Settings.Default.recents_1 = value; }
		public string Recents2 { get => Properties.Settings.Default.recents_2; set => Properties.Settings.Default.recents_2 = value; }
		public string Recents3 { get => Properties.Settings.Default.recents_3; set => Properties.Settings.Default.recents_3 = value; }
		public string Recents4 { get => Properties.Settings.Default.recents_4; set => Properties.Settings.Default.recents_4 = value; }
		public string TemplatePath { get => Properties.Settings.Default.template_path; set => Properties.Settings.Default.template_path = value; }
		public string TranslationPath { get => Properties.Settings.Default.translation_path; set => Properties.Settings.Default.translation_path = value; }
		public TimeSpan AutoSaveInterval { get => Properties.Settings.Default.AutoSaveInterval; set => Properties.Settings.Default.AutoSaveInterval = value; }
		public bool DisplayVAHints { get => Properties.Settings.Default.displayVAHints; set => Properties.Settings.Default.displayVAHints = value; }
		public string StoryPath { get => Properties.Settings.Default.story_path; set => Properties.Settings.Default.story_path = value; }
		public bool UpdateSettings { get => Properties.Settings.Default.UpdateSettings; set => Properties.Settings.Default.UpdateSettings = value; }
		public bool IgnoreCustomStoryWarning { get => Properties.Settings.Default.IgnoreCustomStoryWarning; set => Properties.Settings.Default.IgnoreCustomStoryWarning = value; }

		public void Save() => Properties.Settings.Default.Save();
		internal void Upgrade() => Properties.Settings.Default.Upgrade();
	}
}

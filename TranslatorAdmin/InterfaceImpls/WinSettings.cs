using Translator.Properties;
namespace Translator.InterfaceImpls
{
    internal class WinSettings : Core.ISettings
    {
        public static readonly Core.ISettings Default = new WinSettings();

        public static WinSettings WDefault => (WinSettings)Default;
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
        public bool EnableDiscordRP { get => Settings.Default.enableDiscordRP; set => Settings.Default.enableDiscordRP = value; }
        public bool AllowCustomStories { get => Settings.Default.EnableCustomStories; set => Settings.Default.EnableCustomStories = value; }
        public bool AlsoSaveToGame { get => Settings.Default.alsoSaveToGame; set => Settings.Default.alsoSaveToGame = value; }
        public bool AskForSaveDialog { get => Settings.Default.askForSaveDialog; set => Settings.Default.askForSaveDialog = value; }
        public bool AutoLoadRecent { get => Settings.Default.autoLoadRecent; set => Settings.Default.autoLoadRecent = value; }
        public bool AutoLoadRecentIndex { get => Settings.Default.autoLoadRecentIndex; set => Settings.Default.autoLoadRecentIndex = value; }
        public bool AutoSave { get => Settings.Default.autoSave; set => Settings.Default.autoSave = value; }
        public bool AutoTranslate { get => Settings.Default.autoTranslate; set => Settings.Default.autoTranslate = value; }
        public bool DoDiscordRichPresence { get => Settings.Default.enableDiscordRP; set => Settings.Default.enableDiscordRP = value; }
        public bool UseFalseFolder { get => Settings.Default.useFalseFolder; set => Settings.Default.useFalseFolder = value; }
        public int RecentIndex { get => Settings.Default.recent_index; set => Settings.Default.recent_index = value; }
        public string DbPassword { get => Settings.Default.dbPassword; set => Settings.Default.dbPassword = value; }
        public string DbUserName { get => Settings.Default.dbUserName; set => Settings.Default.dbUserName = value; }
        public string FileVersion { get => Settings.Default.version; set => Settings.Default.version = value; }
        public string Language { get => Settings.Default.language; set => Settings.Default.language = value; }
        public string Recents0 { get => Settings.Default.recents_0; set => Settings.Default.recents_0 = value; }
        public string Recents1 { get => Settings.Default.recents_1; set => Settings.Default.recents_1 = value; }
        public string Recents2 { get => Settings.Default.recents_2; set => Settings.Default.recents_2 = value; }
        public string Recents3 { get => Settings.Default.recents_3; set => Settings.Default.recents_3 = value; }
        public string Recents4 { get => Settings.Default.recents_4; set => Settings.Default.recents_4 = value; }
        public string TemplatePath { get => Settings.Default.template_path; set => Settings.Default.template_path = value; }
        public string TranslationPath { get => Settings.Default.translation_path; set => Settings.Default.translation_path = value; }
        public TimeSpan AutoSaveInterval { get => Settings.Default.AutoSaveInterval; set => Settings.Default.AutoSaveInterval = value; }
        public bool DisplayVAHints { get => Settings.Default.displayVAHints; set => Settings.Default.displayVAHints = value; }
        public string StoryPath { get => Settings.Default.story_path; set => Settings.Default.story_path = value; }
        public bool UpdateSettings { get => Settings.Default.UpdateSettings; set => Settings.Default.UpdateSettings = value; }
        public bool IgnoreCustomStoryWarning { get => Settings.Default.IgnoreCustomStoryWarning; set => Settings.Default.IgnoreCustomStoryWarning = value; }

        public Color ItemNodeColor { get => Settings.Default.NodeItemColor; set => Settings.Default.NodeItemColor = value; }
        public Color ItemGroupNodeColor { get => Settings.Default.NodeItemGroupColor; set => Settings.Default.NodeItemGroupColor = value; }
        public Color InfoNodeColor { get => Settings.Default.NodeInfoColor; set => Settings.Default.NodeInfoColor = value; }
        public Color MovingNodeColor { get => Settings.Default.NodeMovingColor; set => Settings.Default.NodeMovingColor = value; }
        public Color ActionNodeColor { get => Settings.Default.NodeActionColor; set => Settings.Default.NodeActionColor = value; }
        public Color BGCNodeColor { get => Settings.Default.NodeBGCColor; set => Settings.Default.NodeBGCColor = value; }
        public Color DoorNodeColor { get => Settings.Default.NodeDoorColor; set => Settings.Default.NodeDoorColor = value; }
        public Color CutsceneNodeColor { get => Settings.Default.NodeCutsceneColor; set => Settings.Default.NodeCutsceneColor = value; }
        public Color AchievementNodeColor { get => Settings.Default.NodeAchievementColor; set => Settings.Default.NodeAchievementColor = value; }
        public Color ClothingNodeColor { get => Settings.Default.NodeClothingColor; set => Settings.Default.NodeClothingColor = value; }
        public Color CriteriaGroupNodeColor { get => Settings.Default.NodeCriteriaGroupColor; set => Settings.Default.NodeCriteriaGroupColor = value; }
        public Color CriterionNodeColor { get => Settings.Default.NodeCriterionColor; set => Settings.Default.NodeCriterionColor = value; }
        public Color DialogueNodeColor { get => Settings.Default.NodeDialogueColor; set => Settings.Default.NodeDialogueColor = value; }
        public Color DialogueFemaleOnlyNodeColor { get => Settings.Default.NodeDialogueFemaleOnlyColor; set => Settings.Default.NodeDialogueFemaleOnlyColor = value; }
        public Color DialogueMaleOnlyNodeColor { get => Settings.Default.NodeDialogueMaleOnlyColor; set => Settings.Default.NodeDialogueMaleOnlyColor = value; }
        public Color EventNodeColor { get => Settings.Default.NodeEventColor; set => Settings.Default.NodeEventColor = value; }
        public Color InventoryNodeColor { get => Settings.Default.NodeInventoryColor; set => Settings.Default.NodeInventoryColor = value; }
        public Color PersonalityNodeColor { get => Settings.Default.NodePersonalityColor; set => Settings.Default.NodePersonalityColor = value; }
        public Color PoseNodeColor { get => Settings.Default.NodePoseColor; set => Settings.Default.NodePoseColor = value; }
        public Color PropertyNodeColor { get => Settings.Default.NodePropertyColor; set => Settings.Default.NodePropertyColor = value; }
        public Color QuestNodeColor { get => Settings.Default.NodeQuestColor; set => Settings.Default.NodeQuestColor = value; }
        public Color ReactionNodeColor { get => Settings.Default.NodeReactionColor; set => Settings.Default.NodeReactionColor = value; }
        public Color ResponseNodeColor { get => Settings.Default.NodeResponseColor; set => Settings.Default.NodeResponseColor = value; }
        public Color SelectedNodeColor { get => Settings.Default.NodeSelectedColor; set => Settings.Default.NodeSelectedColor = value; }
        public Color SocialNodeColor { get => Settings.Default.NodeSocialColor; set => Settings.Default.NodeSocialColor = value; }
        public Color StateNodeColor { get => Settings.Default.NodeStateColor; set => Settings.Default.NodeStateColor = value; }
        public Color ValueNodeColor { get => Settings.Default.NodeValueColor; set => Settings.Default.NodeValueColor = value; }
        public Color DefaultNodeColor { get => Settings.Default.NodeDefaultColor; set => Settings.Default.NodeDefaultColor = value; }
        public Color DefaultEdgeColor { get => Settings.Default.EdgeDefaultColor; set => Settings.Default.EdgeDefaultColor = value; }
        public bool UseRainbowEdgeColors { get => Settings.Default.UseRainbowEdges; set => Settings.Default.UseRainbowEdges = value; }
        public bool UseRainbowNodeColors { get => Settings.Default.UseRainbowNodes; set => Settings.Default.UseRainbowNodes = value; }

        public void Save() => Settings.Default.Save();
        internal void Upgrade() => Settings.Default.Upgrade();
    }
}

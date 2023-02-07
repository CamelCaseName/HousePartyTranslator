using Translator.Core;
namespace Translator.InterfaceImpls
{
    internal class WinSettings : ISettings
    {
        public static readonly ISettings Default = new WinSettings();

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

        public Color ItemNodeColor { get => Properties.Settings.Default.NodeItemColor; set => Properties.Settings.Default.NodeItemColor = value; }
        public Color ItemGroupNodeColor { get => Properties.Settings.Default.NodeItemGroupColor; set => Properties.Settings.Default.NodeItemGroupColor = value; }
        public Color InfoNodeColor { get => Properties.Settings.Default.NodeInfoColor; set => Properties.Settings.Default.NodeInfoColor = value; }
        public Color MovingNodeColor { get => Properties.Settings.Default.NodeMovingColor; set => Properties.Settings.Default.NodeMovingColor = value; }
        public Color ActionNodeColor { get => Properties.Settings.Default.NodeActionColor; set => Properties.Settings.Default.NodeActionColor = value; }
        public Color BGCNodeColor { get => Properties.Settings.Default.NodeBGCColor; set => Properties.Settings.Default.NodeBGCColor = value; }
        public Color DoorNodeColor { get => Properties.Settings.Default.NodeDoorColor; set => Properties.Settings.Default.NodeDoorColor = value; }
        public Color CutsceneNodeColor { get => Properties.Settings.Default.NodeCutsceneColor; set => Properties.Settings.Default.NodeCutsceneColor = value; }
        public Color AchievementNodeColor { get => Properties.Settings.Default.NodeAchievementColor; set => Properties.Settings.Default.NodeAchievementColor = value; }
        public Color ClothingNodeColor { get => Properties.Settings.Default.NodeClothingColor; set => Properties.Settings.Default.NodeClothingColor = value; }
        public Color CriteriaGroupNodeColor { get => Properties.Settings.Default.NodeCriteriaGroupColor; set => Properties.Settings.Default.NodeCriteriaGroupColor = value; }
        public Color CriterionNodeColor { get => Properties.Settings.Default.NodeCriterionColor; set => Properties.Settings.Default.NodeCriterionColor = value; }
        public Color DialogueNodeColor { get => Properties.Settings.Default.NodeDialogueColor; set => Properties.Settings.Default.NodeDialogueColor = value; }
        public Color DialogueFemaleOnlyNodeColor { get => Properties.Settings.Default.NodeDialogueFemaleOnlyColor; set => Properties.Settings.Default.NodeDialogueFemaleOnlyColor = value; }
        public Color DialogueMaleOnlyNodeColor { get => Properties.Settings.Default.NodeDialogueMaleOnlyColor; set => Properties.Settings.Default.NodeDialogueMaleOnlyColor = value; }
        public Color EventNodeColor { get => Properties.Settings.Default.NodeEventColor; set => Properties.Settings.Default.NodeEventColor = value; }
        public Color InventoryNodeColor { get => Properties.Settings.Default.NodeInventoryColor; set => Properties.Settings.Default.NodeInventoryColor = value; }
        public Color PersonalityNodeColor { get => Properties.Settings.Default.NodePersonalityColor; set => Properties.Settings.Default.NodePersonalityColor = value; }
        public Color PoseNodeColor { get => Properties.Settings.Default.NodePoseColor; set => Properties.Settings.Default.NodePoseColor = value; }
        public Color PropertyNodeColor { get => Properties.Settings.Default.NodePropertyColor; set => Properties.Settings.Default.NodePropertyColor = value; }
        public Color QuestNodeColor { get => Properties.Settings.Default.NodeQuestColor; set => Properties.Settings.Default.NodeQuestColor = value; }
        public Color ReactionNodeColor { get => Properties.Settings.Default.NodeReactionColor; set => Properties.Settings.Default.NodeReactionColor = value; }
        public Color ResponseNodeColor { get => Properties.Settings.Default.NodeResponseColor; set => Properties.Settings.Default.NodeResponseColor = value; }
        public Color SelectedNodeColor { get => Properties.Settings.Default.NodeSelectedColor; set => Properties.Settings.Default.NodeSelectedColor = value; }
        public Color SocialNodeColor { get => Properties.Settings.Default.NodeSocialColor; set => Properties.Settings.Default.NodeSocialColor = value; }
        public Color StateNodeColor { get => Properties.Settings.Default.NodeStateColor; set => Properties.Settings.Default.NodeStateColor = value; }
        public Color ValueNodeColor { get => Properties.Settings.Default.NodeValueColor; set => Properties.Settings.Default.NodeValueColor = value; }
        public Color DefaultNodeColor { get => Properties.Settings.Default.NodeDefaultColor; set => Properties.Settings.Default.NodeDefaultColor = value; }
        public Color DefaultEdgeColor { get => Properties.Settings.Default.EdgeDefaultColor; set => Properties.Settings.Default.EdgeDefaultColor = value; }
        public bool UseRainbowEdgeColors { get => Properties.Settings.Default.UseRainbowEdges; set => Properties.Settings.Default.UseRainbowEdges = value; }

        public void Save() => Properties.Settings.Default.Save();
        internal void Upgrade() => Properties.Settings.Default.Upgrade();
    }
}

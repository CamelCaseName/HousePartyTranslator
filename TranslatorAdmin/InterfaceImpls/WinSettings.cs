using Translator.Core.Helpers;
using Translator.Desktop.Properties;
namespace Translator.Desktop.InterfaceImpls
{
    internal class WinSettings : Core.UICompatibilityLayer.ISettings
    {
        public static Core.UICompatibilityLayer.ISettings Default => Core.UICompatibilityLayer.Settings.Default;
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
        public static bool EnableDiscordRP { get => Settings.Default.enableDiscordRP; set => Settings.Default.enableDiscordRP = value; }
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
        public bool DisplayVoiceActorHints { get => Settings.Default.displayVAHints; set => Settings.Default.displayVAHints = value; }
        public static string StoryPath { get => Settings.Default.story_path; set => Settings.Default.story_path = value; }
        public static bool UpdateSettings { get => Settings.Default.UpdateSettings; set => Settings.Default.UpdateSettings = value; }
        public bool IgnoreCustomStoryWarning { get => Settings.Default.IgnoreCustomStoryWarning; set => Settings.Default.IgnoreCustomStoryWarning = value; }
        public bool IgnoreMissingLinesWarning { get => Settings.Default.IgnoreMissingLinesWarning; set => Settings.Default.IgnoreMissingLinesWarning = value; }

        public static Color ItemNodeColor { get => Settings.Default.NodeItemColor; set => Settings.Default.NodeItemColor = value; }
        public static Color ItemGroupNodeColor { get => Settings.Default.NodeItemGroupColor; set => Settings.Default.NodeItemGroupColor = value; }
        public static Color InfoNodeColor { get => Settings.Default.NodeInfoColor; set => Settings.Default.NodeInfoColor = value; }
        public static Color MovingNodeColor { get => Settings.Default.NodeMovingColor; set => Settings.Default.NodeMovingColor = value; }
        public static Color ActionNodeColor { get => Settings.Default.NodeActionColor; set => Settings.Default.NodeActionColor = value; }
        public static Color BGCNodeColor { get => Settings.Default.NodeBGCColor; set => Settings.Default.NodeBGCColor = value; }
        public static Color DoorNodeColor { get => Settings.Default.NodeDoorColor; set => Settings.Default.NodeDoorColor = value; }
        public static Color CutsceneNodeColor { get => Settings.Default.NodeCutsceneColor; set => Settings.Default.NodeCutsceneColor = value; }
        public static Color AchievementNodeColor { get => Settings.Default.NodeAchievementColor; set => Settings.Default.NodeAchievementColor = value; }
        public static Color ClothingNodeColor { get => Settings.Default.NodeClothingColor; set => Settings.Default.NodeClothingColor = value; }
        public static Color CriteriaGroupNodeColor { get => Settings.Default.NodeCriteriaGroupColor; set => Settings.Default.NodeCriteriaGroupColor = value; }
        public static Color CriterionNodeColor { get => Settings.Default.NodeCriterionColor; set => Settings.Default.NodeCriterionColor = value; }
        public static Color DialogueNodeColor { get => Settings.Default.NodeDialogueColor; set => Settings.Default.NodeDialogueColor = value; }
        public static Color DialogueFemaleOnlyNodeColor { get => Settings.Default.NodeDialogueFemaleOnlyColor; set => Settings.Default.NodeDialogueFemaleOnlyColor = value; }
        public static Color DialogueMaleOnlyNodeColor { get => Settings.Default.NodeDialogueMaleOnlyColor; set => Settings.Default.NodeDialogueMaleOnlyColor = value; }
        public static Color EventNodeColor { get => Settings.Default.NodeEventColor; set => Settings.Default.NodeEventColor = value; }
        public static Color InventoryNodeColor { get => Settings.Default.NodeInventoryColor; set => Settings.Default.NodeInventoryColor = value; }
        public static Color PersonalityNodeColor { get => Settings.Default.NodePersonalityColor; set => Settings.Default.NodePersonalityColor = value; }
        public static Color PoseNodeColor { get => Settings.Default.NodePoseColor; set => Settings.Default.NodePoseColor = value; }
        public static Color PropertyNodeColor { get => Settings.Default.NodePropertyColor; set => Settings.Default.NodePropertyColor = value; }
        public static Color QuestNodeColor { get => Settings.Default.NodeQuestColor; set => Settings.Default.NodeQuestColor = value; }
        public static Color ReactionNodeColor { get => Settings.Default.NodeReactionColor; set => Settings.Default.NodeReactionColor = value; }
        public static Color ResponseNodeColor { get => Settings.Default.NodeResponseColor; set => Settings.Default.NodeResponseColor = value; }
        public static Color SelectedNodeColor { get => Settings.Default.NodeSelectedColor; set => Settings.Default.NodeSelectedColor = value; }
        public static Color SocialNodeColor { get => Settings.Default.NodeSocialColor; set => Settings.Default.NodeSocialColor = value; }
        public static Color StateNodeColor { get => Settings.Default.NodeStateColor; set => Settings.Default.NodeStateColor = value; }
        public static Color ValueNodeColor { get => Settings.Default.NodeValueColor; set => Settings.Default.NodeValueColor = value; }
        public static Color DefaultNodeColor { get => Settings.Default.NodeDefaultColor; set => Settings.Default.NodeDefaultColor = value; }
        public static Color DefaultEdgeColor { get => Settings.Default.EdgeDefaultColor; set => Settings.Default.EdgeDefaultColor = value; }
        public static bool UseRainbowEdgeColors { get => Settings.Default.UseRainbowEdges; set => Settings.Default.UseRainbowEdges = value; }
        public static bool UseRainbowNodeColors { get => Settings.Default.UseRainbowNodes; set => Settings.Default.UseRainbowNodes = value; }
        public static bool EnableStoryExplorerEdit { get => Settings.Default.EnableStoryExplorerEdit; set => Settings.Default.EnableStoryExplorerEdit = value; }
        public static int MaxEdgeCount { get => Settings.Default.ExplorerMaxEdgeCount; set => Settings.Default.ExplorerMaxEdgeCount = value; }
        public static bool ShowExtendedExplorerInfo { get => Settings.Default.ShowExtendedExplorerInfo; set => Settings.Default.ShowExtendedExplorerInfo = value; }
        public static float IdealLength { get => Settings.Default.IdealLength; internal set => Settings.Default.IdealLength = value; }
        public static int ColoringDepth { get => Settings.Default.ColoringDepth; internal set => Settings.Default.ColoringDepth = value; }
        public void Save()
        {
            Settings.Default.Save();
            LogManager.Log("Settings saved successfully");
        }
        internal static void Upgrade() => Settings.Default.Upgrade();
    }
}

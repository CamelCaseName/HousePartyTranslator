using System;
using System.Drawing;
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
        public bool AllowCustomStories { get => Settings.Default.enableCustomStories; set => Settings.Default.enableCustomStories = value; }
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
        public TimeSpan AutoSaveInterval { get => Settings.Default.autoSaveInterval; set => Settings.Default.autoSaveInterval = value; }
        public bool DisplayVoiceActorHints { get => Settings.Default.displayVAHints; set => Settings.Default.displayVAHints = value; }
        public static string StoryPath { get => Settings.Default.story_path; set => Settings.Default.story_path = value; }
        public static bool UpdateSettings { get => Settings.Default.updateSettings; set => Settings.Default.updateSettings = value; }
        public bool IgnoreCustomStoryWarning { get => Settings.Default.ignoreCustomStoryWarning; set => Settings.Default.ignoreCustomStoryWarning = value; }
        public bool IgnoreMissingLinesWarning { get => Settings.Default.ignoreMissingLinesWarning; set => Settings.Default.ignoreMissingLinesWarning = value; }
        public static Color ItemNodeColor { get => Settings.Default.nodeItemColor; set => Settings.Default.nodeItemColor = value; }
        public static Color ItemGroupNodeColor { get => Settings.Default.nodeItemGroupColor; set => Settings.Default.nodeItemGroupColor = value; }
        public static Color InfoNodeColor { get => Settings.Default.nodeInfoColor; set => Settings.Default.nodeInfoColor = value; }
        public static Color MovingNodeColor { get => Settings.Default.nodeMovingColor; set => Settings.Default.nodeMovingColor = value; }
        public static Color ActionNodeColor { get => Settings.Default.nodeActionColor; set => Settings.Default.nodeActionColor = value; }
        public static Color BGCNodeColor { get => Settings.Default.nodeBGCColor; set => Settings.Default.nodeBGCColor = value; }
        public static Color DoorNodeColor { get => Settings.Default.nodeDoorColor; set => Settings.Default.nodeDoorColor = value; }
        public static Color CutsceneNodeColor { get => Settings.Default.nodeCutsceneColor; set => Settings.Default.nodeCutsceneColor = value; }
        public static Color AchievementNodeColor { get => Settings.Default.nodeAchievementColor; set => Settings.Default.nodeAchievementColor = value; }
        public static Color ClothingNodeColor { get => Settings.Default.nodeClothingColor; set => Settings.Default.nodeClothingColor = value; }
        public static Color CriteriaGroupNodeColor { get => Settings.Default.nodeCriteriaGroupColor; set => Settings.Default.nodeCriteriaGroupColor = value; }
        public static Color CriterionNodeColor { get => Settings.Default.nodeCriterionColor; set => Settings.Default.nodeCriterionColor = value; }
        public static Color DialogueNodeColor { get => Settings.Default.nodeDialogueColor; set => Settings.Default.nodeDialogueColor = value; }
        public static Color DialogueFemaleOnlyNodeColor { get => Settings.Default.nodeDialogueFemaleOnlyColor; set => Settings.Default.nodeDialogueFemaleOnlyColor = value; }
        public static Color DialogueMaleOnlyNodeColor { get => Settings.Default.nodeDialogueMaleOnlyColor; set => Settings.Default.nodeDialogueMaleOnlyColor = value; }
        public static Color EventNodeColor { get => Settings.Default.nodeEventColor; set => Settings.Default.nodeEventColor = value; }
        public static Color InventoryNodeColor { get => Settings.Default.nodeInventoryColor; set => Settings.Default.nodeInventoryColor = value; }
        public static Color PersonalityNodeColor { get => Settings.Default.nodePersonalityColor; set => Settings.Default.nodePersonalityColor = value; }
        public static Color PoseNodeColor { get => Settings.Default.nodePoseColor; set => Settings.Default.nodePoseColor = value; }
        public static Color PropertyNodeColor { get => Settings.Default.nodePropertyColor; set => Settings.Default.nodePropertyColor = value; }
        public static Color QuestNodeColor { get => Settings.Default.nodeQuestColor; set => Settings.Default.nodeQuestColor = value; }
        public static Color ReactionNodeColor { get => Settings.Default.nodeReactionColor; set => Settings.Default.nodeReactionColor = value; }
        public static Color ResponseNodeColor { get => Settings.Default.nodeResponseColor; set => Settings.Default.nodeResponseColor = value; }
        public static Color SelectedNodeColor { get => Settings.Default.nodeSelectedColor; set => Settings.Default.nodeSelectedColor = value; }
        public static Color SocialNodeColor { get => Settings.Default.nodeSocialColor; set => Settings.Default.nodeSocialColor = value; }
        public static Color StateNodeColor { get => Settings.Default.nodeStateColor; set => Settings.Default.nodeStateColor = value; }
        public static Color ValueNodeColor { get => Settings.Default.nodeValueColor; set => Settings.Default.nodeValueColor = value; }
        public static Color DefaultNodeColor { get => Settings.Default.nodeDefaultColor; set => Settings.Default.nodeDefaultColor = value; }
        public static Color DefaultEdgeColor { get => Settings.Default.edgeDefaultColor; set => Settings.Default.edgeDefaultColor = value; }
        public static bool UseRainbowEdgeColors { get => Settings.Default.useRainbowEdges; set => Settings.Default.useRainbowEdges = value; }
        public static bool UseRainbowNodeColors { get => Settings.Default.useRainbowNodes; set => Settings.Default.useRainbowNodes = value; }
        public static bool EnableStoryExplorerEdit { get => Settings.Default.enableStoryExplorerEdit; set => Settings.Default.enableStoryExplorerEdit = value; }
        public static int MaxEdgeCount { get => Settings.Default.explorerMaxEdgeCount; set => Settings.Default.explorerMaxEdgeCount = value; }
        public static bool ShowExtendedExplorerInfo { get => Settings.Default.showExtendedExplorerInfo; set => Settings.Default.showExtendedExplorerInfo = value; }
        public static float IdealLength { get => Settings.Default.idealLength; internal set => Settings.Default.idealLength = value; }
        public static int ColoringDepth { get => Settings.Default.coloringDepth; internal set => Settings.Default.coloringDepth = value; }
        public bool ShowTranslationHighlight { get => Settings.Default.showSearchHighlightTranslation; set => Settings.Default.showSearchHighlightTranslation = value; }
        public bool ShowCommentHighlight { get => Settings.Default.showSearchHighlightComments; set => Settings.Default.showSearchHighlightComments = value; }
        public bool HighlightLanguages { get => Settings.Default.highlightLanguages; set => Settings.Default.highlightLanguages = value; }
        public bool ExportTranslatedWithMissingLines { get => Settings.Default.exportTranslationInMissingLines; set => Settings.Default.exportTranslationInMissingLines = value; }
        public bool ExportTemplateDiff { get => Settings.Default.exportTemplateDiff; set => Settings.Default.exportTemplateDiff = value; }

        public void Save()
        {
            Settings.Default.Save();
            LogManager.Log("Settings saved successfully");
        }
        internal static void Upgrade() => Settings.Default.Upgrade();
    }
}

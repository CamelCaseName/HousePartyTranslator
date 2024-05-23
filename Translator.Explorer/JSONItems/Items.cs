using static Translator.Explorer.JSONItems.StoryEnums;

namespace Translator.Explorer.JSONItems
{
    public sealed class Criterion
    {
        public BoolCritera? BoolValue { get; set; }
        public string? Character { get; set; }
        public string? Character2 { get; set; }
        public CompareTypes? CompareType { get; set; }
        public DialogueStatuses? DialogueStatus { get; set; }
        public bool DisplayInEditor { get; set; }
        public DoorOptionValues? DoorOptions { get; set; }
        public EqualsValues? EqualsValue { get; set; }
        public ComparisonEquations? EquationValue { get; set; }
        public ValueSpecificFormulas? ValueFormula { get; set; }
        public ItemComparisonTypes? ItemComparison { get; set; }
        public ItemFromItemGroupComparisonTypes? ItemFromItemGroupComparison { get; set; }
        public string? Key { get; set; }
        public string? Key2 { get; set; }
        public int Order { get; set; }
        public PlayerInventoryOptions? PlayerInventoryOption { get; set; }
        public PoseOptions? PoseOption { get; set; }
        public SocialStatuses? SocialStatus { get; set; }
        public string? Value { get; set; }
        public int Option { get; set; }
        public int GroupSubCompareType { get; set; }
    }

    public sealed class ItemAction
    {
        public string? ActionName { get; set; }
        public List<Criterion>? Criteria { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<GameEvent>? OnTakeActionEvents { get; set; }
    }

    public sealed class UseWith
    {
        public List<Criterion>? Criteria { get; set; }
        public string? CustomCantDoThatMessage { get; set; }
        public bool DisplayInEditor { get; set; }
        public string? ItemName { get; set; }
        public List<GameEvent>? OnSuccessEvents { get; set; }
    }

    public sealed class ItemOverride
    {
        public string? Id { get; set; }
        public bool DisplayInEditor { get; set; }
        public string? DisplayName { get; set; }
        public List<ItemAction>? ItemActions { get; set; }
        public string? ItemName { get; set; }
        public List<UseWith>? UseWiths { get; set; }
        public bool UseDefaultRadialOptions { get; set; }
    }

    public sealed class ItemGroupBehavior
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? GroupName { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<ItemAction>? ItemActions { get; set; }
        public List<object>? UseWiths { get; set; }
    }

    public sealed class Achievement
    {
        public string? Description { get; set; }
        public string? Id { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public bool ShowInEditor { get; set; }
        public string? SteamName { get; set; }
    }

    public sealed class CriteriaList1
    {
        public List<Criterion>? CriteriaList { get; set; }
    }

    public sealed class CriteriaGroup
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? LinkedGroupName { get; set; }
        public bool DisplayInEditor { get; set; }
        public PassCondition PassCondition { get; set; }
        public List<CriteriaList1>? CriteriaList { get; set; }
    }

    public sealed class ItemGroup
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<string>? ItemsInGroup { get; set; }
    }

    public sealed class GameEvent
    {
        public int SortOrder2 { get; set; }
        public string? Version { get; set; }
        public string? Id { get; set; }
        public bool Enabled { get; set; }
        public GameEvents EventType { get; set; }
        public GameEvents GroupSubEventType { get; set; }
        public string? Character { get; set; }
        public string? Character2 { get; set; }
        public string? Key { get; set; }
        public int Option { get; set; }
        public int Option2 { get; set; }
        public int Option3 { get; set; }
        public string? Value { get; set; }
        public string? Value2 { get; set; }
        public int SortOrder { get; set; }
        public double Delay { get; set; }
        public double OriginalDelay { get; set; }
        public double StartDelayTime { get; set; }
        public bool UseConditions { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<Criterion>? Criteria { get; set; }
    }

    public sealed class EventTrigger
    {
        public string? Id { get; set; }
        public string? CharacterToReactTo { get; set; }
        public List<Criterion>? Critera { get; set; }
        public double CurrentIteration { get; set; }
        public bool Enabled { get; set; }
        public List<GameEvent>? Events { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }
        public bool ShowInInspector { get; set; }
        public EventTypes? Type { get; set; }
        public double UpdateIteration { get; set; }
        public string? Value { get; set; }
        public string? LocationTargetOption { get; set; }
    }

    public sealed class CharacterGroup
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<string>? CharactersInGroup { get; set; }
    }

    public sealed class MainStory
    {
        public bool AllowPlayerFemale { get; set; }
        public bool AllowPlayerMale { get; set; }
        public bool UseEekDefaultItemEnableBehaviour { get; set; }
        public List<Achievement>? Achievements { get; set; }
        public List<CharacterGroup>? CharacterGroups { get; set; }
        public List<CriteriaGroup>? CriteriaGroups { get; set; }
        public List<EventTrigger>? PlayerReactions { get; set; }
        public List<GameEvent>? GameStartEvents { get; set; }
        public List<ItemGroup>? ItemGroups { get; set; }
        public List<ItemGroupBehavior>? ItemGroupBehaviors { get; set; }
        public List<ItemOverride>? ItemOverrides { get; set; }
        public List<string>? PlayerValues { get; set; }
        public string? HousePartyVersion { get; set; }
    }

    public sealed class AlternateText
    {
        public List<Criterion>? Critera { get; set; }
        public int Order { get; set; }
        public bool Show { get; set; }
        public string? Text { get; set; }
    }

    public sealed class Response
    {
        public bool Selected { get; set; }
        public string? Id { get; set; }
        public bool AlwaysDisplay { get; set; }
        public int Next { get; set; }
        public int Order { get; set; }
        public List<Criterion>? ResponseCriteria { get; set; }
        public List<GameEvent>? ResponseEvents { get; set; }
        public bool TestingCriteraOverride { get; set; }
        public string? Text { get; set; }
        public ResponseReactionTypes? CurrentNPCReaction { get; set; }
        public bool Show { get; set; }
        public bool ShowResponseCriteria { get; set; }
        public bool ShowResponseEvents { get; set; }
        public bool IsDynamic { get; set; }
        public bool ShowDynamicNegativeCriteria { get; set; }
        public bool ShowDynamicPositiveCriteria { get; set; }
        public bool ShowTopics { get; set; }
        public bool ShowTones { get; set; }
        public bool ShowTypes { get; set; }
        public List<ConversationalTopics>? Topics { get; set; }
        public List<ResponseTones>? Tones { get; set; }
        public List<Criterion>? DynamicPositiveCriteria { get; set; }
        public List<Criterion>? DynamicNegativeCriteria { get; set; }
    }

    public sealed class Dialogue
    {
        public bool Shown { get; set; }
        public List<AlternateText>? AlternateTexts { get; set; }
        public List<GameEvent>? CloseEvents { get; set; }
        public int ID { get; set; }
        public bool Important { get; set; }
        public List<Response>? Responses { get; set; }
        public bool ShowAcceptedItems { get; set; }
        public bool ShowAlternateTexts { get; set; }
        public bool ShowDynamicCriteria { get; set; }
        public bool ShowCloseValueAdjustments { get; set; }
        public bool ShowCritera { get; set; }
        public bool ShowGlobalGoodByeResponses { get; set; }
        public bool IsDynamic { get; set; }
        public List<ConversationalTopics>? TopicMatches { get; set; }
        public List<ResponseTones>? MandatoryTones { get; set; }
        public List<ResponseTones>? ToneMatches { get; set; }
        public ResponseTypes TypeMatch { get; set; }
        public bool ShowDynamicDialogueResponses { get; set; }
        public bool OnlyOnPositiveInteraction { get; set; }
        public bool OnlyOnNegativeInteraction { get; set; }
        public int MatchScore { get; set; }
        public bool PlayVoice { get; set; }
        public int VoiceID { get; set; }
        public bool ShowGlobalResponses { get; set; }
        public bool DoesNotCountAsMet { get; set; }
        public bool ShowResponses { get; set; }
        public bool ShowStartValueAdjustments { get; set; }
        public bool ShowTopics { get; set; }
        public bool ShowTones { get; set; }
        public string? SpeakingToCharacterName { get; set; }
        public List<GameEvent>? StartEvents { get; set; }
        public List<Criterion>? DynamicDialogueCriteria { get; set; }
        public string? Text { get; set; }
    }

    public sealed class BackgroundChatter
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public List<Criterion>? Critera { get; set; }
        public bool IsConversationStarter { get; set; }
        public bool ShowInInspector { get; set; }
        public bool PlaySilently { get; set; }
        public string? Label { get; set; }
        public string? SpeakingTo { get; set; }
        public bool OverrideCombatRestriction { get; set; }
        public List<GameEvent>? StartEvents { get; set; }
        public List<BackgroundChatterResponse>? Responses { get; set; }
        public string? PairedEmote { get; set; }
        public Importance DefaultImportance { get; set; }
        public Importance CurrentImportance { get; set; }
    }

    public sealed class BackgroundChatterResponse
    {
        public string? CharacterName { get; set; }
        public string? Label { get; set; }
        public int ChatterId { get; set; }
        public bool ShowInInspector { get; set; }
    }

    public sealed class Trait
    {
        public PersonalityTraits? Type { get; set; }
        public int Value { get; set; }
    }

    public sealed class Personality
    {
        public List<Trait>? Values { get; set; }
    }

    public sealed class ExtendedDetail
    {
        public int Value { get; set; }
        public string? Details { get; set; }
    }

    public sealed class Quest
    {
        public string? CharacterName { get; set; }
        public int CompleteAt { get; set; }
        public int CurrentValue { get; set; }
        public string? Details { get; set; }
        public string? CompletedDetails { get; set; }
        public string? FailedDetails { get; set; }
        public List<ExtendedDetail>? ExtendedDetails { get; set; }
        public string? ID { get; set; }
        public string? Name { get; set; }
        public bool ObtainOnStart { get; set; }
        public bool SeenByPlayer { get; set; }
        public bool ShowProgress { get; set; }
        public QuestStatus Status { get; set; }
        public int ObtainedDateTime { get; set; }
        public int LastUpdatedDateTime { get; set; }
        public bool ShowInInspector { get; set; }
    }

    public sealed class StoryItem
    {
        public List<Criterion>? Critera { get; set; }
        public string? ItemName { get; set; }
        public List<GameEvent>? OnAcceptEvents { get; set; }
        public List<GameEvent>? OnRefuseEvents { get; set; }
        public bool DisplayInEditor { get; set; }
    }

    public sealed class ItemGroupInteraction
    {
        public List<Criterion>? Critera { get; set; }
        public string? Name { get; set; }
        public string? GroupName { get; set; }
        public string? Id { get; set; }
        public List<GameEvent>? OnAcceptEvents { get; set; }
        public List<GameEvent>? OnRefuseEvents { get; set; }
        public bool DisplayInEditor { get; set; }
    }

    public sealed class CharacterStory
    {
        public bool LockCharacterSelection { get; set; }
        public bool ShowDislikedTopics { get; set; }
        public bool ShowDislikedTones { get; set; }
        public bool ShowLikedTopics { get; set; }
        public bool ShowLikedTones { get; set; }
        public int DialogueID { get; set; }
        public int DynamicDialogueID { get; set; }
        public List<BackgroundChatter>? BackgroundChatter { get; set; }
        public List<Dialogue>? Dialogues { get; set; }
        public List<Dialogue>? DynamicDialogues { get; set; }
        public List<EventTrigger>? Reactions { get; set; }
        public List<ItemGroupInteraction>? CharacterItemGroupInteractions { get; set; }
        public List<Quest>? Quests { get; set; }
        public List<Response>? GlobalGoodbyeResponses { get; set; }
        public List<Response>? GlobalResponses { get; set; }
        public List<StoryItem>? StoryItems { get; set; }
        public List<string>? StoryValues { get; set; }
        public Personality? Personality { get; set; }
        public string? CharacterName { get; set; }
        public Dialogue? CurrentDialogue { get; set; }
        public Dialogue? CurrentDynamicDialogue { get; set; }
        public List<ResponseTones>? LikedTones { get; set; }
        public List<ConversationalTopics>? LikedTopics { get; set; }
        public List<ResponseTones>? DislikedTones { get; set; }
        public List<ConversationalTopics>? DislikedTopics { get; set; }
        public StoryAspects CurrentAspect { get; set; }
        public string? HousePartyVersion { get; set; }
    }

    public sealed class Value
    {
#pragma warning disable IDE0079
#pragma warning disable IDE1006
        public string? value { get; set; }
#pragma warning restore IDE1006
#pragma warning restore IDE0079
    }
}
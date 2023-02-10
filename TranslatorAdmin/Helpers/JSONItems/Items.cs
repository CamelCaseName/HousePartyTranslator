using static Translator.Explorer.JSON.StoryEnums;

namespace Translator.Explorer.JSON
{
#pragma warning disable 1591
    internal sealed class Criterion
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

    internal sealed class ItemAction
    {
        public string? ActionName { get; set; }
        public List<Criterion>? Criteria { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<GameEvent>? OnTakeActionEvents { get; set; }
    }

    internal sealed class UseWith
    {
        public List<Criterion>? Criteria { get; set; }
        public string? CustomCantDoThatMessage { get; set; }
        public bool DisplayInEditor { get; set; }
        public string? ItemName { get; set; }
        public List<GameEvent>? OnSuccessEvents { get; set; }
    }

    internal sealed class ItemOverride
    {
        public string? Id { get; set; }
        public bool DisplayInEditor { get; set; }
        public string? DisplayName { get; set; }
        public List<ItemAction>? ItemActions { get; set; }
        public string? ItemName { get; set; }
        public List<UseWith>? UseWiths { get; set; }
        public bool UseDefaultRadialOptions { get; set; }
    }

    internal sealed class ItemGroupBehavior
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? GroupName { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<ItemAction>? ItemActions { get; set; }
        public List<object>? UseWiths { get; set; }
    }

    internal sealed class Achievement
    {
        public string? Description { get; set; }
        public string? Id { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public bool ShowInEditor { get; set; }
        public string? SteamName { get; set; }
    }

    internal sealed class CriteriaList1
    {
        public List<Criterion>? CriteriaList { get; set; }
    }

    internal sealed class CriteriaGroup
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool DisplayInEditor { get; set; }
        public string? PassCondition { get; set; }
        public List<CriteriaList1>? CriteriaList { get; set; }
    }

    internal sealed class ItemGroup
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool DisplayInEditor { get; set; }
        public List<string>? ItemsInGroup { get; set; }
    }

    internal sealed class GameEvent
    {
        public int SortOrder2 { get; set; }
        public string? Version { get; set; }
        public string? Id { get; set; }
        public bool Enabled { get; set; }
        public GameEvents EventType { get; set; }
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

    internal sealed class EventTrigger
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

    internal sealed class MainStory
    {
        public string? HousePartyVersion { get; set; }
        public List<ItemOverride>? ItemOverrides { get; set; }
        public List<ItemGroupBehavior>? ItemGroupBehaviors { get; set; }
        public List<Achievement>? Achievements { get; set; }
        public List<string>? PlayerValues { get; set; }
        public List<CriteriaGroup>? CriteriaGroups { get; set; }
        public List<ItemGroup>? ItemGroups { get; set; }
        public List<GameEvent>? GameStartEvents { get; set; }
        public List<EventTrigger>? PlayerReactions { get; set; }
    }

    internal sealed class AlternateText
    {
        public List<Criterion>? Critera { get; set; }
        public int Order { get; set; }
        public bool Show { get; set; }
        public string? Text { get; set; }
    }

    internal sealed class Response
    {
        public bool Selected { get; set; }
        public string? Id { get; set; }
        public bool AlwaysDisplay { get; set; }
        public int Next { get; set; }
        public int Order { get; set; }
        public List<Criterion>? ResponseCriteria { get; set; }
        public List<GameEvent>? ResponseEvents { get; set; }
        public bool Show { get; set; }
        public bool ShowResponseCriteria { get; set; }
        public bool ShowResponseEvents { get; set; }
        public bool TestingCriteraOverride { get; set; }
        public string? Text { get; set; }
        public string? CharacterName { get; set; }
        public int ChatterId { get; set; }
        public bool ShowInInspector { get; set; }
    }

    internal sealed class Dialogue
    {
        public bool Shown { get; set; }
        public List<AlternateText>? AlternateTexts { get; set; }
        public List<GameEvent>? CloseEvents { get; set; }
        public int ID { get; set; }
        public bool Important { get; set; }
        public List<Response>? Responses { get; set; }
        public bool ShowAcceptedItems { get; set; }
        public bool ShowAlternateTexts { get; set; }
        public bool ShowCloseValueAdjustments { get; set; }
        public bool ShowCritera { get; set; }
        public bool ShowGlobalGoodByeResponses { get; set; }
        public bool PlayVoice { get; set; }
        public int VoiceID { get; set; }
        public bool ShowGlobalResponses { get; set; }
        public bool DoesNotCountAsMet { get; set; }
        public bool ShowResponses { get; set; }
        public bool ShowStartValueAdjustments { get; set; }
        public string? SpeakingToCharacterName { get; set; }
        public List<GameEvent>? StartEvents { get; set; }
        public string? Text { get; set; }
    }

    internal sealed class GlobalGoodbyeResponse
    {
        public bool Selected { get; set; }
        public string? Id { get; set; }
        public bool AlwaysDisplay { get; set; }
        public int Next { get; set; }
        public int Order { get; set; }
        public List<Criterion>? ResponseCriteria { get; set; }
        public List<GameEvent>? ResponseEvents { get; set; }
        public bool Show { get; set; }
        public bool ShowResponseCriteria { get; set; }
        public bool ShowResponseEvents { get; set; }
        public bool TestingCriteraOverride { get; set; }
        public string? Text { get; set; }
    }

    internal sealed class GlobalResponse
    {
        public bool Selected { get; set; }
        public string? Id { get; set; }
        public bool AlwaysDisplay { get; set; }
        public int Next { get; set; }
        public int Order { get; set; }
        public List<Criterion>? ResponseCriteria { get; set; }
        public List<GameEvent>? ResponseEvents { get; set; }
        public bool Show { get; set; }
        public bool ShowResponseCriteria { get; set; }
        public bool ShowResponseEvents { get; set; }
        public bool TestingCriteraOverride { get; set; }
        public string? Text { get; set; }
    }

    internal sealed class BackgroundChatter
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public List<Criterion>? Critera { get; set; }
        public bool IsConversationStarter { get; set; }
        public bool ShowInInspector { get; set; }
        public string? SpeakingTo { get; set; }
        public bool OverrideCombatRestriction { get; set; }
        public List<GameEvent>? StartEvents { get; set; }
        public List<Response>? Responses { get; set; }
        public string? PairedEmote { get; set; }
        public string? DefaultImportance { get; set; }
    }

    internal sealed class Valuee
    {
        public string? Type { get; set; }
        public int Value { get; set; }
    }

    internal sealed class Personality
    {
        public List<Valuee>? Values { get; set; }
    }

    internal sealed class ExtendedDetail
    {
        public int Value { get; set; }
        public string? Details { get; set; }
    }

    internal sealed class Quest
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
        public string? Status { get; set; }
        public int ObtainedDateTime { get; set; }
        public int LastUpdatedDateTime { get; set; }
        public bool ShowInInspector { get; set; }
    }

    internal sealed class StoryItem
    {
        public List<Criterion>? Critera { get; set; }
        public string? ItemName { get; set; }
        public List<GameEvent>? OnAcceptEvents { get; set; }
        public List<GameEvent>? OnRefuseEvents { get; set; }
        public bool DisplayInEditor { get; set; }
    }

    internal sealed class CharacterStory
    {
        public string? CharacterName { get; set; }
        public string? CurrentAspect { get; set; }
        public int DialogueID { get; set; }
        public List<Dialogue>? Dialogues { get; set; }
        public List<GlobalGoodbyeResponse>? GlobalGoodbyeResponses { get; set; }
        public List<GlobalResponse>? GlobalResponses { get; set; }
        public string? HousePartyVersion { get; set; }
        public List<BackgroundChatter>? BackgroundChatter { get; set; }
        public bool LockCharacterSelection { get; set; }
        public Personality? Personality { get; set; }
        public List<Quest>? Quests { get; set; }
        public List<EventTrigger>? Reactions { get; set; }
        public List<StoryItem>? StoryItems { get; set; }
        public List<object>? CharacterItemGroupInteractions { get; set; }
        public List<string>? StoryValues { get; set; }
    }

#pragma warning restore
}
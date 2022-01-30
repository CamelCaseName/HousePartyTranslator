using System;
using System.Collections.Generic;

public class Criterion
{
    public Criterion(
        string boolValue,
        string character,
        string character2,
        string compareType,
        string dialogueStatus,
        bool displayInEditor,
        string doorOptions,
        string equalsValue,
        string equationValue,
        string valueFormula,
        object itemComparison,
        object itemFromItemGroupComparison,
        string key,
        string key2,
        int order,
        string playerInventoryOption,
        string poseOption,
        string socialStatus,
        string value,
        int option)
    {
        BoolValue = boolValue;
        Character = character;
        Character2 = character2;
        CompareType = compareType;
        DialogueStatus = dialogueStatus;
        DisplayInEditor = displayInEditor;
        DoorOptions = doorOptions;
        EqualsValue = equalsValue;
        EquationValue = equationValue;
        ValueFormula = valueFormula;
        ItemComparison = itemComparison;
        ItemFromItemGroupComparison = itemFromItemGroupComparison;
        Key = key;
        Key2 = key2;
        Order = order;
        PlayerInventoryOption = playerInventoryOption;
        PoseOption = poseOption;
        SocialStatus = socialStatus;
        Value = value;
        Option = option;
    }

    public string BoolValue { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string CompareType { get; set; }
    public string DialogueStatus { get; set; }
    public bool DisplayInEditor { get; set; }
    public string DoorOptions { get; set; }
    public string EqualsValue { get; set; }
    public string EquationValue { get; set; }
    public string ValueFormula { get; set; }
    public object ItemComparison { get; set; }
    public object ItemFromItemGroupComparison { get; set; }
    public string Key { get; set; }
    public string Key2 { get; set; }
    public int Order { get; set; }
    public string PlayerInventoryOption { get; set; }
    public string PoseOption { get; set; }
    public string SocialStatus { get; set; }
    public string Value { get; set; }
    public int Option { get; set; }

    public static explicit operator Criterion(Critera v)
    {
        return new Criterion(
            v.BoolValue,
            v.Character,
            v.Character2,
            v.CompareType,
            v.DialogueStatus,
            v.DisplayInEditor,
            v.DoorOptions,
            v.EqualsValue,
            v.EquationValue,
            v.ValueFormula,
            v.ItemComparison,
            v.ItemFromItemGroupComparison,
            v.Key,
            v.Key2,
            v.Order,
            v.PlayerInventoryOption,
            v.PoseOption,
            v.SocialStatus,
            v.Value,
            v.Option);
    }

    public static explicit operator Criterion(ResponseCriteria v)
    {
        return new Criterion(
            v.BoolValue,
            v.Character,
            v.Character2,
            v.CompareType,
            v.DialogueStatus,
            v.DisplayInEditor,
            v.DoorOptions,
            v.EqualsValue,
            v.EquationValue,
            v.ValueFormula,
            v.ItemComparison,
            v.ItemFromItemGroupComparison,
            v.Key,
            v.Key2,
            v.Order,
            v.PlayerInventoryOption,
            v.PoseOption,
            v.SocialStatus,
            v.Value,
            v.Option);
    }
}

public class OnTakeActionEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class ItemAction
{
    public string ActionName { get; set; }
    public List<Criterion> Criteria { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<OnTakeActionEvent> OnTakeActionEvents { get; set; }
}

public class OnSuccessEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class UseWith
{
    public List<Criterion> Criteria { get; set; }
    public string CustomCantDoThatMessage { get; set; }
    public bool DisplayInEditor { get; set; }
    public string ItemName { get; set; }
    public List<OnSuccessEvent> OnSuccessEvents { get; set; }
}

public class ItemOverride
{
    public string Id { get; set; }
    public bool DisplayInEditor { get; set; }
    public string DisplayName { get; set; }
    public List<ItemAction> ItemActions { get; set; }
    public string ItemName { get; set; }
    public List<UseWith> UseWiths { get; set; }
    public bool UseDefaultRadialOptions { get; set; }
}

public class ItemGroupBehavior
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string GroupName { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<ItemAction> ItemActions { get; set; }
    public List<object> UseWiths { get; set; }
}

public class Achievement
{
    public string Description { get; set; }
    public string Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public bool ShowInEditor { get; set; }
    public string SteamName { get; set; }
}

public class CriteriaList2
{
    public string BoolValue { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string CompareType { get; set; }
    public string DialogueStatus { get; set; }
    public bool DisplayInEditor { get; set; }
    public string DoorOptions { get; set; }
    public string EqualsValue { get; set; }
    public string EquationValue { get; set; }
    public string ValueFormula { get; set; }
    public string ItemComparison { get; set; }
    public object ItemFromItemGroupComparison { get; set; }
    public string Key { get; set; }
    public string Key2 { get; set; }
    public int Order { get; set; }
    public string PlayerInventoryOption { get; set; }
    public string PoseOption { get; set; }
    public string SocialStatus { get; set; }
    public string Value { get; set; }
    public int Option { get; set; }
}

public class CriteriaList1
{
    public List<CriteriaList2> CriteriaList { get; set; }
}

public class CriteriaGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool DisplayInEditor { get; set; }
    public string PassCondition { get; set; }
    public List<CriteriaList1> CriteriaList { get; set; }
}

public class ItemGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<string> ItemsInGroup { get; set; }
}

public class GameStartEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class Critera
{
    public string BoolValue { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string CompareType { get; set; }
    public string DialogueStatus { get; set; }
    public bool DisplayInEditor { get; set; }
    public string DoorOptions { get; set; }
    public string EqualsValue { get; set; }
    public string EquationValue { get; set; }
    public string ValueFormula { get; set; }
    public object ItemComparison { get; set; }
    public object ItemFromItemGroupComparison { get; set; }
    public string Key { get; set; }
    public string Key2 { get; set; }
    public int Order { get; set; }
    public string PlayerInventoryOption { get; set; }
    public string PoseOption { get; set; }
    public string SocialStatus { get; set; }
    public string Value { get; set; }
    public int Option { get; set; }
}

public class Event
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class PlayerReaction
{
    public string Id { get; set; }
    public string CharacterToReactTo { get; set; }
    public List<Critera> Critera { get; set; }
    public double CurrentIteration { get; set; }
    public bool Enabled { get; set; }
    public List<Event> Events { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public bool ShowInInspector { get; set; }
    public string Type { get; set; }
    public double UpdateIteration { get; set; }
    public string Value { get; set; }
    public string LocationTargetOption { get; set; }
}

public class MainStory
{
    public string HousePartyVersion { get; set; }
    public List<ItemOverride> ItemOverrides { get; set; }
    public List<ItemGroupBehavior> ItemGroupBehaviors { get; set; }
    public List<Achievement> Achievements { get; set; }
    public List<string> PlayerValues { get; set; }
    public List<CriteriaGroup> CriteriaGroups { get; set; }
    public List<ItemGroup> ItemGroups { get; set; }
    public List<GameStartEvent> GameStartEvents { get; set; }
    public List<PlayerReaction> PlayerReactions { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class AlternateText
{
    public List<Critera> Critera { get; set; }
    public int Order { get; set; }
    public bool Show { get; set; }
    public string Text { get; set; }
}

public class CloseEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class ResponseCriteria
{
    public string BoolValue { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string CompareType { get; set; }
    public string DialogueStatus { get; set; }
    public bool DisplayInEditor { get; set; }
    public string DoorOptions { get; set; }
    public string EqualsValue { get; set; }
    public string EquationValue { get; set; }
    public string ValueFormula { get; set; }
    public object ItemComparison { get; set; }
    public object ItemFromItemGroupComparison { get; set; }
    public string Key { get; set; }
    public string Key2 { get; set; }
    public int Order { get; set; }
    public string PlayerInventoryOption { get; set; }
    public string PoseOption { get; set; }
    public string SocialStatus { get; set; }
    public string Value { get; set; }
    public int Option { get; set; }
}

public class ResponseEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class Response
{
    public bool Selected { get; set; }
    public string Id { get; set; }
    public bool AlwaysDisplay { get; set; }
    public int Next { get; set; }
    public int Order { get; set; }
    public List<ResponseCriteria> ResponseCriteria { get; set; }
    public List<ResponseEvent> ResponseEvents { get; set; }
    public bool Show { get; set; }
    public bool ShowResponseCriteria { get; set; }
    public bool ShowResponseEvents { get; set; }
    public bool TestingCriteraOverride { get; set; }
    public string Text { get; set; }
    public string CharacterName { get; set; }
    public int ChatterId { get; set; }
    public bool ShowInInspector { get; set; }
}

public class StartEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<Criterion> Criteria { get; set; }
}

public class Dialogue
{
    public bool Shown { get; set; }
    public List<AlternateText> AlternateTexts { get; set; }
    public List<CloseEvent> CloseEvents { get; set; }
    public int ID { get; set; }
    public bool Important { get; set; }
    public List<Response> Responses { get; set; }
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
    public string SpeakingToCharacterName { get; set; }
    public List<StartEvent> StartEvents { get; set; }
    public string Text { get; set; }
}

public class GlobalGoodbyeResponse
{
    public bool Selected { get; set; }
    public string Id { get; set; }
    public bool AlwaysDisplay { get; set; }
    public int Next { get; set; }
    public int Order { get; set; }
    public List<object> ResponseCriteria { get; set; }
    public List<object> ResponseEvents { get; set; }
    public bool Show { get; set; }
    public bool ShowResponseCriteria { get; set; }
    public bool ShowResponseEvents { get; set; }
    public bool TestingCriteraOverride { get; set; }
    public string Text { get; set; }
}

public class GlobalResponse
{
    public bool Selected { get; set; }
    public string Id { get; set; }
    public bool AlwaysDisplay { get; set; }
    public int Next { get; set; }
    public int Order { get; set; }
    public List<ResponseCriteria> ResponseCriteria { get; set; }
    public List<ResponseEvent> ResponseEvents { get; set; }
    public bool Show { get; set; }
    public bool ShowResponseCriteria { get; set; }
    public bool ShowResponseEvents { get; set; }
    public bool TestingCriteraOverride { get; set; }
    public string Text { get; set; }
}

public class BackgroundChatter
{
    public int Id { get; set; }
    public string Text { get; set; }
    public List<Critera> Critera { get; set; }
    public bool IsConversationStarter { get; set; }
    public bool ShowInInspector { get; set; }
    public string SpeakingTo { get; set; }
    public bool OverrideCombatRestriction { get; set; }
    public List<StartEvent> StartEvents { get; set; }
    public List<Response> Responses { get; set; }
    public string PairedEmote { get; set; }
    public string DefaultImportance { get; set; }
}

public class Valuee
{
    public string Type { get; set; }
    public int Value { get; set; }
}

public class Personality
{
    public List<Valuee> Values { get; set; }
}

public class ExtendedDetail
{
    public int Value { get; set; }
    public string Details { get; set; }
}

public class Quest
{
    public string CharacterName { get; set; }
    public int CompleteAt { get; set; }
    public int CurrentValue { get; set; }
    public string Details { get; set; }
    public string CompletedDetails { get; set; }
    public string FailedDetails { get; set; }
    public List<ExtendedDetail> ExtendedDetails { get; set; }
    public string ID { get; set; }
    public string Name { get; set; }
    public bool ObtainOnStart { get; set; }
    public bool SeenByPlayer { get; set; }
    public bool ShowProgress { get; set; }
    public string Status { get; set; }
    public int ObtainedDateTime { get; set; }
    public int LastUpdatedDateTime { get; set; }
    public bool ShowInInspector { get; set; }
}

public class Reaction
{
    public string Id { get; set; }
    public string CharacterToReactTo { get; set; }
    public List<Critera> Critera { get; set; }
    public double CurrentIteration { get; set; }
    public bool Enabled { get; set; }
    public List<Event> Events { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public bool ShowInInspector { get; set; }
    public string Type { get; set; }
    public double UpdateIteration { get; set; }
    public string Value { get; set; }
    public string LocationTargetOption { get; set; }
}

public class OnAcceptEvent
{
    public int SortOrder2 { get; set; }
    public string Version { get; set; }
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int EventType { get; set; }
    public string Character { get; set; }
    public string Character2 { get; set; }
    public string Key { get; set; }
    public int Option { get; set; }
    public int Option2 { get; set; }
    public int Option3 { get; set; }
    public string Value { get; set; }
    public string Value2 { get; set; }
    public int SortOrder { get; set; }
    public double Delay { get; set; }
    public double OriginalDelay { get; set; }
    public double StartDelayTime { get; set; }
    public bool UseConditions { get; set; }
    public bool DisplayInEditor { get; set; }
    public List<object> Criteria { get; set; }
}

public class StoryItem
{
    public List<Critera> Critera { get; set; }
    public string ItemName { get; set; }
    public List<OnAcceptEvent> OnAcceptEvents { get; set; }
    public List<object> OnRefuseEvents { get; set; }
    public bool DisplayInEditor { get; set; }
}

public class CharacterStory
{
    public string CharacterName { get; set; }
    public string CurrentAspect { get; set; }
    public int DialogueID { get; set; }
    public List<Dialogue> Dialogues { get; set; }
    public List<GlobalGoodbyeResponse> GlobalGoodbyeResponses { get; set; }
    public List<GlobalResponse> GlobalResponses { get; set; }
    public string HousePartyVersion { get; set; }
    public List<BackgroundChatter> BackgroundChatter { get; set; }
    public bool LockCharacterSelection { get; set; }
    public Personality Personality { get; set; }
    public List<Quest> Quests { get; set; }
    public List<Reaction> Reactions { get; set; }
    public List<StoryItem> StoryItems { get; set; }
    public List<object> CharacterItemGroupInteractions { get; set; }
    public List<string> StoryValues { get; set; }
}

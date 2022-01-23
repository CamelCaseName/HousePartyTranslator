using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousePartyTranslator.Helpers
{
    class StoryItem
    {
        string HousePartyVersion;
        ItemOverrride[] ItemOverrrides;
        ItemGroupBehavior[] ItemGroupBehaviors;
        Achievement[] Achievements;
        PlayerValue[] PlayerValues;
        CriteraGroup[] CriteraGroups;
        ItemGroup[] ItemGroups;
        Event[] GameStartEvents;
        Reaction[] PlayerRecations;
    }

    class ItemOverrride
    {
        string Id;
        bool DisplayInEditor;
        string DisplayName;
        ItemAction[] ItemActions;
        string ItemName;
        UseWith[] UseWiths;
        bool useDefaultRadialOptions;
    }

    class ItemGroupBehavior
    {
        string Id;
        string Name;
        string GroupName;
        bool DisplayInEditor;
        ItemAction[] ItemActions;
        UseWith[] UseWiths;
    }

    class Achievement
    {
        string Description;
        string Id;
        string Image;
        string Name;
        bool ShowInEditor;
        string SteamName;
    }

    class PlayerValue
    {
        string Value;
    }

    class CriteraGroup
    {
        string Id;
        string Name;
        bool DisplayInEditor;
        string PassCondition;
        CriteriaListItem[] CriteriaList;
    }

    class ItemGroup
    {
        string Id;
        string Name;
        bool DisplayInEditor;
        string[] ItemsInGroup;
    }

    class Reaction
    {
        string Id;
        string CharacterToReactTo;
        Criteria[] Criteria;
        decimal CurrentIteration;
        bool Enabled;
        Event[] Events;
        string Key;
        string Name;
        bool ShowInInspector;
        string Type;
        decimal UpdateIteration;
        string Value;
        string LocationTargetOption;
    }

    class ItemAction
    {
        string ActionName;
        Criteria[] Criteria;
        bool DisplayInEditor;
        Event[] OnTakeActionEvents;
    }

    class Criteria
    {
        bool BoolValue;
        string Character;
        string Character2;
        string CompareType;
        string DialogueStatus;
        bool DisplayInEditor;
        string DoorOptions;
        string EqualsValue;
        string EquationValue;
        string ValueFormula;
        object ItemComparison;
        object ItemFromItemGroupComparison;
        string Key;
        string Key2;
        int Order;
        string PlayerInventoryOption;
        string PoseOption;
        string SocialStatus;
        string Value;
        int Option;
    }

    class UseWith
    {
        Criteria[] Criteria;
        string CustomCantDoThatMessage;
        bool DisplayInEditor;
        string ItemName;
        Event[] OnSuccessEvents;
    }

    class Event
    {
        int SortOrder2;
        string Version;
        string Id;
        bool Enabled;
        int EventType;
        string Character;
        string Character2;
        string Key;
        int Option;
        int Option2;
        int Option3;
        string Value;
        string Value2;
        int SortOrder;
        decimal Delay;
        decimal OriginalDelay;
        decimal StartDelayTime;
        bool UseConditions;
        bool DisplayInEditor;
        Criteria[] Criteria;
    }

    class CriteriaListItem
    {
        Criteria[] CriteriaList;
    }

    class CharacterItem
    {
        string CharacterName;
        string CurrentAspect;
        int DialogueID;
        Dialogue[] Dialogues;
        Response[] GlobalGoodByeResponses;
        Response[] GlobalResponses;
        string HousePartyVersion;
        BackGroundChatter[] BackGroundChatter;
        bool LockCharacterSelection;
        Personality Personality;
        Quest[] Quests;
        Reaction[] Reactions;
        Item[] StoryItems;
        CharacterItemGroupInteraction[] CharacterItemGroupInteractions;
        string[] StoryValues;
    }

    class Item
    {
        Criteria[] Criteria;
        string ItemName;
        Event[] OnAcceptEvents;
        Event[] OnRefuseEvents;
        bool DisplayInEditor;
    }

    class CharacterItemGroupInteraction
    {
        
    }

    class Dialogue
    {
        bool Shown;
        AlternateText[] AlternateTexts;
        Event[] CloseEvents;
        int ID;
        bool Important;
        Response[] Responses;
        bool ShowAccepteditems;
        bool ShowAlternateTexts;
        bool ShowCloseValueAdjustments;
        bool ShowCriteria;
        bool ShowGlobalGoodByeResponses;
        bool PlayVoice;
        int VoiceID;
        bool ShowGlobalResponses;
        bool DoesNotCountAsMet;
        bool ShowResponses;
        bool ShowStartValueAdjustments;
        string SpeakingToCharacterName;
        Event[] StartEvents;
        string Text;
    }

    class AlternateText
    {
        Criteria[] Criteria;
        int Order;
        bool Show;
        string Text;
    }

    class Personality
    {
        PersonalityValue[] Values;
    }

    class Quest
    {
        string CharacterName;
        int CompleteAt;
        int CurrentValue;
        string Details;
        string CompletedDetails;
        string FailedDetails;
        Detail[] ExtendedDetails;
        string ID;
        string Name;
        bool ObtainOnStart;
        bool SeenByPlayer;
        bool ShowProgress;
        string Status;
        int ObtainDateTime;
        int LastUpdatedDateTime;
        bool ShowInInspector;
    }

    class Detail
    {
        int Value;
        string Details;
    }

    class PersonalityValue
    {
        string Type;
        int Value;
    }

    class BackGroundChatter
    {
        int Id;
        string Text;
        Criteria[] Criteria;
        bool IsConversationStarter;
        bool ShowInInspector;
        string SpeakingTo;
        bool OverrideCombatRestriction;
        Event[] StartEvents;
        Response[] Responses;
        string PairedEmote;
        string DefaultImportance;
    }

    class Response
    {
        bool Selected;
        string Id;
        bool AlwaysDisplay;
        int Next;
        int Order;
        Criteria[] ResponseCriteria;
        Event[] ResponseEvents;
        bool Show;
        bool ShowResponseCriteria;
        bool ShowResponseEvents;
        bool TestingCriteraOverride;
        string Text;
    }
}

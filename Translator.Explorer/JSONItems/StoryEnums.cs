using Newtonsoft.Json.Converters;// Token: 0x(\w|d|\s|:)*\n
using System.Text.Json.Serialization;

namespace Translator.Explorer.JSONItems
{
    // // Token: 0x(\w|d|\s|:)*\n
    //search term to remove all token comments
    public static class StoryEnums
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CompareTypes
        {
            Never = 1000,
            CharacterFromCharacterGroup = 17,
            Clothing = 110,
            CoinFlip = 160,
            CompareValues = 1,
            CriteriaGroup = 200,
            CutScene = 175,
            Dialogue = 40,
            Distance = 150,
            Door = 140,
            Gender = 138,
            IntimacyPartner = 136,
            IntimacyState = 135,
            InZone = 910,
            InVicinity = 19,
            InVicinityAndVision = 21,
            Item = 15,
            IsBeingSpokenTo = 81,
            IsAloneWithPlayer,
            IsDLCActive = 85,
            IsOnlyInVicinityOf = 77,
            IsOnlyInVisionOf,
            IsOnlyInVicinityAndVisionOf,
            IsCharacterEnabled = 210,
            IsCurrentlyBeingUsed = 191,
            IsCurrentlyUsing = 190,
            IsExplicitGameVersion = 2000,
            IsGameUncensored,
            IsPackageInstalled,
            IsInFrontOf = 151,
            IsInHouse = 912,
            IsNewGame = 900,
            IsZoneEmpty = 913,
            ItemFromItemGroup = 16,
            MetByPlayer = 30,
            Personality = 5,
            PlayerGender = 170,
            PlayerInventory = 10,
            PlayerPrefs = 950,
            Posing = 120,
            Property = 133,
            Quest = 100,
            SameZoneAs = 911,
            ScreenFadeVisible = 180,
            Social = 50,
            State = 130,
            Value = 0,
            Vision = 20,
            UseLegacyIntimacy = 960,
            None = 10000
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DialogueStatuses
        {
            WasNotShown,
            WasShown = 10,
            IsCurrentlyShowing = 20,
            NotCurrentlyShowing = 30
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DoorOptionValues
        {
            IsOpen,
            IsClosed,
            IsLocked,
            IsUnlocked
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum EqualsValues
        {
            Equals,
            DoesNotEqual
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ComparisonEquations
        {
            Equals,
            DoesNotEqual = 10,
            GreaterThan = 20,
            LessThan = 30
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ItemComparisonTypes
        {
            IsActive = 10,
            IsMounted = 20,
            IsMountedTo = 22,
            IsHeldByPlayer = 25,
            IsInventoriedOrHeldByPlayer,
            IsVisibleTo = 30
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ItemFromItemGroupComparisonTypes
        {
            IsActive = 10,
            IsMounted = 20,
            IsMountedTo = 22,
            IsHeldByPlayer = 25,
            IsInventoriedOrHeldByPlayer,
            IsVisibleTo = 30,
            IsInPlayerInventory = 35
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerInventoryOptions
        {
            HasItem,
            HasAtLeastOneItem
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PoseOptions
        {
            IsCurrentlyPosing,
            CurrentPose = 10
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Gender
        {
            None = 2,
            Female = 1,
            Male = 0
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SocialStatuses
        {
            Mood,
            Drunk = 10,
            Likes = 20,
            Loves = 30,
            Scared = 40
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BoolCritera
        {
            False,
            True
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ValueSpecificFormulas
        {
            EqualsValue,
            DoesNotEqualValue,
            GreaterThanValue,
            LessThanValue
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum EventTypes
        {
            Never,
            GameStarts,
            EntersVision = 10,
            ExitsVision,
            EntersVicinity,
            EntersZone = 14,
            ReachesTarget,
            IsBlockedByLockedDoor,
            IsAttacked = 20,
            GetsKnockedOut,
            Dies,
            GetsHitWithProjectile,
            FallsOver = 30,
            IsNaked = 40,
            IsBottomless,
            IsTopless,
            ExposesGenitals = 50,
            CaughtMasturbating,
            CaughtHavingSex,
            ExposesChest,
            StartedIntimacyAct = 55,
            Orgasms = 60,
            EjaculatesOnMe = 70,
            GropesMyBreast,
            GropesMyAss,
            PlayerGrabsItem,
            PlayerReleasesItem,
            VapesOnMe,
            PopperedMe,
            PhoneBlindedMe,
            Periodically = 80,
            OnItemFunction = 90,
            OnAnyItemAcceptFallback,
            OnAnyItemRefuseFallback,
            CombatModeToggled = 100,
            PokedByVibrator = 110,
            ImpactsGround = 115,
            ImpactsWall,
            ScoredBeerPongPoint = 120,
            PeesOnMe = 130,
            PeesOnItem,
            StartedPeeing,
            StoppedPeeing,
            PlayerThrowsItem = 140,
            StartedUsingActionItem = 150,
            StoppedUsingActionItem,
            OnFriendshipIncreaseWith = 160,
            OnRomanceIncreaseWith,
            OnFriendshipDecreaseWith,
            OnRomanceDecreaseWith,
            IsDancing = 170,
            StartedLapDance,
            PlayerInventoryOpened = 180,
            PlayerInventoryClosed,
            PlayerOpportunityWindowOpened,
            PlayerInteractsWithCharacter = 185,
            PlayerInteractsWithItem,
            OnScreenFadeInComplete,
            OnScreenFadeOutComplete,
            FinishedPopulatingMainDialogueText = 200,
            PlayerTookCameraPhoto = 250,
            OnAfterCutSceneEnds,
            Ejaculates,
            None = 1000
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum InteractiveStates
        {
            Naked,
            WantsToBeAlone,
            Upset,
            Happy,
            ShooOthers,
            DontMoveForOthers,
            Embarrassed,
            Angry,
            IgnoreNavMeshRestrictions,
            Alive = 10,
            KnockedOut,
            GenitalsExposed,
            ChestExposed,
            ForcePlaySexSound,
            DontReleasePoseAfterSex,
            RechargingOrgasm,
            Standing,
            IgnoredByOthers,
            InCombat,
            Dancing,
            CanHearMusic,
            CurrentlyDisplayingDialogue,
            Crouching,
            Topless,
            Bottomless,
            Sitting,
            AbleToRoam,
            AbleToSocialize,
            AbleToDance,
            AbleToBeDistracted,
            Immobile,
            RunWhenCloseToTarget,
            Running,
            IndefinitelyErect,
            IsKneeling,
            IsLayingDown,
            IsLayingDownOnBack,
            IsLayingDownOnStomache,
            ForcedToDance,
            OnThePhone,
            Idling,
            HoldingADrinkRightHand,
            HoldingADrinkLeftHand,
            UnableToFidget,
            UnableToAnimateConverstion,
            Falling,
            UnableToEmote,
            UnableToAnimateEmotes,
            ActingInCinematic,
            CurrentlyPeeing,
            IsOnFire,
            Enabled,
            AbleToMoanDuringSexCutScene,
            UnableToOpenLabiaOrAsshole,
            UnableToLookAt,
            OverridingOralMouthPose,
            AbleToSexEmoteDuringCutScene,
            RunWhenFarAwayFromTarget,
            AbleToRechargeHealthInCombat,
            Floating,
            EyesAlwaysClosedWhenIdling,
            DontMoveForObstacles
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum InteractiveProperties
        {
            PlayerCombatMode = 1,
            PlayerCantBlock,
            IsBlocking,
            HitsGirls = 50,
            DoesNotFightBack,
            AlwaysBlocks,
            NoSprintingEnergyRegenPenalty,
            DoesNotUseIKToOpenDoors = 60,
            ForceBasicIdling = 80,
            IgnoresFireDamage = 85,
            RanStartEvents = 90,
            PlayerBladderAutoRechargeDisabled = 100,
            GivingStripTease = 110,
            ReceivingStripTease,
            IsFlashingShirt = 150,
            GivingBlowJob = 170,
            GivingCunnilingus,
            GivingOral,
            InCutScene = 200
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PersonalityTraits
        {
            Nice,
            Happy,
            Humerous,
            Creative,
            Jealous,
            LikesMen,
            LikesWomen,
            Aggressive,
            Sociable,
            Optimistic,
            Energetic,
            Serious,
            Intelligent,
            Charismatic,
            Shy,
            Perverse,
            Exhibitionism
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Clothes
        {
            Top,
            Bottom,
            Underwear,
            Bra,
            Shoes,
            Accessory,
            StrapOn
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum GameEvents
        {
            AddForce = 250,
            ChangeBodyScale = 450,
            CharacterFromCharacterGroup = 192,
            CharacterFunction = 998,
            Clothing = 60,
            Combat = 140,
            CombineValue = 2,
            CutScene = 400,
            Dialogue = 120,
            DisableNPC = 301,
            DisplayGameMessage = 10,
            Door = 170,
            Emote = 50,
            EnableNPC = 300,
            EnableNPCAsync = 302,
            EventTriggers = 5,
            FadeIn = 220,
            FadeOut,
            IKReach = 230,
            Intimacy = 180,
            Item = 190,
            ItemFromItemGroup,
            LookAt = 70,
            Personality = 137,
            Property = 135,
            MatchValue = 1,
            ModifyValue = 0,
            Player = 160,
            Pose = 150,
            Quest = 80,
            RandomizeIntValue = 3,
            ResetReactionCooldown = 260,
            Roaming = 110,
            SendEvent = 999,
            SetPlayerPref = 800,
            Social = 30,
            State = 130,
            TriggerBGC = 240,
            Turn = 65,
            TurnInstantly,
            UnlockAchievement = 90,
            WalkTo = 100,
            WarpOverTime = 211,
            WarpTo = 210,
            None = 1000
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ResponseReactionTypes
        {
            VeryBad,
            Bad,
            Neutral,
            Good,
            VeryGood
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ResponseTypes
        {
            Generic,
            Declarative = 10,
            Question = 20,
            Compliment = 30,
            Insult = 40,
            Informitive = 50,
            Advice = 60,
            Intimidating = 70,
            Apology = 80
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ResponseTones
        {
            Friendly = 10,
            Mean = 20,
            Flirty = 30,
            Sarcastic = 40,
            Funny = 50,
            Intelligent = 60,
            Crass = 70,
            Insecure = 80,
            Confident = 90,
            Annoying = 100,
            Sweet = 110,
            Clever = 130,
            Gross = 150,
            Sincere = 160,
            Excited = 170,
            Bragging = 180,
            Forward = 190,
            Silly = 200,
            SmallTalk = 210,
            PersonalQuestion = 220
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ConversationalTopics
        {
            Art = 30,
            Weather = 50,
            Shopping = 90,
            Fun = 100,
            Hobbies,
            Drinking,
            Drugs,
            Dancing,
            Business = 110,
            Work,
            Entertainment = 120,
            Music,
            Movies,
            Television,
            Relationships = 130,
            Sex = 140,
            Food = 160,
            Politics = 170,
            Religeon = 180,
            Sports = 190,
            Clothes = 200
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Importance
        {
            None,
            Important,
            VeryImportant
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum QuestStatus
        {
            NotObtained,
            InProgress,
            Complete,
            Failed,
            Missed
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StoryAspects
        {
            CharacterPersonality,
            ScriptedDialogue,
            DynamicDialogue,
            ItemInteractions,
            CharacterQuests,
            EventTriggers,
            BackgroundChatter,
            Values
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PassCondition
        {
            AllSetsAreTrue,
            AnySetIsTrue,
            AllSetsAreFalse,
            AnySetIsFalse
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DoorAction
        {
            Open,
            Close,
            Lock,
            Unlock,
            OpenSlowly,
            CloseSlowly
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum DialogueAction
        {
            Trigger,
            Overhear,
            SetStartDialogue,
            TriggerStartDialogue
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CutsceneAction
        {
            PlayScene,
            PlayRandomSceneFromLocation,
            PlayRandomSceneFromCurrentLocation,
            EndScene,
            EndAnySceneWithPlayer
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ItemEventAction
        {
            SetEnabled,
            Mount,
            Rename,
            ItemFunction,
            TriggerUseWithMenu,
            WarpItemTo,
            ApplyForceTowards,
            ApplyHotSpots,
            SetInventoryIcon
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ItemGroupAction
        {
            SetEnabled,
            RemoveFromPlayerInventory,
            Mount,
            Rename,
            ItemFunction,
            TriggerUseWithMenu,
            WarpItemTo,
            ApplyForceTowards,
            AddToPlayerInventory,
            GrabFromPlayerInventory
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PersonalityAction
        {
            Equals,
            Add
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BGCOption
        {
            Unspecified,
            None,
            Important,
            VeryImportant
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SendEvents
        {
            TurnAround,
            TurnLeft,
            TurnRight,
            StepForward,
            StepBackward,
            StepLeft,
            StepRight,
            PickupLeftHand,
            PickupRightHand,
            Throw,
            ThrowPunch,
            JumpUp,
            JumpDown,
            JumpAndFall,
            Point,
            SipDrinkLeft,
            SipDrinkRight,
            SipDrinkSittingLeft,
            SipDrinkSittingRight,
            SipDrinkHotTubLeft,
            SipDrinkHotTubRight,
            StopUsingActionItem,
            Cheer,
            StartPeeing,
            StopPeeing,
            ToggleGenitals,
            StartStripTease,
            StopStripTease,
            Orgasm,
            GameOver
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum QuestActions
        {
            Start,
            Increment,
            Complete,
            Fail
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerActions
        {
            Inventory,
            TriggerGiveTo,
            Sit,
            LayDown,
            TogglePenis,
            ToggleMasturbate,
            ToggleRadialFor,
            GrabFromInventory,
            DropCurrentlyHeldItem,
            FlashBreasts
        }
    }
}
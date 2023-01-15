﻿using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Translator.Explorer.JSON
{
	public static class StoryEnums
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public enum CompareTypes
		{
			Never = 1000,
			Clothing = 110,
			CoinFlip = 160,
			CompareValues = 1,
			CriteriaGroup = 200,
			CutScene = 175,
			Dialogue = 40,
			Distance = 150,
			Door = 140,
			IntimacyPartner = 136,
			IntimacyState = 135,
			InZone = 910,
			InVicinity = 19,
			InVicinityAndVision = 21,
			Item = 15,
			IsOnlyInVicinityOf = 77,
			IsOnlyInVisionOf,
			IsOnlyInVicinityAndVisionOf,
			IsCharacterEnabled = 210,
			IsCurrentlyBeingUsed = 191,
			IsCurrentlyUsing = 190,
			IsExplicitGameVersion = 2000,
			IsInFrontOf = 151,
			IsInHouse = 912,
			IsNewGame = 900,
			IsZoneEmpty = 913,
			ItemFromItemGroup = 16,
			MetByPlayer = 30,
			Personality = 5,
			IsBeingSpokenTo = 81,
			PlayerGender = 170,
			PlayerInventory = 10,
			Posing = 120,
			Property = 133,
			Quest = 100,
			SameZoneAs = 911,
			Social = 50,
			State = 130,
			Value = 0,
			Vision = 20,
			PlayerPrefs = 950,
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
			FinishedPopulatingMainDialogueText = 200,
			PlayerTookCameraPhoto = 250,
			OnAfterCutSceneEnds
		}
	}
}
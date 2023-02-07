using static Translator.Explorer.JSON.StoryEnums;

namespace Translator.Explorer.JSON
{
#pragma warning disable 1591

	internal interface ICriterion
	{
		public BoolCritera? BoolValue { get; set; }
		public string? Character { get; set; }
		public string? Character2 { get; set; }
		public CompareTypes? CompareType { get; set; }
		//used by criteriagroups now
		public int GroupSubCompareType { get; set; }
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
	}

#pragma warning restore
}
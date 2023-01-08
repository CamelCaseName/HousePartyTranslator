namespace Translator.Explorer.JSON
{
#pragma warning disable 1591

	internal interface ICriterion
	{
		string? BoolValue { get; set; }
		string? Character { get; set; }
		string? Character2 { get; set; }
		string? CompareType { get; set; }
		string? DialogueStatus { get; set; }
		bool DisplayInEditor { get; set; }
		string? DoorOptions { get; set; }
		string? EqualsValue { get; set; }
		string? EquationValue { get; set; }
		object? ItemComparison { get; set; }
		object? ItemFromItemGroupComparison { get; set; }
		string? Key { get; set; }
		string? Key2 { get; set; }
		int Option { get; set; }
		int Order { get; set; }
		string? PlayerInventoryOption { get; set; }
		string? PoseOption { get; set; }
		string? SocialStatus { get; set; }
		string? Value { get; set; }
		string? ValueFormula { get; set; }
	}

#pragma warning restore
}
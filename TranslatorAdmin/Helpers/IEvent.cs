#pragma warning disable 1591
internal interface IEvent
{
    string? Character { get; set; }
    string? Character2 { get; set; }
    List<Criterion>? Criteria { get; set; }
    double Delay { get; set; }
    bool DisplayInEditor { get; set; }
    bool Enabled { get; set; }
    int EventType { get; set; }
    string? Id { get; set; }
    string? Key { get; set; }
    int Option { get; set; }
    int Option2 { get; set; }
    int Option3 { get; set; }
    double OriginalDelay { get; set; }
    int SortOrder { get; set; }
    int SortOrder2 { get; set; }
    double StartDelayTime { get; set; }
    bool UseConditions { get; set; }
    string? Value { get; set; }
    string? Value2 { get; set; }
    string? Version { get; set; }
}

#pragma warning restore
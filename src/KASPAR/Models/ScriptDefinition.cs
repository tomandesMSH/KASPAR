namespace KASPAR.Models;

public enum ScriptTag
{
    Safe,
    Moderate,
    Advanced,
}

public sealed class ScriptDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ScriptTag Tag { get; init; }
    public required string Exe { get; init; }
    public string? RevertExe { get; init; }
    public bool Interactive { get; init; }
    public bool RequiresAdmin { get; init; }
    public string? DocsAnchor { get; init; }
}

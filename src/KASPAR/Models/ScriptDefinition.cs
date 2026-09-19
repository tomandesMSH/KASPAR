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
    /// <summary>In-process tweak; null for <see cref="Interactive"/> entries, which launch <see cref="Exe"/> instead.</summary>
    public Action? Apply { get; init; }
    public Action? Revert { get; init; }
    /// <summary>Path under scripts/ of a standalone tool; only set for <see cref="Interactive"/> entries.</summary>
    public string? Exe { get; init; }
    public bool Interactive { get; init; }
    public string? DocsAnchor { get; init; }
}

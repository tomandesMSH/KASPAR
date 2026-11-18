using Microsoft.UI.Xaml;

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
    /// <summary>In-process tweak; null for <see cref="Interactive"/> entries, which show <see cref="Dialog"/> instead.</summary>
    public Action? Apply { get; init; }
    public Action? Revert { get; init; }
    /// <summary>Shows its own dialog and returns the outcome, or null if the user cancelled; only set for <see cref="Interactive"/> entries.</summary>
    public Func<XamlRoot, Task<ScriptResult?>>? Dialog { get; init; }
    public bool Interactive { get; init; }
    public string? DocsAnchor { get; init; }
}

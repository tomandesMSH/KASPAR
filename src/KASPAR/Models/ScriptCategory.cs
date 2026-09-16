namespace KASPAR.Models;

public sealed class ScriptCategory
{
    public required string Name { get; init; }
    public required IReadOnlyList<ScriptDefinition> Scripts { get; init; }
}

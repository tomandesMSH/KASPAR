namespace KASPAR.Models;

public sealed class CategoryViewModel
{
    public required string Name { get; init; }
    public required IReadOnlyList<ScriptCardViewModel> Scripts { get; init; }
}

namespace Chabot.Commands;

public record UpdateMetadata(
    IReadOnlyDictionary<string, string?> Properties);
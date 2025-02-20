namespace Chabot.Commands;

internal class UpdatePropertiesRestriction
{
    public required string Key { get; init; }
    public required string?[] AllowedValues { get; init; }
}
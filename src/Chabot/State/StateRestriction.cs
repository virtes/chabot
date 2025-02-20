namespace Chabot.State;

public class StateRestriction
{
    public required string StateTargetType { get; init; }
    public required Type[] AllowedStateTypes { get; init; }
    public required bool AllowEmptyState { get; init; }
}
namespace Chabot.State;

public record SerializedState<TValue>(
    Guid Id,
    string TypeKey,
    TValue Value,
    DateTime CreatedAtUtc);
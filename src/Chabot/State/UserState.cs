namespace Chabot.State;

public class UserState
{
    public IState State { get; }

    public DateTime CreatedAtUtc { get; }

    public IReadOnlyDictionary<string, string?> Metadata { get; }

    public UserState(IState state,
        DateTime createdAtUtc, 
        IReadOnlyDictionary<string, string?> metadata)
    {
        State = state;
        CreatedAtUtc = createdAtUtc;
        Metadata = metadata;
    }
}
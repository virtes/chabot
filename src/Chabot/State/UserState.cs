namespace Chabot.State;

public class UserState
{
    public IState? State { get; }

    public DateTime CreatedAtUtc { get; }

    public Dictionary<string, string?>? Metadata { get; }

    public UserState(IState? state, 
        DateTime createdAtUtc, 
        Dictionary<string, string?>? metadata)
    {
        State = state;
        CreatedAtUtc = createdAtUtc;
        Metadata = metadata;
    }
}
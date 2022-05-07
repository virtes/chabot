namespace Chabot.State.Exceptions;

public class StateTypeNotFoundException : Exception
{
    public string StateTypeKey { get; }

    public StateTypeNotFoundException(string stateTypeKey) 
        : base($"State type by key '{stateTypeKey}' not found")
    {
        StateTypeKey = stateTypeKey;
    }
}
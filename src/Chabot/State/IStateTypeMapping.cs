namespace Chabot.State;

public interface IStateTypeMapping
{
    string GetStateTypeKey(Type stateType);

    Type GetStateType(string stateTypeKey);
}
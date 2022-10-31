namespace Chabot.State;

public interface IStateSerializer<TSerializedState>
{
    TSerializedState SerializeState(UserState userState);

    UserState DeserializeState(TSerializedState serializedStateData);
}
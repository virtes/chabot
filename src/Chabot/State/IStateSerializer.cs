namespace Chabot.State;

public interface IStateSerializer<TSerializedState>
{
   TSerializedState Serialize(object value);

   object Deserialize(TSerializedState serializedState, Type type);
}
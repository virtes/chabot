using Chabot.State;

namespace Chabot.MemoryCache.State;

internal class RawObjectStateSerializer : IStateSerializer<object>
{
    public object Serialize(object value)
    {
        return value;
    }

    public object Deserialize(object serializedState, Type type)
    {
        return serializedState;
    }
}
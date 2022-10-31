using System.Collections.Concurrent;
using Chabot.State;

namespace Chabot.InMemoryState.Implementation;

public class InMemoryStateStorage<TMessage, TUser, TKey, TSerializedState>
    : IStateStorage<TMessage, TUser, TSerializedState>
    where TKey : IEquatable<TKey>
{
    private readonly Func<TMessage, TUser, TKey> _keyFactory;
    private readonly ConcurrentDictionary<TKey, TSerializedState> _stateByUserId = new();

    public InMemoryStateStorage(Func<TMessage, TUser, TKey> keyFactory)
    {
        _keyFactory = keyFactory;
    }

    public ValueTask WriteState(TMessage message, TUser user, TSerializedState state)
    {
        var key = _keyFactory(message, user);

        _stateByUserId.AddOrUpdate(key, state, (_, _) => state);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<TSerializedState?> ReadState(TMessage message, TUser user)
    {
        var key = _keyFactory(message, user);

        if (_stateByUserId.TryGetValue(key, out var serializedState))
            return ValueTask.FromResult<TSerializedState?>(serializedState);

        return ValueTask.FromResult<TSerializedState?>(default);
    }
}
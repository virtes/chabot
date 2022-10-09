using System.Collections.Concurrent;
using Chabot.State;

namespace Chabot.InMemoryState.Implementation;

public class InMemoryStateStorage<TUserId, TSerializedState>
    : IStateStorage<TUserId, TSerializedState>
    where TUserId : IEquatable<TUserId>
{
    private readonly ConcurrentDictionary<TUserId, TSerializedState> _stateByUserId = new();

    public ValueTask WriteState(TUserId userId, TSerializedState state)
    {
        _stateByUserId.AddOrUpdate(userId, state, (_, _) => state);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<TSerializedState?> ReadState(TUserId userId)
    {
        if (_stateByUserId.TryGetValue(userId, out var serializedState))
            return ValueTask.FromResult<TSerializedState?>(serializedState);

        return ValueTask.FromResult<TSerializedState?>(default);
    }
}
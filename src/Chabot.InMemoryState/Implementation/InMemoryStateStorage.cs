using System.Collections.Concurrent;
using Chabot.State;

namespace Chabot.InMemoryState.Implementation;

public class InMemoryStateStorage<TStateTarget, TSerializedState>
    : IStateStorage<TStateTarget, TSerializedState>
    where TStateTarget : IEquatable<TStateTarget>
{
    private readonly ConcurrentDictionary<TStateTarget, TSerializedState> _states = new();

    public ValueTask WriteState(TStateTarget stateTarget, TSerializedState state)
    {
        _states.AddOrUpdate(stateTarget, state, (_, _) => state);
        
        return ValueTask.CompletedTask;
    }

    public ValueTask<TSerializedState?> ReadState(TStateTarget stateTarget)
    {
        if (_states.TryGetValue(stateTarget, out var serializedState))
            return ValueTask.FromResult<TSerializedState?>(serializedState);

        return ValueTask.FromResult<TSerializedState?>(default);
    }
}
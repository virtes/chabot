using Microsoft.Extensions.Logging;

namespace Chabot.State;

internal class StateAccessor<TSerializedState> : IStateAccessor
{
    private readonly IStateStorage<TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;
    private readonly IStateTypeResolver _stateTypeResolver;
    private readonly ILogger<StateAccessor<TSerializedState>> _logger;
    private readonly Dictionary<StateKey, object?> _currentStates = new();

    public StateAccessor(IStateStorage<TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer,
        IStateTypeResolver stateTypeResolver,
        ILogger<StateAccessor<TSerializedState>> logger)
    {
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
        _stateTypeResolver = stateTypeResolver;
        _logger = logger;
    }

    public async ValueTask<object?> GetState(IStateTarget stateTarget)
    {
        var stateKey = new StateKey
        {
            Key = stateTarget.Key,
            TargetType = stateTarget.TargetType
        };
        if (_currentStates.TryGetValue(stateKey, out var state))
            return state;

        var serializedState = await _stateStorage.GetState(stateTarget.Key, stateTarget.TargetType);
        if (serializedState is null)
        {
            _logger.LogDebug("State not found in storage ({Key}, {TargetType})",
                stateTarget.Key, stateTarget.TargetType);
            return null;
        }

        Type stateType;

        try
        {
            stateType = _stateTypeResolver.GetType(serializedState.TypeKey);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Could not resolve state type ({StateId}, {TypeKey})",
                serializedState.Id, serializedState.TypeKey);
            return null;
        }

        try
        {
            state = _stateSerializer.Deserialize(serializedState.Value, stateType);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Could not deserialize state ({StateId}, {StateTypeName})",
                serializedState.Id, stateType.FullName);
            return null;
        }

        _logger.LogTrace("State retrieved ({StateId}, {Key}, {TargetType}, {@State})",
            serializedState.Id, stateTarget.Key, stateTarget.TargetType, state);

        return state;
    }

    public async ValueTask SetState(IStateTarget stateTarget, object? state)
    {
        var stateKey = new StateKey
        {
            Key = stateTarget.Key,
            TargetType = stateTarget.TargetType
        };

        if (state is null)
        {
            await _stateStorage.SetState(stateTarget.Key, stateTarget.TargetType, null);
            _currentStates[stateKey] = null;
            _logger.LogDebug("Empty state set ({Key}, {TargetType})", stateTarget.Key, stateTarget.TargetType);
            return;
        }

        var serializedState = new SerializedState<TSerializedState>(
            Id: Guid.NewGuid(),
            TypeKey: _stateTypeResolver.GetTypeKey(state.GetType()),
            Value: _stateSerializer.Serialize(state),
            CreatedAtUtc: DateTime.UtcNow);
        await _stateStorage.SetState(stateTarget.Key, stateTarget.TargetType, serializedState);
        _currentStates[stateKey] = state;

        _logger.LogDebug("State set ({StateId}, {Key}, {TargetType})",
            serializedState.Id, stateTarget.Key, stateTarget.TargetType);
    }

    private readonly struct StateKey : IEquatable<StateKey>
    {
        public required string Key { get; init; }
        public required string TargetType { get; init; }

        public bool Equals(StateKey other) => Key == other.Key && TargetType == other.TargetType;

        public override bool Equals(object? obj) => obj is StateKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Key, TargetType);
    }
}
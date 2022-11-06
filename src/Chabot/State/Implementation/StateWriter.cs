using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation;

public class StateWriter<TStateTarget, TSerializedState> : IStateWriter<TStateTarget>
{
    private readonly ILogger<StateWriter<TStateTarget, TSerializedState>> _logger;
    private readonly IStateStorage<TStateTarget, TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<string, string?> EmptyDictionary
        = new Dictionary<string, string?>();

    public StateWriter(ILogger<StateWriter<TStateTarget, TSerializedState>> logger,
        IStateStorage<TStateTarget, TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer)
    {
        _logger = logger;
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
    }

    public async Task<UserState> WriteState(IState state, TStateTarget stateTarget)
    {
        var userState = new UserState(
            state: state,
            createdAtUtc: DateTime.UtcNow,
            metadata: EmptyDictionary);
        
        var serializedState = _stateSerializer.SerializeState(userState);
        
        await _stateStorage.WriteState(stateTarget, serializedState);
        
        _logger.LogDebug("User state {@UserState} written", userState);

        return userState;
    }
}
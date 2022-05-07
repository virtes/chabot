using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation;

public class StateWriter<TUserId, TSerializedState> : IStateWriter<TUserId>
{
    private readonly ILogger<StateWriter<TUserId, TSerializedState>> _logger;
    private readonly IStateStorage<TUserId, TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;

    public StateWriter(ILogger<StateWriter<TUserId, TSerializedState>> logger,
        IStateStorage<TUserId, TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer)
    {
        _logger = logger;
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
    }

    public async Task<UserState> WriteState(IState? state, TUserId userId)
    {
        var userState = new UserState(
            state: state,
            createdAtUtc: DateTime.UtcNow, 
            metadata: null);
        
        var serializedState = _stateSerializer.SerializeState(userState);
        
        await _stateStorage.WriteState(userId, serializedState);
        
        _logger.LogDebug("User {UserId} state {@UserState} written", userId, userState);

        return userState;
    }
}
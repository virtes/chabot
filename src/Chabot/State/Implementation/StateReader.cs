using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation;

public class StateReader<TUserId, TSerializedState> : IStateReader<TUserId>
{
    private readonly ILogger<StateReader<TUserId, TSerializedState>> _logger;
    private readonly IStateStorage<TUserId, TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;

    public StateReader(ILogger<StateReader<TUserId, TSerializedState>> logger,
        IStateStorage<TUserId, TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer)
    {
        _logger = logger;
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
    }
    
    public async Task<UserState> ReadState(TUserId userId)
    {
        UserState userState;
        
        var serializedState = await _stateStorage.ReadState(userId);
        if (serializedState is null)
        {
            _logger.LogDebug("User {UserId} storage state is null", userId);
            userState = new UserState(null, DateTime.UtcNow, null);
        }
        else
        {
            userState = _stateSerializer.DeserializeState(serializedState);
        }
        
        _logger.LogDebug("User {UserId} state read {@UserState}", userId, userState);

        return userState;
    }
}
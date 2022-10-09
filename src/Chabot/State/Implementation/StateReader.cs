using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation;

public class StateReader<TUserId, TSerializedState> : IStateReader<TUserId>
{
    private readonly ILogger<StateReader<TUserId, TSerializedState>> _logger;
    private readonly IStateStorage<TUserId, TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;
    private readonly IDefaultStateFactory<TUserId> _defaultStateFactory;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<string, string?> EmptyDictionary
        = new Dictionary<string, string?>();

    public StateReader(ILogger<StateReader<TUserId, TSerializedState>> logger,
        IStateStorage<TUserId, TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer,
        IDefaultStateFactory<TUserId> defaultStateFactory)
    {
        _logger = logger;
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
        _defaultStateFactory = defaultStateFactory;
    }
    
    public async Task<UserState> ReadState(TUserId userId)
    {
        UserState userState;
        
        var serializedState = await _stateStorage.ReadState(userId);
        if (serializedState is null)
        {
            _logger.LogDebug("User {UserId} storage state is null, using default state", userId);

            var defaultState = _defaultStateFactory.CreateDefaultState(userId);
            userState = new UserState(defaultState, DateTime.UtcNow, EmptyDictionary);
        }
        else
        {
            userState = _stateSerializer.DeserializeState(serializedState);
        }
        
        _logger.LogDebug("User {UserId} state read {@UserState}", userId, userState);

        return userState;
    }
}
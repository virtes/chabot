using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation;

public class StateReader<TMessage, TUser, TSerializedState> : IStateReader<TMessage, TUser>
{
    private readonly ILogger<StateReader<TMessage, TUser, TSerializedState>> _logger;
    private readonly IStateStorage<TMessage, TUser, TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;
    private readonly IDefaultStateFactory<TMessage, TUser> _defaultStateFactory;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<string, string?> EmptyDictionary
        = new Dictionary<string, string?>();

    public StateReader(ILogger<StateReader<TMessage, TUser, TSerializedState>> logger,
        IStateStorage<TMessage, TUser, TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer,
        IDefaultStateFactory<TMessage, TUser> defaultStateFactory)
    {
        _logger = logger;
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
        _defaultStateFactory = defaultStateFactory;
    }
    
    public async Task<UserState> ReadState(TMessage message, TUser user)
    {
        UserState userState;
        
        var serializedState = await _stateStorage.ReadState(message, user);
        if (serializedState is null)
        {
            _logger.LogDebug("User storage state is null, using default state");

            var defaultState = _defaultStateFactory.CreateDefaultState(message, user);
            userState = new UserState(defaultState, DateTime.UtcNow, EmptyDictionary);
        }
        else
        {
            userState = _stateSerializer.DeserializeState(serializedState);
        }
        
        _logger.LogDebug("User state read {@UserState}", userState);

        return userState;
    }
}
using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation;

public class StateWriter<TMessage, TUser, TSerializedState> : IStateWriter<TMessage, TUser>
{
    private readonly ILogger<StateWriter<TMessage, TUser, TSerializedState>> _logger;
    private readonly IStateStorage<TMessage, TUser, TSerializedState> _stateStorage;
    private readonly IStateSerializer<TSerializedState> _stateSerializer;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<string, string?> EmptyDictionary
        = new Dictionary<string, string?>();

    public StateWriter(ILogger<StateWriter<TMessage, TUser, TSerializedState>> logger,
        IStateStorage<TMessage, TUser, TSerializedState> stateStorage,
        IStateSerializer<TSerializedState> stateSerializer)
    {
        _logger = logger;
        _stateStorage = stateStorage;
        _stateSerializer = stateSerializer;
    }

    public async Task<UserState> WriteState(IState state, TMessage message, TUser user)
    {
        var userState = new UserState(
            state: state,
            createdAtUtc: DateTime.UtcNow,
            metadata: EmptyDictionary);
        
        var serializedState = _stateSerializer.SerializeState(userState);
        
        await _stateStorage.WriteState(message, user, serializedState);
        
        _logger.LogDebug("User state {@UserState} written", userState);

        return userState;
    }
}
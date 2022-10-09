using System.Text.Json;
using Chabot.State.Exceptions;
using Microsoft.Extensions.Logging;

namespace Chabot.State.Implementation.SystemTextJson;

public class SystemTextJsonStateSerializer : IStateSerializer<string>
{
    private readonly ILogger<SystemTextJsonStateSerializer> _logger;
    private readonly IStateTypeMapping _stateTypeMapping;
    private readonly JsonSerializerOptions _stateSerializerOptions;

    public SystemTextJsonStateSerializer(
        JsonSerializerOptions? stateSerializerOptions,
        ILogger<SystemTextJsonStateSerializer> logger,
        IStateTypeMapping stateTypeMapping)
    {
        _stateSerializerOptions = stateSerializerOptions ?? new JsonSerializerOptions();
        _logger = logger;
        _stateTypeMapping = stateTypeMapping;
    }

    public string SerializeState(UserState userState)
    {
        var userStateDto = new UserStateJsonDto
        {
            StateTypeKey = _stateTypeMapping.GetStateTypeKey(userState.State.GetType()),
            SerializedState = JsonSerializer.Serialize((object)userState.State, _stateSerializerOptions),
            CreatedAtUtc = userState.CreatedAtUtc,
            Metadata = userState.Metadata
        };

        return JsonSerializer.Serialize(userStateDto);
    }

    public UserState DeserializeState(string serializedStateData)
    {
        try
        {
            var userStateDto = JsonSerializer.Deserialize<UserStateJsonDto>(serializedStateData)!;

            var stateType = _stateTypeMapping.GetStateType(userStateDto.StateTypeKey);
            var state = (IState)JsonSerializer.Deserialize(
                json: userStateDto.SerializedState,
                returnType: stateType,
                options: _stateSerializerOptions)!;

            return new UserState(state, userStateDto.CreatedAtUtc, userStateDto.Metadata );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not deserialize state data {SerializedStateData}", serializedStateData);

            throw new InvalidStateDataException("State deserialization failed", e);
        }
    }
}
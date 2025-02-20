using System.Text.Json;
using System.Text.Json.Serialization;
using Chabot.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Chabot.StackExchangeRedis.State;

internal class StackExchangeRedisStateStorage : IStateStorage<byte[]>
{
    private readonly ILogger<StackExchangeRedisStateStorage> _logger;
    private readonly IDatabase _database;
    private readonly StackExchangeRedisStateStorageOptions _options;

    public StackExchangeRedisStateStorage(
        IOptions<StackExchangeRedisStateStorageOptions> options,
        ILogger<StackExchangeRedisStateStorage> logger,
        IDatabase database)
    {
        _logger = logger;
        _database = database;
        _options = options.Value;
    }

    public async ValueTask<SerializedState<byte[]>?> GetState(string stateKey, string stateTargetType)
    {
        var cacheKey = GetRedisStateKey(stateKey, stateTargetType);

        byte[]? result = await _database.StringGetAsync(cacheKey);
        if (result is null)
        {
            _logger.LogDebug("State {StateKey} {StateTargetType} not found in redis database ({CacheKey})",
                stateKey, stateTargetType, cacheKey);
            return null;
        }

        var state = JsonSerializer.Deserialize<State>(result);
        if (state is null)
        {
            _logger.LogWarning("Deserialized state is null ({CacheKey})", cacheKey);
            return null;
        }

        return new SerializedState<byte[]>(
            Id: state.Id,
            TypeKey: state.TypeKey,
            Value: state.Value,
            CreatedAtUtc: state.CreatedAtUtc);
    }

    public async ValueTask SetState(string stateKey, string stateTargetType, SerializedState<byte[]>? serializedState)
    {
        var cacheKey = GetRedisStateKey(stateKey, stateTargetType);

        if (serializedState is null)
        {
            await _database.KeyDeleteAsync(cacheKey);
            _logger.LogDebug("State {StateKey} {StateTargetType} deleted ({CacheKey})",
                stateKey, stateTargetType, cacheKey);
            return;
        }

        var redisState = new State
        {
            Id = serializedState.Id,
            TypeKey = serializedState.TypeKey,
            Value = serializedState.Value,
            CreatedAtUtc = serializedState.CreatedAtUtc
        };
        var stateBytes = JsonSerializer.SerializeToUtf8Bytes(redisState);
        await _database.StringSetAsync(cacheKey, stateBytes, _options.Ttl);

        _logger.LogDebug("State {StateKey} {StateTargetType} saved ({CacheKey})",
            stateKey, stateTargetType, cacheKey);
    }

    private string GetRedisStateKey(string stateKey, string stateTargetType)
        => _options.KeyPrefix is null
            ? $"chabot-state:{stateTargetType}:{stateKey}"
            : $"{_options.KeyPrefix}:{stateTargetType}:{stateKey}";

    private class State
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("typeKey")]
        public string TypeKey { get; set; } = default!;

        [JsonPropertyName("value")]
        public byte[] Value { get; set; } = default!;

        [JsonPropertyName("createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
    }
}
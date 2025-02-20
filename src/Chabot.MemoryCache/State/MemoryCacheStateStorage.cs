using Chabot.State;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Chabot.MemoryCache.State;

internal class MemoryCacheStateStorage : IStateStorage<object>
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheStateStorageOptions _options;

    public MemoryCacheStateStorage(
        IOptions<MemoryCacheStateStorageOptions> options,
        IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
    }

    public ValueTask<SerializedState<object>?> GetState(string stateKey, string stateTargetType)
    {
        var cacheKey = GetCacheKey(stateKey, stateTargetType);

        return ValueTask.FromResult(_memoryCache.Get<SerializedState<object>?>(cacheKey));
    }

    public ValueTask SetState(string stateKey, string stateTargetType, SerializedState<object>? serializedState)
    {
        var cacheKey = GetCacheKey(stateKey, stateTargetType);

        if (serializedState is null)
            _memoryCache.Remove(cacheKey);
        else
            _memoryCache.Set(cacheKey, serializedState, _options.Ttl);

        return ValueTask.CompletedTask;
    }

    private string GetCacheKey(string key, string targetType)
    {
        return _options.KeyPrefix is null
            ? $"chabot-state-storage:{key}:{targetType}"
            : $"{_options}:{key}:{targetType}";
    }
}
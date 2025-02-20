namespace Chabot.MemoryCache.State;

public class MemoryCacheStateStorageOptions
{
    public TimeSpan Ttl { get; set; } = TimeSpan.FromHours(1);

    public string? KeyPrefix { get; set; }
}
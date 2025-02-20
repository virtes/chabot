namespace Chabot.StackExchangeRedis.State;

public class StackExchangeRedisStateStorageOptions
{
    public TimeSpan? Ttl { get; set; }

    public string? KeyPrefix { get; set; }
}
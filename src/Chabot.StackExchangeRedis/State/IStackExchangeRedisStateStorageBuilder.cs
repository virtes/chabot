using Chabot.State;
using StackExchange.Redis;

namespace Chabot.StackExchangeRedis.State;

public interface IStackExchangeRedisStateStorageBuilder : ISerializableStateStorageBuilder<byte[]>
{
    IStackExchangeRedisStateStorageBuilder Configure(
        Action<StackExchangeRedisStateStorageOptions> configure);

    IStackExchangeRedisStateStorageBuilder UseDatabase(
        Func<IServiceProvider, IDatabase> databaseFactory);
}
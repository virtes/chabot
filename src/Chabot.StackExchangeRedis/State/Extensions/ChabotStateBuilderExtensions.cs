using Chabot.StackExchangeRedis.State;
using Chabot.State;

// ReSharper disable once CheckNamespace
namespace Chabot.StackExchangeRedis;

public static class ChabotStateBuilderExtensions
{
    public static ChabotStateBuilder<TUpdate> UseStackExchangeRedisStorage<TUpdate>(this ChabotStateBuilder<TUpdate> builder,
        Action<IStackExchangeRedisStateStorageBuilder> configureBuilder)
    {
        var storageBuilder = new StackExchangeRedisStateStorageBuilder<TUpdate>(builder);
        configureBuilder(storageBuilder);

        storageBuilder.Register();

        return builder;
    }
}
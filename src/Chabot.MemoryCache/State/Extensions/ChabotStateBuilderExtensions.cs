using Chabot.MemoryCache.State;
using Chabot.State;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Chabot.MemoryCache;

public static class ChabotStateBuilderExtensions
{
    public static ChabotStateBuilder<TUpdate> UseMemoryCacheStorage<TUpdate>(
        this ChabotStateBuilder<TUpdate> builder,
        Action<MemoryCacheStateStorageOptions>? options = null)
    {
        builder.RegisterChabotStateServices<object>();

        if (options is not null)
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            builder.ChabotBuilder.Services.Configure<MemoryCacheStateStorageOptions>(options);
        }

        builder.ChabotBuilder.Services.TryAddSingleton<IStateStorage<object>, MemoryCacheStateStorage>();
        builder.ChabotBuilder.Services.TryAddSingleton<IStateSerializer<object>, RawObjectStateSerializer>();

        builder.ChabotBuilder.ValidateServiceRegistration<IMemoryCache>();

        return builder;
    }
}
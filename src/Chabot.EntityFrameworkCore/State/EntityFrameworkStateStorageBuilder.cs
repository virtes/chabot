using Chabot.Exceptions;
using Chabot.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chabot.EntityFrameworkCore.State;

internal class EntityFrameworkStateStorageBuilder : ISerializableStateStorageBuilder<string>
{
    private readonly ChabotStateBuilder _stateBuilder;
    private Func<IServiceProvider, IStateSerializer<string>>? _stateSerializerFactory;

    public EntityFrameworkStateStorageBuilder(ChabotStateBuilder stateBuilder)
    {
        _stateBuilder = stateBuilder;
    }

    public ISerializableStateStorageBuilder<string> UseSerializer(
        Func<IServiceProvider, IStateSerializer<string>> stateSerializerFactory)
    {
        _stateSerializerFactory = stateSerializerFactory;
        return this;
    }

    internal void Register<TDbContext>()
        where TDbContext : DbContext, IChabotStateStorageDbContext
    {
        if (_stateSerializerFactory is null)
            throw new InvalidChabotConfigurationException("StackExchangeRedis state storage serializer must be configured");

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        _stateBuilder.ChabotBuilder.Services.TryAddScoped<IStateSerializer<string>>(_stateSerializerFactory);

        _stateBuilder.ChabotBuilder.Services.TryAddScoped<IStateStorage<string>, EntityFrameworkStateStorage<TDbContext>>();

        _stateBuilder.RegisterChabotStateServices<string>();
    }
}
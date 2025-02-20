using Chabot.Exceptions;
using Chabot.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Chabot.StackExchangeRedis.State;

internal class StackExchangeRedisStateStorageBuilder<TUpdate> : IStackExchangeRedisStateStorageBuilder
{
    private readonly ChabotStateBuilder<TUpdate> _stateBuilder;

    private Func<IServiceProvider, IStateSerializer<byte[]>>? _stateSerializerFactory;
    private Func<IServiceProvider, IDatabase>? _databaseFactory;
    private Action<StackExchangeRedisStateStorageOptions>? _configureOptions;

    public StackExchangeRedisStateStorageBuilder(ChabotStateBuilder<TUpdate> stateBuilder)
    {
        _stateBuilder = stateBuilder;
    }

    public IStackExchangeRedisStateStorageBuilder Configure(
        Action<StackExchangeRedisStateStorageOptions> configure)
    {
        _configureOptions = configure;

        return this;
    }

    public IStackExchangeRedisStateStorageBuilder UseDatabase(
        Func<IServiceProvider, IDatabase> databaseFactory)
    {
        _databaseFactory = databaseFactory;

        return this;
    }

    public ISerializableStateStorageBuilder<byte[]> UseSerializer(
        Func<IServiceProvider, IStateSerializer<byte[]>> stateSerializerFactory)
    {
        _stateSerializerFactory = stateSerializerFactory;

        return this;
    }

    internal void Register()
    {
        if (_stateSerializerFactory is null)
            throw new InvalidChabotConfigurationException("StackExchangeRedis state storage serializer must be configured");

        if (_configureOptions is not null)
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            _stateBuilder.ChabotBuilder.Services.Configure<StackExchangeRedisStateStorageOptions>(_configureOptions);
        }

        if (_databaseFactory is null)
        {
            _stateBuilder.ChabotBuilder.ValidateServiceRegistration<IDatabase>();
            _databaseFactory = sp => sp.GetRequiredService<IDatabase>();
        }

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        _stateBuilder.ChabotBuilder.Services.TryAddScoped<IStateSerializer<byte[]>>(_stateSerializerFactory);
        _stateBuilder.ChabotBuilder.Services.TryAddScoped<IStateStorage<byte[]>>(
            sp => new StackExchangeRedisStateStorage(
                options: sp.GetRequiredService<IOptions<StackExchangeRedisStateStorageOptions>>(),
                logger: sp.GetRequiredService<ILogger<StackExchangeRedisStateStorage>>(),
                database: _databaseFactory(sp)));

        _stateBuilder.RegisterChabotStateServices<byte[]>();
    }
}
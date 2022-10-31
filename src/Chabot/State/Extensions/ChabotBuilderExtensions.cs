using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Chabot.Command;
using Chabot.Configuration;
using Chabot.State;
using Chabot.State.Configuration;
using Chabot.State.Implementation;
using Chabot.State.Implementation.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static partial class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser>
        UseState<TMessage, TUser>(
            this ChabotBuilder<TMessage, TUser>  chabotBuilder,
            Action<StateBuilder<TMessage, TUser>> stateBuilderAction)
    {
        chabotBuilder.Services.TryAddSingleton<IStateTypeMapping, StateTypeMapping>();

        chabotBuilder.Services.AddScoped<StateExtractorMiddleware<TMessage, TUser>>();
        chabotBuilder.UseMiddleware<StateExtractorMiddleware<TMessage, TUser>>();

        chabotBuilder.ValidateServiceRegistration<IStateReader<TMessage, TUser>>("State reader");
        chabotBuilder.ValidateServiceRegistration<IStateWriter<TMessage, TUser>>("State writer");

        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser>,
            StateParameterValueResolverFactory<TMessage, TUser>>();

        var stateOptionsBuilder = chabotBuilder.Services
            .AddOptions<StateOptions>()
            .Configure(o => o.AssembliesToScanStateTypes.Add(typeof(DefaultState).Assembly))
            .Configure(o => o.AssembliesToScanStateTypes.Add(Assembly.GetEntryAssembly()!));

        var stateBuilder = new StateBuilder<TMessage, TUser>(
            chabotBuilder: chabotBuilder,
            optionsBuilder: stateOptionsBuilder);
        stateBuilderAction(stateBuilder);

        return chabotBuilder;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddStateReader<TMessage, TUser, TSerializedState>(
        this ChabotBuilder<TMessage, TUser>  chabotBuilder)
    {
        chabotBuilder.Services.TryAddScoped<IStateReader<TMessage, TUser>,
            StateReader<TMessage, TUser, TSerializedState>>();
        chabotBuilder.Services.TryAddSingleton<IDefaultStateFactory<TMessage, TUser>,
            DefaultStateFactory<TMessage, TUser>>();

        chabotBuilder.ValidateServiceRegistration<IStateStorage<TMessage, TUser, TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state storage");
        chabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state serializer");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddStateWriter<TMessage, TUser, TSerializedState>(
        this ChabotBuilder<TMessage, TUser>  chabotBuilder)
    {
        chabotBuilder.Services.TryAddScoped<IStateWriter<TMessage, TUser>,
            StateWriter<TMessage, TUser, TSerializedState>>();

        chabotBuilder.ValidateServiceRegistration<IStateStorage<TMessage, TUser, TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state storage");
        chabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state serializer");
    }

    public static StateBuilder<TMessage, TUser, string>
        UseSystemTextJsonSerializer<TMessage, TUser>(
        this StateBuilder<TMessage, TUser> stateBuilder,
        JsonSerializerOptions? serializerOptions = null)
    {
        stateBuilder.ChabotBuilder.AddStateReader<TMessage, TUser, string>();
        stateBuilder.ChabotBuilder.AddStateWriter<TMessage, TUser, string>();

        stateBuilder.ChabotBuilder.Services
            .AddSingleton<IStateSerializer<string>>(sp =>
                new SystemTextJsonStateSerializer(
                    stateSerializerOptions: serializerOptions,
                    logger: sp.GetRequiredService<ILogger<SystemTextJsonStateSerializer>>(),
                    stateTypeMapping: sp.GetRequiredService<IStateTypeMapping>()));

        return new StateBuilder<TMessage, TUser, string>(
            stateBuilder.ChabotBuilder, stateBuilder.OptionsBuilder);
    }
}
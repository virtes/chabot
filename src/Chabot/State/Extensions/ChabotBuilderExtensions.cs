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
    public static ChabotBuilder<TMessage, TUser, TStateTarget>
        UseState<TMessage, TUser, TStateTarget>(
            this ChabotBuilder<TMessage, TUser, TStateTarget>  chabotBuilder,
            Action<StateBuilder<TMessage, TUser, TStateTarget>> stateBuilderAction)
    {
        chabotBuilder.Services.TryAddSingleton<IStateTypeMapping, StateTypeMapping>();

        chabotBuilder.Services.AddScoped<StateExtractorMiddleware<TMessage, TUser, TStateTarget>>();
        chabotBuilder.UseMiddleware<StateExtractorMiddleware<TMessage, TUser, TStateTarget>>();

        chabotBuilder.ValidateServiceRegistration<IStateTargetFactory<TMessage, TUser, TStateTarget>>(
            "State target factory");
        chabotBuilder.ValidateServiceRegistration<IStateReader<TMessage, TUser, TStateTarget>>("State reader");
        chabotBuilder.ValidateServiceRegistration<IStateWriter<TStateTarget>>("State writer");

        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser>,
            StateParameterValueResolverFactory<TMessage, TUser>>();

        var stateOptionsBuilder = chabotBuilder.Services
            .AddOptions<StateOptions>()
            .Configure(o => o.AssembliesToScanStateTypes.Add(typeof(DefaultState).Assembly))
            .Configure(o => o.AssembliesToScanStateTypes.Add(Assembly.GetEntryAssembly()!));

        var stateBuilder = new StateBuilder<TMessage, TUser, TStateTarget>(
            chabotBuilder: chabotBuilder,
            optionsBuilder: stateOptionsBuilder);
        stateBuilderAction(stateBuilder);

        return chabotBuilder;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddStateReader<TMessage, TUser, TStateTarget, TSerializedState>(
        this ChabotBuilder<TMessage, TUser, TStateTarget>  chabotBuilder)
    {
        chabotBuilder.Services.TryAddScoped<IStateReader<TMessage, TUser, TStateTarget>,
            StateReader<TMessage, TUser, TStateTarget, TSerializedState>>();
        chabotBuilder.Services.TryAddSingleton<IDefaultStateFactory<TMessage, TUser>,
            DefaultStateFactory<TMessage, TUser>>();

        chabotBuilder.ValidateServiceRegistration<IStateStorage<TStateTarget, TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state storage");
        chabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state serializer");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddStateWriter<TMessage, TUser, TStateTarget, TSerializedState>(
        this ChabotBuilder<TMessage, TUser, TStateTarget> chabotBuilder)
    {
        chabotBuilder.Services.TryAddScoped<IStateWriter<TStateTarget>,
            StateWriter<TStateTarget, TSerializedState>>();

        chabotBuilder.ValidateServiceRegistration<IStateStorage<TStateTarget, TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state storage");
        chabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state serializer");
    }

    public static StateBuilder<TMessage, TUser, TStateTarget, string>
        UseSystemTextJsonSerializer<TMessage, TUser, TStateTarget>(
        this StateBuilder<TMessage, TUser, TStateTarget> stateBuilder,
        JsonSerializerOptions? serializerOptions = null)
    {
        stateBuilder.ChabotBuilder.AddStateReader<TMessage, TUser, TStateTarget, string>();
        stateBuilder.ChabotBuilder.AddStateWriter<TMessage, TUser, TStateTarget, string>();

        stateBuilder.ChabotBuilder.Services
            .AddSingleton<IStateSerializer<string>>(sp =>
                new SystemTextJsonStateSerializer(
                    stateSerializerOptions: serializerOptions,
                    logger: sp.GetRequiredService<ILogger<SystemTextJsonStateSerializer>>(),
                    stateTypeMapping: sp.GetRequiredService<IStateTypeMapping>()));

        return new StateBuilder<TMessage, TUser, TStateTarget, string>(
            stateBuilder.ChabotBuilder, stateBuilder.OptionsBuilder);
    }
}
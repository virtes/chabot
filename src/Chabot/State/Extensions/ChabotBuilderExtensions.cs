using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Chabot.Command;
using Chabot.Configuration;
using Chabot.Message;
using Chabot.State;
using Chabot.State.Configuration;
using Chabot.State.Implementation;
using Chabot.State.Implementation.SystemTextJson;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static partial class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser, TUserId>
        UseState<TMessage, TUser, TUserId>(
            this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder,
            Action<StateBuilder<TMessage, TUser, TUserId>> stateBuilderAction)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.TryAddSingleton<IStateTypeMapping, StateTypeMapping>();

        chabotBuilder.Services.AddScoped<StateExtractorMiddleware<TMessage, TUser, TUserId>>();
        chabotBuilder.UseMiddleware<StateExtractorMiddleware<TMessage, TUser, TUserId>>();

        chabotBuilder.ValidateServiceRegistration<IStateReader<TUserId>>("State reader");
        chabotBuilder.ValidateServiceRegistration<IStateWriter<TUserId>>("State writer");

        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>,
            StateParameterValueResolverFactory<TMessage, TUser, TUserId>>();

        var stateOptionsBuilder = chabotBuilder.Services
            .AddOptions<StateOptions>()
            .Configure(o => o.AssembliesToScanStateTypes.Add(typeof(DefaultState).Assembly))
            .Configure(o => o.AssembliesToScanStateTypes.Add(Assembly.GetEntryAssembly()!));

        var stateBuilder = new StateBuilder<TMessage, TUser, TUserId>(
            chabotBuilder: chabotBuilder,
            optionsBuilder: stateOptionsBuilder);
        stateBuilderAction(stateBuilder);

        return chabotBuilder;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddStateReader<TMessage, TUser, TUserId, TSerializedState>(
        this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.TryAddScoped<IStateReader<TUserId>, StateReader<TUserId, TSerializedState>>();
        chabotBuilder.Services.TryAddSingleton<IDefaultStateFactory<TUserId>, DefaultStateFactory<TUserId>>();

        chabotBuilder.ValidateServiceRegistration<IStateStorage<TUserId, TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state storage");
        chabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state serializer");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddStateWriter<TMessage, TUser, TUserId, TSerializedState>(
        this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.TryAddScoped<IStateWriter<TUserId>, StateWriter<TUserId, TSerializedState>>();

        chabotBuilder.ValidateServiceRegistration<IStateStorage<TUserId, TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state storage");
        chabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>(
            $"{typeof(TSerializedState).FullName} state serializer");
    }

    public static StateBuilder<TMessage, TUser, TUserId, string>
        UseSystemTextJsonSerializer<TMessage, TUser, TUserId>(
        this StateBuilder<TMessage, TUser, TUserId> stateBuilder,
        JsonSerializerOptions? serializerOptions = null)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        stateBuilder.ChabotBuilder.AddStateReader<TMessage, TUser, TUserId, string>();
        stateBuilder.ChabotBuilder.AddStateWriter<TMessage, TUser, TUserId, string>();

        stateBuilder.ChabotBuilder.Services
            .AddSingleton<IStateSerializer<string>>(sp =>
                new SystemTextJsonStateSerializer(
                    stateSerializerOptions: serializerOptions,
                    logger: sp.GetRequiredService<ILogger<SystemTextJsonStateSerializer>>(),
                    stateTypeMapping: sp.GetRequiredService<IStateTypeMapping>()));

        return new StateBuilder<TMessage, TUser, TUserId, string>(
            stateBuilder.ChabotBuilder, stateBuilder.OptionsBuilder);
    }
}
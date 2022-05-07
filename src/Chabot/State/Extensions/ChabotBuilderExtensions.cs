using System.Text.Json;
using Chabot.Configuration;
using Chabot.Message;
using Chabot.Message.Implementation;
using Chabot.State;
using Chabot.State.Implementation.SystemTextJson;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static partial class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser, TUserId>
        AddSystemTextJsonState<TMessage, TUser, TUserId>(
            this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder,
            Action<StateBuilder<TMessage, TUser, TUserId, string>> stateBuilderAction,
            JsonSerializerOptions? serializerOptions = null)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.AddSingleton<IStateSerializer<string>>(sp =>
            new SystemTextJsonStateSerializer(
                stateSerializerOptions: serializerOptions,
                logger: sp.GetRequiredService<ILogger<SystemTextJsonStateSerializer>>(),
                stateTypeMapping: sp.GetRequiredService<IStateTypeMapping>()));

        chabotBuilder.AddState<string>();
        
        var stateBuilder = new StateBuilder<TMessage, TUser, TUserId, string>(chabotBuilder);
        stateBuilderAction(stateBuilder);

        return chabotBuilder;
    }

    public static ChabotBuilder<TMessage, TUser, TUserId> UseStateExtractor<TMessage, TUser, TUserId>(
        this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.AddScoped<StateExtractorMiddleware<TMessage, TUser, TUserId>>();

        chabotBuilder.UseMiddleware<StateExtractorMiddleware<TMessage, TUser, TUserId>>();

        chabotBuilder.ValidateServiceRegistration<IStateReader<TUserId>>();

        return chabotBuilder;
    }
}
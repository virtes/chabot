using Chabot.Command;
using Chabot.Command.Implementation;
using Chabot.Configuration;
using Chabot.Message;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static partial class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser, TUserId> UseCommands<TMessage, TUser, TUserId>(
        this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        chabotBuilder.Services.AddSingleton<CommandActionInvokerMiddleware<TMessage, TUser, TUserId>>();

        chabotBuilder.UseMiddleware<CommandActionInvokerMiddleware<TMessage, TUser, TUserId>>();

        chabotBuilder.ValidateServiceRegistration<IActionSelectionMetadataFactory<TMessage, TUser, TUserId>>();
        chabotBuilder.ValidateServiceRegistration<IMessageActionProvider<TMessage, TUser, TUserId>>();

        return chabotBuilder;
    }
}
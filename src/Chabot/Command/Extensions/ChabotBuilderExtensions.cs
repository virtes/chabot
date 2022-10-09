using System.Reflection;
using Chabot.Command;
using Chabot.Command.Configuration;
using Chabot.Command.Implementation;
using Chabot.Configuration;
using Chabot.Message;
using Chabot.Message.Implementation;
using Chabot.User;
using Chabot.User.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static partial class ChabotBuilderExtensions
{
    public static ChabotBuilder<TMessage, TUser, TUserId> UseCommands<TMessage, TUser, TUserId>(
        this ChabotBuilder<TMessage, TUser, TUserId>  chabotBuilder,
        params Assembly[] assembliesToScan)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        if (assembliesToScan.Length == 0)
        {
            assembliesToScan = new[]
            {
                Assembly.GetEntryAssembly()!
            };
        }

        chabotBuilder.Services.AddOptions<CommandsOptions>();

        foreach (var assembly in assembliesToScan)
        {
            chabotBuilder.Services.Configure<CommandsOptions>(o => o.AssembliesToScan.Add(assembly));
            RegisterCommandGroups<TMessage, TUser, TUserId>(chabotBuilder.Services, assembly);
        }

        chabotBuilder.Services.TryAddSingleton<IMessageActionProvider<TMessage, TUser, TUserId>,
            MessageActionProvider<TMessage, TUser, TUserId> >();
        chabotBuilder.Services.TryAddSingleton<ICommandMessageActionBuilder<TMessage, TUser, TUserId>,
            CommandMessageActionBuilder<TMessage, TUser, TUserId>>();
        chabotBuilder.Services.TryAddSingleton<ICommandDescriptorSelector, CommandDescriptorSelector>();
        chabotBuilder.Services.TryAddSingleton<ICommandDescriptorsProvider, CommandDescriptorsProvider>();

        chabotBuilder.Services.AddSingleton<CommandActionInvokerMiddleware<TMessage, TUser, TUserId>>();
        chabotBuilder.UseMiddleware<CommandActionInvokerMiddleware<TMessage, TUser, TUserId>>();

        chabotBuilder.ValidateServiceRegistration<IActionSelectionMetadataFactory<TMessage, TUser, TUserId>>(
            "Action selection metadata factory");

        chabotBuilder.Services.AddHostedService<CommandsValidatorHostedService>();

        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>,
            CommandTextParameterValueResolverFactory<TMessage, TUser, TUserId>>();
        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>,
            UserIdParameterValueResolverFactory<TMessage, TUser, TUserId>>();

        return chabotBuilder;
    }

    private static void RegisterCommandGroups<TMessage, TUser, TUserId>(
        IServiceCollection services, Assembly assembly)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        var commandGroupBaseType = typeof(CommandGroupBase<TMessage, TUser, TUserId>);
        var commandGroupTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract
                        && t.IsClass
                        && t.IsAssignableTo(commandGroupBaseType));

        foreach (var commandGroupType in commandGroupTypes)
        {
            services.TryAddScoped(commandGroupType);
        }
    }
}
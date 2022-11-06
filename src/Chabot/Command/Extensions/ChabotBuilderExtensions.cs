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
    public static ChabotBuilder<TMessage, TUser, TStateTarget> UseCommands<TMessage, TUser, TStateTarget>(
        this ChabotBuilder<TMessage, TUser, TStateTarget>  chabotBuilder,
        params Assembly[] assembliesToScan)
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
            RegisterCommandGroups<TMessage, TUser>(chabotBuilder.Services, assembly);
        }

        chabotBuilder.Services.TryAddSingleton<IMessageActionProvider<TMessage, TUser>,
            MessageActionProvider<TMessage, TUser>>();
        chabotBuilder.Services.TryAddSingleton<ICommandMessageActionBuilder<TMessage, TUser>,
            CommandMessageActionBuilder<TMessage, TUser>>();
        chabotBuilder.Services.TryAddSingleton<ICommandDescriptorSelector, CommandDescriptorSelector>();
        chabotBuilder.Services.TryAddSingleton<ICommandDescriptorsProvider, CommandDescriptorsProvider>();

        chabotBuilder.Services.AddSingleton<CommandActionInvokerMiddleware<TMessage, TUser>>();
        chabotBuilder.UseMiddleware<CommandActionInvokerMiddleware<TMessage, TUser>>();

        chabotBuilder.ValidateServiceRegistration<IActionSelectionMetadataFactory<TMessage, TUser>>(
            "Action selection metadata factory");

        chabotBuilder.Services.AddHostedService<CommandsValidatorHostedService>();

        chabotBuilder.ValidateServiceRegistration<IMessageTextResolver<TMessage>>(
            "Message text resolver");
        chabotBuilder.ValidateServiceRegistration<IUserIdResolver<TMessage, TUser>>(
            "User ID resolver");

        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser>,
            CommandTextParameterValueResolverFactory<TMessage, TUser>>();
        chabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TMessage, TUser>,
            UserIdParameterValueResolverFactory<TMessage, TUser>>();

        return chabotBuilder;
    }

    private static void RegisterCommandGroups<TMessage, TUser>(
        IServiceCollection services, Assembly assembly)
    {
        var commandGroupBaseType = typeof(CommandGroupBase<TMessage, TUser>);
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
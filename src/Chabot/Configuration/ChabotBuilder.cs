using System.ComponentModel;
using System.Reflection;
using Chabot.Command;
using Chabot.Command.Configuration;
using Chabot.Command.Implementation;
using Chabot.Configuration.Exceptions;
using Chabot.Message;
using Chabot.Message.Implementation;
using Chabot.State;
using Chabot.State.Implementation;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chabot.Configuration;

public class ChabotBuilder
{
    protected ChabotBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    // ReSharper disable once MemberCanBeProtected.Global
    public List<Action<IServiceCollection>> ServicesValidators { get; } = new();

    internal void ValidateServiceRegistration<TServiceType>()
    {
        ServicesValidators.Add(services =>
        {
            if (services.All(sd => sd.ServiceType != typeof(TServiceType)))
                throw new InvalidChabotConfigurationException($"{nameof(TServiceType)} must be registered");
        });
    }
}

public class ChabotBuilder<TMessage, TUser, TUserId> : ChabotBuilder
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    public ChabotBuilder(IServiceCollection services)
        : base(services)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Func<HandleMessage<TMessage, TUser, TUserId>, HandleMessage<TMessage, TUser, TUserId>>> HandlingPipeline { get; } = new ();

    internal void RegisterChabot()
    {
        var handleMessageEntrypoint = BuildHandleMessageEntrypoint();
        Services.AddSingleton(handleMessageEntrypoint);

        Services.TryAddSingleton<IMessageHandler<TMessage, TUser, TUserId>,
            ScopedPipelineMessageHandler<TMessage, TUser, TUserId>>();

        ValidateServices();
    }

    private void ValidateServices()
    {
        var exceptions = new List<Exception>();

        foreach (var servicesValidator in ServicesValidators)
        {
            try
            {
                servicesValidator(Services);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        if (exceptions.Any())
        {
            var aggregateException = new AggregateException(exceptions);

            throw new InvalidChabotConfigurationException(
                message: "Invalid chabot configuration, see inner exception for details",
                innerException: aggregateException);
        }
    }

    public ChabotBuilder<TMessage, TUser, TUserId> Use(
        Func<HandleMessage<TMessage, TUser, TUserId>, HandleMessage<TMessage, TUser, TUserId>> use)
    {
        HandlingPipeline.Add(use);

        return this;
    }

    public ChabotBuilder<TMessage, TUser, TUserId> UseMiddleware(
        IMiddleware<TMessage, TUser, TUserId> middleware)
    {
        return Use(next =>
        {
            return async context =>
            {
                await middleware.Invoke(context, next);
            };
        });
    }

    public ChabotBuilder<TMessage, TUser, TUserId> UseMiddleware<TMiddleware>()
        where TMiddleware : IMiddleware<TMessage, TUser, TUserId>
    {
        return Use(next =>
        {
            return async context =>
            {
                var middleware = context.Services.GetRequiredService<TMiddleware>();

                await middleware.Invoke(context, next);
            };
        });
    }

    private HandleMessage<TMessage, TUser, TUserId> BuildHandleMessageEntrypoint()
    {
        HandleMessage<TMessage, TUser, TUserId> entrypoint = _ => Task.CompletedTask;

        foreach (var middleware in HandlingPipeline.AsEnumerable().Reverse())
        {
            entrypoint = middleware(entrypoint);
        }

        return entrypoint;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddState<TSerializedState>()
    {
        Services.TryAddScoped<IStateReader<TUserId>, StateReader<TUserId, TSerializedState>>();
        Services.TryAddScoped<IStateWriter<TUserId>, StateWriter<TUserId, TSerializedState>>();
        Services.TryAddSingleton<IStateTypeMapping, StateTypeMapping>();

        ValidateServiceRegistration<IStateSerializer<TSerializedState>>();
        ValidateServiceRegistration<IStateStorage<TUserId, TSerializedState>>();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddCommands(params Assembly[] assembliesToScan)
    {
        Services.AddOptions<CommandsOptions>();

        foreach (var assembly in assembliesToScan)
        {
            Services.Configure<CommandsOptions>(o => o.AssembliesToScan.Add(assembly));
            RegisterCommandGroups(assembly);
        }

        Services.TryAddSingleton<IMessageActionProvider<TMessage, TUser, TUserId>,
            MessageActionProvider<TMessage, TUser, TUserId> >();
        Services.TryAddSingleton<ICommandMessageActionBuilder, CommandMessageActionBuilder>();
        Services.TryAddSingleton<ICommandDescriptorSelector, CommandDescriptorSelector>();
        Services.TryAddSingleton<ICommandDescriptorsProvider, CommandDescriptorsProvider>();
    }

    private void RegisterCommandGroups(Assembly assembly)
    {
        var commandGroupBaseType = typeof(CommandGroupBase<TMessage, TUser, TUserId>);
        var commandGroupTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract
                        && t.IsClass
                        && t.IsAssignableTo(commandGroupBaseType));

        foreach (var commandGroupType in commandGroupTypes)
        {
            Services.TryAddScoped(commandGroupType);
        }
    }
}
using System.ComponentModel;
using Chabot.Configuration.Exceptions;
using Chabot.Message;
using Chabot.Message.Implementation;
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

    public void ValidateServiceRegistration<TServiceType>(string serviceDescription)
    {
        ServicesValidators.Add(services =>
        {
            if (services.All(sd => sd.ServiceType != typeof(TServiceType)))
                throw new InvalidChabotConfigurationException(
                    $"{serviceDescription} ({typeof(TServiceType).FullName}) must be registered");
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
}
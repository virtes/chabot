using System.ComponentModel;
using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Chabot;

internal class ChabotBuilder<TUpdate> : IChabotBuilder<TUpdate>
{
    public IServiceCollection Services { get; }

    private readonly List<Type> _servicesToValidate = new();

    public void ValidateServiceRegistration<TService>()
    {
        _servicesToValidate.Add(typeof(TService));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Func<HandleUpdate<TUpdate>, HandleUpdate<TUpdate>>> HandlingPipeline { get; } = new ();

    public ChabotBuilder(IServiceCollection services)
    {
        Services = services;
    }

    internal void Register()
    {
        var handleMessageEntrypoint = BuildHandleMessageEntrypoint();
        Services.AddSingleton(handleMessageEntrypoint);

        Services.TryAddSingleton<IChabot<TUpdate>>(
            sp => new Chabot<TUpdate>(
                serviceScopeFactory: sp.GetRequiredService<IServiceScopeFactory>(),
                logger: sp.GetRequiredService<ILogger<Chabot<TUpdate>>>(),
                entrypoint: handleMessageEntrypoint,
                updateMetadataParser: sp.GetRequiredService<IUpdateMetadataParser<TUpdate>>()));

        ValidateServices();
    }

    private void ValidateServices()
    {
        var missedServices = new List<Type>();
        foreach (var serviceType in _servicesToValidate)
        {
            if (Services.All(sd => sd.ServiceType != serviceType))
                missedServices.Add(serviceType);
        }

        if (missedServices.Any())
        {
            throw new InvalidOperationException(
                $"Invalid chabot registration. " +
                $"Missed services: {string.Join(", ", missedServices.Select(t => t.Name))}");
        }
    }

    public IChabotBuilder<TUpdate> Use(Func<HandleUpdate<TUpdate>, HandleUpdate<TUpdate>> use)
    {
        HandlingPipeline.Add(use);

        return this;
    }

    public IChabotBuilder<TUpdate> UseMiddleware<TMiddleware>()
        where TMiddleware : class, IMiddleware<TUpdate>
    {
        Services.TryAddSingleton<TMiddleware>();

        return Use(next =>
        {
            return async context =>
            {
                var middleware = context.ServiceProvider.GetRequiredService<TMiddleware>();

                await middleware.Invoke(context, next);
            };
        });
    }

    private HandleUpdate<TUpdate> BuildHandleMessageEntrypoint()
    {
        HandleUpdate<TUpdate> entrypoint = _ => Task.CompletedTask;

        foreach (var middleware in HandlingPipeline.AsEnumerable().Reverse())
        {
            entrypoint = middleware(entrypoint);
        }

        return entrypoint;
    }
}
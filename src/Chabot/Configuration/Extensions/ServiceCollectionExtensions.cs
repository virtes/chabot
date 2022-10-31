using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChabot<TMessage, TUser>(
        this IServiceCollection services, 
        Action<ChabotBuilder<TMessage, TUser>> builderAction)
    {
        var builder = new ChabotBuilder<TMessage, TUser>(services);
        builderAction(builder);

        builder.RegisterChabot();

        return services;
    } 
}
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChabot<TMessage, TUser, TStateTarget>(
        this IServiceCollection services, 
        Action<ChabotBuilder<TMessage, TUser, TStateTarget>> builderAction)
    {
        var builder = new ChabotBuilder<TMessage, TUser, TStateTarget>(services);
        builderAction(builder);

        builder.RegisterChabot();

        return services;
    } 
}
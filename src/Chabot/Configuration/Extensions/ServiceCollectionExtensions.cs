using System.ComponentModel;
using Chabot.Message;
using Chabot.State;
using Chabot.State.Implementation;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChabot<TMessage, TUser, TUserId>(
        this IServiceCollection services, 
        Action<ChabotBuilder<TMessage, TUser, TUserId>> builderAction)
        where TUser : IUser<TUserId> 
        where TMessage : IMessage
    {
        var builder = new ChabotBuilder<TMessage, TUser, TUserId>(services);
        builderAction(builder);

        builder.RegisterChabot();

        return services;
    } 
}
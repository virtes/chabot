using Chabot.Commands;
using Chabot.Proxy;
using Chabot.State;
using Chabot.Telegram.Commands;
using Chabot.Telegram.Proxy;
using Chabot.Telegram.State;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramChabot(this IServiceCollection services,
        Action<IChabotBuilder<Update>> configureBuilder)
    {
        services.AddSingleton<IUpdatePartitioner<Update>, TelegramUpdatePartitioner>();
        services.AddSingleton<IUpdateMetadataParser<Update>, TelegramUpdateMetadataParser>();

        services.AddSingleton<IStateTargetResolverFactory<Update>, StateTargetResolverFactory>();
        services.AddSingleton<ChatStateTargetResolver>();
        services.AddSingleton<ChatMessageStateTargetResolver>();

        services.AddSingleton<IRestrictionsFactory, AllowedUpdateTypeRestrictionsFactory>();
        services.AddSingleton<ICommandRestrictionHandler<Update, AllowedUpdateTypeRestriction>, AllowedUpdateTypeRestrictionsHandler>();

        services.AddSingleton<IRestrictionsFactory, AllowedCallbackQueryPayloadRestrictionsFactory>();
        services.AddSingleton<ICommandRestrictionHandler<Update, AllowedCallbackQueryPayloadRestriction>, AllowedCallbackQueryPayloadRestrictionHandler>();

        services.AddSingleton<ICommandParameterValueResolverFactory<Update>, FromChatIdParameterValueResolverFactory>();

        services.AddChabot(configureBuilder);

        return services;
    }
}
using Chabot.Command;
using Chabot.Configuration;
using Chabot.Telegram.Configuration;
using Chabot.Telegram.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTgChabot(this IServiceCollection services,
        Action<TelegramBotOptions> configureBotOptions,
        Action<ChabotBuilder<TgMessage, TgUser, long>> builderAction)
    {
        services
            .AddOptions<TelegramBotOptions>()
            .Configure(configureBotOptions)
            .Validate(o => !string.IsNullOrEmpty(o.Token), "Bot Token must be specified");
        
        services.TryAddSingleton<ITelegramBotClientProvider, TelegramBotClientProvider>();
        services.TryAddSingleton<IActionSelectionMetadataFactory<TgMessage, TgUser, long>,
            TgActionSelectionMetadataFactory>();
        
        services.AddChabot<TgMessage, TgUser, long>(builderAction);

        return services;
    } 
}
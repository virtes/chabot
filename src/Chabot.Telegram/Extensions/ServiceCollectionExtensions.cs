using Chabot.Command;
using Chabot.Configuration;
using Chabot.Telegram.Configuration;
using Chabot.Telegram.Implementation;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTgChabot(this IServiceCollection services,
        string botToken,
        Action<ChabotBuilder<TgMessage, TgUser, long>> builderAction)
    {
        services
            .AddOptions<TelegramBotClientOptions>()
            .Configure(o => o.Token = botToken)
            .ValidateDataAnnotations();
        
        services.AddSingleton<ITelegramBotClientProvider, TelegramBotClientProvider>();
        services.AddSingleton<IActionSelectionMetadataFactory<TgMessage, TgUser, long>,
            TgActionSelectionMetadataFactory>();
        
        services.AddChabot<TgMessage, TgUser, long>(builderAction);

        return services;
    } 
}
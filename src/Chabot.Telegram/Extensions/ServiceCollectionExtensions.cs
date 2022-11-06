using Chabot.Command;
using Chabot.Configuration;
using Chabot.Message;
using Chabot.State;
using Chabot.Telegram.Configuration;
using Chabot.Telegram.Implementation;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TelegramUpdate = Telegram.Bot.Types.Update;
using TelegramUser = Telegram.Bot.Types.User;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramChabot(this IServiceCollection services,
        Action<TelegramBotOptions, IServiceProvider> configureBotOptions,
        Action<ChabotBuilder<TelegramUpdate, TelegramUser, TelegramStateTarget>> builderAction)
    {
        services
            .AddOptions<TelegramBotOptions>()
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            .Configure<IServiceProvider>(configureBotOptions)
            .Validate(o => !string.IsNullOrEmpty(o.Token), "Bot Token must be specified");
        
        services.TryAddSingleton<ITelegramBotClientProvider, TelegramBotClientProvider>();
        services.TryAddSingleton<IActionSelectionMetadataFactory<TelegramUpdate, TelegramUser>,
            TelegramActionSelectionMetadataFactory>();
        services.TryAddSingleton<IMessageTextResolver<TelegramUpdate>, TelegramMessageTextResolver>();
        services.TryAddSingleton<IUserIdResolver<TelegramUpdate, TelegramUser>, TelegramUserIdResolver>();
        services.TryAddSingleton<IStateTargetFactory<TelegramUpdate, TelegramUser, TelegramStateTarget>,
            TelegramStateTargetFactory>();

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        services.AddChabot<TelegramUpdate, TelegramUser, TelegramStateTarget>(builderAction);

        return services;
    } 
}
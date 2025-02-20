using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ChabotBuilderExtensions
{
    public static IChabotBuilder<Update> AddTelegramLongPollingListener(this IChabotBuilder<Update> builder)
    {
        builder.Services.AddHostedService<TelegramLongPollingListenerHostedService>();

        return builder;
    }

    public static IChabotBuilder<Update> AddTelegramBotClient(this IChabotBuilder<Update> builder,
        Func<IServiceProvider, ITelegramBotClient> telegramBotClientFactory)
    {
        builder.Services.TryAddSingleton<ITelegramBotClientProvider>(
            sp => new TelegramBotClientProvider(telegramBotClientFactory(sp)));

        return builder;
    }

    public static IChabotBuilder<Update> AddTelegramBotClient(this IChabotBuilder<Update> builder,
        Action<TelegramBotClientOptions> configureOptions)
    {
        builder.Services.BindOptions(configureOptions);

        builder.Services.TryAddSingleton<ITelegramBotClientProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value;
            return new TelegramBotClientProvider(new TelegramBotClient(options.Token!));
        });

        return builder;
    }
}
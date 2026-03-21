using System.Net;
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
        builder.Services.TryAddTransient<ITelegramBotClientProvider>(
            sp => new TelegramBotClientProvider(telegramBotClientFactory(sp)));

        return builder;
    }

    public static IChabotBuilder<Update> AddTelegramBotClient(this IChabotBuilder<Update> builder,
        Action<TelegramBotClientOptions> configureOptions)
    {
        builder.Services.BindOptions(configureOptions);

        builder.Services.AddHttpClient("telegram-bot-client")
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var options = sp.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value;

                if (options.Proxy is null)
                    return new HttpClientHandler();

                var proxy = new WebProxy(options.Proxy.Address)
                {
                    Credentials = new NetworkCredential(options.Proxy.Username, options.Proxy.Password)
                };

                return new SocketsHttpHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };
            });

        builder.Services.TryAddTransient<ITelegramBotClientProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value;
            var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("telegram-bot-client");

            var telegramBotClient = new TelegramBotClient(
                new global::Telegram.Bot.TelegramBotClientOptions(options.Token), httpClient);

            return new TelegramBotClientProvider(telegramBotClient);
        });

        return builder;
    }
}
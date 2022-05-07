using Chabot.Configuration;
using Chabot.Telegram.Implementation;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ChabotBuilderExtensions
{
    public static ChabotBuilder<TgMessage, TgUser, long> UseTelegramPollingUpdates(
        this ChabotBuilder<TgMessage, TgUser, long> chabotBuilder)
    {
        chabotBuilder.Services.AddHostedService<TelegramListenerHostedService>();

        return chabotBuilder;
    }
}
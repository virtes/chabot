using Chabot.Configuration;
using Chabot.Telegram.Implementation;
using Microsoft.Extensions.DependencyInjection;
using TelegramMessage = Telegram.Bot.Types.Message;
using TelegramUser = Telegram.Bot.Types.User;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ChabotBuilderExtensions
{
    public static ChabotBuilder<TelegramMessage, TelegramUser> UseTelegramPollingUpdates(
        this ChabotBuilder<TelegramMessage, TelegramUser> chabotBuilder)
    {
        chabotBuilder.Services.AddHostedService<TelegramListenerHostedService>();

        return chabotBuilder;
    }
}
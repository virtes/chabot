using Chabot.Configuration;
using Chabot.Telegram.Implementation;
using Microsoft.Extensions.DependencyInjection;
using TelegramUpdate = Telegram.Bot.Types.Update;
using TelegramUser = Telegram.Bot.Types.User;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public static class ChabotBuilderExtensions
{
    public static ChabotBuilder<TelegramUpdate, TelegramUser> UseTelegramPollingUpdates(
        this ChabotBuilder<TelegramUpdate, TelegramUser> chabotBuilder)
    {
        chabotBuilder.Services.AddHostedService<TelegramListenerHostedService>();

        return chabotBuilder;
    }
}
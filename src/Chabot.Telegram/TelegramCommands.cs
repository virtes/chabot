using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Chabot.Telegram;

public class TelegramCommands : CommandsBase<Update>
{
    protected ITelegramBotClient BotClient
        => Context.ServiceProvider.GetRequiredService<ITelegramBotClientProvider>().GetClient();
}
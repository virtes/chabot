using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

// ReSharper disable once CheckNamespace
namespace Chabot.Telegram;

public class TextMessageBuilder
{
    private readonly string _text;
    private readonly ChatId _chatId;

    public TextMessageBuilder(string text, ChatId chatId)
    {
        _text = text;
        _chatId = chatId;
    }

    internal async Task SendMessage(IServiceProvider serviceProvider)
    {
        var clientProvider = serviceProvider.GetRequiredService<ITelegramBotClientProvider>();
        var client = clientProvider.GetClient();

        await client.SendTextMessageAsync(_chatId, _text);
    }
}
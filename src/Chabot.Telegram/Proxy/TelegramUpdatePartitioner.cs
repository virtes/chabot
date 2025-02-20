using Chabot.Proxy;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Proxy;

internal class TelegramUpdatePartitioner : IUpdatePartitioner<Update>
{
    public byte[] GetPartitionKey(Update update)
    {
        var chatId = update.Type switch
        {
            UpdateType.Message => update.Message!.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(update.Type), update.Type, "Unsupported update type")
        };

        return BitConverter.GetBytes(chatId);
    }
}
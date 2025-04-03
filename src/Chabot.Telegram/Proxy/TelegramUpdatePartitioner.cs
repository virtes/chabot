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
            UpdateType.CallbackQuery => update.CallbackQuery!.Message?.Chat.Id,
            UpdateType.Unknown => null,
            UpdateType.InlineQuery => update.InlineQuery!.From.Id,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult!.From.Id,
            UpdateType.EditedMessage => update.EditedMessage!.From?.Id,
            UpdateType.ChannelPost => update.ChannelPost!.Chat.Id,
            UpdateType.EditedChannelPost => update.EditedChannelPost!.Chat.Id,
            UpdateType.ShippingQuery => update.ShippingQuery!.From.Id,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery!.From.Id,
            UpdateType.Poll => null,
            UpdateType.PollAnswer => null,
            UpdateType.MyChatMember => update.MyChatMember!.Chat.Id,
            UpdateType.ChatMember => update.ChatMember!.Chat.Id,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest!.Chat.Id,
            UpdateType.MessageReaction => update.MessageReaction!.Chat.Id,
            UpdateType.MessageReactionCount => update.MessageReactionCount!.Chat.Id,
            UpdateType.ChatBoost => update.ChatBoost!.Chat.Id,
            UpdateType.RemovedChatBoost => update.RemovedChatBoost!.Chat.Id,
            UpdateType.BusinessConnection => update.BusinessConnection!.User.Id,
            UpdateType.BusinessMessage => update.BusinessMessage!.Chat.Id,
            UpdateType.EditedBusinessMessage => update.EditedBusinessMessage!.Chat.Id,
            UpdateType.DeletedBusinessMessages => update.DeletedBusinessMessages!.Chat.Id,
            UpdateType.PurchasedPaidMedia => update.PurchasedPaidMedia!.From.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(update.Type), update.Type, "Unsupported update type")
        };

        if (chatId is null)
            return Array.Empty<byte>();

        return BitConverter.GetBytes(chatId.Value);
    }
}
using Chabot.Command;
using Telegram.Bot.Types.Enums;
using TelegramUpdate = Telegram.Bot.Types.Update;
using TelegramUser = Telegram.Bot.Types.User;

namespace Chabot.Telegram.Implementation;

public class TelegramActionSelectionMetadataFactory
    : IActionSelectionMetadataFactory<TelegramUpdate, TelegramUser>
{
    public ActionSelectionMetadata GetMetadata(TelegramUpdate message, TelegramUser user)
    {
        var commandText = message.Type switch
        {
            UpdateType.Message => message.Message?.Text ?? message.Message?.Caption,
            UpdateType.CallbackQuery => message.CallbackQuery?.Data,
            _ => throw new ArgumentOutOfRangeException(nameof(message.Type),
                message.Type, "Message type is not supported")
        };

        if (commandText is not null && commandText.StartsWith('/'))
            commandText = commandText.Substring(1, commandText.Length - 1);

        return new ActionSelectionMetadata(commandText: commandText);
    }
}
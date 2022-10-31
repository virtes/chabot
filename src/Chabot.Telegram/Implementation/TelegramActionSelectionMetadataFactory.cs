using Chabot.Command;
using TelegramMessage = global::Telegram.Bot.Types.Message;
using TelegramUser = global::Telegram.Bot.Types.User;

namespace Chabot.Telegram.Implementation;

public class TelegramActionSelectionMetadataFactory
    : IActionSelectionMetadataFactory<TelegramMessage, TelegramUser>
{
    public ActionSelectionMetadata GetMetadata(TelegramMessage message, TelegramUser user)
    {
        var commandText = message.Text;

        if (commandText is not null && commandText.StartsWith('/'))
            commandText = commandText.Substring(1, commandText.Length - 1);

        return new ActionSelectionMetadata(commandText: commandText);
    }
}
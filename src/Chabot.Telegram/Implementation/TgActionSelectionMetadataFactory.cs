using Chabot.Command;

namespace Chabot.Telegram.Implementation;

public class TgActionSelectionMetadataFactory
    : IActionSelectionMetadataFactory<TgMessage, TgUser, long>
{
    public ActionSelectionMetadata GetMetadata(TgMessage message, TgUser user)
    {
        var commandText = message.Text;

        if (commandText is not null && commandText.StartsWith('/'))
            commandText = commandText.Substring(1, commandText.Length - 1);

        return new ActionSelectionMetadata(commandText: commandText);
    }
}
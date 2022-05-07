using Chabot.Command;

namespace Chabot.Telegram.Implementation;

public class TgActionSelectionMetadataFactory
    : IActionSelectionMetadataFactory<TgMessage, TgUser, long>
{
    public ActionSelectionMetadata GetMetadata(TgMessage message, TgUser user)
    {
        return new ActionSelectionMetadata(commandText: message.Text);
    }
}
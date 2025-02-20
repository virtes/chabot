using Chabot.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal class TelegramUpdateMetadataParser : IUpdateMetadataParser<Update>
{
    public UpdateMetadata ParseUpdateMetadata(Update update)
    {
        var properties = new Dictionary<string, string?>();

        if (update.Type == UpdateType.Message)
            properties[UpdateProperties.MessageText] = update.Message!.Text;

        if (update.Type == UpdateType.InlineQuery)
            properties[TelegramUpdateProperties.InlineQuery] = update.InlineQuery!.Query;

        return new UpdateMetadata(properties);
    }
}
using Chabot.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal class AllowedMessageTypeRestrictionHandler
    : ICommandRestrictionHandler<Update, AllowedMessageTypeRestriction>
{
    public ValueTask<bool> IsAllowed(Update update, UpdateMetadata updateMetadata,
        AllowedMessageTypeRestriction restriction)
    {
        if (update.Type != UpdateType.Message)
            return ValueTask.FromResult(false);

        return ValueTask.FromResult(restriction.MessageTypes.Contains(update.Message!.Type));
    }
}
using Chabot.Commands;
using Telegram.Bot.Types;

namespace Chabot.Telegram.Commands;

internal class AllowedUpdateTypeRestrictionsHandler
    : ICommandRestrictionHandler<Update, AllowedUpdateTypeRestriction>
{
    public ValueTask<bool> IsAllowed(Update update,
        UpdateMetadata updateMetadata, AllowedUpdateTypeRestriction restriction)
    {
        return ValueTask.FromResult<bool>(restriction.UpdateTypes.Contains(update.Type));
    }
}
using Chabot.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal class AllowedCallbackQueryPayloadRestrictionHandler
    : ICommandRestrictionHandler<Update, AllowedCallbackQueryPayloadRestriction>
{
    public ValueTask<bool> IsAllowed(Update update, UpdateMetadata updateMetadata,
        AllowedCallbackQueryPayloadRestriction restriction)
    {
        if (update.Type != UpdateType.CallbackQuery)
            return ValueTask.FromResult(false);

        var payload = restriction.UseQueryParameters
            ? update.CallbackQuery!.Data!.Split('?', StringSplitOptions.RemoveEmptyEntries).First()
            : update.CallbackQuery!.Data;

        return ValueTask.FromResult(restriction.AllowedPayloads.Contains(payload));
    }
}
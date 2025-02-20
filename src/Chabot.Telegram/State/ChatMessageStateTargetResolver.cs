using Chabot.State;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.State;

internal class ChatMessageStateTargetResolver : IStateTargetResolver<Update>
{
    public bool TryResolveStateTarget(Update update, out IStateTarget stateTarget)
    {
        if (update.Type == UpdateType.Message)
        {
            stateTarget = new ChatMessageStateTarget(update.Message!.Chat.Id, update.Message!.MessageId);
            return true;
        }

        if (update.Type == UpdateType.CallbackQuery)
        {
            stateTarget = new ChatMessageStateTarget(update.CallbackQuery!.Message!.Chat.Id, update.CallbackQuery.Message.MessageId);
            return true;
        }

        stateTarget = null!;
        return false;
    }
}
using Chabot.State;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.State;

internal class ChatStateTargetResolver : IStateTargetResolver<Update>
{
    public bool TryResolveStateTarget(Update update, out IStateTarget stateTarget)
    {
        var chatId = update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id,
            _ => null
        };

        if (chatId is null)
        {
            stateTarget = null!;
            return false;
        }

        stateTarget = new ChatStateTarget(chatId.Value);
        return true;
    }
}
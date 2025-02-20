using System.Reflection;
using Chabot.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal class FromChatIdParameterValueResolver : ICommandParameterValueResolver<Update>
{
    public ValueTask<object?> ResolveParameterValue(ParameterInfo parameterInfo, UpdateContext<Update> updateContext)
    {
        if (updateContext.Update.Type == UpdateType.Message)
            return ValueTask.FromResult<object?>(updateContext.Update.Message!.Chat.Id);

        if (updateContext.Update.Type == UpdateType.CallbackQuery)
            return ValueTask.FromResult<object?>(updateContext.Update.CallbackQuery!.Message!.Chat.Id);

        return ValueTask.FromResult<object?>(0L);
    }
}
using Chabot.Command;

namespace Chabot.Telegram;

public abstract class TgCommandGroup : CommandGroupBase<TgMessage, TgUser, long>
{
    protected async Task SendTextMessage(string text,
        Action<TextMessageBuilder>? messageBuilderAction = null)
    {
        var messageBuilder = new TextMessageBuilder(text, Context.User.Id);
        messageBuilderAction?.Invoke(messageBuilder);

        await messageBuilder.SendMessage(Context.Services);
    }
}
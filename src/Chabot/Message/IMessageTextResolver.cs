namespace Chabot.Message;

public interface IMessageTextResolver<in TMessage>
{
    string? GetMessageText(TMessage message);
}
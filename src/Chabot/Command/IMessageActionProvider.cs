namespace Chabot.Command;

public interface IMessageActionProvider<TMessage, TUser>
{
    IMessageAction<TMessage, TUser>? GetMessageAction(
        ActionSelectionMetadata actionSelectionMetadata, Type stateType);
}
using Chabot.Message;
using Chabot.State;
using Chabot.User;

namespace Chabot.Command;

public interface IMessageActionProvider<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    IMessageAction<TMessage, TUser, TUserId>? GetMessageAction(
        ActionSelectionMetadata actionSelectionMetadata,
        IState? state);
}
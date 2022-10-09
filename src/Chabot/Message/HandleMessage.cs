using Chabot.User;

namespace Chabot.Message;

public delegate Task HandleMessage<TMessage, TUser, TUserId>(
    MessageContext<TMessage, TUser, TUserId> context)
    where TUser : IUser<TUserId>
    where TMessage : IMessage;
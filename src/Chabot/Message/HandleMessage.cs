namespace Chabot.Message;

public delegate Task HandleMessage<TMessage, TUser>(
    MessageContext<TMessage, TUser> context);
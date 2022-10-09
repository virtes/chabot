using Chabot.Message;
using Chabot.User;
using Microsoft.Extensions.Logging;

namespace Chabot.Command.Implementation;

public class CommandActionInvokerMiddleware<TMessage, TUser, TUserId> 
    : IMiddleware<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    private readonly IActionSelectionMetadataFactory<TMessage, TUser, TUserId> 
        _actionSelectionMetadataFactory;
    private readonly IMessageActionProvider<TMessage, TUser, TUserId> _messageActionProvider;
    private readonly ILogger _logger;

    public CommandActionInvokerMiddleware(
        IActionSelectionMetadataFactory<TMessage, TUser, TUserId> 
            actionSelectionMetadataFactory,
        IMessageActionProvider<TMessage, TUser, TUserId> messageActionProvider,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ILogger<CommandActionInvokerMiddleware<TMessage, TUser, TUserId>> logger)
    {
        _actionSelectionMetadataFactory = actionSelectionMetadataFactory;
        _messageActionProvider = messageActionProvider;
        _logger = logger;
    }
    
    public async Task Invoke(MessageContext<TMessage, TUser, TUserId> messageContext, 
        HandleMessage<TMessage, TUser, TUserId> next)
    {
        var actionSelectionMetadata = _actionSelectionMetadataFactory.GetMetadata(
            message: messageContext.Message,
            user: messageContext.User);

        var messageAction = _messageActionProvider.GetMessageAction(
            actionSelectionMetadata: actionSelectionMetadata,
            stateType: messageContext.UserState.State.GetType());

        if (messageAction is null)
        {
            _logger.LogError("Could not find message action");
            throw new InvalidOperationException("Message action not found");
        }

        await messageAction.Execute(messageContext);

        await next(messageContext);
    }
}
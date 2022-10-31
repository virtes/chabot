using Chabot.Message;
using Microsoft.Extensions.Logging;

namespace Chabot.Command.Implementation;

public class CommandActionInvokerMiddleware<TMessage, TUser>
    : IMiddleware<TMessage, TUser>
{
    private readonly IActionSelectionMetadataFactory<TMessage, TUser>
        _actionSelectionMetadataFactory;
    private readonly IMessageActionProvider<TMessage, TUser> _messageActionProvider;
    private readonly ILogger _logger;

    public CommandActionInvokerMiddleware(
        IActionSelectionMetadataFactory<TMessage, TUser> actionSelectionMetadataFactory,
        IMessageActionProvider<TMessage, TUser> messageActionProvider,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ILogger<CommandActionInvokerMiddleware<TMessage, TUser>> logger)
    {
        _actionSelectionMetadataFactory = actionSelectionMetadataFactory;
        _messageActionProvider = messageActionProvider;
        _logger = logger;
    }
    
    public async Task Invoke(MessageContext<TMessage, TUser> messageContext,
        HandleMessage<TMessage, TUser> next)
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
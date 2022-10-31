using Chabot.Message;

namespace Chabot.State.Implementation;

public class StateExtractorMiddleware<TMessage, TUser>
    : IMiddleware<TMessage, TUser>
{
    private readonly IStateReader<TMessage, TUser> _stateReader;

    public StateExtractorMiddleware(IStateReader<TMessage, TUser> stateReader)
    {
        _stateReader = stateReader;
    }

    public async Task Invoke(MessageContext<TMessage, TUser> messageContext,
        HandleMessage<TMessage, TUser> next)
    {
        var userState = await _stateReader.ReadState(
            message: messageContext.Message,
            user: messageContext.User);

        messageContext.UserState = userState;

        await next(messageContext);
    }
}
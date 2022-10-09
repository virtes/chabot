using Chabot.Message;
using Chabot.User;

namespace Chabot.State.Implementation;

public class StateExtractorMiddleware<TMessage, TUser, TUserId>
    : IMiddleware<TMessage, TUser, TUserId> 
    where TMessage : IMessage
    where TUser : IUser<TUserId> 
{
    private readonly IStateReader<TUserId> _stateReader;

    public StateExtractorMiddleware(IStateReader<TUserId> stateReader)
    {
        _stateReader = stateReader;
    }

    public async Task Invoke(MessageContext<TMessage, TUser, TUserId> messageContext, 
        HandleMessage<TMessage, TUser, TUserId> next)
    {
        var userState = await _stateReader.ReadState(messageContext.User.Id);
        messageContext.UserState = userState;

        await next(messageContext);
    }
}
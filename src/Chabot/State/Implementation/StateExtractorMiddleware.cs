using Chabot.Message;

namespace Chabot.State.Implementation;

public class StateExtractorMiddleware<TMessage, TUser, TStateTarget>
    : IMiddleware<TMessage, TUser>
{
    private readonly IStateReader<TMessage, TUser, TStateTarget> _stateReader;
    private readonly IStateTargetFactory<TMessage, TUser, TStateTarget> _stateTargetFactory;

    public StateExtractorMiddleware(
        IStateReader<TMessage, TUser, TStateTarget> stateReader,
        IStateTargetFactory<TMessage, TUser, TStateTarget> stateTargetFactory)
    {
        _stateReader = stateReader;
        _stateTargetFactory = stateTargetFactory;
    }

    public async Task Invoke(MessageContext<TMessage, TUser> messageContext,
        HandleMessage<TMessage, TUser> next)
    {
        var stateTarget = _stateTargetFactory.GetStateTarget(
            message: messageContext.Message,
            user: messageContext.User);

        var userState = await _stateReader.ReadState(
            message: messageContext.Message,
            user: messageContext.User,
            stateTarget: stateTarget);
        messageContext.UserState = userState;

        await next(messageContext);
    }
}
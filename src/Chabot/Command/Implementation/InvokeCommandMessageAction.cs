using Chabot.Message;
using Chabot.State;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Command.Implementation;

public class InvokeCommandMessageAction<TMessage, TUser, TUserId>
    : IMessageAction<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId> 
{
    private readonly Type _commandGroupType;
    private readonly Func<CommandGroupBase<TMessage, TUser, TUserId>, IState?, Task> _action;

    public InvokeCommandMessageAction(Type commandGroupType, 
        Func<CommandGroupBase<TMessage, TUser, TUserId>, IState?, Task> action)
    {
        _commandGroupType = commandGroupType;
        _action = action;
    }
    
    public async Task Execute(MessageContext<TMessage, TUser, TUserId> messageContext)
    {
        var instance = (CommandGroupBase<TMessage, TUser, TUserId>)messageContext.Services.GetRequiredService(_commandGroupType);

        instance.Context = messageContext;
        
        await _action(instance, messageContext.UserState?.State);
    }
}
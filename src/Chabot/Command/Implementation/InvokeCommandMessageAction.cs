using Chabot.Message;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Command.Implementation;

public class InvokeCommandMessageAction<TMessage, TUser>
    : IMessageAction<TMessage, TUser>
{
    private readonly Type _commandGroupType;
    private readonly Func<CommandGroupBase<TMessage, TUser>, MessageContext<TMessage, TUser>, Task> _action;

    public InvokeCommandMessageAction(Type commandGroupType, 
        Func<CommandGroupBase<TMessage, TUser>, MessageContext<TMessage, TUser>, Task> action)
    {
        _commandGroupType = commandGroupType;
        _action = action;
    }
    
    public async Task Execute(MessageContext<TMessage, TUser> messageContext)
    {
        var instance = (CommandGroupBase<TMessage, TUser>)
            messageContext.Services.GetRequiredService(_commandGroupType);

        instance.Context = messageContext;
        
        await _action(instance, messageContext);
    }
}
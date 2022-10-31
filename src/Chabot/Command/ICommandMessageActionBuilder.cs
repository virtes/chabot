using System.Reflection;
using Chabot.Message;

namespace Chabot.Command;

public interface ICommandMessageActionBuilder<TMessage, TUser>
{
    Func<CommandGroupBase<TMessage, TUser>, MessageContext<TMessage, TUser>, Task>
        BuildInvokeCommand(Type type, MethodInfo method);
}
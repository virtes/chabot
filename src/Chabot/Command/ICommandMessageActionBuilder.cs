using System.Reflection;
using Chabot.Message;
using Chabot.User;

namespace Chabot.Command;

public interface ICommandMessageActionBuilder<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    Func<CommandGroupBase<TMessage, TUser, TUserId>, MessageContext<TMessage, TUser, TUserId>, Task>
        BuildInvokeCommand(Type type, MethodInfo method);
}
using System.Reflection;
using Chabot.Message;
using Chabot.State;
using Chabot.User;

namespace Chabot.Command;

public interface ICommandMessageActionBuilder
{
    Func<CommandGroupBase<TMessage, TUser, TUserId>, IState?, Task> BuildInvokeCommand
        <TMessage, TUser, TUserId>(
            Type type, MethodInfo method, Type? stateType)
        where TMessage : IMessage
        where TUser : IUser<TUserId>;
}
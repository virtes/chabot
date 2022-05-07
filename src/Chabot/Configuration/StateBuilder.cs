using Chabot.Message;
using Chabot.User;

namespace Chabot.Configuration;

public class StateBuilder<TMessage, TUser, TUserId, TSerializedState>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    public ChabotBuilder<TMessage, TUser, TUserId> ChabotBuilder { get; }

    public StateBuilder(
        ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder)
    {
        ChabotBuilder = chabotBuilder;
    }
}
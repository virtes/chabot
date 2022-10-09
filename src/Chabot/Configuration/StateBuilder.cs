using System.Reflection;
using Chabot.Message;
using Chabot.State.Configuration;
using Chabot.User;
using Microsoft.Extensions.Options;

namespace Chabot.Configuration;

public class StateBuilder<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    internal readonly OptionsBuilder<StateOptions> OptionsBuilder;

    public ChabotBuilder<TMessage, TUser, TUserId> ChabotBuilder { get; }

    public StateBuilder(
        ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder,
        OptionsBuilder<StateOptions> optionsBuilder)
    {
        OptionsBuilder = optionsBuilder;
        ChabotBuilder = chabotBuilder;
    }

    public StateBuilder<TMessage, TUser, TUserId> ScanStateTypes(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            OptionsBuilder.Configure(o => o.AssembliesToScanStateTypes.Add(assembly));
        }

        return this;
    }
}

public class StateBuilder<TMessage, TUser, TUserId, TSerializedState>
    : StateBuilder<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    public StateBuilder(ChabotBuilder<TMessage, TUser, TUserId> chabotBuilder,
        OptionsBuilder<StateOptions> optionsBuilder)
        : base(chabotBuilder, optionsBuilder)
    {
    }
}
using System.Reflection;
using Chabot.State.Configuration;
using Microsoft.Extensions.Options;

namespace Chabot.Configuration;

public class StateBuilder<TMessage, TUser>
{
    internal readonly OptionsBuilder<StateOptions> OptionsBuilder;

    public ChabotBuilder<TMessage, TUser> ChabotBuilder { get; }

    public StateBuilder(
        ChabotBuilder<TMessage, TUser> chabotBuilder,
        OptionsBuilder<StateOptions> optionsBuilder)
    {
        OptionsBuilder = optionsBuilder;
        ChabotBuilder = chabotBuilder;
    }

    public StateBuilder<TMessage, TUser> ScanStateTypes(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            OptionsBuilder.Configure(o => o.AssembliesToScanStateTypes.Add(assembly));
        }

        return this;
    }
}

public class StateBuilder<TMessage, TUser, TSerializedState>
    : StateBuilder<TMessage, TUser>
{
    public StateBuilder(ChabotBuilder<TMessage, TUser> chabotBuilder,
        OptionsBuilder<StateOptions> optionsBuilder)
        : base(chabotBuilder, optionsBuilder)
    {
    }
}
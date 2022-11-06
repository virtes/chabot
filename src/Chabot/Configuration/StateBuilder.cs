using System.Reflection;
using Chabot.State.Configuration;
using Microsoft.Extensions.Options;

namespace Chabot.Configuration;

public class StateBuilder<TMessage, TUser, TStateTarget>
{
    internal readonly OptionsBuilder<StateOptions> OptionsBuilder;

    public ChabotBuilder<TMessage, TUser, TStateTarget> ChabotBuilder { get; }

    public StateBuilder(
        ChabotBuilder<TMessage, TUser, TStateTarget> chabotBuilder,
        OptionsBuilder<StateOptions> optionsBuilder)
    {
        OptionsBuilder = optionsBuilder;
        ChabotBuilder = chabotBuilder;
    }

    public StateBuilder<TMessage, TUser, TStateTarget> ScanStateTypes(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            OptionsBuilder.Configure(o => o.AssembliesToScanStateTypes.Add(assembly));
        }

        return this;
    }
}

public class StateBuilder<TMessage, TUser, TStateTarget, TSerializedState>
    : StateBuilder<TMessage, TUser, TStateTarget>
{
    public StateBuilder(ChabotBuilder<TMessage, TUser, TStateTarget> chabotBuilder,
        OptionsBuilder<StateOptions> optionsBuilder)
        : base(chabotBuilder, optionsBuilder)
    {
    }
}
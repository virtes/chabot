using Chabot.Configuration;
using Chabot.InMemoryState.Implementation;
using Chabot.State;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static class StateBuilderExtensions
{
    public static StateBuilder<TMessage, TUser, TStateTarget, TSerializedState>
        UseInMemoryStateStorage<TMessage, TUser, TStateTarget, TSerializedState>(
        this StateBuilder<TMessage, TUser, TStateTarget, TSerializedState> stateBuilder)
        where TStateTarget : IEquatable<TStateTarget>
    {
        stateBuilder.ChabotBuilder.Services
            .AddSingleton<IStateStorage<TStateTarget, TSerializedState>,
                InMemoryStateStorage<TStateTarget, TSerializedState>>();
        
        return stateBuilder;
    }
}
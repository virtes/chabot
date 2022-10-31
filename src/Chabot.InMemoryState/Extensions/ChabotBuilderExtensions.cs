using Chabot.Configuration;
using Chabot.InMemoryState.Implementation;
using Chabot.State;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static class StateBuilderExtensions
{
    public static StateBuilder<TMessage, TUser, TSerializedState>
        UseInMemoryStateStorage<TMessage, TUser, TSerializedState, TKey>(
        this StateBuilder<TMessage, TUser, TSerializedState> stateBuilder,
        Func<TMessage, TUser, TKey> keyFactory)
        where TKey : IEquatable<TKey>
    {
        stateBuilder.ChabotBuilder.Services
            .AddSingleton<IStateStorage<TMessage, TUser, TSerializedState>>(
                new InMemoryStateStorage<TMessage, TUser, TKey, TSerializedState>(keyFactory));
        
        return stateBuilder;
    }
}
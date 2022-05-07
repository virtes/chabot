using Chabot.Configuration;
using Chabot.InMemoryState.Implementation;
using Chabot.Message;
using Chabot.State;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static class StateBuilderExtensions
{
    public static StateBuilder<TMessage, TUser, TUserId, TSerializedState> 
        UseInMemoryStateStorage<TMessage, TUser, TUserId, TSerializedState>(
        this StateBuilder<TMessage, TUser, TUserId, TSerializedState> stateBuilder)
        where TMessage : IMessage
        where TUser : IUser<TUserId>
        where TUserId : IEquatable<TUserId>
    {
        stateBuilder.ChabotBuilder.Services.AddSingleton<
            IStateStorage<TUserId, TSerializedState>,
            InMemoryStateStorage<TUserId, TSerializedState>>();
        
        return stateBuilder;
    }
}
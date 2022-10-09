using Chabot.Message;
using Chabot.State;
using Chabot.User;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Command;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public abstract class CommandGroupBase<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    // ReSharper disable once MemberCanBeProtected.Global
    public MessageContext<TMessage, TUser, TUserId> Context { get; set; } = default!;

    protected async Task SetState(IState state)
    {   
        var stateWriter = Context.Services.GetRequiredService<IStateWriter<TUserId>>();
        Context.UserState = await stateWriter.WriteState(state, Context.User.Id);
    }
}
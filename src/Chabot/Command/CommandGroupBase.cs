using Chabot.Message;
using Chabot.State;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Command;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public abstract class CommandGroupBase<TMessage, TUser>
{
    // ReSharper disable once MemberCanBeProtected.Global
    public MessageContext<TMessage, TUser> Context { get; set; } = default!;

    protected TMessage Message => Context.Message;

    protected TUser User => Context.User;

    protected async Task SetState(IState state)
    {   
        var stateWriter = Context.Services.GetRequiredService<IStateWriter<TMessage, TUser>>();
        Context.UserState = await stateWriter.WriteState(state, Message, User);
    }
}
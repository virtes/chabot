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
}

public abstract class CommandGroupBase<TMessage, TUser, TStateTarget>
    : CommandGroupBase<TMessage, TUser>
{
    protected async Task SetState(IState state)
    {
        var stateTargetFactory = Context.Services
            .GetRequiredService<IStateTargetFactory<TMessage, TUser, TStateTarget>>();
        var stateTarget = stateTargetFactory.GetStateTarget(Context.Message, Context.User);

        await SetState(state, stateTarget);
    }

    protected async Task SetState(IState state, TStateTarget stateTarget)
    {
        var stateWriter = Context.Services
            .GetRequiredService<IStateWriter<TStateTarget>>();

        Context.UserState = await stateWriter.WriteState(state, stateTarget);
    }
}
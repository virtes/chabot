using JetBrains.Annotations;

namespace Chabot.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public abstract class CommandsBase<TUpdate> : CommandsBase
{
    public UpdateContext<TUpdate> Context { get; internal set; } = default!;
}

public abstract class CommandsBase
{
}
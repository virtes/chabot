using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chabot.State;

public static class UpdateContextExtensions
{
    public static async ValueTask SetState(this UpdateContext updateContext,
        IStateTarget stateTarget, object? state)
    {
        var stateAccessor = updateContext.ServiceProvider.GetRequiredService<IStateAccessor>();
        await stateAccessor.SetState(stateTarget, state);
    }
}
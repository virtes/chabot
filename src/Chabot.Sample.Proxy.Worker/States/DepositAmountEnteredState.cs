using Chabot.State;

namespace Chabot.Sample.Proxy.Worker.States;

public class DepositAmountEnteredState : IState
{
    public decimal Amount { get; set; }
}
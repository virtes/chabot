using Chabot.State;

namespace Chabot.Sample.Telegram.States;

public class DepositAmountEnteredState : IState
{
    public decimal Amount { get; set; }
}
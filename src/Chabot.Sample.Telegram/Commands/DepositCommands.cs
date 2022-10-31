using Chabot.Command;
using Chabot.Sample.Telegram.States;
using Chabot.Telegram;
using Telegram.Bot;

namespace Chabot.Sample.Telegram.Commands;

public class DepositCommands : TelegramCommandGroup
{
    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(DefaultState))]
    public async Task Hello()
    {
        await BotClient.SendTextMessageAsync(ChatId,
            $"Command {MessageText} not found", replyToMessageId: MessageId);
    }

    [Command("deposit")]
    [AllowedState(typeof(DefaultState))]
    public async Task Text()
    {
        await BotClient.SendTextMessageAsync(ChatId, "Enter amount");

        await SetState(new EnterDepositAmountState());
    }

    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(EnterDepositAmountState))]
    public async Task EnterDepositAmount(
        [FromMessageText]string messageText)
    {
        if (!decimal.TryParse(messageText, out var amount))
        {
            await BotClient.SendTextMessageAsync(ChatId,
                "Invalid amount, please enter a digit", replyToMessageId: MessageId);
            return;
        }

        await BotClient.SendTextMessageAsync(ChatId,
            $"Confirm deposit {amount} - /confirm");

        await SetState(new DepositAmountEnteredState
        {
            Amount = amount
        });
    }

    [Command("confirm")]
    [AllowedState(typeof(DepositAmountEnteredState))]
    public async Task ConfirmDeposit(
        DepositAmountEnteredState depositAmountEntered)
    {
        await BotClient.SendTextMessageAsync(ChatId,
            $"Deposit {depositAmountEntered.Amount} confirmed");

        await SetState(DefaultState.Instance);
    }

    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(DepositAmountEnteredState))]
    public async Task ConfirmDepositInvalidCommand()
    {
        await BotClient.SendTextMessageAsync(ChatId,
            "Invalid action - type /confirm");
    }
}
using Chabot.Sample.Proxy.Receiver.States;
using Chabot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Chabot.Sample.Proxy.Receiver.Commands;

public class DepositCommands : TelegramCommands
{
    [Order(-1000)]
    [AllowedChatState<MenuState>(AllowEmptyState = true)]
    public async Task Hello([FromChatId]long chatId)
    {
        await BotClient.SendTextMessageAsync(chatId, "Hello!");

        await Context.SetCurrentChatState(new MenuState());
    }

    [AllowedMessageText("/create_deposit")]
    [AllowedChatState<MenuState>]
    public async Task Text()
    {
        await BotClient.SendTextMessageAsync(ChatId, "Enter amount", replyToMessageId: MessageId);

        await Context.SetCurrentChatState(new AwaitingForDepositAmountState());
    }

    [AllowedChatState<AwaitingForDepositAmountState>]
    public async Task EnterDepositAmount(
        [FromMessageText]string messageText)
    {
        if (!decimal.TryParse(messageText, out var amount))
        {
            await BotClient.SendTextMessageAsync(ChatId,
                "Invalid amount, please enter a digit", replyToMessageId: MessageId);
            return;
        }

        var message = await BotClient.SendTextMessageAsync(ChatId,
            $"Confirm deposit {amount}",
            replyMarkup: new InlineKeyboardMarkup(new []
            {
                InlineKeyboardButton.WithCallbackData("Approve", "approve"),
                InlineKeyboardButton.WithCallbackData("Decline", "decline")
            }));

        await Context.SetChatMessageState(message, new AwaitingForDepositApprove
        {
            Amount = amount
        });
        await Context.SetCurrentChatState(new MenuState());
    }

    [AllowedCallbackQueryPayload("approve")]
    [AllowedChatMessageState<AwaitingForDepositApprove>]
    public async Task ApproveDeposit(
        [FromChatMessageState]AwaitingForDepositApprove awaitingForDepositAmountEntered)
    {
        await BotClient.SendTextMessageAsync(ChatId,
            $"Deposit {awaitingForDepositAmountEntered.Amount} approved");

        await Context.SetChatMessageState(ChatId, MessageId, new MenuState());
    }

    [AllowedCallbackQueryPayload("decline")]
    [AllowedChatMessageState<AwaitingForDepositApprove>]
    public async Task DeclineDeposit(
        [FromChatMessageState]AwaitingForDepositApprove awaitingForDepositAmountEntered)
    {
        await BotClient.SendTextMessageAsync(ChatId,
            $"Deposit {awaitingForDepositAmountEntered.Amount} declined");

        await Context.SetChatMessageState(ChatId, MessageId, null);
    }
}
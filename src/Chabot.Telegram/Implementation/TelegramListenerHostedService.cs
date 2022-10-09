using System.Diagnostics;
using Chabot.Message;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Implementation;

public class TelegramListenerHostedService : IHostedService
{
    private readonly ITelegramBotClientProvider _telegramBotClientProvider;
    private readonly ILogger<TelegramListenerHostedService> _logger;
    private readonly IMessageHandler<TgMessage, TgUser, long> _messageHandler;

    public TelegramListenerHostedService(
        ITelegramBotClientProvider telegramBotClientProvider,
        ILogger<TelegramListenerHostedService> logger,
        IMessageHandler<TgMessage, TgUser, long> messageHandler)
    {
        _telegramBotClientProvider = telegramBotClientProvider;
        _logger = logger;
        _messageHandler = messageHandler;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var client = _telegramBotClientProvider.GetClient();

        client.StartReceiving(
            UpdateHandler,
            ErrorHandler,
            new ReceiverOptions(),
            cancellationToken);

        _logger.LogInformation("Telegram poll listener started");

        return Task.CompletedTask;
    }

    private async Task UpdateHandler(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        using var activity = new Activity("Handle telegram message");
        activity.Start();

        _logger.LogTrace("Received telegram update {@Update}", update);

        if (update.Type != UpdateType.Message)
        {
            _logger.LogInformation("Telegram update skipped (update type is {UpdateType})", update.Type);
            return;
        }

        if (update.Message is null)
        {
            _logger.LogWarning("Telegram update skipped (message is null)");
            return;
        }

        if (update.Message.From is null)
        {
            _logger.LogWarning("Telegram update skipped (from is null)");
            return;
        }

        var telegramMessage = new TgMessage
        {
            Id = update.Message.MessageId,
            Text = update.Message.Text
        };

        var telegramUser = new TgUser
        {
            Id = update.Message.From.Id, 
            Username = update.Message.From.Username, 
            IsBot = update.Message.From.IsBot
        };

        try
        {
            await _messageHandler.HandleMessage(telegramMessage, telegramUser);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception while handling telegram message {@Message} from {@User}",
                telegramMessage, telegramUser);
            activity.SetException(e);
            throw;
        }
    }

    private Task ErrorHandler(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram unhandled exception");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Chabot.Telegram;

internal class TelegramLongPollingListenerHostedService : IHostedService
{
    private readonly ITelegramBotClientProvider _telegramBotClientProvider;
    private readonly ILogger<TelegramLongPollingListenerHostedService> _logger;
    private readonly IChabot<Update> _chabot;

    public TelegramLongPollingListenerHostedService(
        ILogger<TelegramLongPollingListenerHostedService> logger,
        IChabot<Update> chabot,
        ITelegramBotClientProvider telegramBotClientProvider)
    {
        _telegramBotClientProvider = telegramBotClientProvider;
        _logger = logger;
        _chabot = chabot;
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

    private async Task UpdateHandler(ITelegramBotClient telegramBotClient,
        Update update, CancellationToken cancellationToken)
    {
        using var activity = ChabotActivitySource.ActivitySource.StartActivity();
        _logger.LogTrace("Received telegram update {@Update}", update);

        try
        {
            await _chabot.HandleUpdate(update);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception while handling telegram update {@Update}", update);
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            throw;
        }
    }

    private Task ErrorHandler(ITelegramBotClient telegramBotClient,
        Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram unhandled exception");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
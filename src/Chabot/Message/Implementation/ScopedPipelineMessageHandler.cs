using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chabot.Message.Implementation;

public class ScopedPipelineMessageHandler<TMessage, TUser>
    : IMessageHandler<TMessage, TUser>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ScopedPipelineMessageHandler<TMessage, TUser>> _logger;
    private readonly HandleMessage<TMessage, TUser> _handleMessage;

    public ScopedPipelineMessageHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ScopedPipelineMessageHandler<TMessage, TUser>> logger,
        HandleMessage<TMessage, TUser> handleMessage)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _handleMessage = handleMessage;
    }
    
    public async Task HandleMessage(TMessage message, TUser user)
    {
        using var activity = new Activity("Handle message");
        activity.Start();

        try
        {
            var sw = Stopwatch.StartNew();
            _logger.LogTrace("Handling message");

            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var messageContext = new MessageContext<TMessage, TUser>(
                services: scope.ServiceProvider,
                message: message,
                user: user);
            await _handleMessage(messageContext);
            
            _logger.LogInformation("Message handled in {ElapsedMs} ms", sw.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception while handling message ({@Message})", message);
            activity.SetException(e);
            throw;
        }
    }
}
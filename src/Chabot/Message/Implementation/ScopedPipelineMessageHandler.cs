using System.Diagnostics;
using Chabot.Telemetry;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chabot.Message.Implementation;

public class ScopedPipelineMessageHandler<TMessage, TUser, TUserId> 
    : IMessageHandler<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
    where TMessage : IMessage
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ScopedPipelineMessageHandler<TMessage, TUser, TUserId>> _logger;
    private readonly HandleMessage<TMessage, TUser, TUserId> _handleMessage;

    public ScopedPipelineMessageHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ScopedPipelineMessageHandler<TMessage, TUser, TUserId>> logger,
        HandleMessage<TMessage, TUser, TUserId> handleMessage)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _handleMessage = handleMessage;
    }
    
    public async Task HandleMessage(TMessage message, TUser user)
    {
        using var activity = ChabotActivitySource.StartActivity();

        try
        {
            var sw = Stopwatch.StartNew();
            _logger.LogTrace("Handling message from {UserId}", user.Id);

            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var messageContext = new MessageContext<TMessage, TUser, TUserId>(
                services: scope.ServiceProvider,
                message: message,
                user: user);
            await _handleMessage(messageContext);
            
            _logger.LogInformation("Message from {UserId} user handled in {ElapsedMs} ms",
                user.Id, sw.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception while handling message ({@Message})", message);
            activity.SetException(e);
            throw;
        }
    }
}
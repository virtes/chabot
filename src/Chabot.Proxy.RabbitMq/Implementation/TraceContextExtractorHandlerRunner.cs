using System.Diagnostics;
using System.Text;
using EasyNetQ.Consumer;
using Microsoft.Extensions.Logging;

namespace Chabot.Proxy.RabbitMq.Implementation;

public class TraceContextExtractorHandlerRunner : IHandlerRunner
{
    private readonly ILogger<TraceContextExtractorHandlerRunner> _logger;
    private readonly IConsumerErrorStrategy _consumerErrorStrategy;

    public TraceContextExtractorHandlerRunner(
        ILogger<TraceContextExtractorHandlerRunner> logger,
        IConsumerErrorStrategy consumerErrorStrategy)
    {
        _logger = logger;
        _consumerErrorStrategy = consumerErrorStrategy;
    }

    public async Task<AckStrategy> InvokeUserMessageHandlerAsync(
        ConsumerExecutionContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using var activity = new Activity("Handle RabbitMQ message");

        if (context.Properties.Headers != null
            && context.Properties.Headers.TryGetValue(TelemetryHeaders.ParentId, out var parentIdBytesObject)
            && parentIdBytesObject is byte[] parentIdBytes)
        {
            var parentId = Encoding.UTF8.GetString(parentIdBytes);
            if (!string.IsNullOrEmpty(parentId))
                activity.SetParentId(parentId);
        }

        if (context.Properties.Headers != null
            && context.Properties.Headers.TryGetValue(TelemetryHeaders.TraceState, out var traceStateBytesObject)
            && traceStateBytesObject is byte[] traceStateBytes)
        {
            var traceState = Encoding.UTF8.GetString(traceStateBytes);
            if (!string.IsNullOrEmpty(traceState))
                activity.TraceStateString = traceState;
        }

        activity.Start();
        _logger.LogDebug("Received message ({@ReceivedInfo})", context.ReceivedInfo);

        try
        {
            try
            {
                return await context.Handler(context.Body, context.Properties, context.ReceivedInfo, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return await _consumerErrorStrategy.HandleConsumerCancelledAsync(context, cancellationToken);
            }
            catch (Exception exception)
            {
                return await _consumerErrorStrategy.HandleConsumerErrorAsync(context, exception, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Consumer error strategy has failed");
            activity.SetException(e);
            return AckStrategies.NackWithRequeue;
        }
    }

    public void Dispose()
    {
    }
}
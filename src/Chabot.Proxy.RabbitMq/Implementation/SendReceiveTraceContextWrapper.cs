using System.Diagnostics;
using EasyNetQ;
using EasyNetQ.Internals;

namespace Chabot.Proxy.RabbitMq.Implementation;

public class SendReceiveTraceContextWrapper : ISendReceive
{
    private readonly DefaultSendReceive _defaultSendReceive;

    public SendReceiveTraceContextWrapper(
        DefaultSendReceive defaultSendReceive)
    {
        _defaultSendReceive = defaultSendReceive;
    }

    public async Task SendAsync<T>(string queue, T message,
        Action<ISendConfiguration> configure,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var activity = Activity.Current;

        Action<ISendConfiguration>? newConfigure = null;

        if (activity is { IdFormat: ActivityIdFormat.W3C })
        {
            var parentId = activity.Id;
            var traceState = activity.TraceStateString;

            if (!string.IsNullOrEmpty(parentId))
            {
                newConfigure = c =>
                {
                    var headers = new Dictionary<string, object>
                    {
                        [TelemetryHeaders.ParentId] = parentId
                    };

                    if (!string.IsNullOrEmpty(traceState))
                        headers[TelemetryHeaders.TraceState] = traceState;

                    c.WithHeaders(headers);
                    configure(c);
                };
            }
        }

        await _defaultSendReceive.SendAsync(queue, message, newConfigure ?? configure, cancellationToken);
    }

    public AwaitableDisposable<IDisposable> ReceiveAsync(string queue,
        Action<IReceiveRegistration> addHandlers, Action<IReceiveConfiguration> configure,
        CancellationToken cancellationToken)
    {
        return _defaultSendReceive.ReceiveAsync(queue,
            addHandlers, configure, cancellationToken);
    }
}
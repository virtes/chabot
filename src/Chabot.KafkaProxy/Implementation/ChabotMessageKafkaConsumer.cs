using Chabot.KafkaProxy.Configuration;
using Chabot.KafkaProxy.Models;
using Chabot.Message;
using Chabot.User;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chabot.KafkaProxy.Implementation;

public class ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>
    where TUser : IUser<TUserId>
{
    private readonly ILogger<ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>> _logger;
    private readonly IMessageHandler<TMessage, TUser, TUserId> _messageHandler;
    private readonly ChabotKafkaProxyConsumerOptions _options;
    private readonly string _consumerId = Guid.NewGuid().ToString();
    private readonly CancellationTokenSource _cts = new ();
    private readonly TaskCompletionSource _consumingStoppedTcs = new ();
    private bool _started;

    public ChabotMessageKafkaConsumer(
        IOptions<ChabotKafkaProxyConsumerOptions> options,
        ILogger<ChabotMessageKafkaConsumer<TMessage, TUser, TUserId>> logger,
        IMessageHandler<TMessage, TUser, TUserId> messageHandler)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _options = options.Value;
    }

    public async Task Start()
    {
        if (_started)
            throw new InvalidOperationException("Consumer already started");
        _started = true;

        await Task.Yield();

        var cancellationToken = _cts.Token;

        while (!cancellationToken.IsCancellationRequested)
        {
            IConsumer<string, ChabotUserMessageDto<TUser, TMessage>>? consumer = null;

            try
            {
                consumer = BuildConsumer();

                consumer.Subscribe(_options.Topic);
                _logger.LogInformation("Chabot message kafka consumer {ConsumerId} subscribed to {Topic}",
                    _consumerId, _options.Topic);

                await Consume(consumer, cancellationToken);
            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "ConsumeException occured while consuming {Topic} ({@Error})",
                    _options.Topic, e.Error);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consuming {Topic} cancelled",
                    _options.Topic);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while consuming {Topic} occured",
                    _options.Topic);
            }
            finally
            {
                consumer?.Close();
                consumer?.Dispose();
            }

            if (cancellationToken.IsCancellationRequested)
                break;

            await Task.Delay(_options.WaitForReconnect, cancellationToken);
            _logger.LogInformation("Chabot message kafka consumer {ConsumerId} reconnecting to {Topic}...",
                _consumerId, _options.Topic);
        }

        _logger.LogInformation("Chabot message kafka consumer {ConsumerId} stopped",
            _consumerId);

        _consumingStoppedTcs.SetResult();
    }

    public Task Stop()
    {
        _logger.LogTrace("Chabot message kafka consumer {ConsumerId} stopping requested", _consumerId);

        _cts.Cancel();

        return _consumingStoppedTcs.Task;
    }

    private IConsumer<string, ChabotUserMessageDto<TUser, TMessage>> BuildConsumer()
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = _options.ConsumerGroup,
            BootstrapServers = _options.BootstrapServers,
            ClientId = _consumerId,
            AutoOffsetReset = _options.AutoOffsetReset,
            EnableAutoCommit = false,
            SocketTimeoutMs = 30_000
        };
        var consumerBuilder = new ConsumerBuilder<string, ChabotUserMessageDto<TUser, TMessage>>(consumerConfig);

        consumerBuilder.SetValueDeserializer(new SystemTextJsonSerializer<ChabotUserMessageDto<TUser, TMessage>>());

        var topic = _options.Topic;

        consumerBuilder.SetErrorHandler((_, error) =>
        {
            _logger.LogError("Chabot message kafka consumer {ConsumerId} error {@Error}",
                _consumerId, error);
        });

        consumerBuilder.SetLogHandler((_, message) =>
        {
            var level = message.Level switch
            {
                SyslogLevel.Emergency => LogLevel.Error,
                SyslogLevel.Alert => LogLevel.Error,
                SyslogLevel.Critical => LogLevel.Error,
                SyslogLevel.Error => LogLevel.Error,
                SyslogLevel.Warning => LogLevel.Warning,
                SyslogLevel.Notice => LogLevel.Information,
                SyslogLevel.Info => LogLevel.Debug,
                SyslogLevel.Debug => LogLevel.Trace,
                _ => LogLevel.Debug
            };

            _logger.Log(level, "Chabot message kafka consumer {ConsumerId} log message {@Message}",
                _consumerId, message);
        });

        consumerBuilder.SetPartitionsAssignedHandler((_, list) =>
        {
            _logger.LogInformation("Topic {Topic} partitions {@TopicPartitions} assigned to chabot message kafka consumer {ConsumerId}",
                topic, list, _consumerId);
        });

        consumerBuilder.SetPartitionsLostHandler((_, list) =>
        {
            _logger.LogInformation("Topic {Topic} partitions {@TopicPartitions} lost from chabot message kafka consumer {ConsumerId}",
                topic, list, _consumerId);
        });

        consumerBuilder.SetPartitionsRevokedHandler((_, list) =>
        {
            _logger.LogInformation("Topic {Topic} partitions {@TopicPartitions} revoked from chabot message kafka consumer {ConsumerId}",
                topic, list, _consumerId);
        });

        return consumerBuilder.Build();
    }

    private async Task Consume(
        IConsumer<string, ChabotUserMessageDto<TUser, TMessage>> consumer,
        CancellationToken cancellationToken)
    {
        ConsumeResult<string, ChabotUserMessageDto<TUser, TMessage>>? consumeResultToCommit = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(_options.WaitForMessage);
            if (consumeResult == null)
            {
                try
                {
                    await Task.Delay(_options.DelayAfterEndOfPartition, cancellationToken);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                continue;
            }

            _logger.LogTrace("Message consumed at {Offset} in {Partition} partition of {Topic} topic by {MemberId}",
                consumeResult.Offset.Value, consumeResult.Partition.Value, consumeResult.Topic, consumer.MemberId);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_options.MessageTtl is not null
                        && consumeResult.Message.Headers.TryGetLastBytes(ChabotKafkaHeaders.ReceivedTimestampUtc, out var timeStampBytes)
                        && BitConverter.ToInt64(timeStampBytes) is var timestamp
                        && DateTimeOffset.FromUnixTimeSeconds(timestamp) is var messageDate
                        && messageDate.Add(_options.MessageTtl.Value) < DateTimeOffset.UtcNow)
                    {
                        _logger.LogWarning("Chabot message {@Message} is expired ({MessageDate})",
                            consumeResult.Message.Value.Message, messageDate);

                        consumeResultToCommit = consumeResult;
                        break;
                    }

                    await _messageHandler.HandleMessage(consumeResult.Message.Value.Message, consumeResult.Message.Value.User);

                    _logger.LogTrace("Message handled at {Offset} in {Partition} partition of {Topic} topic by {MemberId}",
                        consumeResult.Offset.Value, consumeResult.Partition.Value, consumeResult.Topic, consumer.MemberId);

                    consumeResultToCommit = consumeResult;

                    break;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("Message handling cancelled {@Message} at {Offset} offset in {Partition} partition of {Topic} topic by {MemberId}",
                        consumeResult.Message, consumeResult.Offset.Value, consumeResult.Partition.Value, consumeResult.Topic, consumer.MemberId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception while handling message {@Message} at {Offset} offset in {Partition} partition of {Topic} topic by {MemberId}",
                        consumeResult.Message, consumeResult.Offset.Value, consumeResult.Partition.Value, consumeResult.Topic, consumer.MemberId);

                    throw;
                }
            }

            if (consumeResultToCommit is not null)
            {
                try
                {
                    consumer.Commit(consumeResultToCommit);

                    _logger.LogTrace("Offset committed {Offset} in {Partition} partition of {Topic} topic by {MemberId}",
                        consumeResultToCommit.Offset.Value, consumeResultToCommit.Partition.Value,
                        consumeResultToCommit.Topic, consumer.MemberId);

                    consumeResultToCommit = null;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception while commiting {Offset} in {Partition} partition of {Topic} topic by {MemberId}",
                        consumeResultToCommit?.Offset.Value, consumeResultToCommit?.Partition.Value,
                        consumeResultToCommit?.Topic, consumer.MemberId);
                    throw;
                }
            }
        }
    }
}
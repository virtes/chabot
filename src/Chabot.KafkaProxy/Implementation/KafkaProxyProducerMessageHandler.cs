using Chabot.KafkaProxy.Configuration;
using Chabot.KafkaProxy.Models;
using Chabot.Message;
using Chabot.User;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chabot.KafkaProxy.Implementation;

public class KafkaProxyProducerMessageHandler<TMessage, TUser, TId>
    : IMessageHandler<TMessage, TUser, TId> where TUser
    : IUser<TId>
{
    private readonly ChabotKafkaProxyProducerOptions _options;
    private readonly ILogger<KafkaProxyProducerMessageHandler<TMessage, TUser, TId>> _logger;
    private readonly IProducer<string, ChabotUserMessageDto<TUser, TMessage>> _producer;

    public KafkaProxyProducerMessageHandler(
        IOptions<ChabotKafkaProxyProducerOptions> options,
        ILogger<KafkaProxyProducerMessageHandler<TMessage, TUser, TId>> logger)
    {
        _options = options.Value;
        _logger = logger;

        _producer = BuildProducer(_options, logger);
    }

    public async Task HandleMessage(TMessage message, TUser user)
    {
        var key = user.Id?.ToString() ?? "null";

        var chabotUserMessage = new ChabotUserMessageDto<TUser, TMessage>
        {
            User = user,
            Message = message
        };

        var kafkaMessage = new Message<string, ChabotUserMessageDto<TUser, TMessage>>
        {
            Key = key,
            Value = chabotUserMessage,
            Headers = new Headers
            {
                { ChabotKafkaHeaders.ReceivedTimestampUtc, BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) }
            },
            Timestamp = Timestamp.Default
        };

        var deliveryResult = await _producer.ProduceAsync(_options.Topic, kafkaMessage);

        _logger.LogDebug("Chabot message {@Key} {@Value} produced to {Topic} at {Offset} offset in {Partition} partition",
            key, kafkaMessage, deliveryResult.Topic, deliveryResult.Offset.Value, deliveryResult.Partition.Value);
    }

    private static IProducer<string, ChabotUserMessageDto<TUser, TMessage>> BuildProducer(
        ChabotKafkaProxyProducerOptions options, ILogger logger)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers,
            Acks = Acks.All
        };

        var producerBuilder = new ProducerBuilder<string, ChabotUserMessageDto<TUser, TMessage>>(producerConfig);

        producerBuilder.SetValueSerializer(new SystemTextJsonSerializer<ChabotUserMessageDto<TUser, TMessage>>());

        producerBuilder.SetErrorHandler((_, error) =>
        {
            logger.LogError("Chabot message kafka producer error {@Error}", error);
        });

        producerBuilder.SetLogHandler((_, message) =>
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

            logger.Log(level, "Chabot message kafka producer log {@Message}", message);
        });

        return producerBuilder.Build();
    }
}
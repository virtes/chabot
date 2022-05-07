using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace Chabot.KafkaProxy.Configuration;

public class ChabotKafkaProxyConsumerOptions : ChabotKafkaProxyOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ConsumerGroup { get; set; } = default!;

    public TimeSpan? MessageTtl { get; set; }

    public int? MultiplyConsumer { get; set; }

    public TimeSpan WaitForReconnect { get; set; } = TimeSpan.FromSeconds(5);

    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

    public TimeSpan WaitForMessage { get; set; } = TimeSpan.FromMilliseconds(100);

    public TimeSpan DelayAfterEndOfPartition { get; set; } = TimeSpan.FromMilliseconds(200);
}
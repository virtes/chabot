using System.ComponentModel.DataAnnotations;

namespace Chabot.KafkaProxy.Configuration;

public class ChabotKafkaProxyOptions
{
    [Required(AllowEmptyStrings = false)]
    public string BootstrapServers { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string Topic { get; set; } = default!;
}
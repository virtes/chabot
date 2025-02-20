using System.ComponentModel.DataAnnotations;

namespace Chabot.RabbitMq.Configuration;

public class RabbitMqHostOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Host { get; set; } = default!;

    public ushort Port { get; set; } = 5672;
}
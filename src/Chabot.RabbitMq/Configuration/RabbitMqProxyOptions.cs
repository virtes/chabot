using System.ComponentModel.DataAnnotations;

namespace Chabot.RabbitMq.Configuration;

public class RabbitMqProxyOptions : IValidatableObject
{
    [Required]
    public List<RabbitMqHostOptions> Hosts { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string Queue { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Hosts.Any())
        {
            return new ValidationResult[]
            {
                new ValidationResult("At least 1 Host must be specified", new []{ nameof(Hosts) })
            };
        }

        return Array.Empty<ValidationResult>();
    }
}
namespace Chabot.EntityFrameworkCore.Entities;

public class State
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Key { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string TargetType { get; init; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public required Guid Id { get; set; }
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public required DateTime CreatedAtUtc { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string TypeKey { get; set; }
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Value { get; set; }
}
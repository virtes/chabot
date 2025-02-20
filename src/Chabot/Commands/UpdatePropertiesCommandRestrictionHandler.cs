namespace Chabot.Commands;

internal class UpdatePropertiesCommandRestrictionHandler<TUpdate>
    : ICommandRestrictionHandler<TUpdate, UpdatePropertiesRestriction>
{
    public ValueTask<bool> IsAllowed(TUpdate update, UpdateMetadata updateMetadata,
        UpdatePropertiesRestriction restriction)
    {
        if (!updateMetadata.Properties.TryGetValue(restriction.Key, out var value))
            return ValueTask.FromResult(false);

        return ValueTask.FromResult(restriction.AllowedValues.Contains(value));
    }
}
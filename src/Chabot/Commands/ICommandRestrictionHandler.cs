namespace Chabot.Commands;

public interface ICommandRestrictionHandler<in TUpdate, in TRestriction>
{
    ValueTask<bool> IsAllowed(TUpdate update, UpdateMetadata updateMetadata,
        TRestriction restriction);
}
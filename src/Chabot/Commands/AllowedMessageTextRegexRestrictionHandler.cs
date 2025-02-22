using Microsoft.Extensions.Logging;

namespace Chabot.Commands;

internal class AllowedMessageTextRegexRestrictionHandler<TUpdate>
    : ICommandRestrictionHandler<TUpdate, AllowedMessageTextRegexRestriction>
{
    private readonly ILogger<AllowedMessageTextRegexRestrictionHandler<TUpdate>> _logger;

    public AllowedMessageTextRegexRestrictionHandler(
        ILogger<AllowedMessageTextRegexRestrictionHandler<TUpdate>> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> IsAllowed(TUpdate update,
        UpdateMetadata updateMetadata, AllowedMessageTextRegexRestriction restriction)
    {
        if (!updateMetadata.Properties.TryGetValue(UpdateProperties.MessageText, out var messageText))
        {
            _logger.LogTrace("Message text update property not found");
            return ValueTask.FromResult(false);
        }

        if (messageText is null)
        {
            _logger.LogTrace("Message text is null");
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(restriction.Regex.IsMatch(messageText));
    }
}
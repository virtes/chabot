using System.Text.RegularExpressions;

namespace Chabot.Commands;

internal class AllowedMessageTextRegexRestriction
{
    public required Regex Regex { get; init; }
}
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class AllowedMessageTextRegexAttribute : Attribute
{
    public string Pattern { get; }

    public AllowedMessageTextRegexAttribute(string pattern)
    {
        Pattern = pattern;
    }
}
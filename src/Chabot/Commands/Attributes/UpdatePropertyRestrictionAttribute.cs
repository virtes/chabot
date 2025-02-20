// ReSharper disable once CheckNamespace
namespace Chabot;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class UpdatePropertyRestrictionAttribute : Attribute
{
    public string Key { get; }

    public string?[] AllowedValues { get; }

    public UpdatePropertyRestrictionAttribute(string key, params string?[] allowedValues)
    {
        Key = key;
        AllowedValues = allowedValues;
    }
}
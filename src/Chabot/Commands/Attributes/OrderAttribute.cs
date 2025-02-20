using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Chabot;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public class OrderAttribute : Attribute
{
    public int Order { get; }

    public OrderAttribute(int order)
    {
        Order = order;
    }
}
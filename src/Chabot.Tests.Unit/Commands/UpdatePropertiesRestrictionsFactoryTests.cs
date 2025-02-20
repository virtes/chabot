using System.Reflection;
using FluentAssertions;
using Chabot.Commands;
using Xunit;

namespace Chabot.Tests.Unit.Commands;

public class UpdatePropertiesRestrictionsFactoryTests
{
    private readonly UpdatePropertiesRestrictionsFactory _subject = new();

    [Fact]
    public void CreateRestrictions_UpdatePropertyRestrictionAttribute_ShouldReturnUpdatePropertiesRestriction()
    {
        var methodInfo = GetMethodInfo(nameof(MethodWithUpdatePropertyRestrictionAttribute));

        var restrictions = _subject.CreateRestrictions(typeof(UpdatePropertiesRestrictionsFactoryTests), methodInfo);
        restrictions.Should().BeEquivalentTo(new []
        {
            new UpdatePropertiesRestriction
            {
                Key = "key-1",
                AllowedValues = ["allowed-value-1-1", "allowed-value-1-2"]
            },
            new UpdatePropertiesRestriction
            {
                Key = "key-2",
                AllowedValues = ["allowed-value-2-1", "allowed-value-2-2"]
            }
        });
    }

    [Fact]
    public void CreateRestrictions_UpdatePropertyRestrictionAttributeInheritor_ShouldReturnUpdatePropertiesRestriction()
    {
        var methodInfo = GetMethodInfo(nameof(MethodWithUpdatePropertyRestrictionAttributeInheritor));

        var restrictions = _subject.CreateRestrictions(typeof(UpdatePropertiesRestrictionsFactoryTests), methodInfo);
        restrictions.Should().BeEquivalentTo(new []
        {
            new UpdatePropertiesRestriction
            {
                Key = "key",
                AllowedValues = ["allowed-value"]
            }
        });
    }

    private static MethodInfo GetMethodInfo(string methodName)
    {
        return typeof(UpdatePropertiesRestrictionsFactoryTests).GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    private class UpdatePropertyRestrictionInheritorAttribute : UpdatePropertyRestrictionAttribute
    {
        public UpdatePropertyRestrictionInheritorAttribute(string allowedValue)
            : base("key", [ allowedValue ])
        {
        }
    }

    [UpdatePropertyRestriction("key-1", "allowed-value-1-1", "allowed-value-1-2")]
    [UpdatePropertyRestriction("key-2", "allowed-value-2-1", "allowed-value-2-2")]
#pragma warning disable CA1822
    private Task MethodWithUpdatePropertyRestrictionAttribute() => Task.CompletedTask;
#pragma warning restore CA1822

    [UpdatePropertyRestrictionInheritor("allowed-value")]
#pragma warning disable CA1822
    private Task MethodWithUpdatePropertyRestrictionAttributeInheritor() => Task.CompletedTask;
#pragma warning restore CA1822
}
using System.Reflection;
using FluentAssertions;
using Chabot.Commands;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Chabot.Tests.Unit.Commands;

public class CommandResolverTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock = new(MockBehavior.Strict);
    private readonly CommandResolver<string>.RestrictionHandlersCache _cache = new();
    private readonly CommandResolver<string> _subject;

    public const string Update = "update";

    public CommandResolverTests()
    {
        _subject = new CommandResolver<string>(
            serviceProvider: _serviceProviderMock.Object,
            restrictionHandlersCache: _cache,
            logger: NullLogger<CommandResolver<string>>.Instance);
    }

    [Fact]
    public async Task ResolveCommand_ShouldReturnCommandWithoutRestrictions()
    {
        var commandMetadata = GenerateCommandMetadata();

        var actualCommand = await _subject.ResolveCommand(GenerateUpdateMetadata(), [commandMetadata], Update);
        actualCommand.Should().Be(commandMetadata);
    }

    [Fact]
    public async Task ResolveCommand_MultipleCommandResolved_TheSameOrder_ShouldThrow()
    {
        var commandMetadata1 = GenerateCommandMetadata(order: 1);
        var commandMetadata2 = GenerateCommandMetadata(order: 1);

        var resolveCommand = () => _subject.ResolveCommand(GenerateUpdateMetadata(), [commandMetadata1, commandMetadata2], Update);
        await resolveCommand.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ResolveCommand_MultipleCommandResolved_WithDifferentOrder_ShouldReturnCommandWithHigherOrder()
    {
        var commandMetadata1 = GenerateCommandMetadata(order: 1);
        var commandMetadata2 = GenerateCommandMetadata(order: 2);
        var commandMetadata3 = GenerateCommandMetadata(order: 1);

        var actualCommand = await _subject.ResolveCommand(GenerateUpdateMetadata(),
            [commandMetadata1, commandMetadata2, commandMetadata3], Update);
        actualCommand.Should().Be(commandMetadata2);
    }

    [Fact]
    public async Task ResolveCommand_CommandWithRestriction_ShouldReturnAllowedCommand()
    {
        var allowedRestriction = new SomeRestriction();
        var notAllowedRestriction = new SomeRestriction();
        var allowedCommand = GenerateCommandMetadata(restrictions: [ allowedRestriction ], order: 1);
        var notAllowedCommand = GenerateCommandMetadata(restrictions: [ allowedRestriction, notAllowedRestriction ], order: 1);
        var updateMetadata = GenerateUpdateMetadata();

        var someRestrictionHandlerMock = new Mock<ICommandRestrictionHandler<string, SomeRestriction>>();
        someRestrictionHandlerMock
            .Setup(rh => rh.IsAllowed(Update, updateMetadata, allowedRestriction))
            .Returns(() => ValueTask.FromResult(true));
        someRestrictionHandlerMock
            .Setup(rh => rh.IsAllowed(Update, updateMetadata, notAllowedRestriction))
            .Returns(() => ValueTask.FromResult(false));

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandRestrictionHandler<string, SomeRestriction>)))
            .Returns(someRestrictionHandlerMock.Object);

        var actualCommand = await _subject.ResolveCommand(updateMetadata, [allowedCommand, notAllowedCommand], Update);
        actualCommand.Should().Be(allowedCommand);

        _cache.IsAllowedByRestrictionType.Should().HaveCount(1);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    internal class SomeRestriction
    {
    }

#pragma warning disable CA1822
    private void SomeMethod()
#pragma warning restore CA1822
    {
    }

    private static CommandMetadata GenerateCommandMetadata(
        IReadOnlyList<object>? restrictions = null,
        int? order = null,
        string? method = null)
    {
        return new CommandMetadata
        {
            Id = Guid.NewGuid().ToString(),
            Order = order ?? Random.Shared.Next(),
            Type = typeof(CommandResolverTests),
            MethodInfo = method is null ? GetMethodInfo(nameof(SomeMethod)) : GetMethodInfo(method),
            Restrictions = restrictions ?? Array.Empty<object>()
        };
    }

    private static UpdateMetadata GenerateUpdateMetadata()
    {
        return new UpdateMetadata(
            Properties: new Dictionary<string, string?>());
    }

    private static MethodInfo GetMethodInfo(string methodName)
    {
        return typeof(CommandResolverTests).GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.NonPublic)!;
    }
}
using System.Text.Json.Serialization;
using Chabot.State;
using Chabot.State.Implementation.SystemTextJson;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.State.SystemTextJson;

public class SystemTextJsonStateSerializerTests
{
    private Mock<IStateTypeMapping> _stateTypeMappingMock = default!;

    private SystemTextJsonStateSerializer _subject = default!;
    
    [SetUp]
    public void Setup()
    {
        _stateTypeMappingMock = new Mock<IStateTypeMapping>();
        
        _subject = new SystemTextJsonStateSerializer(
            stateSerializerOptions: null,
            logger: NullLogger<SystemTextJsonStateSerializer>.Instance,
            stateTypeMapping: _stateTypeMappingMock.Object);
    }

    [Test]
    public void ShouldSerializeState()
    {
        _stateTypeMappingMock
            .Setup(stm => stm.GetStateTypeKey(typeof(TestState)))
            .Returns(nameof(TestState));
        
        var testState = new TestState
        {
            Value = "some-value"
        };
        var createdAt = new DateTime(2020, 01, 01);
        var metadata = new Dictionary<string, string?>
        {
            ["key1"] = "some-meta-value",
            ["key2"] = null
        };

        var stateData = new UserState(testState, createdAt, metadata);

        var serializedStateData = _subject.SerializeState(stateData);

        serializedStateData.Should().Be(
            "{\"stateTypeKey\":\"TestState\"," +
            "\"serializedState\":\"{\\u0022val\\u0022:\\u0022some-value\\u0022}\"," +
            "\"createdAtUtc\":\"2020-01-01T00:00:00\"," +
            "\"metadata\":{\"key1\":\"some-meta-value\",\"key2\":null}}");
    }

    [Test]
    public void ShouldSerializeNullState()
    {
        var createdAt = new DateTime(2020, 01, 01);
        var metadata = new Dictionary<string, string?>
        {
            ["key1"] = "some-meta-value",
            ["key2"] = null
        };

        var stateData = new UserState(null, createdAt, metadata);

        var serializedStateData = _subject.SerializeState(stateData);

        serializedStateData.Should().Be(
            "{\"stateTypeKey\":null," +
            "\"serializedState\":null," +
            "\"createdAtUtc\":\"2020-01-01T00:00:00\"," +
            "\"metadata\":{\"key1\":\"some-meta-value\",\"key2\":null}}");
    }

    [Test]
    public void ShouldDeserializeState()
    {
        _stateTypeMappingMock
            .Setup(stm => stm.GetStateType(nameof(TestState)))
            .Returns(typeof(TestState));
        
        const string serializedStateData =
            "{\"stateTypeKey\":\"TestState\"," +
            "\"serializedState\":\"{\\u0022val\\u0022:\\u0022some-value\\u0022}\"," +
            "\"createdAtUtc\":\"2020-01-01T00:00:00\"," +
            "\"metadata\":{\"key1\":\"some-meta-value\",\"key2\":null}}";

        var expectedState = new TestState
        {
            Value = "some-value"
        };
        var expectedCreatedAt = new DateTime(2020, 01, 01);
        var expectedMetadata = new Dictionary<string, string?>
        {
            ["key1"] = "some-meta-value",
            ["key2"] = null
        };
        
        var actualStateData = _subject.DeserializeState(serializedStateData);

        actualStateData.State.Should().BeOfType<TestState>();
        actualStateData.State.Should().BeEquivalentTo(expectedState);

        actualStateData.CreatedAtUtc.Should().Be(expectedCreatedAt);

        actualStateData.Metadata.Should().BeEquivalentTo(expectedMetadata);
    }
    
    [Test]
    public void ShouldDeserializeNullState()
    {
        const string serializedStateData =
            "{\"stateTypeKey\":null," +
            "\"serializedState\":null," +
            "\"createdAtUtc\":\"2020-01-01T00:00:00\"," +
            "\"metadata\":{\"key1\":\"some-meta-value\",\"key2\":null}}";

        var expectedCreatedAt = new DateTime(2020, 01, 01);
        var expectedMetadata = new Dictionary<string, string?>
        {
            ["key1"] = "some-meta-value",
            ["key2"] = null
        };
        
        var actualStateData = _subject.DeserializeState(serializedStateData);

        actualStateData.State.Should().BeNull();

        actualStateData.CreatedAtUtc.Should().Be(expectedCreatedAt);

        actualStateData.Metadata.Should().BeEquivalentTo(expectedMetadata);
    }

    private class TestState : IState
    {
        [JsonPropertyName("val")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? Value { get; set; }
    }
}
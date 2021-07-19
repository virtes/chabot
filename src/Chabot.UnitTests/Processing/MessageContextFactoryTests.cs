using Chabot.Processing.Implementation;
using Chabot.UnitTests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Processing
{
    public class MessageContextFactoryTests
    {
        private Mock<IServiceScopeFactory> _serviceScopeFactoryMock;

        [SetUp]
        public void Setup()
        {
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        }

        [Test]
        public void ShouldCreateMessageContextWithNewScope()
        {
            var message = new Message();

            var contextFactory = CreateContextFactory();

            var actualContext = contextFactory.CreateContext(message);

            actualContext.Message.Should().Be(message);

            _serviceScopeFactoryMock.Verify(f => f.CreateScope(), Times.Once);
        }

        [Test]
        public void ShouldNotDisposeCreatedScope()
        {
            var serviceScopeMock = new Mock<IServiceScope>();
            _serviceScopeFactoryMock
                .Setup(f => f.CreateScope())
                .Returns(serviceScopeMock.Object);

            var contextFactory = CreateContextFactory();

            contextFactory.CreateContext(new Message());

            serviceScopeMock.Verify(ss => ss.Dispose(), Times.Never);
        }

        private MessageContextFactory<Message> CreateContextFactory()
            => new MessageContextFactory<Message>(_serviceScopeFactoryMock.Object);
    }
}
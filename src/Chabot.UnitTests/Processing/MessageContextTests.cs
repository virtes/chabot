using System;
using Chabot.Processing.Implementation;
using Chabot.UnitTests.Fakes;
using Chabot.User;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Processing
{
    public class MessageContextTests
    {
        private Mock<IServiceScope> _serviceScopeMock;

        [SetUp]
        public void Setup()
        {
            _serviceScopeMock = new Mock<IServiceScope>();
        }

        [Test]
        public void ShouldDisposeServiceScopeWhenContextDisposed()
        {
            var context = CreateMessageContext(new Message());

            _serviceScopeMock.Verify(ss => ss.Dispose(), Times.Never);

            context.Dispose();

            _serviceScopeMock.Verify(ss => ss.Dispose(), Times.Once);
        }

        [Test]
        public void ShouldProvideServicesFromScope()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeMock
                .Setup(ss => ss.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            var context = CreateMessageContext(new Message());

            context.Services.Should().Be(serviceProviderMock.Object);
        }

        [Test]
        public void ShouldProvideMessage()
        {
            var message = new Message();

            var context = CreateMessageContext(message);

            context.Message.Should().Be(message);
        }

        private MessageContext<Message> CreateMessageContext(Message message)
            => new MessageContext<Message>(message, _serviceScopeMock.Object);
    }
}
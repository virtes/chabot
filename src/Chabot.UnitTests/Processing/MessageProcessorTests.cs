using System.Threading.Tasks;
using Chabot.Configuration;
using Chabot.Processing;
using Chabot.Processing.Implementation;
using Chabot.UnitTests.Fakes;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Processing
{
    public class MessageProcessorTests
    {
        private Mock<IMessageContextFactory<Message>> _messageContextFactory;
        private Mock<IMessageProcessingConfiguration<Message>> _processingConfigurationMock;

        [SetUp]
        public void Setup()
        {
            _messageContextFactory = new Mock<IMessageContextFactory<Message>>();
            _processingConfigurationMock = new Mock<IMessageProcessingConfiguration<Message>>();
        }

        [Test]
        public async Task ShouldCreateMessageContextAndExecuteEntryPoint()
        {
            var message = new Message();

            var messageContext = new FakeMessageContext();
            _messageContextFactory
                .Setup(f => f.CreateContext(message))
                .Returns(messageContext);

            _processingConfigurationMock
                .Setup(c => c.ProcessingEntryPoint)
                .Returns(async context =>
                {
                    context.Should().Be(messageContext);
                    await Task.Yield();
                });

            var processor = CreateProcessor();

            await processor.ProcessAsync(message);

            _messageContextFactory.VerifyAll();
            _messageContextFactory.VerifyNoOtherCalls();

            _processingConfigurationMock.VerifyAll();
            _processingConfigurationMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task ShouldDisposeCreatedMessageContext()
        {
            var message = new Message();

            var messageContextMock = new Mock<IMessageContext<Message>>();
            _messageContextFactory
                .Setup(f => f.CreateContext(message))
                .Returns(messageContextMock.Object);

            var processor = CreateProcessor();

            await processor.ProcessAsync(message);

            messageContextMock.Verify(c => c.Dispose(), Times.Once);
        }

        private MessageProcessor<Message> CreateProcessor()
            => new MessageProcessor<Message>(
                messageContextFactory: _messageContextFactory.Object,
                configuration: _processingConfigurationMock.Object);
    }
}
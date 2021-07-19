using Chabot.Authentication.Implementation;
using Chabot.Authorization.Implementation;
using Chabot.Processing;
using Chabot.UnitTests.Fakes;
using Moq;
using NUnit.Framework;

// ReSharper disable InvokeAsExtensionMethod
namespace Chabot.UnitTests.Processing
{
    public class ProcessingPipelineBuilderExtensionsTests
    {
        private Mock<IProcessingPipelineBuilder<Message>> _pipelineBuilderMock;

        [SetUp]
        public void Setup()
        {
            _pipelineBuilderMock = new Mock<IProcessingPipelineBuilder<Message>>();
        }

        [Test]
        public void ShouldAddAuthenticationMiddleware()
        {
            ProcessingPipelineBuilderExtensions.Authenticate(_pipelineBuilderMock.Object);

            _pipelineBuilderMock.Verify(b => b.UseMiddleware<AuthenticationMiddleware<Message>>(), Times.Once);
            _pipelineBuilderMock.VerifyNoOtherCalls();
        }

        [Test]
        public void ShouldAddAuthorizationMiddleware()
        {
            ProcessingPipelineBuilderExtensions.Authorize(_pipelineBuilderMock.Object);

            _pipelineBuilderMock.Verify(b => b.UseMiddleware<AuthorizationMiddleware<Message>>(), Times.Once);
            _pipelineBuilderMock.VerifyNoOtherCalls();
        }
    }
}
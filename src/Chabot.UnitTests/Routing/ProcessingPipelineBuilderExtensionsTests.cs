using Chabot.Processing;
using Chabot.Routing.Extensions;
using Chabot.Routing.Implementation;
using Chabot.UnitTests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Routing
{
    public class ProcessingPipelineBuilderExtensionsTests
    {
        [Test]
        public void ShouldRegisterAndUseMiddleware()
        {
            var serviceCollectionMock = new Mock<IServiceCollection>();

            var pipelineBuilderMock = new Mock<IProcessingPipelineBuilder<Message>>();
            pipelineBuilderMock
                .Setup(b => b.Services)
                .Returns(serviceCollectionMock.Object);

            pipelineBuilderMock.Object.UseRouting();

            serviceCollectionMock
                .Verify(s => s.Add(It.Is<ServiceDescriptor>(sd => sd.ImplementationType == typeof(RoutingMiddleware<Message>)
                                                                  && sd.ServiceType == typeof(RoutingMiddleware<Message>))));

            pipelineBuilderMock.Verify(b => b.UseMiddleware<RoutingMiddleware<Message>>(), Times.Once);
        }
    }
}
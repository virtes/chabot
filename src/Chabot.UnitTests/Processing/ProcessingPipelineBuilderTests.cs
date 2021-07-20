using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chabot.Processing;
using Chabot.Processing.Exceptions;
using Chabot.Processing.Implementation;
using Chabot.UnitTests.Fakes;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Processing
{
    public class ProcessingPipelineBuilderTests
    {
        [Test]
        public async Task ShouldBuildEntryPointWhichExecutesDelegatesInOrder()
        {
            var executionOrderChecker = new ExecutionOrderChecker();

            var builder = CreateBuilder();

            builder.ThrowWhenPipelineReachedTheEnd = false;

            builder.Use(next => context =>
            {
                executionOrderChecker.Executed(1);
                return next(context);
            });
            builder.Use(next => context =>
            {
                executionOrderChecker.Executed(2);
                return next(context);
            });

            var entryPoint = builder.BuildProcessingEntryPoint();

            await entryPoint(null!);

            executionOrderChecker.ValidateExecutions(2);
        }

        [Test]
        public async Task ShouldAddProcessingDelegateWhichRetrievesMiddlewareFromContextServicesAndExecutesIt()
        {
            var middlewareMock = new Mock<IMiddleware<Message>>();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IMiddleware<Message>)))
                .Returns(middlewareMock.Object);
            var contextMock = new Mock<IMessageContext<Message>>();
            contextMock
                .Setup(c => c.Services)
                .Returns(serviceProviderMock.Object);

            var builder = CreateBuilder();

            builder.ThrowWhenPipelineReachedTheEnd = false;

            builder.UseMiddleware<IMiddleware<Message>>();

            var entryPoint = builder.BuildProcessingEntryPoint();

            await entryPoint(contextMock.Object);

            middlewareMock.Verify(m => m.ExecuteAsync(contextMock.Object, It.IsAny<ProcessingDelegate<Message>>()));

            serviceProviderMock.VerifyAll();
            serviceProviderMock.VerifyNoOtherCalls();

            contextMock.VerifyAll();
            contextMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task ShouldBuildEntryPointWhichExecutesDelegatesAndMiddlewaresInOrder()
        {
            var executionOrderChecker = new ExecutionOrderChecker();

            var middleware = new Middleware(() =>
            {
                executionOrderChecker.Executed(2);
                return Task.CompletedTask;
            });

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(Middleware)))
                .Returns(middleware);
            var contextMock = new Mock<IMessageContext<Message>>();
            contextMock
                .Setup(c => c.Services)
                .Returns(serviceProviderMock.Object);

            var builder = CreateBuilder();

            builder.ThrowWhenPipelineReachedTheEnd = false;

            builder.Use(next => context =>
            {
                executionOrderChecker.Executed(1);
                return next(context);
            });
            builder.UseMiddleware<Middleware>();
            builder.Use(next => context =>
            {
                executionOrderChecker.Executed(3);
                return next(context);
            });

            var entryPoint = builder.BuildProcessingEntryPoint();

            await entryPoint(contextMock.Object);

            executionOrderChecker.ValidateExecutions(3);
        }

        [Test]
        public void ShouldBuildEntryPointWithThrowAtTheEnd()
        {
            var builder = CreateBuilder();

            builder.Use(next => next);

            var entryPoint = builder.BuildProcessingEntryPoint();

            Func<ValueTask> executePipeline = () => entryPoint(null!);
            executePipeline.Should().Throw<MessageProcessorReachedTheEndException>();
        }

        [Test]
        public void ShouldBuildEntryPointWithThrowAtTheEndWithEmptyConfig()
        {
            var builder = CreateBuilder();

            var entryPoint = builder.BuildProcessingEntryPoint();

            Func<ValueTask> executePipeline = () => entryPoint(null!);
            executePipeline.Should().Throw<MessageProcessorReachedTheEndException>();
        }

        [Test]
        public async Task ShouldBuildEntryPointWithoutThrowAtTheEnd()
        {
            var builder = CreateBuilder();

            builder.ThrowWhenPipelineReachedTheEnd = false;

            builder.Use(next => next);

            var entryPoint = builder.BuildProcessingEntryPoint();

            await entryPoint(null!);
        }

        [Test]
        public async Task ShouldBuildEntryPointWithoutThrowAtTheEndWithEmptyConfig()
        {
            var builder = CreateBuilder();

            builder.ThrowWhenPipelineReachedTheEnd = false;

            var entryPoint = builder.BuildProcessingEntryPoint();

            await entryPoint(null!);
        }

        private ProcessingPipelineBuilder<Message> CreateBuilder()
            => new ProcessingPipelineBuilder<Message>();

        private class ExecutionOrderChecker
        {
            private readonly List<int> _executions = new List<int>();

            public void Executed(int number)
            {
                _executions.Add(number);
            }

            public void ValidateExecutions(int expectedExecutionsNumber)
            {
                _executions.Count.Should().Be(expectedExecutionsNumber);

                if (expectedExecutionsNumber > 1)
                {
                    var previousExecutedNumber = _executions.First();

                    for (int i = 1; i < _executions.Count; i++)
                    {
                        var currentExecutedNumber = _executions[i];

                        previousExecutedNumber.Should().BeLessThan(currentExecutedNumber);

                        previousExecutedNumber = currentExecutedNumber;
                    }
                }
            }
        }
    }
}
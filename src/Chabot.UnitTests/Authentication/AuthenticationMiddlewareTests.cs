using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Chabot.Authentication;
using Chabot.Authentication.Implementation;
using Chabot.Processing;
using Chabot.UnitTests.Fakes;
using Chabot.User;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chabot.UnitTests.Authentication
{
    public class AuthenticationMiddlewareTests
    {
        private Mock<IAuthenticationHandler<Message>> _authenticationHandlerMock;

        [SetUp]
        public void Setup()
        {
            _authenticationHandlerMock = new Mock<IAuthenticationHandler<Message>>();
        }

        [Test]
        public void ShouldThrowWhenAuthenticationHandlerIsNotRegistered()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var messageContextMock = new Mock<IMessageContext<Message>>();
            messageContextMock
                .Setup(c => c.Services)
                .Returns(serviceProviderMock.Object);

            var middleware = new AuthenticationMiddleware<Message>();

            Func<Task> execute = async () => await middleware.ExecuteAsync(messageContextMock.Object, _ => ValueTask.CompletedTask);

            execute.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public async Task ShouldCreateAndSetUserIdentityToMessageContextUsingAuthenticationResult()
        {
            var authenticationResult = new AuthenticationResult(true, "test-state", new List<Claim>
            {
                new Claim("test-claim", "test-value")
            });

            var message = new Message
            {
                SenderId = "sender-id"
            };

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationHandler<Message>)))
                .Returns(_authenticationHandlerMock.Object);

            var messageContextMock = new Mock<IMessageContext<Message>>();
            messageContextMock
                .Setup(c => c.Services)
                .Returns(serviceProviderMock.Object);
            messageContextMock
                .Setup(c => c.Message)
                .Returns(message);

            _authenticationHandlerMock
                .Setup(ah => ah.AuthenticateAsync(messageContextMock.Object))
                .Returns(ValueTask.FromResult(authenticationResult));

            var middleware = new AuthenticationMiddleware<Message>();

            messageContextMock.Object.User.Should().BeNull();

            await middleware.ExecuteAsync(messageContextMock.Object, _ => ValueTask.CompletedTask);

            messageContextMock.VerifySet(c => c.User = It.Is<UserIdentity>(u =>
                u.Id == message.SenderId
                && u.Claims.Equals(authenticationResult.Claims)
                && u.StateKey == authenticationResult.StateKey
                && u.IsAuthenticated == authenticationResult.Succeeded), Times.Once);
            messageContextMock.VerifyAll();
        }
    }
}
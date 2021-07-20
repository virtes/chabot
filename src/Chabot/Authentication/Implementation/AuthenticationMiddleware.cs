using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;
using Chabot.Processing.Implementation;
using Chabot.User;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Authentication.Implementation
{
    public class AuthenticationMiddleware<TMessage> : IMiddleware<TMessage>
        where TMessage : IMessage
    {
        public async ValueTask ExecuteAsync(IMessageContext<TMessage> context, ProcessingDelegate<TMessage> next)
        {
            var authenticationHandler = context.Services.GetRequiredService<IAuthenticationHandler<TMessage>>();

            var authenticateTask = authenticationHandler.AuthenticateAsync(context);
            var authenticationResult = authenticateTask.IsCompletedSuccessfully
                ? authenticateTask.Result
                : await authenticateTask;

            var userIdentity = new UserIdentity(
                id: context.Message.SenderId,
                isAuthenticated: authenticationResult.Succeeded,
                stateKey: authenticationResult.StateKey,
                claims: authenticationResult.Claims);
            context.User = userIdentity;

            await next(context);
        }
    }
}
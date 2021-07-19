using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;

namespace Chabot.Authentication
{
    public interface IAuthenticationHandler<TMessage>
        where TMessage : IMessage
    {
        public ValueTask<AuthenticationResult> AuthenticateAsync(MessageContext<TMessage> context);
    }
}
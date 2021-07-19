using System.Threading.Tasks;
using Chabot.Messages;
using Chabot.Processing;

namespace Chabot.Authentication
{
    public interface IAuthenticationHandler<in TMessage>
        where TMessage : IMessage
    {
        public ValueTask<AuthenticationResult> AuthenticateAsync(IMessageContext<TMessage> context);
    }
}
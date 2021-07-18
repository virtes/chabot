using System.Threading.Tasks;

namespace Chabot.Authentication
{
    public interface IAuthenticationHandler<TMessage>
        where TMessage : IMessage
    {
        public ValueTask<AuthenticationResult> AuthenticateAsync(MessageContext<TMessage> context);
    }
}
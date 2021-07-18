namespace Chabot.Example
{
    public interface IMessageService
    {
        Message GetNewMessage();

        void SendMessage(string receiver, string text, string replyToMessage);
    }

    public class Message
    {
        public Message(string sender, string text)
        {
            Sender = sender;
            Text = text;
        }

        public string Sender { get; }

        public string Text { get; }
    }
}
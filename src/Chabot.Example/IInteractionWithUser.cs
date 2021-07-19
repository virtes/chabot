namespace Chabot.Example
{
    // Simple example of interaction with user,
    // in real chat bot app it can be Telegram and any other messengers
    public interface IInteractionWithUser
    {
        Message GetNewMessage();

        void SendMessage(string userId, string text, string replyToMessage);
    }

    public class Message
    {
        public Message(string senderId, string text)
        {
            SenderId = senderId;
            Text = text;
        }

        public string SenderId { get; }

        public string Text { get; }
    }
}
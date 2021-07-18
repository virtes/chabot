using System;

namespace Chabot.Example
{
    public class ConsoleMessageService : IMessageService
    {
        public Message GetNewMessage()
        {
            WriteLine(" Enter your name:", ConsoleColor.Yellow);
            var name = Console.ReadLine();

            WriteLine(" Enter a message:", ConsoleColor.Yellow);
            var text = Console.ReadLine();

            return new Message(name, text);
        }

        public void SendMessage(string receiver, string text, string replyToMessage)
        {
            if (string.IsNullOrEmpty(replyToMessage))
            {
                WriteLine(text, ConsoleColor.Green);
            }
            else
            {
                WriteLine($"\t{replyToMessage}", ConsoleColor.DarkGreen);
                WriteLine(text, ConsoleColor.Green);
            }
        }

        private void WriteLine(string text, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(text);

            Console.ForegroundColor = oldColor;
        }
    }
}
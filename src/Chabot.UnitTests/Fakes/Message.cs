using System.Collections.Generic;
using Chabot.Messages;

namespace Chabot.UnitTests.Fakes
{
    public class Message : IMessage
    {
        public string Id { get; set; }

        public string RawText { get; set; }

        public string SenderId { get; set; }

        public IReadOnlyDictionary<string, string> Items { get; set; }
    }
}
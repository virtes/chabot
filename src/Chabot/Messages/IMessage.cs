using System.Collections.Generic;

namespace Chabot.Messages
{
    public interface IMessage
    {
        string Id { get; }

        string RawText { get; }

        string SenderId { get; }

        IReadOnlyDictionary<string, string> Items { get; }
    }
}
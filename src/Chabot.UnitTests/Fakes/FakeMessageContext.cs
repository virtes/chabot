using System;
using Chabot.Commands;
using Chabot.Processing;
using Chabot.User;

namespace Chabot.UnitTests.Fakes
{
    public class FakeMessageContext : IMessageContext<Message>
    {
        public Message Message { get; set; }

        public IServiceProvider Services { get; set; }

        public UserIdentity User { get; set; }

        public CommandInfo Command { get; set; }

        public void Dispose()
        {
        }
    }
}
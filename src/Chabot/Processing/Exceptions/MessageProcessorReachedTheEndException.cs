using System;

namespace Chabot.Processing.Exceptions
{
    public class MessageProcessorReachedTheEndException : Exception
    {
        public MessageProcessorReachedTheEndException(Type messageType)
            : base($"{messageType.Name} message processor reached the end of execution pipeline")
        {
        }
    }
}
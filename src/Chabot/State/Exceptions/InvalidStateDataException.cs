namespace Chabot.State.Exceptions;

public class InvalidStateDataException : Exception
{
    public InvalidStateDataException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}
namespace Chabot.Configuration.Exceptions;

public class InvalidChabotConfigurationException : Exception
{
    public InvalidChabotConfigurationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
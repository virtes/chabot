namespace Chabot.Exceptions;

public class InvalidChabotConfigurationException : Exception
{
    public InvalidChabotConfigurationException(string message) : base(message)
    {
    }
}
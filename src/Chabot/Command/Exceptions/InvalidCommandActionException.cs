using System.Reflection;

namespace Chabot.Command.Exceptions;

public class InvalidCommandActionException : Exception
{
    public InvalidCommandActionException(Type commandGroupType, MethodInfo methodInfo, string message) 
        : base($"Command group {commandGroupType.FullName} action {methodInfo.Name} is invalid - {message}")
    {
    }
}
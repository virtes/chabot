using System.Reflection;

namespace Chabot.Commands
{
    public interface ICommandInfoFactory
    {
        CommandInfo CreateFromCommandMethod(MethodInfo methodInfo);
    }
}
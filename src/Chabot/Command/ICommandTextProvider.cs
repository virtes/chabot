using System.Reflection;

namespace Chabot.Command;

public interface ICommandTextProvider
{
    string[] GetCommandTexts(MethodInfo methodInfo);
}
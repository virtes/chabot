using System.Reflection;

namespace Chabot.Commands;

internal interface ICommandActionBuilder<TUpdate>
{
    CommandAction<TUpdate> BuildCommandAction(Type type, MethodInfo method);
}
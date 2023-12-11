using System.Reflection;
using JetBrains.Annotations;

namespace Chabot.Command;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[MeansImplicitUse]
public abstract class CommandAttributeBase : Attribute
{
    internal abstract string[] GetCommandTexts(MethodInfo methodInfo, IServiceProvider serviceProvider);

    internal abstract bool IsAllowedWithAnyCommandText(MethodInfo methodInfo);
}
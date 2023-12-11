using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Command;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[MeansImplicitUse]
public class CommandAttribute : CommandAttributeBase
{
    public CommandAttribute(params string[] commandTexts)
    {
        CommandTexts = commandTexts;
        AllowedWithAnyCommandText = false;
    }

    public string[] CommandTexts { get; }

    public bool AllowedWithAnyCommandText { get; set; }

    internal override string[] GetCommandTexts(MethodInfo methodInfo, IServiceProvider serviceProvider)
    {
        return CommandTexts;
    }

    internal override bool IsAllowedWithAnyCommandText(MethodInfo methodInfo)
    {
        return AllowedWithAnyCommandText;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[MeansImplicitUse]
public class CommandAttribute<TCommandTextProvider> : CommandAttributeBase
    where TCommandTextProvider : ICommandTextProvider
{
    internal override string[] GetCommandTexts(MethodInfo methodInfo, IServiceProvider serviceProvider)
    {
        var commandTextProvider = serviceProvider.GetService<TCommandTextProvider>()
                                  ?? Activator.CreateInstance<TCommandTextProvider>();

        return commandTextProvider.GetCommandTexts(methodInfo);
    }

    internal override bool IsAllowedWithAnyCommandText(MethodInfo methodInfo)
    {
        return false;
    }
}
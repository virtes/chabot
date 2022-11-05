using JetBrains.Annotations;

namespace Chabot.Command;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[MeansImplicitUse]
public class CommandAttribute : Attribute
{
    public CommandAttribute(params string[] commandTexts)
    {
        CommandTexts = commandTexts;
        AllowedWithAnyCommandText = false;
    }

    public string[] CommandTexts { get; }

    public bool AllowedWithAnyCommandText { get; set; }
}
namespace Chabot.Command;

public class ActionSelectionMetadata
{
    public ActionSelectionMetadata(string? commandText)
    {
        CommandText = commandText;
    }

    public string? CommandText { get; }
}
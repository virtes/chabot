namespace Chabot.Command;

public interface IActionSelectionMetadataFactory<in TMessage, in TUser>
{
    ActionSelectionMetadata GetMetadata(TMessage message, TUser user);
}
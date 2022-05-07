using Chabot.User;

namespace Chabot.Command;

public interface IActionSelectionMetadataFactory<in TMessage, in TUser, TUserId> 
    where TUser : IUser<TUserId>
{
    ActionSelectionMetadata GetMetadata(TMessage message, TUser user);
}
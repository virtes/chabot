namespace Chabot.User;

public interface IUserIdResolver<in TMessage, in TUser>
{
    object? GetUserId(TMessage message, TUser user);
}
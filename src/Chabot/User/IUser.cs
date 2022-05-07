namespace Chabot.User;

public interface IUser<out TId>
{
    public TId Id { get; }
}
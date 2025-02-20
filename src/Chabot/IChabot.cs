namespace Chabot;

public interface IChabot<in TUpdate>
{
    Task HandleUpdate(TUpdate update);
}
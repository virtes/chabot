namespace Chabot;

public interface IMiddleware<TUpdate>
{
    Task Invoke(UpdateContext<TUpdate> context,
        HandleUpdate<TUpdate> next);
}
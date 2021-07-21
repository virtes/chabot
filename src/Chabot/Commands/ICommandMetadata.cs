namespace Chabot.Commands
{
    public interface ICommandMetadata
    {
        T[] GetOrderedMetadata<T>();
    }
}
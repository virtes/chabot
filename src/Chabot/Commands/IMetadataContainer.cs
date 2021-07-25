namespace Chabot.Commands
{
    public interface IMetadataContainer
    {
        T[] GetAllOfType<T>() where T : class;
    }
}
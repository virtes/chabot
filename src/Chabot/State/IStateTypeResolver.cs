namespace Chabot.State;

public interface IStateTypeResolver
{
    string GetTypeKey(Type type);

    Type GetType(string typeKey);
}
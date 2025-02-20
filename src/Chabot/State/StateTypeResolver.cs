namespace Chabot.State;

internal class StateTypeResolver : IStateTypeResolver
{
    public string GetTypeKey(Type type)
    {
        return type.AssemblyQualifiedName!;
    }

    public Type GetType(string typeKey)
    {
        var type = Type.GetType(typeKey);
        if (type is null)
            throw new InvalidOperationException("Type not found");

        return type;
    }
}
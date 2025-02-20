namespace Chabot.State;

public interface ISerializableStateStorageBuilder<T>
{
    ISerializableStateStorageBuilder<T> UseSerializer(
        Func<IServiceProvider, IStateSerializer<T>> stateSerializerFactory);
}
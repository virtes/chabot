namespace Chabot.Proxy;

public interface IUpdatePartitioner<in TUpdate>
{
    byte[] GetPartitionKey(TUpdate update);
}
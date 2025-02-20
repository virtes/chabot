namespace Chabot.Proxy;

internal class UpdateProxySenderMiddleware<TUpdate, TSerializedUpdate> : IMiddleware<TUpdate>
{
    private readonly IUpdateSerializer<TUpdate, TSerializedUpdate> _updateSerializer;
    private readonly IUpdateProxySender<TSerializedUpdate> _updateProxySender;
    private readonly IUpdatePartitioner<TUpdate> _updatePartitioner;

    public UpdateProxySenderMiddleware(
        IUpdateSerializer<TUpdate, TSerializedUpdate> updateSerializer,
        IUpdateProxySender<TSerializedUpdate> updateProxySender,
        IUpdatePartitioner<TUpdate> updatePartitioner)
    {
        _updateSerializer = updateSerializer;
        _updateProxySender = updateProxySender;
        _updatePartitioner = updatePartitioner;
    }

    public async Task Invoke(UpdateContext<TUpdate> context, HandleUpdate<TUpdate> next)
    {
        var partitionKey = _updatePartitioner.GetPartitionKey(context.Update);
        var serializedUpdate = _updateSerializer.Serialize(context.Update);

        await _updateProxySender.SendUpdate(partitionKey, serializedUpdate);

        await next(context);
    }
}
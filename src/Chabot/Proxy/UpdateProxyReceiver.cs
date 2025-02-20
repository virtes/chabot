namespace Chabot.Proxy;

internal class UpdateProxyReceiver<TUpdate, TSerializedUpdate> : IUpdateProxyReceiver<TSerializedUpdate>
{
    private readonly IUpdateSerializer<TUpdate, TSerializedUpdate> _updateSerializer;
    private readonly IChabot<TUpdate> _update;

    public UpdateProxyReceiver(
        IUpdateSerializer<TUpdate, TSerializedUpdate> updateSerializer,
        IChabot<TUpdate> update)
    {
        _updateSerializer = updateSerializer;
        _update = update;
    }

    public async Task UpdateReceived(TSerializedUpdate serializedUpdate)
    {
        var update = _updateSerializer.Deserialize(serializedUpdate);

        await _update.HandleUpdate(update);
    }
}
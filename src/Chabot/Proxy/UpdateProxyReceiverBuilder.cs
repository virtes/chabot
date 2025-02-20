using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Proxy;

internal class UpdateProxyReceiverBuilder<TUpdate> : IUpdateProxyReceiverBuilder<TUpdate>
{
    public IChabotBuilder<TUpdate> ChabotBuilder { get; }

    private bool _configured = false;

    public UpdateProxyReceiverBuilder(IChabotBuilder<TUpdate> chabotBuilder)
    {
        ChabotBuilder = chabotBuilder;
    }

    public void Register<TSerializedUpdate>()
    {
        ChabotBuilder.Services.AddScoped<IUpdateProxyReceiver<TSerializedUpdate>, UpdateProxyReceiver<TUpdate, TSerializedUpdate>>();

        ChabotBuilder.ValidateServiceRegistration<IUpdateSerializer<TUpdate, TSerializedUpdate>>();

        _configured = true;
    }

    internal void CheckRegisterCalled()
    {
        if (!_configured)
            throw new InvalidOperationException("Invalid proxy receiver registration");
    }
}
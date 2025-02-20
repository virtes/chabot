using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Proxy;

internal class UpdateProxySenderBuilder<TUpdate> : IUpdateProxySenderBuilder<TUpdate>
{
    private bool _configured = false;

    public UpdateProxySenderBuilder(IChabotBuilder<TUpdate> chabotBuilder)
    {
        ChabotBuilder = chabotBuilder;
    }

    public IChabotBuilder<TUpdate> ChabotBuilder { get; }

    public void Register<TSerializedUpdate>()
    {
        ChabotBuilder.Services.AddScoped<UpdateProxySenderMiddleware<TUpdate, TSerializedUpdate>>();
        ChabotBuilder.UseMiddleware<UpdateProxySenderMiddleware<TUpdate, TSerializedUpdate>>();

        ChabotBuilder.ValidateServiceRegistration<IUpdateSerializer<TUpdate, TSerializedUpdate>>();
        ChabotBuilder.ValidateServiceRegistration<IUpdateProxySender<TSerializedUpdate>>();
        ChabotBuilder.ValidateServiceRegistration<IUpdatePartitioner<TUpdate>>();

        _configured = true;
    }

    internal void CheckRegisterCalled()
    {
        if (!_configured)
            throw new InvalidOperationException("Invalid proxy sender registration");
    }
}
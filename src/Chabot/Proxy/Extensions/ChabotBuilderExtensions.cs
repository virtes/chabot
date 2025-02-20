// ReSharper disable once CheckNamespace
namespace Chabot.Proxy;

public static class ChabotBuilderExtensions
{
    public static IChabotBuilder<TUpdate> UseProxySender<TUpdate>(
        this IChabotBuilder<TUpdate> chabotBuilder,
        Action<IUpdateProxySenderBuilder<TUpdate>> configureProxyBuilder)
    {
        var builder = new UpdateProxySenderBuilder<TUpdate>(chabotBuilder);
        configureProxyBuilder(builder);

        builder.CheckRegisterCalled();

        return chabotBuilder;
    }

    public static IChabotBuilder<TUpdate> UseProxyReceiver<TUpdate>(
        this IChabotBuilder<TUpdate> chabotBuilder,
        Action<IUpdateProxyReceiverBuilder<TUpdate>> configureProxyBuilder)
    {
        var builder = new UpdateProxyReceiverBuilder<TUpdate>(chabotBuilder);
        configureProxyBuilder(builder);

        builder.CheckRegisterCalled();

        return chabotBuilder;
    }
}
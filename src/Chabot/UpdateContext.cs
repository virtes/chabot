using Chabot.Commands;

namespace Chabot;

public class UpdateContext
{
    protected UpdateContext(IServiceProvider serviceProvider,
        UpdateMetadata updateMetadata)
    {
        ServiceProvider = serviceProvider;
        UpdateMetadata = updateMetadata;
    }

    public IServiceProvider ServiceProvider { get; }
    public UpdateMetadata UpdateMetadata { get; }
}

public class UpdateContext<TUpdate> : UpdateContext
{
    public UpdateContext(IServiceProvider serviceProvider,
        UpdateMetadata updateMetadata, TUpdate update)
        : base(serviceProvider, updateMetadata)
    {
        Update = update;
    }

    public TUpdate Update { get; }
}
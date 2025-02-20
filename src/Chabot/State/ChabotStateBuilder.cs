using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chabot.State;

public abstract class ChabotStateBuilder
{
    protected ChabotStateBuilder(IChabotBuilder chabotBuilder)
    {
        ChabotBuilder = chabotBuilder;
    }

    public IChabotBuilder ChabotBuilder { get; }

    public abstract void RegisterChabotStateServices<TSerializedState>();
}

public class ChabotStateBuilder<TUpdate> : ChabotStateBuilder
{
    public ChabotStateBuilder(IChabotBuilder chabotBuilder) : base(chabotBuilder)
    {
    }

    private bool _configured = false;

    public override void RegisterChabotStateServices<TSerializedState>()
    {
        ChabotBuilder.Services.TryAddScoped<IStateAccessor, StateAccessor<TSerializedState>>();
        ChabotBuilder.Services.TryAddScoped<IStateTypeResolver, StateTypeResolver>();

        ChabotBuilder.Services.AddScoped<ICommandRestrictionHandler<TUpdate, StateRestriction>, StateCommandRestrictionHandler<TUpdate>>();
        ChabotBuilder.Services.AddSingleton<IRestrictionsFactory, StateRestrictionsFactory>();

        ChabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TUpdate>, StateCommandParameterValueResolverFactory<TUpdate>>();
        ChabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TUpdate>, FromMessageTextParameterValueResolverFactory<TUpdate>>();
        ChabotBuilder.Services.AddSingleton<ICommandParameterValueResolverFactory<TUpdate>, FromServicesParameterValueResolverFactory<TUpdate>>();

        ChabotBuilder.ValidateServiceRegistration<IStateSerializer<TSerializedState>>();
        ChabotBuilder.ValidateServiceRegistration<IStateStorage<TSerializedState>>();
        ChabotBuilder.ValidateServiceRegistration<IStateTargetResolverFactory<TUpdate>>();

        _configured = true;
    }

    internal void CheckRegisterCalled()
    {
        if (!_configured)
        {
            throw new InvalidOperationException("Invalid state registration");
        }
    }
}
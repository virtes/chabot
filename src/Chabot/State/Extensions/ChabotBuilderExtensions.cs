// ReSharper disable once CheckNamespace
namespace Chabot.State;

public static partial class ChabotBuilderExtensions
{
    public static IChabotBuilder<TUpdate> AddState<TUpdate>(
        this IChabotBuilder<TUpdate> builder,
        Action<ChabotStateBuilder<TUpdate>> configureBuilder)
    {
        var stateBuilder = new ChabotStateBuilder<TUpdate>(builder);
        configureBuilder(stateBuilder);

        stateBuilder.CheckRegisterCalled();

        return builder;
    }
}
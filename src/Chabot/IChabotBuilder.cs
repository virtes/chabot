using Microsoft.Extensions.DependencyInjection;

namespace Chabot;

public interface IChabotBuilder
{
    public IServiceCollection Services { get; }

    public void ValidateServiceRegistration<TService>();
}

public interface IChabotBuilder<TUpdate> : IChabotBuilder
{
    IChabotBuilder<TUpdate> Use(Func<HandleUpdate<TUpdate>, HandleUpdate<TUpdate>> use);

    IChabotBuilder<TUpdate> UseMiddleware<TMiddleware>()
        where TMiddleware : class, IMiddleware<TUpdate>;
}
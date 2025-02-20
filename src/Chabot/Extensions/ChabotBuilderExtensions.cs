// ReSharper disable once CheckNamespace
namespace Chabot;

public static partial class ChabotBuilderExtensions
{
    public static IChabotBuilder<TUpdate> UseMiddleware<TUpdate>(
        this IChabotBuilder<TUpdate> builder,
        IMiddleware<TUpdate> middleware)
    {
        builder.Use(next =>
        {
            return async context =>
            {
                await middleware.Invoke(context, next);
            };
        });

        return builder;
    }
}
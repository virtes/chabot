using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chabot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChabot<TUpdate>(this IServiceCollection services,
        Action<IChabotBuilder<TUpdate>> configureBuilder)
    {
        var builder = new ChabotBuilder<TUpdate>(services);
        configureBuilder(builder);

        builder.Register();

        return services;
    }

    public static OptionsBuilder<TOptions> BindOptions<TOptions>(this IServiceCollection services,
        Action<TOptions>? configureOptions) where TOptions : class
    {
        var optionsBuilder = services.AddOptions<TOptions>();

        if (configureOptions is not null)
            optionsBuilder = optionsBuilder.Configure(configureOptions);

        return optionsBuilder
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}

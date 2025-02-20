using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Chabot.Commands;

public class CommandsBuilder<TUpdate>
{
    public IChabotBuilder<TUpdate> ChabotBuilder { get; }

    public CommandsBuilder(IChabotBuilder<TUpdate> chabotBuilder)
    {
        ChabotBuilder = chabotBuilder;
    }

    public void ScanCommandsFromAssembly(Assembly assembly)
    {
        ChabotBuilder.Services.Configure<CommandOptions>(c => c.AssembliesToScanCommands.Add(assembly));
    }

    internal void Register()
    {
        ChabotBuilder.Services.AddScoped<IRestrictionsFactory, UpdatePropertiesRestrictionsFactory>();
        ChabotBuilder.Services.TryAddSingleton<ICommandRestrictionHandler<TUpdate, UpdatePropertiesRestriction>, UpdatePropertiesCommandRestrictionHandler<TUpdate>>();
        ChabotBuilder.Services.AddScoped<ICommandResolver<TUpdate>, CommandResolver<TUpdate>>();
        ChabotBuilder.Services.AddSingleton<CommandResolver<TUpdate>.RestrictionHandlersCache>();
        ChabotBuilder.Services.AddScoped<ICommandsProvider, CommandsProvider<TUpdate>>();
        ChabotBuilder.Services.AddSingleton<CommandsProvider<TUpdate>.CommandsCache>();
        ChabotBuilder.Services.AddScoped<ICommandMetadataParser, CommandMetadataParser>();
        ChabotBuilder.Services.AddScoped<ICommandActionExecutor<TUpdate>, CommandActionExecutor<TUpdate>>();
        ChabotBuilder.Services.AddSingleton<CommandActionExecutor<TUpdate>.CommandActionsCache>();
        ChabotBuilder.Services.AddSingleton<ICommandActionBuilder<TUpdate>, CommandActionBuilder<TUpdate>>();

        ChabotBuilder.Services.TryAddScoped<CommandInvokerMiddleware<TUpdate>>();
        ChabotBuilder.UseMiddleware<CommandInvokerMiddleware<TUpdate>>();
    }
}
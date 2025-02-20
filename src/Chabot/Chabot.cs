using System.Diagnostics;
using Chabot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chabot;

internal class Chabot<TUpdate> : IChabot<TUpdate>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Chabot<TUpdate>> _logger;
    private readonly HandleUpdate<TUpdate> _entrypoint;
    private readonly IUpdateMetadataParser<TUpdate> _updateMetadataParser;

    public Chabot(IServiceScopeFactory serviceScopeFactory,
        ILogger<Chabot<TUpdate>> logger,
        HandleUpdate<TUpdate> entrypoint,
        IUpdateMetadataParser<TUpdate> updateMetadataParser)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _entrypoint = entrypoint;
        _updateMetadataParser = updateMetadataParser;
    }

    public async Task HandleUpdate(TUpdate update)
    {
        using var activity = ChabotActivitySource.ActivitySource.StartActivity();
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var updateMetadata = _updateMetadataParser.ParseUpdateMetadata(update);

        var context = new UpdateContext<TUpdate>(
            update: update,
            serviceProvider: scope.ServiceProvider,
            updateMetadata: updateMetadata);

        try
        {
            await _entrypoint(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while handling chabot update");
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            throw;
        }

        _logger.LogInformation("Chabot update handled");
    }
}
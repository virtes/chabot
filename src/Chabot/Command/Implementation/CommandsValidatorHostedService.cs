using Microsoft.Extensions.Hosting;

namespace Chabot.Command.Implementation;

public class CommandsValidatorHostedService : IHostedService
{
    private readonly ICommandDescriptorsProvider _commandDescriptorsProvider;

    public CommandsValidatorHostedService(
        ICommandDescriptorsProvider commandDescriptorsProvider)
    {
        _commandDescriptorsProvider = commandDescriptorsProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = _commandDescriptorsProvider.GetCommandDescriptors();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
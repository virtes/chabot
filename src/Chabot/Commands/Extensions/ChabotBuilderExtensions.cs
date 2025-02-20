// ReSharper disable once CheckNamespace
namespace Chabot.Commands;

public static partial class ChabotBuilderExtensions
{
    public static IChabotBuilder UseCommands<TUpdate>(this IChabotBuilder<TUpdate> builder,
        Action<CommandsBuilder<TUpdate>> configureBuilder)
    {
        var commandsBuilder = new CommandsBuilder<TUpdate>(builder);
        configureBuilder(commandsBuilder);

        commandsBuilder.Register();

        return builder;
    }
}
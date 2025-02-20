using Chabot.State;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Chabot.Telegram.State;

internal class StateTargetResolverFactory : IStateTargetResolverFactory<Update>
{
    private readonly IServiceProvider _serviceProvider;

    public StateTargetResolverFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IStateTargetResolver<Update> CreateStateTargetResolver(string stateTargetType)
    {
        return stateTargetType switch
        {
            TelegramStateTargetTypes.Chat => _serviceProvider.GetRequiredService<ChatStateTargetResolver>(),
            TelegramStateTargetTypes.ChatMessage => _serviceProvider.GetRequiredService<ChatMessageStateTargetResolver>(),
            _ => throw new ArgumentOutOfRangeException(nameof(stateTargetType), stateTargetType, "Invalid state target type")
        };
    }
}
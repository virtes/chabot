using Chabot.State;

// ReSharper disable once CheckNamespace
namespace Chabot;

public class DefaultState : IState
{
    public static readonly DefaultState Instance = new DefaultState();
}
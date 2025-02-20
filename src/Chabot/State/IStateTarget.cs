namespace Chabot.State;

public interface IStateTarget
{
    public string Key { get; }

    public string TargetType { get; }
}
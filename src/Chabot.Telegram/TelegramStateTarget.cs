namespace Chabot.Telegram;

public readonly struct TelegramStateTarget : IEquatable<TelegramStateTarget>
{
    public TelegramStateTarget(long userId, long chatId, int? messageId)
    {
        UserId = userId;
        ChatId = chatId;
        MessageId = messageId;
    }

    public readonly long UserId;

    public readonly long ChatId;

    public readonly int? MessageId;

    public bool Equals(TelegramStateTarget other)
        => UserId == other.UserId
           && ChatId == other.ChatId
           && MessageId == other.MessageId;

    public override bool Equals(object? obj)
        => obj is TelegramStateTarget other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(UserId, ChatId, MessageId);
}
using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal record AllowedMessageTypeRestriction(MessageType[] MessageTypes);
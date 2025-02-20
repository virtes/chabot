using Telegram.Bot.Types.Enums;

namespace Chabot.Telegram.Commands;

internal record AllowedUpdateTypeRestriction(UpdateType[] UpdateTypes);
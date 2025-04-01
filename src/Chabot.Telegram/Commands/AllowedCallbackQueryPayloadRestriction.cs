namespace Chabot.Telegram.Commands;

internal record AllowedCallbackQueryPayloadRestriction(
    string[] AllowedPayloads,
    bool UseQueryParameters);
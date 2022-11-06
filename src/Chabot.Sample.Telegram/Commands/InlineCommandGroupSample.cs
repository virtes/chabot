using System.Collections.Concurrent;
using Chabot.Command;
using Chabot.Sample.Telegram.States.Settings;
using Chabot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Chabot.Sample.Telegram.Commands;

public class InlineCommandGroupSample : TelegramCommandGroup
{
    // Sample feature state storage
    private static readonly ConcurrentDictionary<long, bool> FeatureAStatus = new();

    [Command(CommandCodes.Settings)]
    [AllowedInAnyState]
    public async Task GetSettings()
    {
        var message = await BotClient.SendTextMessageAsync(ChatId,
            text: "Settings:",
            replyToMessageId: MessageId,
            replyMarkup: GetSettingsKeyboard());

        await SetMessageState(InlineSettingsDefaultState.Instance, message);
    }

    [Command(CommandCodes.SettingsSubgroup)]
    [AllowedState(typeof(InlineSettingsDefaultState))]
    public async Task SettingsSubgroup()
    {
        await BotClient.EditMessageReplyMarkupAsync(ChatId, MessageId,
            GetSubgroupSettingsKeyboard());

        await SetState(InlineSettingsSubgroupState.Instance);
    }

    [Command(CommandCodes.BackToSettings)]
    [AllowedState(typeof(InlineSettingsSubgroupState))]
    public async Task BackToSettings()
    {
        await BotClient.EditMessageReplyMarkupAsync(ChatId, MessageId,
            GetSettingsKeyboard());

        await SetState(InlineSettingsDefaultState.Instance);
    }

    [Command(CommandCodes.SettingsEnableFeatureA)]
    [AllowedState(typeof(InlineSettingsSubgroupState))]
    public async Task EnableFeatureA()
    {
        FeatureAStatus[UserId] = true;

        await BotClient.EditMessageReplyMarkupAsync(ChatId, MessageId,
            GetSubgroupSettingsKeyboard());
    }

    [Command(CommandCodes.SettingsDisableFeatureA)]
    [AllowedState(typeof(InlineSettingsSubgroupState))]
    public async Task DisableFeatureA()
    {
        FeatureAStatus[UserId] = false;

        await BotClient.EditMessageReplyMarkupAsync(ChatId, MessageId,
            GetSubgroupSettingsKeyboard());
    }

    private static InlineKeyboardMarkup GetSettingsKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new InlineKeyboardButton("Some subgroup")
            {
                CallbackData = CommandCodes.SettingsSubgroup
            }
        });
    }

    private InlineKeyboardMarkup GetSubgroupSettingsKeyboard()
    {
        var featureAButton = FeatureAStatus.GetValueOrDefault(UserId, false)
            ? new InlineKeyboardButton("Disable feature A")
            {
                CallbackData = CommandCodes.SettingsDisableFeatureA
            }
            : new InlineKeyboardButton("Enable feature A")
            {
                CallbackData = CommandCodes.SettingsEnableFeatureA
            };

        return new InlineKeyboardMarkup(new []
        {
            new[]
            {
                featureAButton
            },
            new[]
            {
                new InlineKeyboardButton("Back to settings")
                {
                    CallbackData = CommandCodes.BackToSettings
                }
            }
        });
    }
}
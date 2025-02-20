# chabot

| | Integrations |
| --- | --- |
| **Build** | [![chabot-ci](https://github.com/virtes/chabot/actions/workflows/chabot-ci.yml/badge.svg)](https://github.com/virtes/chabot/actions/workflows/chabot-ci.yml) |
| **Coverage** | ![Codecov branch](https://img.shields.io/codecov/c/github/virtes/chabot/master) |
| **Quality** | [![CodeFactor](https://www.codefactor.io/repository/github/virtes/chabot/badge)](https://www.codefactor.io/repository/github/virtes/chabot) | 

Chabot - library that helps you to build stateful chat bots easily in ASP.NET MVC-style.

Messenger-idempotent. Currently supported messengers:
- Telegram (using https://github.com/TelegramBots/Telegram.Bot)

## Quick start

Register required chabot services (e.g. using **Telegram** messenger with **in memory** state storage):
```csharp
services.AddTelegramChabot((c, _) => c.Token = <BOT_TOKEN>, c =>
{
    // Specify to listen telegran using long polling updates
    c.UseTelegramPollingUpdates();
    
    // Use user state 
    c.UseState(s => s
        .UseSystemTextJsonSerializer()
        // Use in memory state storage
        .UseInMemoryStateStorage((_, user) => user.Id));
    
    // Use commands
    c.UseCommands();
});
```
Define a command:
```csharp
public class HelloWorldCommandGroup : TelegramCommandGroup
{
    // Command is allowed with any message
    [Command(AllowedWithAnyCommandText = true)]
    // Command is allowed in any state
    [AllowedInAnyState]
    public async Task HelloCommand(
        // Automatically binded message text
        [FromMessageText]string messageText)
    {
        await BotClient.SendTextMessageAsync(ChatId, $"Hello, {messageText}");
    }
}
```

## Message handling
Chabot is pipeline-based message handling library, where you can define your own middlewares and handlers.

Built-in handling stages:
- `UseState()` - fetches and sets the user's state
- `UseCommands()` - adds message routing to defined commands

You can add your custom middlewares (metrics, logging, whitelists, etc) to message handling pipeline using `UseMiddleware...` methods and `IMiddleware` interface.

Middlewares are executing in order of addition to the pipeline.

## Commands and routing
To define a command you need to create a command group (like ASP.NET controller) using `...CoomandGroupType` as base type, e.g. `TelegramCommandGroup` and a mark method with `CommandAttribute`.

#### Routing
Available ways to configure message routing:
- `CommandAttribute.AllowedWithAnyCommandText = true` - specify that command can be reached with any command text
- `AllowedInAnyStateAttribute` - specify that command can be reached in any user current state
- `AllowedStateAttribute(Type)` - specify that command can be reached only in specified user state types

Chabot selects most specific command for user's current state and message text. 

#### Commmand method parameters
Chabot supports built-in method parameters binding in following cases:
- User state types - for `IState` parameters (if user is in the specified state)
- Message text - using `FromMessageTextAttribute` attribute
- User ID - using `FromUserIdAttribute` attribute


You can add any other parameter binding rules implementing `ICommandParameterValueResolverFactory` and `ICommandParameterValueResolver` interfaces.

## User state

Chabot allows you to define any custom state type and use it for message routing and persist data between user message.

To define a state type you just need to implement an empty `IState` interface:

`public class SomeState : IState {}`

State is persisted between user messages and can be saved in any storage. Built-in storages:
- In memory storage - call the `UseInMemoryStateStorage()` method to use in memory state storage

You can implement any state storage (e.g. save it to a database) implementing `IStateStorage` interface and registering it. 

## Sample

Sample, where user enters some value, then confirms it in the next message and the server uses a previously entered value.

Commands:
```csharp
public class TestCommandGroup : TelegramCommandGroup
{
    [Command(AllowedWithAnyCommandText = true)]
    [AllowedInAnyState]
    public async Task DefaultCommand()
    {
        await BotClient.SendTextMessageAsync(ChatId,
            "Enter a value", replyToMessageId: MessageId);
        await SetState(new WaitingForValueState());
    }

    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(WaitingForValueState))]
    public async Task FooCommand(
        [FromMessageText]string messageText)
    {
        await BotClient.SendTextMessageAsync(ChatId,
            $"Type /confirm to confirm {messageText}");
        await SetState(new ValueEnteredState
        {
            Value = messageText
        });
    }

    [Command("confirm")]
    [AllowedState(typeof(ValueEnteredState))]
    public async Task ConfirmEnteredValue(ValueEnteredState valueEnteredState)
    {
        var enteredValue = valueEnteredState.Value;
        
        // some logic with previously entered value

        await BotClient.SendTextMessageAsync(ChatId,
            $"{enteredValue} confirmed");
        await SetState(DefaultState.Instance);
    }

    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(ValueEnteredState))]
    public async Task EnteredValueNotConfirmed()
    {
        await BotClient.SendTextMessageAsync(ChatId,
            "Invalid confirmation, type /confirm to confirm");
    }
}
```
Used state types:
```csharp
public class WaitingForValueState : IState
{
}

public class ValueEnteredState : IState
{
    public string Value { get; set; } = default!;
}
```

For more complete samples take a look at the `Chabot.Sample.Telegram`, `Chabot.Sample.Proxy.Listener`, `Chabot.Sample.Proxy.Worker` projects.
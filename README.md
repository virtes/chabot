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
services.AddTelegramChabot(b =>
{
    b.AddTelegramBotClient(host.Configuration.GetSection("Telegram").Bind);
    b.AddTelegramLongPollingListener();

    b.AddState(s => s.UseMemoryCacheStorage());

    b.UseCommands(c => c.ScanCommandsFromAssembly(typeof(Program).Assembly));
});
```
Define a command:
```csharp
public class StartCommands : TelegramCommands
{
    public async Task Start(
        [FromMessageText]string text,
        [FromChatId]long chatId)
    {
        await BotClient.SendTextMessageAsync(chatId, "Hello");
    }
}
```

## Message handling
Chabot is pipeline-based message handling library, so you can define your own middlewares and handlers.

Built-in handling stages:
- `UseCommands()` - adds message routing to defined commands

You can add your custom middlewares (metrics, logging, whitelists, etc) to message handling pipeline using `UseMiddleware...` methods and `IMiddleware` interface.

Middlewares are executing in order of addition to the pipeline.

## Commands and routing
To define a command you need to create a command group (like ASP.NET controller) using `...CommandsBase` as base type, e.g. `TelegramCommands`.

#### Routing
Built-in ways to configure message routing (you can implement your own rules):
- `AllowedMessageText` - specify that command can be reached with specific command text
- `AllowedState` - specify that command can be reached with specific state (Chat state, Message state etc)
- `AllowedUpdateType` - telegram update type filter (`CallbackQuery`, `Message`, `Vote`, etc)
- `Order` - commands ordering (if multiple commands fit the restrictions)

Chabot selects most specific command for user's current state and message text. 

#### Commmand method parameters
Chabot supports built-in method parameters binding in following cases:
- State types - for state parameters (`FromChatMessageState`, `FromChatState`, etc)
- Message text - using `FromMessageTextAttribute` attribute
- Chat ID - using `FromChatIdAttribute` attribute


You can add any other parameter binding rules implementing `ICommandParameterValueResolverFactory` and `ICommandParameterValueResolver` interfaces.

## State

Chabot allows you to define any custom state type for any target (user, chat, message, etc) and use it for message routing and persist data between updates.

State is persisted between user messages and can be saved in any storage. Implemented storages:
- In memory (`Chabot.MemoryCache`)
- Redis (`Chabot.StackExchangeRedis`)
- EF Core (`Chabot.EntityFrameworkCore`)

You can implement any state storage (e.g. save it to a database) implementing `IStateStorage` interface and registering it. 

## Sample

For more complete samples take a look at the `Chabot.Sample.Proxy.Receiver`, `Chabot.Sample.Proxy.Sender` projects.
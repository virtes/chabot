using Chabot;
using Chabot.Proxy.RabbitMq;
using Chabot.Telegram;
using Serilog;
using Serilog.Enrichers.Span;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddTelegramChabot((c, _) => c.Token = host.Configuration["TelegramBotOptions:Token"], c =>
        {
            c.UseRabbitMqWorkerProxy(o => host.Configuration.Bind("RabbitMqProxyOptions", o));

            c.UseState(s => s
                .UseSystemTextJsonSerializer()
                .UseInMemoryStateStorage());

            c.UseCommands();
        });
    })
    .UseSerilog((host, _, configuration) => configuration
        .ReadFrom.Configuration(host.Configuration)
        .Enrich.WithSpan())
    .Build();

await host.RunAsync();
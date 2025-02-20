using Chabot.Commands;
using Chabot.MemoryCache;
using Chabot.NewtonsoftJson;
using Chabot.Proxy;
using Chabot.RabbitMq;
using Chabot.State;
using Chabot.Telegram;
using Serilog;
using Serilog.Enrichers.Span;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddMemoryCache();

        services.AddTelegramChabot(b =>
        {
            b.AddTelegramBotClient(host.Configuration.GetSection("Telegram").Bind);

            b.UseProxyReceiver(r =>
            {
                r.UseRabbitMq(k =>
                {
                    k.Configure(host.Configuration.GetSection("RabbitMq").Bind);
                    k.UseNewtonsoftJsonUpdateSerializer();
                });
            });

            b.AddState(s => s.UseMemoryCacheStorage());

            b.UseCommands(c => c.ScanCommandsFromAssembly(typeof(Program).Assembly));
        });

    })
    .UseSerilog((host, _, configuration) => configuration
        .ReadFrom.Configuration(host.Configuration)
        .Enrich.WithSpan())
    .Build();

await host.RunAsync();
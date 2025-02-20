using Chabot.NewtonsoftJson;
using Chabot.Proxy;
using Chabot.RabbitMq;
using Chabot.Telegram;
using Serilog;
using Serilog.Enrichers.Span;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddTelegramChabot(b =>
        {
            b.AddTelegramBotClient(host.Configuration.GetSection("Telegram").Bind);
            b.AddTelegramLongPollingListener();

            b.UseProxySender(r =>
            {
                r.UseRabbitMq(k =>
                {
                    k.Configure(host.Configuration.GetSection("RabbitMq").Bind);
                    k.UseNewtonsoftJsonUpdateSerializer();
                });
            });
        });
    })
    .UseSerilog((host, _, configuration) => configuration
        .ReadFrom.Configuration(host.Configuration)
        .Enrich.WithSpan())
    .Build();

await host.RunAsync();
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chabot.Configuration.Extensions;
using Chabot.Example.Middlewares;
using Chabot.Messages;
using Chabot.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Example
{
    // Simple message representation. It can be extended for specific platform,
    // e.g attachments and any other information can be added and used while processing the message
    public class SimpleMessage : IMessage
    {
        public string Id { get; set; }

        public string RawText { get; set; }

        public string SenderId { get; set; }

        public IReadOnlyDictionary<string, string> Items { get; set; }
    }

    public class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMessageService, ConsoleMessageService>();

            services.AddSingleton<ExceptionHandlingMiddleware>();
            services.AddSingleton<LoggingMiddleware>();

            services.AddChabot(c =>
            {
                // Configure chabot to process messages of type 'SimpleMessage'
                c.ProcessMessage<SimpleMessage>(m =>
                {
                    // Configure message processing pipeline
                    m.ConfigurePipeline(pipeline =>
                    {
                        // Add some custom middlewares
                        pipeline.UseMiddleware<ExceptionHandlingMiddleware>();
                        pipeline.UseMiddleware<LoggingMiddleware>();
                    });
                });
            });

            await using var serviceProvider = services.BuildServiceProvider();

            // Simple example of interaction with user
            var messageService = serviceProvider.GetRequiredService<IMessageService>();

            // Entry point service for processing messages
            var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor<SimpleMessage>>();

            while (true)
            {
                var newMessage = messageService.GetNewMessage();

                var message = new SimpleMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    RawText = newMessage.Text,
                    SenderId = newMessage.Sender,
                    Items = new Dictionary<string, string>()
                };
                await messageProcessor.ProcessAsync(message);

                Console.WriteLine("-------------");
            }
        }
    }
}
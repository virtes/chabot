using System;
using Chabot.Configuration.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Chabot.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddChabot(this IServiceCollection services,
            Action<ChabotBuilder> configureChabot)
        {
            var chabotBuilder = new ChabotBuilder(services);

            configureChabot(chabotBuilder);

            chabotBuilder.Build();
        }
    }
}
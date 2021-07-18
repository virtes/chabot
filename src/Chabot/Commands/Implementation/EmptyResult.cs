using System;
using System.Threading.Tasks;

namespace Chabot.Commands.Implementation
{
    public class EmptyResult : ICommandResult
    {
        public static readonly EmptyResult Instance = new EmptyResult();

        public ValueTask ExecuteResultAsync(IServiceProvider serviceProvider)
        {
            return ValueTask.CompletedTask;
        }
    }
}
using Chabot.Authorization;
using Chabot.Commands;

namespace Chabot.Example.Commands
{
    public class HelloCommands : CommandGroup
    {
        [AllowAnonymous]
        public ICommandResult Hello()
        {
            return Reply("Hi");
        }
    }
}
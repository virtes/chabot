using System.Collections.Generic;
using System.Reflection;

namespace Chabot.Commands
{
    public interface IMetadataCollector
    {
        IReadOnlyList<object> Collect(MethodInfo methodInfo);
    }
}
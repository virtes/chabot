using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Chabot.Telemetry;

public static class ChabotActivitySource
{
    public const string SourceName = "Chabot.Telemetry.Telemetry.ChabotActivitySource";
    
    internal static readonly ActivitySource Source = new ActivitySource(SourceName);
    
    internal static Activity? StartActivity([CallerMemberName]string name = "", 
        ActivityKind kind = ActivityKind.Internal)
    {
        return Source.StartActivity(name, kind);
    }
}
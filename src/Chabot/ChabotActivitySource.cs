using System.Diagnostics;

namespace Chabot;

public static class ChabotActivitySource
{
    public static ActivitySource ActivitySource { get; } = new ActivitySource(nameof(ChabotActivitySource));
}
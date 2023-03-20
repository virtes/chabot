// ReSharper disable once CheckNamespace
namespace System.Diagnostics;

public static class ActivityExtensions
{
    public static string? GetTraceId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.RootId,
            ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };
    }
    
    public static void SetException(this Activity? activity, Exception exception)
    {
        if (activity is null)
            return;

        activity.SetTag("error", true);
        activity.SetTag("exception.message", exception.Message);
        activity.SetTag("exception.type", exception.GetType().FullName);
    }
}

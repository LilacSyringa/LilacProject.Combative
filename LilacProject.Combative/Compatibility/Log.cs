namespace LilacProject.Combative.Compatibility;


[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Make your accuracy additive instead.", true)]
public static class Log
{
    public static Action<string>? log_warning;

    public static void Warning(string message) => log_warning?.Invoke(message);
}

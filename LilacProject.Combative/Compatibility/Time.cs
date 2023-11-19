using System.Diagnostics.CodeAnalysis;

namespace LilacProject.Combative.Compatibility;

[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Make your accuracy additive instead.", true)]
public static class Time
{
    internal static double Scaled => ScaledTime();
    internal static double ScaledDelta => DeltaTime();

    [NotNull] public static Func<double>? ScaledTime { private get; set; }
    [NotNull] public static Func<double>? DeltaTime { private get; set; }

    public static bool AllDefined => ScaledTime != null && DeltaTime != null;
}
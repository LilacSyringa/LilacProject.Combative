namespace LilacProject.Combative.Effect;

public struct EffectOperation
{
    [Flags]
    public enum Flags : uint
    {
        None = 0,
        Start = 1 << Value.Start,
        Update = 1 << Value.Update,
        Hit = 1 << Value.Hit,
        /// <summary>
        /// Called when the Effect ends. (Must be on <see cref="Instance.Active.EffectInstance.End"/>). Not to be called during clean-up.
        /// </summary>
        End = 1 << Value.End
    }

    /// <summary>
    /// Can't bother writing the same comments when i already wrote the comments of each values in <see cref="Flags"/>.
    /// </summary>
    public enum Value : int
    {
        Start = 0,
        Update = 1,
        Hit = 2,
        End = 3
    }
}

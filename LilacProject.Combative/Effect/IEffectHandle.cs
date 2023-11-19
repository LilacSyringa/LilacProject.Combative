using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Combative.Compatibility;

namespace LilacProject.Combative.Effect;

/// <summary>
/// Interface for effect handles. Examples is the projectile object. You're farely likely to cast this fairly often.
/// </summary>
public interface IEffectHandle : IGameObjectAccess
{
    internal void SetEffectHandle(EffectInstance effect_instance);

    /// <summary>
    /// Call the <see cref="EffectInstance.ForcedCleanup()"/> here.
    /// <br/>
    /// Called when it cycles to the next loop and has to cancel that effect first before overriding it.
    /// </summary>
    internal void ForcedFinalize();

    public void Cleanup();
}
using LilacProject.Combative.Weaponry;

namespace LilacProject.Combative.Effect.Instance.Active;

/// <summary>
/// The EffectInstance does not require a <see cref="IEffectHandle"/> but is still pooled in <see cref="HandledEffectInstancePool"/>. Intended to handle effects in a separate and independent object such as bullets or any non-instantaneous projectiles.
/// <br/>
/// This is also cloned and pooled in <see cref="HandledEffectInstancePool"/>.
/// <br/>
/// <br/>
/// <see cref="EffectHandlePool.AddPools(int)"/> and <see cref="HandledEffectInstancePool.AddPools(int)"/> is likely to be necessary when using this, be sure to call it when taking <see cref="IEffectProjector"/> and you can also get <see cref="EffectHandlePool"/> through <see cref="HandledEffectInstancePool.Handle_pool"/> as well.
/// </summary>
public abstract class UnhandledEffectInstance : EffectInstance
{
    internal void ProjectInternal(in InputInfo projection_data, IWeaponPortion component, IUnit? unit)
    {
        this.unit = unit;
        Project(projection_data, component);
    }
}
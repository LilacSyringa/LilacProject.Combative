using LilacProject.Combative.Weaponry;

namespace LilacProject.Combative.Effect.Instance.Active;

/// <summary>
/// The EffectInstance is handled by a <see cref="IEffectHandle"/> which is also provided by a <see cref="EffectHandlePool"/>. Intended to handle effects in a separate and independent object such as bullets or any non-instantaneous projectiles.
/// <br/>
/// This is also cloned and pooled in <see cref="HandledEffectInstancePool"/>.
/// <br/>
/// <br/>
/// <see cref="EffectHandlePool.AddPools(int)"/> and <see cref="HandledEffectInstancePool.AddPools(int)"/> is likely to be necessary when using this, be sure to call it when taking <see cref="IEffectProjector"/> and you can also get <see cref="EffectHandlePool"/> through <see cref="HandledEffectInstancePool.Handle_pool"/> as well.
/// </summary>
public abstract class HandledEffectInstance : EffectInstance
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected IEffectHandle Handle { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override void Cleanup() => Handle.Cleanup();

    internal void ProjectInternal(in InputInfo projection_data, IWeaponPortion component, IUnit? unit, IEffectHandle handle)
    {
        this.unit = unit;
        this.Handle = handle;
        Project(projection_data, component);
    }
}

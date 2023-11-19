using LilacProject.Combative.Weaponry;

namespace LilacProject.Combative.Effect.Instance.Active;

/// <summary>
/// Directly gives you the <see cref="IEffectProjector"/> without the need for <see cref="HandledEffectInstancePool"/> and <see cref="EffectHandlePool"/>.
/// <br/>
/// This is particularly because the effect occurs in the same frame and thus, can wrap up before there's a chance to even intersect.
/// </summary>
public abstract class DirectEffectInstance : EffectInstance, IEffectProjector
{
    Type IEffectProjector.Effect_type => GetType();

    void IEffectProjector.EffectProjection(in InputInfo projection_data, IWeaponPortion component, IUnit? unit, Action<EffectInstance>? effect_process)
    {
        this.unit = unit;
        effect_process?.Invoke(this);
        Project(projection_data, component);
    }
}

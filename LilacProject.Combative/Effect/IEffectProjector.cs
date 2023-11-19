using LilacProject.Combative.Effect.Instance.Constructs;
using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Combative.Effect.Instance;
using LilacProject.Combative.Weaponry;

namespace LilacProject.Combative.Effect;

/// <summary>
/// Interface which lets you be able to project a certain effect onto the world.
/// <br></br>
/// <br></br>
/// Contains the method <see cref="EffectProjection(IWeaponPortion, IUnit)"/>, a method which calls to create a certain effect, as instructed by the <see cref="EffectInstance"/>.
/// <br></br>
/// Not meant to be implemented directly. This is only implemented in <see cref="HandledEffectInstancePool"/> and <see cref="UnpooledEffectInstance"/>.
/// </summary>
public interface IEffectProjector
{
    /// <summary>
    /// The type of the effect the projector creates.
    /// </summary>
    public Type Effect_type { get; }

    /// <summary>
    /// Projects the effect.
    /// </summary>
    /// <param name="projection_data">The origin, direction, and target of the effect.</param>
    /// <param name="input_act">The input modifier for the previous argument.</param>
    /// <param name="component">The component used to create the effect.</param>
    /// <param name="unit">The user.</param>
    /// <param name="effect_process">Method to process the effect <b>before</b> projection, most likely to modify additives.</param>
    public sealed void EffectProjection(InputInfo projection_data, InputAct? input_act, IWeaponPortion component, IUnit? unit, Action<EffectInstance>? effect_process = null)
    {
        input_act?.Invoke(ref projection_data);
        EffectProjection(projection_data, input_act, component, unit, effect_process);
    }

    /// <summary>
    /// Projects the effect.
    /// </summary>
    /// <param name="projection_data">The origin, direction, and target of the effect.</param>
    /// <param name="component">The component used to create the effect.</param>
    /// <param name="unit">The user.</param>
    /// <param name="effect_process">Method to process the effect <b>before</b> projection, most likely to modify additives.</param>
    public void EffectProjection(in InputInfo projection_data, IWeaponPortion component, IUnit? unit, Action<EffectInstance>? effect_process = null);
}

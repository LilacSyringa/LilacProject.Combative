using LilacProject.Combative.Weaponry.Mechanism;
using LilacProject.Combative.Effect;
using System.Collections.Immutable;

namespace LilacProject.Combative.Weaponry.Additives.Active.Defaults;

/// <summary>
/// Additive to project a certain effect.
/// </summary>
[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Make your effect projector instead.", true)]
public sealed class EffectProjectorAdditive : WeaponAdditive
{
    /// <summary>
    /// Construct a new EffectAdditive. An additive responsible to fire off an effect.
    /// </summary>
    /// <param name="weapon_operation">The operation to fire the effect. Yes, even on hand at every update why not.</param>
    /// <param name="weapon_component">The weapon component the additive will be based on.</param>
    public EffectProjectorAdditive(WeaponOperation.Flags weapon_operation, IWeaponPortion weapon_component, int repetitions)
    {
        if (repetitions <= 0) throw new ArgumentOutOfRangeException(nameof(repetitions), "Repetitions cannot be equal or less than 0.");

        this.weapon_operation = weapon_operation;
        this.weapon_component = weapon_component ?? throw new ArgumentNullException(nameof(weapon_component));
        this.repetitions = repetitions;
    }

    protected internal override IEnumerable<(WeaponOperation.Flags, WeapAct)> AdditiveActions
    {
        get
        {
            yield return (weapon_operation, Fire);
        }
    }

    private readonly WeaponOperation.Flags weapon_operation;
    private readonly IWeaponPortion weapon_component;
    private readonly int repetitions;

    private ImmutableArray<IEffectProjector> effect_projectors;
    private InputAct? input_act;

    [WeaponAction(nameof(weapon_operation))]
    private void Fire(IWeaponMechanism mechanism)
    {
        InputInfo info = weapon_component.Input_info;

        foreach (IEffectProjector handle in effect_projectors)
        {
            for (int i = 0; i < repetitions; i++)
            {
                InputInfo projection_info = info;

                input_act?.Invoke(ref projection_info);

                handle.EffectProjection(projection_info, weapon_component, mechanism.User);
            }
        }
    }

    protected internal override void RecieveInputActions(InputAct? input_act) => this.input_act = input_act;

    protected internal override void RecieveProjectorReferences(IEnumerable<IEffectProjector> projectors)
    {
        effect_projectors = projectors.ToImmutableArray();
    }
}

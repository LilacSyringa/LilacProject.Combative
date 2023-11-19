using LilacProject.Combative.Compatibility;

namespace LilacProject.Combative.Weaponry.Mechanism.Active;

/// <summary>
/// Base class of all the weapons which fires.
/// </summary>
[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Use {nameof(IWeaponMechanism)} instead.", true)]
public abstract class FiringMechanism : WeaponMechanism
{
    protected FiringMechanism(double atk_delay, double atk_cooldown, bool semi_auto, bool mandatory_fire)
    {
        this.atk_delay = atk_delay;
        this.atk_cooldown = atk_cooldown;
        this.semi_auto = semi_auto;
        this.mandatory_fire = mandatory_fire;

        state_delayed = new DelayedState(this);
        state_inactive = new InactiveState(this);
    }

    /// <summary>
    /// Wind-up time.
    /// </summary>
    protected readonly double atk_delay;
    /// <summary>
    /// Cooldown time.
    /// </summary>
    protected readonly double atk_cooldown;
    /// <summary>
    /// Dictates that the attack has to be pressed again.
    /// </summary>
    protected readonly bool semi_auto;
    /// <summary>
    /// Combined with atk_delay. Dictates that the attack may not be cancelled upon starting.
    /// </summary>
    protected readonly bool mandatory_fire;

    protected readonly DelayedState state_delayed;
    protected readonly InactiveState state_inactive;

    protected abstract WeaponState State_active { get; set; }

    protected sealed override WeaponState State_default => state_inactive;

    protected sealed class InactiveState : WeaponState
    {
        public InactiveState(FiringMechanism mechanism) => this.mechanism = mechanism;

        private readonly FiringMechanism mechanism;

        public override void Passive()
        {
            if (!mechanism.on_use) return;

            if (mechanism.atk_delay == 0) mechanism.ChangeWeaponState(mechanism.State_active);
            else mechanism.ChangeWeaponState(mechanism.state_delayed);
        }
    }

    protected sealed class DelayedState : WeaponState
    {
        public DelayedState(FiringMechanism mechanism) => this.mechanism = mechanism;
        private readonly FiringMechanism mechanism;

        public sealed override void Passive()
        {
            if (!mechanism.mandatory_fire && !mechanism.on_use)
            {
                mechanism.ChangeWeaponState(mechanism.state_inactive);
                return;
            }

            if (Time.Scaled < mechanism.state_start + mechanism.atk_delay) return;

            mechanism.ChangeWeaponState(mechanism.State_active);
        }
    }
}
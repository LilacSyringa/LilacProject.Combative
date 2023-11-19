using LilacProject.Combative.Compatibility;

namespace LilacProject.Combative.Weaponry.Mechanism.Active.Defaults;

/// <summary>
/// Weapons which attacks of any kind without the need to reload or dependence to ammunition.
/// </summary>
[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Use {nameof(IWeaponMechanism)} instead.", true)]
public sealed class FiringShotMechanism : FiringMechanism
{
    public FiringShotMechanism(double atk_delay, double atk_cooldown, bool semi_auto, bool mandatory_fire) : base(atk_delay, atk_cooldown, semi_auto, mandatory_fire)
    {
        State_active = new ActiveState(this);
    }

    protected sealed override WeaponState State_active { get; set; }

    private sealed class ActiveState : WeaponState
    {
        public ActiveState(FiringShotMechanism mechanism) => this.mechanism = mechanism;

        private readonly FiringShotMechanism mechanism;

        public override void Passive()
        {
            if (Time.Scaled < mechanism.last_use + mechanism.atk_cooldown) return;

            if (mechanism.on_use)
            {
                mechanism.last_use = Time.Scaled;
                WeaponOperationException.Throw("ConstantFire");
            }
            else mechanism.ChangeWeaponState(mechanism.state_inactive);
        }

        public override void EnterState()
        {
            mechanism.last_use = Time.Scaled;
            WeaponOperationException.Throw("InitialFire");
            WeaponOperationException.Throw("ConstantFire");
        }
    }
}

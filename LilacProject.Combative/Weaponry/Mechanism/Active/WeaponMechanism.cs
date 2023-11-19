using LilacProject.Combative.Weaponry.Additives;
using LilacProject.Combative.Compatibility;
using System.Diagnostics.CodeAnalysis;

namespace LilacProject.Combative.Weaponry.Mechanism.Active;

[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Use {nameof(IWeaponMechanism)} instead.", true)]
public abstract class WeaponMechanism : IWeaponMechanism
{
    public required ActiveWeaponry Root { get; init; }
    public AdditiveActions Additives => Root.additives;
    public IUnit? User => Root.user;

    public bool on_hand;
    public bool on_use;

    protected double state_start;
    protected double last_use;

    [NotNull] private WeaponState? state_current = null;

    protected abstract WeaponState State_default { get; }
    public double Last_use => last_use;

    protected void ChangeWeaponState(WeaponState new_state)
    {
        if (state_current == new_state) return;

        state_current.ExitState();
        state_current = new_state;

        state_start = Time.Scaled;

        state_current.EnterState();
    }
    public void ResetDefaultCurrentState() => state_current = State_default;

    void IWeaponMechanism.Update()
    {
        if (!on_hand)
        {
            state_current.OffHandPassive();
            Additives.OffHandPassive(this);
        }
        else
        {
            state_current.OnHandPassive();
            Additives.OnHandPassive(this);
        }

        state_current.Passive();
        Additives.Passive(this);
    }

    void IWeaponMechanism.Unequip()
    {
        state_current.Unequip();
        Additives.Unequip(this);
    }

    void IWeaponMechanism.Equip()
    {
        state_current.Equip();
        Additives.Equip(this);
    }

    void IWeaponMechanism.Drop()
    {
        state_current.Drop();
        Additives.Drop(this);
    }

    void IWeaponMechanism.Pickup()
    {
        state_current.Pickup();
        Additives.Pickup(this);
    }

    /// <summary>
    /// Base class for all the possible states that a weapon may have.
    /// </summary>
    protected abstract class WeaponState
    {
        public virtual void EnterState() { }
        public virtual void ExitState() { }

        public virtual void Passive() { }
        public virtual void OnHandPassive() { }
        public virtual void OffHandPassive() { }

        public virtual void InitialFire() { }
        public virtual void EachFire() { }

        public virtual void TriggerPull() { }
        public virtual void TriggerRelease() { }

        public virtual void Unequip() { }
        public virtual void Equip() { }

        public virtual void Pickup() { }
        public virtual void Drop() { }
    }
}
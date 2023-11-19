namespace LilacProject.Combative.Weaponry.Mechanism;

/// <summary>
/// Interface for mechanisms of each and every weapon.
/// <see cref="Active.WeaponMechanism"/> contains some default functionality but using this is also just as possible.
/// This is primarily used by WeaponAdditives.
/// </summary>
public interface IWeaponMechanism
{
    /// <summary>
    /// Last time the weapon is used or fired from.
    /// </summary>
    public double Last_use { get; }

    /// <summary>
    /// The root object the effects, and particularly the mechanism is tied to.
    /// </summary>
    public ActiveWeaponry Root { get; }

    /// <summary>
    /// The weapon holder and the entity itself.
    /// </summary>
    public sealed IUnit? User => Root.user;

    public void ResetDefaultCurrentState();

    public void Update();

    public void Unequip();

    public void Equip();

    public void Drop();

    public void Pickup();
}

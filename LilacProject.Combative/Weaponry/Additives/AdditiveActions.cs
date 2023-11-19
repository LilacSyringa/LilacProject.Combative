using static LilacProject.Combative.Weaponry.WeaponOperation;
using LilacProject.Combative.Weaponry.Mechanism;
using LilacProject.Miscellaneous;
using System.Runtime.CompilerServices;

namespace LilacProject.Combative.Weaponry.Additives;

/// <summary>
/// Contains all of the additives in each operations for a weapon mechanism.
/// </summary>
public sealed class AdditiveActions
{
    private readonly static int operations = Enum.GetNames<Flags>().Length;

    private readonly WeapAct?[] Integrated_actions;
    private readonly WeapAct?[] Temprary_actions;

    public void Passive(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Passive);
    public void OnHandPassive(IWeaponMechanism mechanism) => Invoke(mechanism, Value.OnHandPassive);
    public void OffHandPassive(IWeaponMechanism mechanism) => Invoke(mechanism, Value.OnHandPassive);

    public void InitialFire(IWeaponMechanism mechanism) => Invoke(mechanism, Value.InitialFire);
    public void EachFire(IWeaponMechanism mechanism) => Invoke(mechanism, Value.EachFire);

    public void TriggerPull(IWeaponMechanism mechanism) => Invoke(mechanism, Value.TriggerPull);
    public void TriggerRelease(IWeaponMechanism mechanism) => Invoke(mechanism, Value.TriggerRelease);

    public void Unequip(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Unequip);
    public void Equip(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Equip);

    public void Pickup(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Pickup);
    public void Drop(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Drop);

    public void Hit(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Hit);
    public void Break(IWeaponMechanism mechanism) => Invoke(mechanism, Value.Break);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Invoke(IWeaponMechanism mechanism, Value index)
    {
        int i = unchecked((int)index);
        Invoke(mechanism, i);
    }

    public void Invoke(IWeaponMechanism mechanism, int operation_id)
    {
        Integrated_actions[operation_id]?.Invoke(mechanism);
        Temprary_actions[operation_id]?.Invoke(mechanism);
    }

    internal AdditiveActions(IEnumerable<(Flags, WeapAct)> weap_methods)
    {
        Integrated_actions = new WeapAct[operations];
        Temprary_actions = new WeapAct[operations];

        AddMethods(weap_methods, Integrated_actions);
    }

    internal void AddTemporaryActions(IEnumerable<(Flags, WeapAct)> weap_methods) => AddMethods(weap_methods, Temprary_actions);

    internal void RemoveTemporaryActions(IEnumerable<(Flags, WeapAct)> weap_methods) => RemoveMethds(weap_methods, Temprary_actions);

    private static void AddMethods(IEnumerable<(Flags, WeapAct)> weap_methods, WeapAct?[] methods)
    {
        foreach ((Flags operation, WeapAct action) in weap_methods)
        {
            foreach (int indexes in BitFlags32.FlagIndexes(unchecked((uint)operation)))
            {
                methods[indexes] += action;
            }
        }
    }

    private static void RemoveMethds(IEnumerable<(Flags, WeapAct)> weap_methods, WeapAct?[] methods)
    {
        foreach ((Flags operation, WeapAct action) in weap_methods)
        {
            foreach (int indexes in BitFlags32.FlagIndexes(unchecked((uint)operation)))
            {
                methods[indexes] -= action;
            }
        }
    }
}

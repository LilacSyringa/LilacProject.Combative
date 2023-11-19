using LilacProject.Combative.Weaponry.Additives.Constructs;
using LilacProject.Combative.Weaponry.Additives.Active;
using LilacProject.Combative.Weaponry.Additives;
using LilacProject.Combative.Weaponry.Mechanism.Constructs;
using LilacProject.Combative.Weaponry.Mechanism;
using System.Collections.Immutable;

namespace LilacProject.Combative.Weaponry;

/// <summary>
/// Active instance of a certain weaponry. 
/// <br/>
/// This class is only responsible to manage the functionality of the weapon. The object structure must be created and known beforehand.
/// <br/>
/// While this does hold multiple mechanism, that does not mean that the weapon is actually meant to contain multiple weapons. 
/// </summary>
public class ActiveWeaponry
{
    /// <summary>
    /// Creates a new, usable weaponry.
    /// </summary>
    /// <param name="portions">The portions of each weapon.</param>
    /// <param name="mechanisms">
    ///     The mechanisms of the weapon. 
    ///     Not to be confused with the fact that it can be multiple weapons at once, this just allows you to have different base functions.
    ///     <br/>
    ///     <br/>
    ///     To specify, having the burst, semi, and full auto option for example.
    /// </param>
    /// <param name="integrated_additives">
    ///     The additives of the weapon. This applies to all mechanisms.
    /// </param>
    public ActiveWeaponry(IList<IWeaponPortion> portions, IList<IWeaponMechanismConstruct> mechanisms, IList<WeaponAdditiveConstruct> integrated_additives)
    {
        if (portions is null) throw new ArgumentNullException(nameof(portions));
        if (mechanisms is null) throw new ArgumentNullException(nameof(mechanisms));
        if (integrated_additives is null) throw new ArgumentNullException(nameof(integrated_additives));

        if (portions.Count == 0) throw new ArgumentException("Cannot have 0 weapon portions.", nameof(portions));
        if (mechanisms.Count == 0) throw new ArgumentException("Cannot have 0 weapon mechanisms.", nameof(mechanisms));

        portions_dict = portions.ToDictionary(pair => pair.ID);

        (IWeaponMechanismConstruct construct, IWeaponMechanism mechanism)[] mechanism_pairs = mechanisms.Select(item => (item, item.ConstructWeaponMechanism(this))).ToArray();
        primary = mechanism_pairs[0].mechanism;

        mechanisms_dict = mechanism_pairs.ToImmutableDictionary(item => item.construct.NameID(), item => item.mechanism);

        for (int i = 0; i < mechanism_pairs.Length; i++)
        {
            mechanism_pairs[i].mechanism.ResetDefaultCurrentState();
        }

        builtin_additives = WeaponBuilder.BuildAdditives(integrated_additives, mechanisms_dict);
        WeaponBuilder.CompleteEffectInitialise(this, builtin_additives);

        additives = WeaponBuilder.GetWeaponAdditives(builtin_additives);

        (global_components, specific_components) = WeaponBuilder.ComponentTypes(integrated_additives.SelectMany(item => item.RequiredComponents(portions)));

        AddComponents(global_components);
        AddComponents(specific_components);
    }


    private readonly ImmutableArray<WeaponAdditive> builtin_additives;

    private readonly HashSet<(Type, IWeaponPortion)> specific_components;
    private readonly HashSet<Type> global_components;

    private readonly ImmutableDictionary<string, IWeaponMechanism> mechanisms_dict;
    private readonly Dictionary<string, IWeaponPortion> portions_dict;

    public readonly AdditiveActions additives;

    private IWeaponMechanism primary;

    /// <summary>
    /// When the weapon is going to be picked up by someone, this must be set.
    /// </summary>
    public IUnit? user;

    public IWeaponPortion this[string key] => portions_dict[key];

    public IWeaponMechanism Current_mechanism => primary;

    public int Mechanisms_count => mechanisms_dict.Count;

    public bool HasWeaponPortion(string name) => portions_dict.ContainsKey(name);
    public bool HasWeaponmechanism(string name) => mechanisms_dict.ContainsKey(name);

    public ImmutableArray<WeaponAdditive> WeaponAdditives => builtin_additives;

    /// <summary>
    /// I'm not a fan of adding (and also removing) weapon portions at runtime. Preferably, the portion should just simply exist even if it's not used that often.
    /// </summary>
    /// <param name="portion"></param>
    public void AddWeaponPortion(IWeaponPortion portion) => portions_dict.Add(portion.ID, portion);
    public void RemoveWeaponPortion(IWeaponPortion portion) => portions_dict.Remove(portion.ID);

    public void SwitchMechanism(IWeaponMechanism target)
    {
        if (target is null) throw new ArgumentNullException(nameof(target));
        if (!mechanisms_dict.ContainsValue(target)) throw new ArgumentException("The argument must actually be a port of the Weapon", nameof(target));

        primary = target;
    }

    public void Update() => primary.Update();

    public void Unequip() => primary.Unequip();

    public void Equip() => primary.Equip();

    public void Drop() => primary.Drop();

    public void Pickup() => primary.Pickup();

    /// <summary>
    /// Global variant. Adds the specified type to all components.
    /// </summary>
    public void AddComponent(Type type)
    {
        foreach (IWeaponPortion portion in portions_dict.Values)
        {
            portion.AddComponent(type);
        }
    }

    /// <summary>
    /// Global variant. Adds the specified type to all components.
    /// </summary>
    public void AddComponents(IEnumerable<Type> types)
    {
        foreach (IWeaponPortion portion in portions_dict.Values)
        {
            foreach (Type type in types)
            {
                portion.AddComponent(type);
            }
        }
    }

    public void AddComponents(IEnumerable<(Type type, IWeaponPortion portion)> component_pairs)
    {
        foreach ((Type type, IWeaponPortion portion) in component_pairs)
        {
            if (!portions_dict.ContainsValue(portion)) throw new InvalidOperationException($"The weapon does not have such weapon portion \"{portion.ID}\".");
            portion.AddComponent(type);
        }
    }

    public void AddComponentsChecked(IEnumerable<(Type type, IWeaponPortion? portion)> component_pairs)
    {
        foreach ((Type type, IWeaponPortion? portion) in component_pairs)
        {
            if (portion == null)
            {
                AddComponent(type);
                continue;
            }

            if (!portions_dict.ContainsValue(portion)) throw new InvalidOperationException($"The weapon does not have such weapon portion \"{portion.ID}\".");
            portion.AddComponent(type);
        }
    }
}

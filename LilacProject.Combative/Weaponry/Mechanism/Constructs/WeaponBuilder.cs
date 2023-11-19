using LilacProject.Combative.Weaponry.Additives.Constructs;
using LilacProject.Combative.Weaponry.Additives.Active;
using LilacProject.Combative.Weaponry.Additives;
using LilacProject.Combative.Effect;
using System.Collections.Immutable;

namespace LilacProject.Combative.Weaponry.Mechanism.Constructs;

internal static class WeaponBuilder
{
    internal static (HashSet<Type>, HashSet<(Type, IWeaponPortion)>) ComponentTypes(IEnumerable<(Type, IWeaponPortion?)> values)
    {
        HashSet<Type> global_components = values.Where(item => item.Item2 == null).Select(item => item.Item1).ToHashSet();
        HashSet<(Type, IWeaponPortion)> specific_components = new HashSet<(Type, IWeaponPortion)>();

        foreach ((Type type, IWeaponPortion? portion) in values)
        {
            if (global_components.Contains(type)) continue;
            if (portion == null) continue;

            specific_components.Add((type, portion));
        }

        return (global_components, specific_components);
    }

    internal static ImmutableArray<WeaponAdditive> BuildAdditives(IList<WeaponAdditiveConstruct> additive_constructs, ImmutableDictionary<string, IWeaponMechanism> mechanisms)
    {
        WeaponAdditive[] weapon_additives = new WeaponAdditive[additive_constructs.Count];

        for (int i = 0;i < weapon_additives.Length; i++)
        {
            weapon_additives[i] = additive_constructs[i].BuildAdditive();
            weapon_additives[i].construct = additive_constructs[i];
        }

        ImmutableArray<WeaponAdditive> immutable_instances = weapon_additives.ToImmutableArray();

        SetAdditiveRecievers(immutable_instances);
        SetMechanismsRecievers(immutable_instances, mechanisms);
        SetProjectorsRecievers(immutable_instances);

        return immutable_instances;
    }

    internal static void CompleteEffectInitialise(ActiveWeaponry root, ImmutableArray<WeaponAdditive> weapon_additives)
    {
        for (int i = 0; i < weapon_additives.Length; i++)
        {
            weapon_additives[i].PostInitialize(root);
        }
    }

    internal static AdditiveActions GetWeaponAdditives(ImmutableArray<WeaponAdditive> weapon_additives)
    {
        static IEnumerable<(WeaponOperation.Flags, WeapAct)> SelecMany(ImmutableArray<WeaponAdditive> weapon_additives)
        {
            for (int i = 0; i < weapon_additives.Length; i++)
            {
                foreach ((WeaponOperation.Flags, WeapAct) item in weapon_additives[i].AdditiveActions) yield return item;
            }
        }

        return new AdditiveActions(SelecMany(weapon_additives));
    }

    private static void SetAdditiveRecievers(ImmutableArray<WeaponAdditive> weapon_additives)
    {
        int len = weapon_additives.Length;

        for (int i = 0; i < len; i++)
        {
            InputAct? input_actions = FilteredInputAct(weapon_additives, weapon_additives[i].construct);
            weapon_additives[i].RecieveInputActions(input_actions);

            IEnumerable<WeaponAdditive> filtered_additives = FilteredWeaponAdditives(weapon_additives, weapon_additives[i].construct);
            weapon_additives[i].RecieveAdditiveReferences(weapon_additives, filtered_additives);
        }
    }

    private static void SetMechanismsRecievers(ImmutableArray<WeaponAdditive> weapon_additives, ImmutableDictionary<string, IWeaponMechanism> mechanisms)
    {
        static IEnumerable<(IWeaponMechanism mechanism, string name)> FilteredMechanisms(WeaponAdditiveConstruct construct, ImmutableDictionary<string, IWeaponMechanism> mechanisms)
        {
            foreach ((string name, IWeaponMechanism mechanism) in mechanisms)
            {
                if (construct.TestMechanismsReferenceFilter(mechanism, name)) yield return (mechanism, name);
            }
        }

        int len = weapon_additives.Length;
        
        for (int i = 0; i < len; i++)
        {
            weapon_additives[i].RecieveMechanismReferences(mechanisms.Select(item => (item.Value, item.Key)), FilteredMechanisms(weapon_additives[i].construct, mechanisms));
        }
    }

    private static void SetProjectorsRecievers(ImmutableArray<WeaponAdditive> weapon_additives)
    {
        static IEnumerable<IEffectProjector> FilteredProjectors(WeaponAdditiveConstruct construct)
        {
            string[] names = construct.ProvideEffectProjector();
            for (int i =0; i < names.Length; i++)
            {
                yield return EffectProjectorBochord.GetProjector(names[i]);
            }
        }

        for (int i = 0; i < weapon_additives.Length; i++)
        {
            weapon_additives[i].RecieveProjectorReferences(FilteredProjectors(weapon_additives[i].construct));
        }
    }

    private static IEnumerable<WeaponAdditive> FilteredWeaponAdditives(ImmutableArray<WeaponAdditive> effect_additives, WeaponAdditiveConstruct target_construct)
    {
        for (int i = 0; i < effect_additives.Length; i++)
        {
            if (!target_construct.TestAdditiveReferenceFilter(effect_additives[i], i)) continue;

            yield return effect_additives[i];
        }
    }

    private static InputAct? FilteredInputAct(ImmutableArray<WeaponAdditive> effect_additives, WeaponAdditiveConstruct target_construct)
    {
        InputAct? input_actions = null;

        for (int i = 0; i < effect_additives.Length; i++)
        {
            if (!target_construct.TestInputActionsFilter(effect_additives[i], i)) continue;

            input_actions += effect_additives[i].InputActions;
        }

        return input_actions;
    }
}

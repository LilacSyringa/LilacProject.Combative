using LilacProject.Combative.Effect.Additives.Constructs;
using LilacProject.Combative.Effect.Additives.Active;
using LilacProject.Combative.Effect.Additives;
using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Combative.Compatibility;
using System.Collections.Immutable;

namespace LilacProject.Combative.Effect.Instance.Constructs;
public static class EffectBuilder
{
    internal static IEffectProjector BuildProjector(IEffectInstanceConstruct constructs, EffectProjectorBochord.CustomEffectInstanceBuilder? custom_builder, object? pooler)
    {
        EffectInstance instance = constructs.BuildInstance();

        if (instance is DirectEffectInstance direct)
        {
            CompleteEffectInitialise(direct, BuildAdditives(constructs));
            return direct;
        }
        else if (instance is UnhandledEffectInstance unhandled)
        {
            return new UnhandledEffectInstancePool(constructs, unhandled);
        }
        else if (instance is HandledEffectInstance handled)
        {
            EffectHandlePool handle_pool;
            if (pooler is Func<IEffectInstanceConstruct, HandledEffectInstance, EffectHandlePool> pool_grabber)
            {
                handle_pool = pool_grabber(constructs, handled);
            }
            else if (pooler is EffectHandlePool handlePool) handle_pool = handlePool;
            else if (pooler == null) throw new ArgumentNullException(nameof(pooler));
            else throw new InvalidCastException();

            return new HandledEffectInstancePool(handle_pool, constructs, handled);
        }
        else if (custom_builder != null) return custom_builder(constructs, instance);
        else throw new InvalidCastException($"You must inherit from {nameof(DirectEffectInstance)}, {nameof(UnhandledEffectInstance)}, {nameof(HandledEffectInstance)} or define a non-null overload for EffectProjectorLibrary.Custom_projectorbuilder");
    }

    /// <summary>
    /// The additives returned here are partially initialised. With the only initialisation methods left are on <see cref="CompleteEffectInitialise(EffectInstance, ImmutableArray{EffectAdditive})"/>.
    /// </summary>
    /// <returns>Pass the result to <see cref="CompleteEffectInitialise(EffectInstance, ImmutableArray{EffectAdditive)"/></returns>
    public static ImmutableArray<EffectAdditive> BuildAdditives(IEffectInstanceConstruct constructs)
    {
        IList<EffectAdditiveConstruct> additive_constructs = constructs.GetEffectAdditiveConstructs();
        int len = additive_constructs.Count;

        EffectAdditive[] effect_additives = new EffectAdditive[len];

        for (int i = 0; i < len; i++)
        {
            effect_additives[i] = additive_constructs[i].BuildAdditive();
            effect_additives[i].construct = additive_constructs[i];
        }

        for (int i = 0; i < len; i++)
        {
            if (effect_additives[i] is not EffectAdditive.ICommonCompositor common_additive) continue;

            if (effect_additives[i].GetType().IsDefined(typeof(EffectAdditive.CommonAdditiveAttribute), true))
            {
                throw new InvalidOperationException($"{effect_additives[i].GetType().Name} implements {nameof(EffectAdditive.ICommonCompositor)} but is also defined with {nameof(EffectAdditive.CommonAdditiveAttribute)} which doesn't make sense because the additive is common to all effect instance in the pool.");
            }

            common_additive.CommonField = common_additive.BuildCommons();
        }

        ImmutableArray<EffectAdditive> immutable_additive = effect_additives.ToImmutableArray();

        SetAdditiveRecievers(immutable_additive);
        SetProjectorsRecievers(immutable_additive);

        return immutable_additive;
    }

    /// <summary>
    /// Create a new batch of additives with respect <see cref="EffectAdditive.CommonAdditiveAttribute"/> and <see cref="EffectAdditive.ICommonCompositor"/>. <see cref="EffectAdditive"/>
    /// <br/>
    /// The additives returned here are partially initialised. With the only initialisation methods left are on <see cref="CompleteEffectInitialise(EffectInstance, ImmutableArray{EffectAdditive})"/>.
    /// </summary>
    /// <returns>Pass the result to <see cref="CompleteEffectInitialise(EffectInstance, ImmutableArray{EffectAdditive})"/></returns>
    public static ImmutableArray<EffectAdditive> BuildAdditivesWithCommons(IEffectInstanceConstruct constructs, ImmutableArray<EffectAdditive> cached_additives)
    {
        static IEnumerable<EffectAdditive> Internal(IList<EffectAdditiveConstruct> effect_constructs, ImmutableArray<EffectAdditive> cached_additives)
        {
            int len = effect_constructs.Count;

            for (int i = 0; i < len; i++)
            {
                if (cached_additives[i].GetType().IsDefined(typeof(EffectAdditive.CommonAdditiveAttribute), true))
                {
                    yield return cached_additives[i];
                    continue;
                }

                EffectAdditive effect_additive = effect_constructs[i].BuildAdditive();
                effect_additive.construct = effect_constructs[i];

                if (effect_additive is EffectAdditive.ICommonCompositor common_additive)
                {
                    EffectAdditive.ICommonCompositor cached_common = (EffectAdditive.ICommonCompositor)cached_additives[i];
                    common_additive.CommonField = cached_common.CommonField;
                }

                yield return effect_additive;
            }
        }

        IList<EffectAdditiveConstruct> additive_constructs = constructs.GetEffectAdditiveConstructs();

        ImmutableArray<EffectAdditive> result = Internal(additive_constructs, cached_additives).ToImmutableArray();

        SetAdditiveRecievers(result);
        SetProjectorsRecievers(result);

        return result;
    }

    private static void SetAdditiveRecievers(ImmutableArray<EffectAdditive> effect_additives)
    {
        int len = effect_additives.Length;

        for (int i = 0; i < len; i++)
        {
            if (effect_additives[i].
                    GetType().
                    IsDefined(typeof(EffectAdditive.CommonAdditiveAttribute), true)
                    )
                continue;

            InputAct? input_actions = FilteredInputAct(effect_additives, effect_additives[i].construct);
            effect_additives[i].RecieveInputActions(input_actions);

            effect_additives[i].RecieveAdditiveReference(effect_additives, FilteredEffectAdditives(effect_additives, effect_additives[i].construct));
        }
    }

    private static void SetProjectorsRecievers(ImmutableArray<EffectAdditive> weapon_additives)
    {
        static IEnumerable<IEffectProjector> FilteredProjectors(EffectAdditiveConstruct construct)
        {
            string[] names = construct.ProvideEffectProjector();
            for (int i = 0; i < names.Length; i++)
            {
                yield return EffectProjectorBochord.GetProjector(names[i]);
            }
        }

        for (int i = 0; i < weapon_additives.Length; i++)
        {
            weapon_additives[i].RecieveProjectorReferences(FilteredProjectors(weapon_additives[i].construct));
        }
    }

    /// <summary>
    /// Places all the <see cref="EffectAdditive"/> to <see cref="EffectInstance"/> and calls the <see cref="EffectAdditive.PostInitialize(EffectInstance)"/> method.
    /// </summary>
    /// <param name="instance">The <see cref="EffectInstance"/> to initialise.</param>
    public static void CompleteEffectInitialise(EffectInstance instance, ImmutableArray<EffectAdditive> effect_additives)
    {
        instance.EffectAdditives = effect_additives;

        instance.SetAdditives(new AdditiveActions(instance.EffectAdditives.SelectMany(item => item.AdditiveActions)));

        for (int i = 0; i < instance.EffectAdditives.Length; i++)
        {
            if (instance.
                    EffectAdditives[i].
                    GetType().
                    IsDefined(typeof(EffectAdditive.CommonAdditiveAttribute), true)
                    )
                continue;

            instance.EffectAdditives[i].PostInitialize(instance);
        }
    }

    private static IEnumerable<EffectAdditive> FilteredEffectAdditives(ImmutableArray<EffectAdditive> effect_additives, EffectAdditiveConstruct target_construct)
    {
        for (int i = 0; i < effect_additives.Length; i++)
        {
            if (!target_construct.TestAdditiveReferenceFilter(effect_additives[i], i)) continue;

            yield return effect_additives[i];
        }
    }

    private static InputAct? FilteredInputAct(ImmutableArray<EffectAdditive> effect_additives, EffectAdditiveConstruct target_construct)
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

using LilacProject.Combative.Effect.Additives.Constructs;
using LilacProject.Combative.Effect.Additives.Active;
using LilacProject.Combative.Effect.Instance.Constructs;
using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Combative.Weaponry;
using System.Collections.Immutable;

namespace LilacProject.Combative.Effect.Instance;

/// <summary>
/// Used with <see cref="UnhandledEffectInstance"/>, designed to cycle around clones of itself.
/// </summary>
public sealed class UnhandledEffectInstancePool : IEffectProjector
{
    internal UnhandledEffectInstancePool(IEffectInstanceConstruct effect_construct, IList<EffectAdditiveConstruct> additive_constructs, UnhandledEffectInstance? first = null)
    {
        this.additive_constructs = additive_constructs;
        this.effect_construct = effect_construct;

        first ??= (UnhandledEffectInstance)effect_construct.BuildInstance();

        ImmutableArray<EffectAdditive> additives = EffectBuilder.BuildAdditives(additive_constructs);
        EffectBuilder.CompleteEffectInitialise(first, additives);

        instances = new(1)
        {
            [0] = first
        };
    }

    private readonly IList<EffectAdditiveConstruct> additive_constructs;
    private readonly IEffectInstanceConstruct effect_construct;
    private readonly List<UnhandledEffectInstance> instances;

    private int index;

    public Type Effect_type => instances[0].GetType();

    /// <summary>
    /// How many Instances in the pool.
    /// </summary>
    public int PoolCount => instances.Count;

    /// <exception cref="ArgumentOutOfRangeException">Count is or less than 0.</exception>
    public void AddPools(int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "Cannot be equal to 0 or lower, this does nothing.");

        for (int i = 0; i < count; i++)
        {
            UnhandledEffectInstance instance = (UnhandledEffectInstance)effect_construct.BuildInstance();
            ImmutableArray<EffectAdditive> additives = EffectBuilder.BuildAdditivesWithCommons(additive_constructs, instances[0].EffectAdditives);
            EffectBuilder.CompleteEffectInitialise(instance, additives);

            instances.Add(instance);
        }
    }

    /// <summary>
    /// Removes pools by a given number. The pool <b>must</b> always have at least 1 <see cref="UnhandledEffectInstance"/> in it.
    /// </summary>
    /// <param name="count">How many objects to remove from the pooling. If the input is greater than the count, it is ignored.</param>
    /// <exception cref="ArgumentOutOfRangeException">Count is or less than 0.</exception>
    /// <returns>The amount of <see cref="UnhandledEffectInstance"/> removed.</returns>
    public int RemovePools(int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "Cannot be equal to 0 or lower, this does nothing.");

        int i = 0;
        for (; i < count && count > 1; i++)
        {
            int j = PoolCount - 1 - i;
            instances[j].ForcedCleanup();
            instances.RemoveAt(j);
        }

        if (index >= PoolCount) index = 0;

        return i;
    }

    void IEffectProjector.EffectProjection(in InputInfo projection_data, IWeaponPortion component, IUnit? unit, Action<EffectInstance>? effect_process)
    {
        UnhandledEffectInstance curr_instance = instances[index];

        curr_instance.ForcedCleanup();

        effect_process?.Invoke(curr_instance);

        curr_instance.ProjectInternal(in projection_data, component, unit);

        index = ++index % PoolCount;
    }
}
using LilacProject.Combative.Effect.Instance.Constructs;
using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Combative.Weaponry;
using LilacProject.Combative.Effect.Additives.Active;
using System.Collections.Immutable;

namespace LilacProject.Combative.Effect.Instance;

/// <summary>
/// Used with <see cref="HandledEffectInstancePool"/>, designed to cycle around clones of itself, they don't contain the handles themselves and so needs <see cref="EffectHandlePool"/>. To get it, use the <see cref="HandledEffectInstancePool.handle_pool"/>
/// </summary>
public sealed class HandledEffectInstancePool : IEffectProjector, IDisposable
{
    internal HandledEffectInstancePool(EffectHandlePool handle_pool, IEffectInstanceConstruct effect_construct, HandledEffectInstance? first = null)
    {
        this.handle_pool = handle_pool;
        this.effect_construct = effect_construct;

        handle_pool.ConsiderAdditiveConstructs(effect_construct.GetEffectAdditiveConstructs());

        first ??= (HandledEffectInstance)effect_construct.BuildInstance();

        ImmutableArray<EffectAdditive> additives = EffectBuilder.BuildAdditives(effect_construct);
        EffectBuilder.CompleteEffectInitialise(first, additives);

        instances = new(1)
        {
            [0] = first
        };

        handle_pool.TriggerInstancePoolSubscribeEvent(this);
    }

    private readonly IEffectInstanceConstruct effect_construct;
    private readonly List<HandledEffectInstance> instances;
    private readonly EffectHandlePool handle_pool;

    private int index;

    public Type Effect_type => instances[0].GetType();

    public EffectHandlePool Handle_pool => handle_pool;

    public int Count => instances.Count;

    public void AddPools(int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "Cannot be equal to 0 or lower, this does nothing.");

        for (int i = 0; i < count; i++)
        {
            HandledEffectInstance instance = (HandledEffectInstance)effect_construct.BuildInstance();
            ImmutableArray<EffectAdditive> additives = EffectBuilder.BuildAdditivesWithCommons(effect_construct, instances[0].EffectAdditives);
            EffectBuilder.CompleteEffectInitialise(instance, additives);

            instances.Add(instance);
        }

        handle_pool.TriggerInstancePoolCountChangeEvent(this, count);
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
            int j = Count - 1 - i;
            instances[j].ForcedCleanup();
            instances.RemoveAt(j);
        }

        if (index >= Count) index = 0;

        if (i > 0) handle_pool.TriggerInstancePoolCountChangeEvent(this, -i);

        return i;
    }

    public void Dispose()
    {
        handle_pool.TriggerInstancePoolUnsubscribeEvent(this);
    }

    void IEffectProjector.EffectProjection(in InputInfo projection_data, IWeaponPortion component, IUnit? unit, Action<EffectInstance>? effect_process)
    {
        HandledEffectInstance curr_instance = instances[index];

        curr_instance.ForcedCleanup();

        IEffectHandle handle = handle_pool.HandleEffectProjection(curr_instance);

        effect_process?.Invoke(curr_instance);

        curr_instance.ProjectInternal(in projection_data, component, unit, handle);

        index = ++index % Count;
    }
}
using LilacProject.Combative.Effect.Additives.Constructs;
using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Combative.Effect.Instance;

namespace LilacProject.Combative.Effect;

/// <summary>
/// Contains pools of effect handles for (multiple) <see cref="HandledEffectInstancePool"/> to use.
/// </summary>
public class EffectHandlePool
{
    /// <summary>
    /// Creates a new EffectHandlePool. Use the <see cref="AddPools(int)"/>, <see cref="RemovePools(int)"/> or <see cref="Count"/> to set the pool size after initialisation.
    /// </summary>
    /// <param name="constructor">Function to construct a new IEffectHandle when it needs to resize.</param>
    /// <param name="destroyer">Function to remove IEffecthandle when downsizing.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EffectHandlePool(Func<IEffectHandle> constructor, Action<IEffectHandle> destroyer)
    {
        this.constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        this.destroyer = destroyer ?? throw new ArgumentNullException(nameof(destroyer));

        handles = new();

        handle_components = new();
    }

    public required Action<EffectHandlePool, HandledEffectInstancePool, int>? InstancePoolCountChangeEvent { get; set; }
    public required Action<EffectHandlePool, HandledEffectInstancePool>? InstancePoolSubscribeEvent { get; set; }
    public required Action<EffectHandlePool, HandledEffectInstancePool>? InstancePoolUnsubscribeEvent { get; set; }


    private readonly Func<IEffectHandle> constructor;
    private readonly Action<IEffectHandle> destroyer;

    private readonly HashSet<Type> handle_components;
    private readonly List<IEffectHandle> handles;

    private int index = 0;
    public int Count
    {
        get => handles.Count;
        set
        {
            int val = value - Count;

            if (val > 0) AddPools(val);
            else if (val < 0) RemovePools(-val);
        }
    }

    public void TriggerInstancePoolCountChangeEvent(HandledEffectInstancePool pool, int count)
    {
        InstancePoolCountChangeEvent?.Invoke(this, pool, count);
    }

    public void TriggerInstancePoolSubscribeEvent(HandledEffectInstancePool pool)
    {
        InstancePoolSubscribeEvent?.Invoke(this, pool);
    }

    public void TriggerInstancePoolUnsubscribeEvent(HandledEffectInstancePool pool)
    {
        InstancePoolUnsubscribeEvent?.Invoke(this, pool);
    }

    public void AddPools(int count)
    {
        for (int i = 0; i < count; i++)
        {
            handles.Add(constructor());
        }
    }

    public void RemovePools(int count)
    {
        for (int i = 1; i <= count && handles.Count > 0; i++)
        {
            destroyer(handles[^i]);
            handles.RemoveAt(handles.Count - i);
        }

        if (index >= handles.Count) index = 0;
    }

    internal IEffectHandle HandleEffectProjection(EffectInstance instance)
    {
        if (handles.Count <= 0) throw new InvalidOperationException("No pools to use, the current pool count is at 0.");

        IEffectHandle handle = handles[index];
        handle.SetEffectHandle(instance);

        index = ++index % Count;

        return handle;
    }

    internal void ConsiderAdditiveConstructs(IEnumerable<EffectAdditiveConstruct> additive_constructs)
    {
        foreach (Type type in additive_constructs.SelectMany(item => item.RequiredComponents()))
        {
            if (!handle_components.Add(type)) continue;

            for (int i = 0; i < handles.Count; i++)
            {
                handles[i].AddComponent(type);
            }
        }
    }
}

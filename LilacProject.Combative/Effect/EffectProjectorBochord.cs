using LilacProject.Combative.Effect.Instance.Constructs;
using LilacProject.Combative.Effect.Instance.Active;

namespace LilacProject.Combative.Effect;

/// <summary>
/// Singleton implementation of the class where all <see cref="IEffectProjector"/> can be accessed through. This is abstract
/// <br/>
/// All the functions related to constructing the effect should be done internally but can be customised for any <see cref="EffectInstance"/> that doesn't inherit from any of the default three. It is through <see cref="Custom_projectorbuilder"/>
/// <br/>
/// Fully constructed <see cref="IEffectProjector"/> should not have its <see cref="Additives.Active.EffectAdditive"/> <b>modified</b>.
/// </summary>
public abstract class EffectProjectorBochord
{
    protected EffectProjectorBochord() => projectors = new();

    protected internal delegate IEffectProjector CustomEffectInstanceBuilder(IEffectInstanceConstruct constructs, EffectInstance first_instance);
    protected internal delegate EffectHandlePool GetEffectHandlePool(IEffectInstanceConstruct constructs, HandledEffectInstance first_instance);

    private static EffectProjectorBochord? instance;
    private readonly static object obj = new object();

    public static EffectProjectorBochord Instance
    {
        get
        {
            if (instance != null) return instance;

            lock (obj)
            {
                if (instance != null) return instance;

                throw new NullReferenceException();
            }
        }
        protected set
        {
            lock (obj)
            {
                instance = value;
            }
        }
    }

    public IEnumerable<IEffectProjector> this[IEnumerable<string> names]
    {
        get
        {
            foreach (string name in names)
            {
                yield return this[name];
            }
        }
    }

    public IEffectProjector this[string name] => projectors[name];

    protected Func<IEffectInstanceConstruct, EffectInstance, IEffectProjector>? custom_projectorbuilder;
    private readonly Dictionary<string, IEffectProjector> projectors;

    /// <summary>
    /// Function to add a new item on the dictionary.
    /// </summary>
    /// <param name="constructs">The constructs that will be used to build a new <see cref="IEffectProjector"/>.</param>
    /// <param name="custom_projectorbuilder">
    ///     This if you are not going to use <see cref="DirectEffectInstance"/>, <see cref="UnhandledEffectInstance"/>, and <see cref="HandledEffectInstance"/>.
    ///     <br/>
    ///     All the functions that handle building the additives are inside of <see cref="IEffectInstanceConstruct"/>.
    /// </param>
    /// <param name="handle_pool">The object that a <see cref="HandledEffectInstance"/> needs to cycle through different <see cref="IEffectHandle"/>. This can be null if it's not expected to be a <see cref="HandledEffectInstance"/></param>
    protected void AddEffectProjector(IEffectInstanceConstruct constructs, CustomEffectInstanceBuilder? custom_projectorbuilder = null, EffectHandlePool? handle_pool = null)
    {
        projectors.Add(constructs.NameID(), EffectBuilder.BuildProjector(constructs, custom_projectorbuilder, handle_pool));
    }

    /// <summary>
    /// Function to add a new item on the dictionary.
    /// </summary>
    /// <param name="constructs">The constructs that will be used to build a new <see cref="IEffectProjector"/>.</param>
    /// <param name="custom_projectorbuilder">
    ///     This if you are not going to use <see cref="DirectEffectInstance"/>, <see cref="UnhandledEffectInstance"/>, and <see cref="HandledEffectInstance"/>.
    ///     <br/>
    ///     All the functions that handle building the additives are inside of <see cref="IEffectInstanceConstruct"/>.
    /// </param>
    /// <param name="pool_grabber">A function which returns a <see cref="EffectHandlePool"/>. This cannot be null if the <see cref="IEffectInstanceConstruct"/> returns a <see cref="HandledEffectInstance"/></param>
    protected void AddEffectProjector(IEffectInstanceConstruct constructs, CustomEffectInstanceBuilder? custom_projectorbuilder = null, GetEffectHandlePool ? pool_grabber = null)
    {
        projectors.Add(constructs.NameID(), EffectBuilder.BuildProjector(constructs, custom_projectorbuilder, pool_grabber));
    }

    public static IEnumerable<IEffectProjector> GetProjectors(IEnumerable<string> names) => Instance[names];

    public static IEffectProjector GetProjector(string name) => Instance[name];
}

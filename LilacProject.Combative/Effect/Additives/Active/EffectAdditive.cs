using LilacProject.Combative.Effect.Additives.Constructs;
using LilacProject.Combative.Effect.Instance.Active;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LilacProject.Combative.Effect.Additives.Active;

/// <summary>
/// Base class for adding additional mechanics for an effect. 
/// Whilst the Instance namespace defines the base functionality for the effect, additives add miscelanous functionalities of any kind.
/// </summary>
public abstract class EffectAdditive
{
    [NotNull] internal EffectAdditiveConstruct? construct = null;

    public EffectAdditiveConstruct Construct => construct;

    public string? Tag
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Construct.GetTag();
    }

    /// <summary>
    /// Indicates that the additive is currently animating. This could be used with a WaitUntil function for any sort of post process before cleaning up the <see cref="IEffectHandle"/>.
    /// </summary>
    public virtual bool IsAnimating => false;

    /// <summary>
    /// Outlet to specify methods to subscribe methods to designated operations. Order should be guaranteed based on the order you laid out each additive in constructs.
    /// </summary>
    protected internal virtual IEnumerable<(EffectOperation.Flags, EffectAct)> AdditiveActions { get { yield break; } }

    /// <summary>
    /// Outlet to specify methods to subscribe to a certain additive, by the recieving additive, to modify <u>Origin</u>, <u>Direction</u>, and <u>Target</u> inputs upon projecting an effect.
    /// </summary>
    protected internal virtual InputAct? InputActions => null;

    #region Initializers
    /// <summary>
    /// All the result of <see cref="InputActions"/> from all the additives an <see cref="EffectInstance"/> has filtered by <see cref="EffectAdditiveConstruct.TestInputActionsFilter(EffectAdditive, int)"/>.  
    /// The order that each additive are passed in and retrieved the <see cref="InputAct"/> should be guaranteed.
    /// </summary>
    protected internal virtual void RecieveInputActions(InputAct? input_act) { }

    /// <summary>
    /// All the additives of a <see cref="EffectInstance"/>. The order that each additive are passed in should be guaranteed.
    /// </summary>
    /// <param name="unfiltered">All the additives of a <see cref="EffectInstance"/>.</param>
    /// <param name="filtered">
    ///     Filtered by <see cref="EffectAdditiveConstruct.TestAdditiveReferenceFilter(EffectAdditive, int)"/>
    ///     <br/>
    ///     This is preferred because you do not have to make a field in here.
    /// </param>
    protected internal virtual void RecieveAdditiveReference(ImmutableArray<EffectAdditive> unfiltered, IEnumerable<EffectAdditive> filtered) { }

    /// <summary>
    /// Recieve <see cref="IEffectProjector"/> provided by <see cref="EffectAdditiveConstruct.ProvideEffectProjector()"/>.
    /// </summary>
    /// <param name="projectors">Provided by <see cref="EffectAdditiveConstruct.ProvideEffectProjector()"/></param>
    protected internal virtual void RecieveProjectorReferences(IEnumerable<IEffectProjector> projectors) { }

    /// <summary>
    /// Last stage to initialise the additive.
    /// </summary>
    /// <param name="effect_instance">The <see cref="EffectInstance"/> the <see cref="EffectAdditive"/> is tied with.</param>
    protected internal virtual void PostInitialize(EffectInstance effect_instance) { }
    #endregion

    /// <summary>
    /// An interface for <see cref="EffectAdditive"/>.
    /// <br/>
    /// Implementing this interface dictates that the additive must have a <u>common</u> object in an <u>EffectInstancePool</u> where each <see cref="EffectInstance"/> are pooled along with <see cref="EffectAdditive"/> that adds to it.
    /// <br/>
    /// Through this interface, a common reference will be constructed and provided by the Constructs.
    /// <br/>
    /// <br/>
    /// Additionally, this is initialised first before any of the other methods of the <see cref="EffectAdditive"/>.
    /// <br/>
    /// <br/>
    /// Note: <see cref="EffectAdditive"/> is still constructed every time a new clone is added, if this is not what you want, see <see cref="CommonAdditiveAttribute"/> where the reference to an <see cref="EffectAdditive"/> itself is common to all instances in the pool.
    /// </summary>
    protected internal interface ICommonCompositor
    {
        protected internal object BuildCommons();

        protected internal object CommonField { get; set; }
    }

    /// <summary>
    /// Specifies that a <see cref="EffectAdditive"/> is common to the sets of <see cref="EffectInstance"/> in an <u>EffectInstancePool</u>.
    /// <br/>
    /// The additive will only be built once and reused to other clones of <see cref="EffectInstance"/> in <u>EffectInstancePool</u>. Additionaly, the order of the <see cref="EffectAdditive"/> is also maintained.
    /// <br/>
    /// <br/>
    /// Because this is used by multiple <see cref="EffectInstance"/>, the additive is completely exempted from all initialisation methods, such as methods that start with <u><b>Receive</b></u> like <see cref="EffectAdditive.RecieveAdditiveReference(ImmutableArray{EffectAdditive}, IEnumerable{EffectAdditive})"/>.
    /// <see cref="EffectAdditive.PostInitialize(EffectInstance)"/> is also not called. 
    /// Additives that are defined by this attribute are expected to only be initialised during the construction of the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    protected internal class CommonAdditiveAttribute : Attribute { }
}

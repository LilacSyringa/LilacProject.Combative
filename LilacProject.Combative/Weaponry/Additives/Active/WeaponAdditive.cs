using LilacProject.Combative.Weaponry.Additives.Constructs;
using LilacProject.Combative.Weaponry.Mechanism;
using LilacProject.Combative.Effect;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LilacProject.Combative.Weaponry.Additives.Active;

/// <summary>
/// Base class for adding additional mechanics for a weaponry. 
/// Whilst the Mechanism namespace defines the base functionality for the operation of the gun, additives add miscelanous functionalities of any kind.
/// </summary>
public abstract class WeaponAdditive
{
    [NotNull] internal WeaponAdditiveConstruct? construct = null;

    public WeaponAdditiveConstruct Construct => construct;

    public string? Tag 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Construct.GetTag();
    }

    /// <summary>
    /// Outlet to specify methods to subscribe methods to designated operations. Order should be guaranteed based on the order you laid out each additive in constructs.
    /// </summary>
    protected internal virtual IEnumerable<(WeaponOperation.Flags, WeapAct)> AdditiveActions { get { yield break; } }

    /// <summary>
    /// Outlet to specify methods to subscribe to a certain additive, by the recieving additive, to modify <u>Origin</u>, <u>Direction</u>, and <u>Target</u> inputs upon projecting an effect.
    /// </summary>
    protected internal virtual InputAct? InputActions => null;

    #region Initializers
    /// <summary>
    /// All the result of <see cref="InputActions"/> from all the additives a <see cref="WeaponAdditive"/> has filtered by <see cref="WeaponAdditiveConstruct.TestInputActionsFilter(WeaponAdditive, int)"/>.  
    /// The order that each additive are passed in and retrieved the <see cref="InputAct"/> should be guaranteed.
    /// </summary>
    protected internal virtual void RecieveInputActions(InputAct? input_act) { }

    /// <summary>
    /// All the additives of the <b>Weaponry</b>. The order that each additive are passed in should be guaranteed.
    /// </summary>
    /// <param name="unfiltered">All the additives of a <b>Weaponry</b>.</param>
    /// <param name="filtered">
    ///     Filtered by <see cref="WeaponAdditiveConstruct.TestAdditiveReferenceFilter(WeaponAdditive, int)"/>
    ///     <br/>
    ///     This is preferred because you do not have to make a field in here.
    /// </param>
    protected internal virtual void RecieveAdditiveReferences(ImmutableArray<WeaponAdditive> unfiltered, IEnumerable<WeaponAdditive> filtered) { }

    protected internal virtual void RecieveMechanismReferences(IEnumerable<(IWeaponMechanism mechanism, string name)> unfiltered, IEnumerable<(IWeaponMechanism mechanism, string name)> filtered) { }

    /// <summary>
    /// Recieve <see cref="IEffectProjector"/> provided by <see cref="WeaponAdditiveConstruct.ProvideEffectProjector()"/>.
    /// </summary>
    /// <param name="projectors">Provided by <see cref="WeaponAdditiveConstruct.ProvideEffectProjector()"/></param>
    protected internal virtual void RecieveProjectorReferences(IEnumerable<IEffectProjector> projectors) { }

    /// <summary>
    /// Last stage to initialise the additive.
    /// </summary>
    /// <param name="root">The <see cref="ActiveWeaponry"/> that all the <see cref="WeaponAdditive"/> and <see cref="IWeaponMechanism"/> are managed in.</param>
    protected internal virtual void PostInitialize(ActiveWeaponry root) { }
    #endregion

    #region Markers
    /// <summary>
    /// This does nothing but help explicitly mark it as it a weapon additive method.
    /// <br/>
    /// I guess you can also use this to get these said methods through reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class WeaponActionAttribute : Attribute
    {
        /// <summary>
        /// Creates an attribute that specifies that it is an additive method.
        /// </summary>
        /// <param name="field">The parameter name used to determine which operation it activates in.</param>
        public WeaponActionAttribute(string field)
        {
            target_operation = null;
            referenced_value = field;
        }
        /// <summary>
        /// Creates an attribute that specifies that it is an additive method.
        /// </summary>
        /// <param name="target_operation">The operation the method activates in.</param>
        public WeaponActionAttribute(WeaponOperation.Flags target_operation) => this.target_operation = target_operation;

        public readonly WeaponOperation.Flags? target_operation;
        public readonly string? referenced_value;
    }

    /// <summary>
    /// This does nothing but help explicitly mark it as it an input additive method.
    /// I guess you can also use this to get these said methods through reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class InputActionAttribute : Attribute
    {
    }
    #endregion
}

using LilacProject.Combative.Effect;
using LilacProject.Combative.Weaponry.Additives.Active;
using LilacProject.Combative.Weaponry.Mechanism;

namespace LilacProject.Combative.Weaponry.Additives.Constructs;

/// <summary>
/// Base class for laying out serialized data for <see cref="WeaponAdditive"/>. <see cref="WeaponAdditiveConstruct"/> and <see cref="WeaponAdditive"/> must come in pairs.
/// </summary>
[Serializable]
public abstract class WeaponAdditiveConstruct
{
    public virtual string? GetTag() => null;

    /// <summary>
    /// This method defines what components are required for a <see cref="IWeaponPortion"/> to have. Which are then called to <see cref="Compatibility.IGameObjectAccess.AddComponent(Type)"/>
    /// <br/>
    /// Optionally, you can also have to include a <see cref="IWeaponPortion"/> to specifiy which it is meant to add the component in. Otherwise, if it's null, the component <see cref="Type"/> applies to all additives.
    /// </summary>
    /// <param name="weapon_portions">All of the weapon portions of a weapon.</param>
    /// <returns>The component types and the target <see cref="IWeaponPortion"/> if null, the component type applies to all portions.</returns>
    public virtual IEnumerable<(Type, IWeaponPortion?)> RequiredComponents(IEnumerable<IWeaponPortion> weapon_portions) => Enumerable.Empty<(Type, IWeaponPortion?)>();

    /// <summary>
    /// Constructs a new <see cref="WeaponAdditive"/> here and returns it.
    /// </summary>
    protected internal abstract WeaponAdditive BuildAdditive();

    /// <summary>
    /// Method to check for each <see cref="WeaponAdditive"/> is conditioned to pass to <see cref="WeaponAdditive.RecieveInputActions(InputAct?)"/>.
    /// For some purpose, you may be able to use <see cref="TestAdditiveReferenceFilter(WeaponAdditive, int)"/> as it passes the <see cref="WeaponAdditive"/> directly and get the <see cref="InputAct"/> from there. 
    /// But i'd like you to keep this and that separate, one to get reference, the other to get <see cref="InputAct"/> specifically.
    /// </summary>
    /// <param name="additive">The current <see cref="WeaponAdditive"/>. You can use this for type checking or something.</param>
    /// <param name="index">The current index if the iteration. You can use this for index specific checks, see <see cref="Miscellaneous.BitFlags32.FlagInOperation(int, int)"/></param>
    /// <returns>The result. By default is <c>false</c>.</returns>
    protected internal virtual bool TestInputActionsFilter(WeaponAdditive additive, int index) => true;

    /// <summary>
    /// Method to check for each <see cref="WeaponAdditive"/> is conditioned to pass to the second parameter of <see cref="WeaponAdditive.RecieveAdditiveReferences(System.Collections.Immutable.ImmutableArray{WeaponAdditive}, IEnumerable{WeaponAdditive})"/>.
    /// </summary>
    /// <param name="additive">The current <see cref="WeaponAdditive"/>. You can use this for type checking or something.</param>
    /// <param name="index">The current index if the iteration. You can use this for index specific checks, see <see cref="Miscellaneous.BitFlags32.FlagInOperation(int, int)"/></param>
    /// <returns>The result. By default is <c>true</c>.</returns>
    protected internal virtual bool TestAdditiveReferenceFilter(WeaponAdditive additive, int index) => true;

    protected internal virtual bool TestMechanismsReferenceFilter(IWeaponMechanism mechanism, string name) => false;

    /// <summary>
    /// Provides premade filters for <see cref="WeaponAdditive.RecieveProjectorReferences(IEnumerable{IEffectProjector})"/> by providing a list of ids.
    /// </summary>
    /// <returns></returns>
    protected internal virtual string[] ProvideEffectProjector() => Array.Empty<string>();
}

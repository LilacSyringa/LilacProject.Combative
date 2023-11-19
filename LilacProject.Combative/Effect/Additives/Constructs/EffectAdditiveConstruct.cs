using LilacProject.Combative.Effect.Additives.Active;

namespace LilacProject.Combative.Effect.Additives.Constructs;

/// <summary>
/// Base class for laying out serialized data for <see cref="EffectAdditive"/>. <see cref="EffectAdditiveConstruct"/> and <see cref="EffectAdditive"/> must come in pairs.
/// </summary>
[Serializable]
public abstract class EffectAdditiveConstruct
{
    public virtual string? GetTag() => null;

    /// <summary>
    /// This method defines what components are required for a <see cref="IEffectHandle"/> to have. Which are then called to <see cref="Compatibility.IGameObjectAccess.AddComponent(Type)"/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<Type> RequiredComponents() => Enumerable.Empty<Type>();

    /// <summary>
    /// Constructs a new <see cref="EffectAdditive"/> here and returns it.
    /// </summary>
    public abstract EffectAdditive BuildAdditive();

    /// <summary>
    /// Method to check for each <see cref="EffectAdditive"/> is conditioned to pass to <see cref="EffectAdditive.RecieveInputActions(InputAct?)"/>.
    /// For some purpose, you may be able to use <see cref="TestAdditiveReferenceFilter(EffectAdditive, int)"/> as it passes the <see cref="EffectAdditive"/> directly and get the <see cref="InputAct"/> from there. 
    /// But i'd like you to keep this and that separate, one to get reference, the other to get <see cref="InputAct"/> specifically.
    /// </summary>
    /// <param name="additive">The current <see cref="EffectAdditive"/> of the <see cref="Instance.Active.EffectInstance"/>. You can use this for type checking.</param>
    /// <param name="index">The current index if the iteration. You can use this for index specific checks, see <see cref="Miscellaneous.BitFlags32.FlagInOperation(int, int)"/></param>
    /// <returns>The result. By default is <c>false</c>.</returns>
    protected internal virtual bool TestInputActionsFilter(EffectAdditive additive, int index) => false;

    /// <summary>
    /// Method to check for each <see cref="EffectAdditive"/> is conditioned to pass to second parameter of <see cref="EffectAdditive.RecieveAdditiveReference(System.Collections.Immutable.ImmutableArray{EffectAdditive}, IEnumerable{EffectAdditive})"/>.
    /// </summary>
    /// <param name="additive">The current <see cref="EffectAdditive"/> of the <see cref="Instance.Active.EffectInstance"/>. You can use this for type checking.</param>
    /// <param name="index">The current index if the iteration. You can use this for index specific checks, see <see cref="Miscellaneous.BitFlags32.FlagInOperation(int, int)"/></param>
    /// <returns>The result. By default is <c>true</c>.</returns>
    protected internal virtual bool TestAdditiveReferenceFilter(EffectAdditive additive, int index) => true;

    /// <summary>
    /// Provides premade filters for <see cref="EffectAdditive.RecieveProjectorReferences(IEnumerable{IEffectProjector})"/> by providing a list of ids.
    /// </summary>
    /// <returns></returns>
    protected internal virtual string[] ProvideEffectProjector() => Array.Empty<string>();
}

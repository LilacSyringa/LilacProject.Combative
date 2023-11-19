using LilacProject.Combative.Effect.Additives.Constructs;
using LilacProject.Combative.Effect.Instance.Active;

namespace LilacProject.Combative.Effect.Instance.Constructs;

public interface IEffectInstanceConstruct
{
    public sealed IEnumerable<Type> AllRequiredComponents() => GetEffectAdditiveConstructs().SelectMany(item => item.RequiredComponents()).Distinct();

    public string NameID();

    public EffectInstance BuildInstance();

    /// <summary>
    /// Getter function, you can use arrays or lists since they both implement <see cref="IList{T}"/> anyways.
    /// </summary>>
    public IList<EffectAdditiveConstruct> GetEffectAdditiveConstructs();
}

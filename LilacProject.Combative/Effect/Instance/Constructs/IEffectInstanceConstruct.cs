using LilacProject.Combative.Effect.Additives.Constructs;
using LilacProject.Combative.Effect.Instance.Active;

namespace LilacProject.Combative.Effect.Instance.Constructs;

public interface IEffectInstanceConstruct
{
    public string NameID();

    public EffectInstance BuildInstance();
}

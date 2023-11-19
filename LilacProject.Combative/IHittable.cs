namespace LilacProject.Combative;

public interface IHittable
{
    public abstract void HitReaction(DamageOrigin hitter, DamageData damages);
}

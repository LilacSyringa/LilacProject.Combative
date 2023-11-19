namespace LilacProject.Combative;

public interface IHittable
{
    /// <summary>
    /// The reaction of the object when hit.
    /// </summary>
    /// <param name="damage">The origin, angle of attack and the damage dealt.</param>
    /// <param name="unit">Nullable, the user responsible for damaging the object.</param>
    /// <returns>Damage dealt.</returns>
    public abstract float HitReaction(DamageData damage, IUnit? unit);
}

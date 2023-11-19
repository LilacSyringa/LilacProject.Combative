using LilacProject.Combative.Effect.Additives.Active;
using LilacProject.Combative.Effect.Additives;
using LilacProject.Combative.Weaponry;
using System.Collections.Immutable;

namespace LilacProject.Combative.Effect.Instance.Active;

public abstract class EffectInstance
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected AdditiveActions Additives { get; private protected set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ImmutableArray<EffectAdditive>? effect_additives;

    /// <summary>
    /// The user who created the effect.
    /// <br/>
    /// If you need to, you can set this at any time to change the unit of "who owns the effect" such as when an enemy deflects a projectile, you may want to change the unit field.
    /// </summary>
    protected IUnit? unit;

    public ImmutableArray<EffectAdditive> EffectAdditives
    {
        get => effect_additives ?? throw new NullReferenceException();
        set => effect_additives = value;
    }

    /// <summary>
    /// This value is taken when the current effect instance is active.
    /// </summary>
    protected internal abstract bool IsActive { get; protected set; }

    /// <summary>
    /// This value is taken when the current effect instance is alive. A bit different from <see cref="IsActive"/> where it should also include post processing.
    /// </summary>
    protected internal abstract bool IsAlive { get; protected set; }

    internal void SetAdditives(AdditiveActions additives) => Additives = additives;

    /// <summary>
    /// Projects the effect.
    /// <br/>
    /// <br/>
    /// For <see cref="HandledEffectInstance"/>, the <see cref="IEffectHandle"/> this is contained in is cached to <see cref="HandledEffectInstance.Handle"/> before calling this.
    /// </summary>
    /// <param name="projection_data">The origin, direction, and target directed to the effect.</param>
    /// <param name="component">The portion of the weapon used to create the effect.</param>
    public abstract void Project(in InputInfo projection_data, IWeaponPortion component);

    public abstract void Update();

    /// <summary>
    /// Called at end of the effect's lifespan. You ought to end this manually.
    /// </summary>
    protected abstract void End();

    /// <summary>
    /// Wrapping up the handle, such as making it invisible and disabling all functionalities.
    /// </summary>
    protected virtual void Cleanup() { }

    /// <summary>
    /// Method to call on a hit event by any responsible effect additives.
    /// </summary>
    /// <param name="hits">The objects that were hit.</param>
    public virtual void Hit(IReadOnlyList<IHittable> hits) { for (int i = 0; i < hits.Count; i++) Hit(hits[i]); }

    /// <summary>
    /// Method to call on a hit event by any responsible effect additives.
    /// </summary>
    /// <param name="hits">The objects that were hit.</param>
    public abstract void Hit(IHittable hit);

    /// <summary>
    /// Forcibly cancels a currently running effect.
    /// </summary>
    internal void ForcedCleanup()
    {
        if (!IsActive) return;
        Cleanup();
    }
}

using LilacProject.Combative.Weaponry.Mechanism;
using LilacProject.Combative.Compatibility;

namespace LilacProject.Combative.Weaponry.Additives.Active.Defaults;

/// <summary>
/// Default additive which adds accuracy in various different ways to a weapon. /// </summary>
[Obsolete($"Sample/Test code. And is therefore, not fully implemented. Make your accuracy additive instead.", true)]
public abstract class Accuracy2DAdditive : WeaponAdditive
{
    /// <summary>
    /// Construct a new AccuracyAdditive
    /// </summary>
    /// <param name="max_accuracy">Maximum accuracy in degrees</param>
    /// <param name="min_accuracy">Minimum accuracy in degrees</param>
    /// <param name="lost_accuracy">Lost accuracy in degrees for <b>fire</b></param>
    /// <param name="rec_accuracy">Lost accuracy in degrees per <b>second</b></param>
    /// <param name="rec_buffer">Time since the last fire before accuracy recovers.</param>
    public Accuracy2DAdditive(double max_accuracy, double min_accuracy, double lost_accuracy, double rec_accuracy, double rec_buffer)
    {
        this.max_accuracy = max_accuracy;
        this.min_accuracy = min_accuracy;
        this.lost_accuracy = lost_accuracy;
        this.rec_accuracy = rec_accuracy;
        this.rec_buffer = rec_buffer;
    }

    private readonly double max_accuracy;
    private readonly double min_accuracy;
    private readonly double lost_accuracy;
    private readonly double rec_accuracy;
    private readonly double rec_buffer;

    private double current_acc;

    [InputAction]
    protected abstract void RandomAccuracy(ref InputInfo info);
/*    {
        if (current_acc <= 0.0625f) return;

        double accuracy_cone = current_acc / 2;
        double range = Random.Range(accuracy_cone, -accuracy_cone);

        Vector2 dir = new Vector2(info.direction.X, info.direction.Y).RotateVectorRad(range);

        info.direction = new Vector3(dir.X, dir.Y, info.direction.Y);
    }*/

    [WeaponAction(WeaponOperation.Flags.EachFire)]
    private void AccuracyAfterEachFire(IWeaponMechanism mechanism) //Method to adjust accuracy after firing each time
    {
        current_acc += lost_accuracy;

        if (current_acc > min_accuracy) current_acc = min_accuracy;
    }

    [WeaponAction(WeaponOperation.Flags.OnHandPassive)]
    private void AccuracyRecovery(IWeaponMechanism mechanism) //Method to recover accuracy on a given amount of time.
    {
        if (rec_buffer + Time.Scaled < mechanism.Last_use) return;

        current_acc -= rec_accuracy * Time.ScaledDelta;

        if (current_acc < max_accuracy) current_acc = max_accuracy;
    }

    protected internal override IEnumerable<(WeaponOperation.Flags, WeapAct)> AdditiveActions
    {
        get
        {
            yield return (WeaponOperation.Flags.OnHandPassive, AccuracyRecovery);
            yield return (WeaponOperation.Flags.EachFire, AccuracyAfterEachFire);
        }
    }

    protected internal override InputAct? InputActions => RandomAccuracy;
}

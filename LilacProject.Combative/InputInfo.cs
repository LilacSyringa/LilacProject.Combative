namespace LilacProject.Combative;

/// <summary>
/// Structs which contain <see cref="origin"/>, <see cref="direction"/>, and <see cref="target"/>.
/// </summary>
public struct InputInfo
{
    public InputInfo(Vector3 origin, Vector3 direction, Vector3 target)
    {
        this.origin = origin;
        this.direction = direction;
        this.target = target;
    }

    /// <summary>
    /// Origin where it started.
    /// </summary>
    public Vector3 origin;
    /// <summary>
    /// The direction of the efffect.
    /// </summary>
    public Vector3 direction;
    /// <summary>
    /// The point where the cursor was hovering on (on 2d).
    /// </summary>
    public Vector3 target;
}

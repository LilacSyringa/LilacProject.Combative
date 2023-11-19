namespace LilacProject.Combative
{
    public interface IUnit
    {
        /// <summary>
        /// The point where the character is facing at. Doesn't make as much sense in First or Third person shooter but in 2D, this is meant to be the point where your mouse hovers on.
        /// </summary>
        public Vector3 Facing_point { get; }
    }
}

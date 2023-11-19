using LilacProject.Combative.Compatibility;

namespace LilacProject.Combative.Weaponry
{
    /// <summary>
    /// Represents a certain part of a weapon.
    /// </summary>
    public interface IWeaponPortion : IGameObjectAccess
    {
        /// <summary>
        /// The point where the character is facing at. Doesn't make as much sense in First or Third person shooter but in 2D, this is meant to be the point where your mouse hovers on. If they're in console however, that may have to be estimated.
        /// </summary>
        public sealed Vector3 Target => User.Facing_point;

        /// <summary>
        /// The facing direction of the component.
        /// </summary>
        public sealed Vector3 Direction => Forward;

        public sealed InputInfo Input_info => new(Position, Direction, Target);

        /// <summary>
        /// The weapon holder and the entity itself.
        /// </summary>
        public IUnit User { get; }

        /// <summary>
        /// Name of the portion.
        /// </summary>
        public string ID { get; }
    }
}

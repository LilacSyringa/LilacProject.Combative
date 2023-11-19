namespace LilacProject.Combative.Weaponry
{
    public struct WeaponOperation
    {
        [Flags]
        public enum Flags : uint
        {
            None            = 0,
            /// <summary>
            /// Activated every frame.
            /// </summary>
            Passive         = 1 << Value.Passive,
            /// <summary>
            /// Activated every frame so long as the gun is on hand.
            /// </summary>
            OnHandPassive   = 1 << Value.OnHandPassive,
            /// <summary>
            /// Activated every frame so long as the gun is not on hand.
            /// </summary>
            OffHandPassive  = 1 << Value.OffHandPassive,

            /// <summary>
            /// Activitated on the initial firing of the weapon. E.g., in a full auto burst, only the first shot matters.
            /// </summary>
            InitialFire     = 1 << Value.InitialFire,
            /// <summary>
            /// Activates everytime the weapon is used or fired. 
            /// </summary>
            EachFire        = 1 << Value.EachFire,

            /// <summary>
            /// Called upon trying to fire the gun.
            /// </summary>
            TriggerPull     = 1 << Value.TriggerPull,
            TriggerRelease  = 1 << Value.TriggerRelease,

            Unequip = 1 << Value.Unequip,
            Equip = 1 << Value.Equip,

            Pickup = 1 << Value.Pickup,
            Drop = 1 << Value.Drop,

            Hit = 1 << Value.Hit,
            Break = 1 << Value.Break,
        }

        /// <summary>
        /// Can't bother writing the same comments when i already wrote the comments of each values in <see cref="Flags"/>.
        /// </summary>
        public enum Value : int
        {
            Passive,
            OnHandPassive,
            OffHandPassive,

            InitialFire,
            EachFire,

            TriggerPull,
            TriggerRelease,

            Unequip,
            Equip,

            Pickup,
            Drop,

            Hit,
            Break,
        }
    }
}

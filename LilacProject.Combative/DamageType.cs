namespace LilacProject.Combative
{
    /// <summary>
    /// A struct for the type of damage. I figured this might be necessary in case you want to add new damage types since enum aren't extendable. 
    /// In that case, create a new enum and implicitly cast it to DamageType and make sure to DamageType.RegisterType() the new name for object.ToString() to use. Do not use negative id however as the default ids use so.
    /// </summary>
    public readonly struct DamageType
    {
        private static readonly Dictionary<DamageType, string> custom_names;
        private static readonly DefaultDamageTypeNames default_names;

        static DamageType()
        {
            string[] value_set = Enum.GetNames<Def>();

            custom_names = new();

            default_names = new(value_set, int.MinValue);
        }

        public DamageType(Def type) => type_index = (int)type;
        public DamageType(int id)
        {
            type_index = id;
        }

        private readonly int type_index;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not DamageType latter) return false;

            return this.type_index == latter.type_index;
        }

        public override int GetHashCode() => type_index;

        public override string ToString()
        {
            if (type_index < 0) return default_names[type_index];
            if (custom_names.TryGetValue(this, out string? value)) return value;

            return $"UnkownId({type_index})";
        }

        public static void RegisterType(DamageType id, string name)
        {
            if (id.type_index < 0) throw new ArgumentException($"Negative ids such cannot be added as it is reserved.");
            if (custom_names.TryAdd(id, name)) return;

            throw new ArgumentException($"\"{custom_names[id]}\" is already registered with id \"{id.type_index}\" but you tried to add \"{name}\"");
        }

        public static void UnregisterType(DamageType id)
        {
            int index = id.type_index;

            if (index < 0) throw new ArgumentException($"Negative ids such as \"{custom_names[id]}\" cannot be removed as it is reserved.");

            custom_names.Remove(id);
        }

        public static implicit operator DamageType(Def type_enum) => new DamageType(type_enum);

        public static bool operator ==(DamageType former, DamageType latter) => former.type_index == latter.type_index;
        public static bool operator !=(DamageType former, DamageType latter) => former.type_index != latter.type_index;
        public static bool operator ==(DamageType former, int latter) => former.type_index == latter;
        public static bool operator !=(DamageType former, int latter) => former.type_index != latter;

        /// <summary>
        /// These are just the default values of DamageType. This is meant to be immediately and implicitly casted to DamageType.
        /// </summary>
        public enum Def
        {
            Blunt = int.MinValue,
            Slash,
            Pierce,
            Magic,
            Lightning,
            Poison,
            Fire,
            Freeze,
            Shock
        }

        private readonly struct DefaultDamageTypeNames
        {
            public DefaultDamageTypeNames(string[] names, int starting_point)
            {
                this.names = names;
                this.starting_point = starting_point;
            }

            private readonly string[] names;
            private readonly int starting_point;

            public string this[int index] => names[index + starting_point];
        }
    }
}

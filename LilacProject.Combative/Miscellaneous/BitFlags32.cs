using System.Collections.Specialized;

namespace LilacProject.Miscellaneous;

/// <summary>
/// Stores <see cref="bool"/> as an immutable size-32 collection that works more like bit flags.
/// </summary>
public readonly struct BitFlags32 : IEquatable<BitFlags32>
{
    public const int size = 32;

    public BitFlags32(int data) => this.data = unchecked((uint)data);
    public BitFlags32(uint data) => this.data = data;

    private readonly uint data;

    /// <summary>
    /// Gets the boolean value at the specified index of 0 through 31.
    /// </summary>
    /// <param name="index">The index to check the bit of.</param>
    /// <returns>The bool value at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">The inputted index is less than 0 or greater or equal than 32.</exception>
    public bool this[int index]
    {
        get
        {
            if (index < 0) throw new IndexOutOfRangeException("Index cannot be less than 0.");
            if (index >= size) throw new IndexOutOfRangeException("Index cannot be greater or equal to 32.");

            return unchecked(data & (1 << index)) != 0;
        }
    }

    /// <summary>
    /// Gets the boolean value at the specified index.
    /// </summary>
    /// <param name="index">The index to check the bit of.</param>
    /// <returns>The bool value at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">The inputted index is less than 0 or greater or equal than 32.</exception>
    public bool this[Index index]
    {
        get
        {
            int point = index.IsFromEnd ? size - index.Value : index.Value;
            return this[point];
        }
    }

    public override bool Equals(object? obj) => obj is BitFlags32 bitwise && Equals(bitwise);

    public override int GetHashCode() => unchecked((int)data);

    /// <summary>
    /// Sets the value at the specified index and returns a new one.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">The inputted index is less than 0 or greater or equal than 32.</exception>
    public BitFlags32 Set(Index index, bool value)
    {
        int point = index.IsFromEnd ? size - index.Value : index.Value;

        return Set(point, value);
    }

    /// <summary>
    /// Sets the value at the specified index and returns a new one.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">The inputted index is less than 0 or greater or equal than 32.</exception>
    public BitFlags32 Set(int index, bool value)
    {
        if (index < 0) throw new IndexOutOfRangeException("Index cannot be less than 0.");
        if (index >= size) throw new IndexOutOfRangeException("Index cannot be greater or equal to 32.");

        uint val;

        if (value)
        {
            val = data | (1U << index);
        }
        else
        {
            val = data & ~(1U << index);
        }


        return new BitFlags32(val);
    }

    /// <summary>
    /// Sets a value for all the specified bits.
    /// <br></br>
    /// This is like the <see cref="BitVector32"/>[].
    /// </summary>
    /// <returns>The new <see cref="BitFlags32"/> with all the values set.</returns>
    public BitFlags32 SetSection(BitFlags32 bits, bool value) => SetSection(bits.data, value);

    /// <summary>
    /// Sets a value for all the specified bits.
    /// <br></br>
    /// This is like the <see cref="BitVector32[]"/>[].
    /// </summary>
    /// <returns>The new <see cref="BitFlags32"/> with all the values set.</returns>
    public BitFlags32 SetSection(uint bits, bool value)
    {
        uint val = unchecked(value ? data | bits : data & ~bits);
        return new BitFlags32(val);
    }

    /// <summary>
    /// Gets a value indicating whether all the specified bits are set.
    /// <br></br>
    /// This is like the <see cref="BitVector32"/>[].
    /// </summary>
    public bool MatchSection(BitFlags32 bits) => MatchSection(bits.data);

    /// <summary>
    /// Gets a value indicating whether all the specified bits are set.
    /// <br></br>
    /// This is like the <see cref="BitVector32"/>[].
    /// </summary>
    public bool MatchSection(uint bits) => (data & bits) == bits;

    public bool[] ToArray()
    {
        bool[] bools = new bool[size];

        for (int i = 0; i < size; i++) bools[i] = this[i];

        return bools;
    }

    public bool Equals(BitFlags32 other) => data == other.data;

    public Enumerator GetEnumerator() => new Enumerator(this);

    public override string ToString()
    {
        Span<char> chars = stackalloc char[size];

        for (int i = 0; i < size; i++)
        {
            int j = size - 1 - i;

            chars[j] = this[i] ? '1' : '0';
        }

        return new string(chars);
    }

    /// <summary>
    /// Treats an int as a 32-long <see cref="bool"/> array and returns the value at the specified index.
    /// <br/>
    /// This performs the same operation like you would with a <see cref="BitFlags32"/> without the need to create or cast nto <see cref="BitFlags32"/>.
    /// </summary>
    /// <param name="flag">The flags input</param>
    /// <param name="index">The index you want to check.</param>
    /// <returns>The value of the bit at the specified index.</returns>
    public static bool FlagInOperation(int flag, int index)
    {
        if (index >= 32) throw new ArgumentOutOfRangeException(nameof(index), "Cannot be greater or equal to 31.");
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Cannot be less than 0.");
        return unchecked(flag & (1 << index)) != 0;
    }

    /// <summary>
    /// Returns the first index with a bit ON.
    /// This is designed to convert <b>singular</b> Flag enum values to singular int values.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>The first index with a bit ON. -1 if there's no other result</returns>
    public static int OneFlagIndex(int flag) => OneFlagIndex(unchecked((uint)flag));

    /// <summary>
    /// Returns the first index with a bit ON.
    /// This is designed to convert <b>singular</b> Flag enum values to singular int values.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>The first index with a bit ON. -1 if there's no other result</returns>
    public static int OneFlagIndex(uint flag)
    {
        unchecked
        {
            for (int i = 0; i < size; i++)
            {
                flag >>= 1;
                if (flag == 1) return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns a set of indexes with a bit ON.
    /// </summary>
    /// <param name="flag"></param>
    public static IEnumerable<int> FlagIndexes(int flag) => FlagIndexes(unchecked((uint)flag));

    /// <summary>
    /// Returns a set of indexes with a bit ON.
    /// </summary>
    /// <param name="flag"></param>
    public static IEnumerable<int> FlagIndexes(uint flag)
    {
        unchecked
        {
            for (int i = 0; i < size; i++)
            {
                flag >>= 1;
                if ((flag & 1) != 0) yield return i;
            }
        }
    }

    public static implicit operator BitFlags32(BitVector32 bits) => new(bits.Data);
    public static explicit operator uint(BitFlags32 bits) => bits.data;
    public static explicit operator int(BitFlags32 bits) => unchecked((int)bits.data);

    public static bool operator ==(BitFlags32 left, BitFlags32 right) => left.Equals(right);

    public static bool operator !=(BitFlags32 left, BitFlags32 right) => !(left == right);

    public static BitFlags32 operator &(BitFlags32 left, BitFlags32 right) => new BitFlags32(left.data & right.data);
    public static BitFlags32 operator |(BitFlags32 left, BitFlags32 right) => new BitFlags32(left.data | right.data);
    public static BitFlags32 operator ~(BitFlags32 curr) => new BitFlags32(~curr.data);

    public struct Enumerator
    {
        private readonly BitFlags32 bool_set;
        private int index;

        internal Enumerator(BitFlags32 bool_set)
        {
            this.bool_set = bool_set;
            index = 0;
        }

        public bool MoveNext()
        {
            index++;
            return index - 1 < size;
        }

        public readonly bool Current => bool_set[index - 1];
    }
}

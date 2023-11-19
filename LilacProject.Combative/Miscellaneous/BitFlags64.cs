using System.Collections.Specialized;

namespace LilacProject.Miscellaneous;

/// <summary>
/// Stores <see cref="bool"/> as an immutable size-64 collection that works more like bit flags.
/// </summary>
public readonly struct BitFlags64 : IEquatable<BitFlags64>
{
    public const int size = 64;

    public BitFlags64(long data) => this.data = unchecked((ulong)data);
    public BitFlags64(ulong data) => this.data = data;

    private readonly ulong data;

    /// <summary>
    /// Gets the boolean value at the specified index of 0 through 63.
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

            return unchecked(data & (1UL << index)) != 0;
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

    public override bool Equals(object? obj) => obj is BitFlags64 bitwise && Equals(bitwise);

    public override int GetHashCode() => unchecked((int)data);

    /// <summary>
    /// Sets the value at the specified index and returns a new one.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">The inputted index is less than 0 or greater or equal than 64.</exception>
    public BitFlags64 Set(Index index, bool value)
    {
        int point = index.IsFromEnd ? size - index.Value : index.Value;

        return Set(point, value);
    }

    /// <summary>
    /// Sets the value at the specified index and returns a new one.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">The inputted index is less than 0 or greater or equal than 64.</exception>
    public BitFlags64 Set(int index, bool value)
    {
        if (index < 0) throw new IndexOutOfRangeException("Index cannot be less than 0.");
        if (index >= size) throw new IndexOutOfRangeException("Index cannot be greater or equal to 32.");

        ulong val;

        if (value)
        {
            val = data | (1UL << index);
        }
        else
        {
            val = data & ~(1UL << index);
        }


        return new BitFlags64(val);
    }

    /// <summary>
    /// Sets a value for all the specified bits.
    /// <br></br>
    /// This is like the <see cref="BitVector32"/>[].
    /// </summary>
    /// <returns>The new <see cref="BitFlags64"/> with all the values set.</returns>
    public BitFlags64 SetSection(BitFlags64 bits, bool value) => SetSection(bits.data, value);

    /// <summary>
    /// Sets a value for all the specified bits.
    /// <br></br>
    /// This is like the <see cref="BitVector32[]"/>[].
    /// </summary>
    /// <returns>The new <see cref="BitFlags64"/> with all the values set.</returns>
    public BitFlags64 SetSection(ulong bits, bool value)
    {
        ulong val = unchecked(value ? data | bits : data & ~bits);
        return new BitFlags64(val);
    }

    /// <summary>
    /// Gets a value indicating whether all the specified bits are set.
    /// <br></br>
    /// This is like the <see cref="BitVector32"/>[].
    /// </summary>
    public bool MatchSection(BitFlags64 bits) => MatchSection(bits.data);

    /// <summary>
    /// Gets a value indicating whether all the specified bits are set.
    /// <br></br>
    /// This is like the <see cref="BitVector32"/>[].
    /// </summary>
    public bool MatchSection(ulong bits) => (data & bits) == bits;

    public bool[] ToArray()
    {
        bool[] bools = new bool[size];

        for (int i = 0; i < size; i++) bools[i] = this[i];

        return bools;
    }

    public bool Equals(BitFlags64 other) => data == other.data;

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
    /// This performs the same operation like you would with a <see cref="BitFlags64"/> without the need to create or cast nto <see cref="BitFlags64"/>.
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
    public static int OneFlagIndex(long flag) => OneFlagIndex(unchecked((ulong)flag));

    /// <summary>
    /// Returns the first index with a bit ON.
    /// This is designed to convert <b>singular</b> Flag enum values to singular int values.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>The first index with a bit ON. -1 if there's no other result</returns>
    public static int OneFlagIndex(ulong flag)
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
    public static IEnumerable<int> FlagIndexes(long flag) => FlagIndexes(unchecked((ulong)flag));

    /// <summary>
    /// Returns a set of indexes with a bit ON.
    /// </summary>
    /// <param name="flag"></param>
    public static IEnumerable<int> FlagIndexes(ulong flag)
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

    public static implicit operator BitFlags64(BitVector32 bits) => new(bits.Data);
    public static explicit operator ulong(BitFlags64 bits) => bits.data;
    public static explicit operator long(BitFlags64 bits) => unchecked((long)bits.data);

    public static bool operator ==(BitFlags64 left, BitFlags64 right) => left.Equals(right);

    public static bool operator !=(BitFlags64 left, BitFlags64 right) => !(left == right);

    public static BitFlags64 operator &(BitFlags64 left, BitFlags64 right) => new BitFlags64(left.data & right.data);
    public static BitFlags64 operator |(BitFlags64 left, BitFlags64 right) => new BitFlags64(left.data | right.data);
    public static BitFlags64 operator ~(BitFlags64 curr) => new BitFlags64(~curr.data);

    public struct Enumerator
    {
        private readonly BitFlags64 bool_set;
        private int index;

        internal Enumerator(BitFlags64 bool_set)
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

namespace LilacProject.Combative
{
    public readonly ref struct DamageData
    {
        public readonly ReadOnlySpan<DamageValue> damages;

        public float Total_damage
        {
            get
            {
                float total = 0f;

                foreach (DamageValue d in damages)
                {
                    total += d.amount;
                }

                return total;
            }
        }

        public ReadOnlySpan<DamageValue>.Enumerator GetEnumerator() => damages.GetEnumerator();
    }
}

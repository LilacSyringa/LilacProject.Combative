namespace LilacProject.Combative
{
    public readonly ref struct DamageData
    {
        public DamageData(ReadOnlySpan<DamageValue> damages, ref Vector3 contact_point, ref Vector3 direction)
        {
            this.damages = damages;
            this.contact_point = contact_point;
            this.direction = direction;
        }

        public readonly ReadOnlySpan<DamageValue> damages;
        public readonly ref Vector3 contact_point;
        public readonly ref Vector3 direction;


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

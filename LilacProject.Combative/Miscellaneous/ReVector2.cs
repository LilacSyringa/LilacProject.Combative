namespace LilacProject.Miscellaneous
{
    public static class ReVector2
    {
        public static Vector2 RotateVectorRad(this Vector2 dir, double delta)
        {
            float delta_cos = (float)Math.Cos(delta);
            float delta_sin = (float)Math.Sin(delta);

            return new Vector2(
                dir.X * delta_cos - dir.Y * delta_sin,
                dir.X * delta_sin + dir.Y * delta_cos
            );
        }
    }
}

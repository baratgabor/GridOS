namespace IngameScript
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public static Vector2 operator -(Vector2 left, Vector2 right)
            => new Vector2() { X = left.X - right.X, Y = left.Y - right.Y };

        public static Vector2 operator +(Vector2 left, Vector2 right)
            => new Vector2() { X = left.X + right.X, Y = left.Y + right.Y };

        public static Vector2 operator /(Vector2 left, float right)
            => new Vector2() { X = left.X / right, Y = left.Y / right };
    }

    public struct Vector2D
    {
        public double X;
        public double Y;
    }
}

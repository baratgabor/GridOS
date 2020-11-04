namespace IngameScript
{
    public struct Thickness
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }

        public bool IsZero => Left + Top + Right + Bottom == 0;
        public float Horizontal => Left + Right;
        public float Vertical => Top + Bottom;

        public Thickness(float uniformThickness)
        {
            Left = uniformThickness;
            Top = uniformThickness;
            Right = uniformThickness;
            Bottom = uniformThickness;
        }

        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static Thickness operator *(Thickness a, float b)
        {
            return new Thickness()
            {
                Left = a.Left * b,
                Top = a.Top * b,
                Right = a.Right * b,
                Bottom = a.Bottom * b
            };
        }

        public static Thickness operator /(Thickness a, float b)
        {
            return new Thickness()
            {
                Left = a.Left / b,
                Top = a.Top / b,
                Right = a.Right / b,
                Bottom = a.Bottom / b
            };
        }

        public static Thickness operator +(Thickness a, float b)
        {
            return new Thickness()
            {
                Left = a.Left + b,
                Top = a.Top + b,
                Right = a.Right + b,
                Bottom = a.Bottom + b
            };
        }

        public static Thickness operator -(Thickness a, float b)
        {
            return new Thickness()
            {
                Left = a.Left - b,
                Top = a.Top - b,
                Right = a.Right - b,
                Bottom = a.Bottom - b
            };
        }

        public static Vector2 operator +(Thickness a, Vector2 b)
        {
            return new Vector2()
            {
                X = b.X + a.Left + a.Right,
                Y = b.Y + a.Top + a.Bottom
            };
        }
    }
}

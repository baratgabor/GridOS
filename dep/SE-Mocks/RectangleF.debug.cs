using System;

namespace IngameScript
{
    //
    // Summary:
    //     Structure using the same layout than System.Drawing.RectangleF
    public struct RectangleF : IEquatable<RectangleF>
    {
        public Vector2 Position;
        public Vector2 Size;

        public RectangleF(Vector2 position, Vector2 size)
        {
            Y = X = Height = Width = 0f;
            Position = position;
            Size = size;
        }

        public RectangleF(float x, float y, float width, float height)
        {
            Y = y;
            X = x;
            Width = width;
            Height = height;
            Position = new Vector2();
            Size = new Vector2();
        }

        public float Y { get; set; }
        public float X { get; set; }
        public float Right => 0;
        public float Bottom => 0;
        public Vector2 Center => new Vector2();
        public float Height { get; set; }
        public float Width { get; set; }

        public static bool Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            result = new RectangleF();
            return false;
        }

        public static RectangleF Min(RectangleF? rectangle, RectangleF? scissors) => new RectangleF();
        //public bool Contains(Point point);
        public bool Contains(Vector2 vector2D) => false;
        public bool Contains(float x, float y) => false;
        public bool Contains(int x, int y) => false;
        public override bool Equals(object obj) => false;
        public bool Equals(RectangleF other) => false;
        public override int GetHashCode() => base.GetHashCode();
        //public override string ToString();

        public static bool operator ==(RectangleF left, RectangleF right) => false;
        public static bool operator !=(RectangleF left, RectangleF right) => false;
    }
}

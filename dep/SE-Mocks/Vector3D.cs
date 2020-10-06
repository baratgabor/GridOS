namespace IngameScript
{
    public struct Vector3D
    {
        //
        // Summary:
        //     Gets or sets the x-component of the vector.
        public double X;
        //
        // Summary:
        //     Gets or sets the y-component of the vector.
        public double Y;
        //
        // Summary:
        //     Gets or sets the z-component of the vector.
        public double Z;
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public static float Distance(Vector3 a, Vector3 b)
        {
            return 0;
        }

        public static float Distance(Vector3D a, Vector3D b)
        {
            return 0;
        }
    }
}

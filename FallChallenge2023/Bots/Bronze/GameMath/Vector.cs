namespace FallChallenge2023.Bots.Bronze.GameMath
{
    public class Vector
    {
        public int X { get; }
        public int Y { get; }

        public Vector(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector a) => a;
        public static Vector operator -(Vector a) => -1 * a;
        public static Vector operator *(int k, Vector a) => new Vector(k * a.X, k * a.Y);
        public static Vector operator *(Vector a, int k) => k * a;
        public static Vector operator *(Vector a, Vector b) => new Vector(a.X * b.X, a.Y * b.Y);
        public static Vector operator /(Vector a, int k) => new Vector(a.X / k, a.Y / k);
        public static Vector operator /(Vector a, Vector b) => new Vector(a.X / b.X, a.Y / b.Y);
        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);

        public static int Dot(Vector a, Vector b) => a.X * b.X + a.Y * b.Y;

        public int LengthSqr => X * X + Y * Y;

        public override string ToString() => string.Format("({0}, {1})", X, Y);
    }
}

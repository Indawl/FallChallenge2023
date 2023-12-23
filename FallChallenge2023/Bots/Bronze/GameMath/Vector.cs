namespace FallChallenge2023.Bots.Bronze.GameMath
{
    public class Vector
    {
        public int X { get; }
        public int Y { get; }

        public Vector() { }
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => string.Format("({0}, {1})", X, Y);
    }
}

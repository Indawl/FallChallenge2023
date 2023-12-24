using System;

namespace FallChallenge2023.Bots.Bronze.GameMath
{
    public class RectangleRange
    {
        public double X { get; }
        public double Y { get; }
        public double ToX { get; }
        public double ToY { get; }

        public Vector Center => new Vector((ToX + X) / 2, (ToY + Y) / 2);

        public RectangleRange(double x, double y, double toX, double toY)
        {
            X = x;
            Y = y;
            ToX = toX;
            ToY = toY;
        }

        public RectangleRange(Vector coord, Vector toCoord) : this(coord.X, coord.Y, toCoord.X, toCoord.Y) { }

        public RectangleRange Intersect(RectangleRange range)
        {
            var x = (int)Math.Max(X, range.X);
            var y = (int)Math.Max(Y, range.Y);
            var toX = (int)Math.Min(ToX, range.ToX);
            var toY = (int)Math.Min(ToY, range.ToY);

            if (toX < x || toY < y) return null;
            return new RectangleRange(x, y, toX, toY);
        }

        public Vector Intersect(Vector a, Vector b)
        {
            var dir = a - b;
            if (dir.IsZero()) return new Vector(b);

            double x, y;
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                x = dir.X > 0 ? ToX : X;
                y = b.Y + dir.Y / dir.X * (x - b.X);
            }
            else
            {
                y = dir.Y > 0 ? ToY : Y;
                x = b.X + dir.X / dir.Y * (y - b.Y);
            }

            return new Vector(x, y);
        }
    }
}

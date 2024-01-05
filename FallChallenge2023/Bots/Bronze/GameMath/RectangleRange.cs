using System;

namespace FallChallenge2023.Bots.Bronze.GameMath
{
    public class RectangleRange : IEquatable<RectangleRange>
    {
        public Vector From { get; }
        public Vector To { get; }

        public Vector Center => (From + To) / 2;

        public RectangleRange(Vector from, Vector to)
        {
            From = from;
            To = to;
        }
        public RectangleRange(double x, double y, double toX, double toY) : this(new Vector(x, y), new Vector(toX, toY)) { }

        public static RectangleRange operator +(RectangleRange range, Vector offset) => new RectangleRange(range.From + offset, range.To + offset);
        public static RectangleRange operator -(RectangleRange range, Vector offset) => new RectangleRange(range.From - offset, range.To - offset);

        public bool Equals(RectangleRange other) => From.Equals(other.From) && To.Equals(other.To);        

        public RectangleRange HSymmetric(double x = 0) => new RectangleRange(To.HSymmetric(x), From.HSymmetric(x));
        public bool InRange(Vector coord) => From.X <= coord.X && To.X >= coord.X && From.Y <= coord.Y && To.Y >= coord.Y;

        public RectangleRange Intersect(RectangleRange range)
        {
            var x = (int)Math.Max(From.X, range.From.X);
            var y = (int)Math.Max(From.Y, range.From.Y);
            var toX = (int)Math.Min(To.X, range.To.X);
            var toY = (int)Math.Min(To.Y, range.To.Y);

            return new RectangleRange(x, y, toX, toY);
        }

        public Vector Intersect(Vector from, Vector to)
        {
            var dir = from - to;
            if (dir.IsZero()) return new Vector(to);

            double x, y;
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                x = dir.X > 0 ? To.X : From.X;
                y = Math.Round(to.Y + dir.Y / dir.X * (x - to.X));
            }
            else
            {
                y = dir.Y > 0 ? To.Y : From.Y;
                x = Math.Round(to.X + dir.X / dir.Y * (y - to.Y));
            }

            return new Vector(x, y);
        }

        public RectangleRange Intersect(Vector position, double radius)
        {
            var range = this - position;

            double fromX = range.From.X, toX = range.To.X;
            double fromY = range.From.Y, toY = range.To.Y;

            if (!GetSide(range.From.X, range.To.X, radius, range.From.Y, ref fromX, ref toX)) fromY = -radius;
            if (!GetSide(range.From.X, range.To.X, radius, range.To.Y, ref fromX, ref toX)) toY = radius;
            if (!GetSide(range.From.Y, range.To.Y, radius, range.From.X, ref fromX, ref toX)) fromX = -radius;
            if (!GetSide(range.From.Y, range.To.Y, radius, range.To.X, ref fromX, ref toX)) toX = radius;

            return new RectangleRange(fromX, fromY, toX, toY) + position;
        }

        private bool GetSide(double from, double to, double radius, double coord, ref double fromCoord, ref double toCoord)
        {
            if (radius < Math.Abs(coord)) return false;

            var delta = Math.Round(Math.Sqrt(radius * radius - coord * coord));
            fromCoord = Math.Min(fromCoord, Math.Min(to, Math.Max(from, -delta)));
            toCoord = Math.Max(toCoord, Math.Min(to, Math.Max(from, delta)));
            return true;
        }

        public RectangleRange Increase(Vector position)
        {
            var x = Math.Min(From.X, position.X);
            var y = Math.Min(From.Y, position.Y);
            var toX = Math.Max(To.X, position.X);
            var toY = Math.Max(To.Y, position.Y);

            return new RectangleRange(x, y, toX, toY);
        }

        public override string ToString() => string.Format("{0} {1}", From, To);
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.GameMath
{
    public class Vector : IEquatable<Vector>
    {
        public double X { get; }
        public double Y { get; }

        [JsonConstructor]
        public Vector(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        public Vector(Vector v) : this(v.X, v.Y) { }

        public static Vector operator +(Vector a) => a;
        public static Vector operator -(Vector a) => -1 * a;
        public static Vector operator *(double k, Vector a) => new Vector(k * a.X, k * a.Y);
        public static Vector operator *(Vector a, double k) => k * a;
        public static Vector operator *(Vector a, Vector b) => new Vector(a.X * b.X, a.Y * b.Y);
        public static Vector operator /(Vector a, double k) => new Vector(a.X / k, a.Y / k);
        public static Vector operator /(Vector a, Vector b) => new Vector(a.X / b.X, a.Y / b.Y);
        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);

        public static double Dot(Vector a, Vector b) => a.X * b.X + a.Y * b.Y;

        public override string ToString() => string.Format("({0}, {1})", X, Y);
        public string ToIntString() => string.Format("({0}, {1})", (int)X, (int)Y);

        public bool IsZero() => X == 0 && Y == 0;
        public bool Equals(Vector other) => X == other.X && Y == other.Y;
        public Vector HSymmetric(double x = 0) => new Vector(2 * x - X, Y);
        public Vector VSymmetric(double y = 0) => new Vector(X, 2 * y - Y);
        public double LengthSqr() => X * X + Y * Y;
        public double Length() => Math.Sqrt(LengthSqr());
        public double Distance(Vector other) => Math.Sqrt(DistanceSqr(other));
        public double DistanceSqr(Vector other) => (other - this).LengthSqr();
        public Vector Round() => new Vector((int)X, (int)Y);
        public Vector EpsilonRound() => new Vector(Math.Round(X * 10000000.0) / 10000000.0, Math.Round(Y * 10000000.0) / 10000000.0);
        public Vector Normalize()
        {
            double length = Length();
            if (length == 0) return new Vector();
            return this / length;
        }
        public Vector Rotate(double angle) => new Vector(
            X * Math.Cos(angle) - Y * Math.Sin(angle), 
            X * Math.Sin(angle) + Y * Math.Cos(angle));
        public bool InRange(int radius) => LengthSqr() <= radius * radius;
        public bool InRange(Vector coord, int radius) => (coord - this).InRange(radius);
        public bool InRange(RectangleRange range) => X >= range.X && X <= range.ToX && Y >= range.Y && Y <= range.ToY;

        public List<Vector> GetClosest(List<Vector> coords)
        {
            var closest = new List<Vector>();
            double minDist = 0;

            foreach (var coord in coords)
            {
                var dist = DistanceSqr(coord);
                if (closest.Count == 0 || dist < minDist)
                {
                    closest.Clear();
                    closest.Add(coord);
                    minDist = dist;
                }
                else if (dist == minDist) 
                    closest.Add(coord);
            }

            return closest;
        }
    }
}

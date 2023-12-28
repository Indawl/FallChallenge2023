using FallChallenge2023.Bots.Bronze.GameMath;
using System;

namespace FallChallenge2023.Bots.Bronze
{
    public class Fish : ICloneable
    {
        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }

        public FishStatus Status { get; set; } = FishStatus.UNKNOWED;
        public Vector Position { get; set; }
        public Vector Speed { get; set; }

        public Fish(int id, FishColor color, FishType type)
        {
            Id = id;
            Color = color;
            Type = type;
        }

        public override string ToString() => string.Format("[{0}] {1} {2} {3} {4} V{5}{6})", Id, Color, Type, Status, Position?.ToIntString(), Speed == null ? 0 : (int)Speed.Length(), Speed?.ToIntString());

        public object Clone() => MemberwiseClone();
    }
}

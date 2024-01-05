using FallChallenge2023.Bots.Bronze.GameMath;
using System;

namespace FallChallenge2023.Bots.Bronze
{
    public class Fish : ICloneable
    {
        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }

        public Vector Position { get; set; }
        public Vector Speed { get; set; }

        public RectangleRange Location { get; set; }
             
        public Fish(int id, FishColor color = FishColor.UGLY, FishType type = FishType.ANGLER)
        {
            Id = id;
            Color = color;
            Type = type;
        }

        public override string ToString() => string.Format("[{0}] {1} {2} {3} V {4} {5}", Id, Color, Type, Position?.ToIntString(), Speed == null ? 0 : (int)Speed.Length(), Speed?.ToIntString());

        public virtual object Clone() => MemberwiseClone();
    }
}

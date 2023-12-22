using System;

namespace FallChallenge2023.Bots.Bronze
{
    public class RadarBlip : ICloneable
    {
        public int Id { get; }
        public RadarType Type { get; }

        public RadarBlip(int id, RadarType type)
        {
            Id = id;
            Type = type;
        }

        public override string ToString() => string.Format("{0} {1}", Id, Type);

        public object Clone() => MemberwiseClone();
    }
}

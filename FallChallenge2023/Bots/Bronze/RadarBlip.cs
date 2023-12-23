using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze
{
    public class RadarBlip
    {
        public int FishId { get; }
        public BlipType Type { get; }

        public RadarBlip(int fishId, BlipType type)
        {
            FishId = fishId;
            Type = type;
        }

        public static int GetDirectionX(BlipType type) => (type == BlipType.BR || type == BlipType.TR) ? 1 : -1;
        public static int GetDirectionY(BlipType type) => (type == BlipType.BL || type == BlipType.BR) ? 1 : -1;
        public static Vector GetDirection(BlipType type) => new Vector(GetDirectionX(type), GetDirectionY(type));

        public override string ToString() => string.Format("{0} {1}", FishId, Type);
    }
}

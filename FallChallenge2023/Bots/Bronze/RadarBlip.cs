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
        public override string ToString() => string.Format("{0} {1}", FishId, Type);

        public RectangleRange GetRange(Vector position)
        {
            double x = 0, y = 0, toX = GameProperties.MAP_SIZE - 1, toY = GameProperties.MAP_SIZE - 1;
            
            if (Type == BlipType.BL || Type == BlipType.TL)
                toX = position.X;
            else
                x = position.X + 1;

            if (Type == BlipType.TL || Type == BlipType.TR)
                toY = position.Y;
            else
                y = position.Y + 1;

            return new RectangleRange(x, y, toX, toY);
        }
    }
}

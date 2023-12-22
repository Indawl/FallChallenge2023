namespace FallChallenge2023.Bots.Bronze
{
    public struct RadarBlip
    {
        public int FishId { get; }
        public BlipType Type { get; }

        public RadarBlip(int fishId, BlipType type)
        {
            FishId = fishId;
            Type = type;
        }

        public override string ToString() => string.Format("{0} {1}", FishId, Type);
    }
}

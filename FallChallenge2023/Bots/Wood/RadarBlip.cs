namespace FallChallenge2023.Bots.Wood
{
    public class RadarBlip
    {
        public int Id { get; }
        public RadarType Type { get; }

        public RadarBlip(int id, RadarType type)
        {
            Id = id;
            Type = type;
        }
    }
}

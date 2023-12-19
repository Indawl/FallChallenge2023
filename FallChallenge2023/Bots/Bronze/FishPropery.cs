namespace FallChallenge2023.Bots.Bronze
{
    public class FishPropery
    {        
        public FishColor Color { get; }
        public FishType Type { get; }

        public FishPropery(FishColor color, FishType type)
        {
            Color = color;
            Type = type;
        }

        public override string ToString() => string.Format("{0} {1}", Color, Type);
    }
}

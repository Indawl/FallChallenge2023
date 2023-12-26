using DevLib.GameMath;

namespace FallChallenge2023.Bots.Wood
{
    public class Fish
    {
        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }
        public VectorD Position { get; set; }
        public VectorD Speed { get; set; }

        public Fish(int id, FishColor color, FishType type, VectorD position, VectorD speed)
        {
            Id = id;
            Color = color;
            Type = type;
            Position = position;
            Speed = speed;
        }

        public Fish(int id, FishColor color, FishType type, int x, int y, int vx, int vy) :
            this(id, color, type, new VectorD(x, y), new VectorD(vx, vy))
        {
        }
    }
}

using DevLib.GameMath;

namespace FallChallenge2023.Bots.Bronze
{
    public class Fish
    {
        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }
        public Vector Position { get; set; }
        public Vector Speed { get; set; }

        public Fish(int id, FishColor color, FishType type, Vector position, Vector speed)
        {
            Id = id;
            Color = color;
            Type = type;
            Position = position;
            Speed = speed;
        }

        public Fish(int id, FishColor color, FishType type, int x, int y, int vx, int vy) :
            this(id, color, type, new Vector(x, y), new Vector(vx, vy))
        {
        }
    }
}

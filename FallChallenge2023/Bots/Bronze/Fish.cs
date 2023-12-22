using System;

namespace FallChallenge2023.Bots.Bronze
{
    public class Fish : ICloneable
    {
        public const int SPEED = 200;
        public const int FRIGHTENED_SPEED = 400;
        public const int MIN_DISTANCE_BT_FISH = 600;        
        public const int MONSTER_SPEED = 270;
        public const int MONSTER_ATTACK_SPEED = 540;
        public const int MONSTER_ATTACK_RADIUS = 500;
        public const int MIN_DISTANCE_BT_MONSTER = 600;        

        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }

        public FishStatus Status { get; set; } = FishStatus.UNKNOWED;
        public int X { get; set; }
        public int Y { get; set; }
        public int Vx { get; set; }
        public int Vy { get; set; }        

        public Fish(int id, FishColor color, FishType type)
        {
            Id = id;
            Color = color;
            Type = type;
        }

        public override string ToString() => string.Format("[{0}] {1} {2} {3} ({4}, {5}) V({6}, {7})", Id, Color, Type, Status, X, Y, Vx, Vy);

        public object Clone() => MemberwiseClone();
    }
}

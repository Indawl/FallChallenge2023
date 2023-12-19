using DevLib.GameMath;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class Fish
    {
        public const int SPEED = 200;
        public const int MIN_DISTANCE_BT_FISH = 600;
        public const int HEARING_RANGE = 1400;
        public const int FRIGHTENED_SPEED = 400;
        public const int ATTACK_RADIUS = 500;
        public const int MONSTER_ATTACK_SPEED = 540;
        public const int MONSTER_SPEED = 270;
        public const int MIN_DISTANCE_BT_MONSTER = 600;
        public const int MONSTER_MIN_START_Y = 5000;

        public static Dictionary<FishType, Vector> HABITAT = new Dictionary<FishType, Vector>()
        {
            { FishType.ANGLER, new Vector(2500, 10000) },
            { FishType.JELLY, new Vector(2500, 5000) },
            { FishType.FISH, new Vector(5000, 7500) },
            { FishType.CRAB, new Vector(7500, 10000) }
        };

        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }

        public Vector Position { get; set; }
        public Vector Speed { get; set; }

        public Fish(int id, FishColor color, FishType type)
        {
            Id = id;
            Color = color;
            Type = type;
        }

        public override string ToString() => string.Format("{0} {1} {2} P{3} S{4}", Color, Type, Id, Position.ToIntString(), Speed.ToIntString());
    }
}

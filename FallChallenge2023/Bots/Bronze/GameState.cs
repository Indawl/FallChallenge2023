using DevLib.Game;
using DevLib.GameMath;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : IGameState
    {
        public const int FIRST_SAVE_KOEF = 2;
        public const int LAST_TURN = 200;

        public static Vector MAP_SIZE = new Vector(10000, 10000);
        public static Dictionary<FishType, int> REWARDS = new Dictionary<FishType, int>()
        {
            { FishType.JELLY, 1 },
            { FishType.FISH, 2 },
            { FishType.CRAB, 3 },
            { FishType.ONE_COLOR, 3 },
            { FishType.ONE_TYPE, 4 }
        };

        public int Turn { get; set; }
        public int[] Score { get; } = new int[2];
        public List<int>[] Scans { get; } = new List<int>[2];
        public Dictionary<int, Drone> Drones { get; } = new Dictionary<int, Drone>();
        public Dictionary<int, Fish> Fishes { get; } = new Dictionary<int, Fish>();
    }
}

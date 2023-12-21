using DevLib.Game;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : IGameState
    {
        public const int FIRST_SAVE_KOEF = 2;
        public const int LAST_TURN = 200;
        public const int MAP_SIZE = 10000;

        public static Dictionary<FishType, int> REWARDS = new Dictionary<FishType, int>()
        {
            { FishType.JELLY, 1 },
            { FishType.FISH, 2 },
            { FishType.CRAB, 3 },
            { FishType.ONE_COLOR, 3 },
            { FishType.ONE_TYPE, 4 }
        };

        public int Turn { get; set; }
        public int MyScore { get; set; }
        public int EnemyScore { get; set; }
        public List<int> MyScans { get; set; } = new List<int>();
        public List<int> EnemyScans { get; set; } = new List<int>();
        public Dictionary<int, Drone> Drones { get; set; } = new Dictionary<int, Drone>();
        public Dictionary<int, Fish> Fishes { get; set; } = new Dictionary<int, Fish>();

        public List<int> GetScans(int playerId) => playerId == 0 ? MyScans : EnemyScans;
        public int GetScore(int playerId) => playerId == 0 ? MyScore : EnemyScore;
        public int SetScore(int playerId, int score) => playerId == 0 ? MyScore = score : EnemyScore = score;
    }
}

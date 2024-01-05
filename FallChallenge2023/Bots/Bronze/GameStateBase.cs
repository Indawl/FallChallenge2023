using DevLib.Game;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameStateBase : IGameState
    {
        public int Turn { get; set; }
        public int MyScore { get; set; }
        public int EnemyScore { get; set; }
        public HashSet<int> MyScans { get; protected set; } = new HashSet<int>();
        public HashSet<int> EnemyScans { get; protected set; } = new HashSet<int>();
        public List<Drone> MyDrones { get; protected set; } = new List<Drone>();
        public List<Drone> EnemyDrones { get; protected set; } = new List<Drone>();
        public List<Fish> Fishes { get; protected set; } = new List<Fish>();
        public List<Fish> Monsters { get; protected set; } = new List<Fish>();
        public List<Fish> LostedFishes { get; protected set; } = new List<Fish>();
        public HashSet<int> VisibleFishes { get; protected set; } = new HashSet<int>();
    }
}

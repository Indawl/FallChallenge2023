using DevLib.Game;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameStateBase : IGameState
    {
        public int Turn { get; set; }
        public int MyScore { get; set; }
        public int EnemyScore { get; set; }
        public List<int> MyScans { get; protected set; } = new List<int>();
        public List<int> EnemyScans { get; protected set; } = new List<int>();
        public List<Drone> Drones { get; protected set; } = new List<Drone>();
        public List<Fish> Fishes { get; protected set; } = new List<Fish>();        
    }
}

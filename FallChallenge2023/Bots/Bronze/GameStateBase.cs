﻿using DevLib.Game;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameStateBase : IGameState
    {
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

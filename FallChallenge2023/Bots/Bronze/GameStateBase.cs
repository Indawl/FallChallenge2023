using DevLib.Game;
using System.Collections.Generic;
using System.Linq;

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
         
        public List<int> GetScans(int playerId) => playerId == 0 ? MyScans : EnemyScans;
        public int GetScore(int playerId) => playerId == 0 ? MyScore : EnemyScore;
        public int SetScore(int playerId, int score) => playerId == 0 ? MyScore = score : EnemyScore = score;
        public Fish GetFish(int id) => Fishes.FirstOrDefault(_ => _.Id == id);
        public IEnumerable<Drone> GetDrones(int playerId) => Drones.Where(_ => _.PlayerId == playerId);
        public Drone GetDrone(int id) => Drones.FirstOrDefault(_ => _.Id == id);
        
        public Drone GetNewDrone(int droneId, int playerId)
        {
            var drone = new Drone(droneId, playerId);
            Drones.Add(drone);
            return drone;
        }
    }
}

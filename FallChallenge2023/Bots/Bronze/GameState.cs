using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
    {
        public List<Fish>[] ScannedFishes { get; private set; } = new List<Fish>[2];
        public List<Fish>[] UnscannedFishes { get; private set; } = new List<Fish>[2];

        public int Score => MyScore - EnemyScore;
        public IEnumerable<Fish> SwimmingFishes => Fishes.Where(_ => _.Status != FishStatus.LOSTED && _.Color != FishColor.UGLY);

        public void Initialize()
        {
            for (int i = 0; i < 2; i++)
            {
                ScannedFishes[i] = GetDrones(i).SelectMany(_ => _.Scans).Distinct().Union(GetScans(i)).Select(_ => GetFish(_)).ToList();
                UnscannedFishes[i] = SwimmingFishes.Where(_ => !ScannedFishes[i].Contains(_)).ToList();
            }
        }

        public object Clone()
        {
            var state = (GameState)MemberwiseClone();
            state.MyScans = new List<int>(MyScans);
            state.EnemyScans = new List<int>(EnemyScans);
            state.Drones = new List<Drone>();
            foreach (var drone in Drones)
                state.Drones.Add((Drone)drone.Clone());
            state.Fishes = new List<Fish>();
            foreach (var fish in Fishes)
                state.Fishes.Add((Fish)fish.Clone());
            state.Initialize();
            return state;
        }

        public List<int> GetScans(int playerId) => playerId == 0 ? MyScans : EnemyScans;
        public int GetScore(int playerId) => playerId == 0 ? MyScore : EnemyScore;
        public int SetScore(int playerId, int score) => playerId == 0 ? MyScore = score : EnemyScore = score;
        public int AddScore(int playerId, int score) => playerId == 0 ? MyScore += score : EnemyScore += score;
        public Fish GetFish(int id) => Fishes.FirstOrDefault(_ => _.Id == id);
        public IEnumerable<Drone> GetDrones(int playerId) => Drones.Where(_ => _.PlayerId == playerId);
        public Drone GetDrone(int id) => Drones.FirstOrDefault(_ => _.Id == id);

        public Drone GetNewDrone(int droneId, int playerId)
        {
            var drone = new Drone(droneId, playerId);
            Drones.Add(drone);
            return drone;
        }

        public Fish GetSymmetricFish(Fish fish)
        {
            var offset = fish.Color == FishColor.UGLY ? 16 : 4;
            var fishId = fish.Id + ((fish.Id - offset) % 2 == 0 ? 1 : -1);
            return GetFish(fishId);
        }
    }
}

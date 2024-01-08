using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
    {
        public List<Decision> DefferedDecisions { get; set; } = new List<Decision>();
        public HashSet<int> SavedDroneId { get; set; } = new HashSet<int>();
        public bool NewEvent { get; set; }

        public int Score => MyScore - EnemyScore;
        public IEnumerable<Drone> Drones => MyDrones.Union(EnemyDrones);
        public IEnumerable<Fish> SwimmingFishes => Fishes.Union(Monsters);

        #region Buffer
        private HashSet<int>[] _scannedFish;
        private HashSet<int>[] _unscannedFish;

        public void RefreshBuffer()
        {
            _scannedFish = new HashSet<int>[2];
            _unscannedFish = new HashSet<int>[2];
        }
        #endregion

        public GameState()
        {
            RefreshBuffer();
        }

        public object Clone()
        {
            var state = (GameState)MemberwiseClone();
            state.MyScans = new HashSet<int>(MyScans);
            state.EnemyScans = new HashSet<int>(EnemyScans);
            CloneDrones(MyDrones, state.MyDrones = new List<Drone>());
            CloneDrones(EnemyDrones, state.EnemyDrones = new List<Drone>());
            CloneFishes(Fishes, state.Fishes = new List<Fish>());
            CloneFishes(Monsters, state.Monsters = new List<Fish>());
            CloneFishes(LostedFishes, state.LostedFishes = new List<Fish>());
            state.VisibleFishes = new HashSet<int>(VisibleFishes);

            state.DefferedDecisions = new List<Decision>(DefferedDecisions);
            state.SavedDroneId = new HashSet<int>(SavedDroneId);

            state.RefreshBuffer();
            return state;
        }
        public static void CloneDrones(List<Drone> source, List<Drone> dest)
        {
            foreach (var drone in source)
                dest.Add((Drone)drone.Clone());
        }
        public static void CloneFishes(List<Fish> source, List<Fish> dest)
        {
            foreach (var fish in source)
                dest.Add((Fish)fish.Clone());
        }

        public int GetScore(int playerId) => playerId == 0 ? MyScore : EnemyScore;
        public int SetScore(int playerId, int score) => playerId == 0 ? MyScore = score : EnemyScore = score;
        public int AddScore(int playerId, int score) => playerId == 0 ? MyScore += score : EnemyScore += score;
        public HashSet<int> GetScans(int playerId) => playerId == 0 ? MyScans : EnemyScans;
        public HashSet<int> GetScans() => MyScans.Union(EnemyScans).ToHashSet();
        public List<Drone> GetDrones(int playerId) => playerId == 0 ? MyDrones : EnemyDrones;
        public Drone GetDrone(int playerId, int id) => GetDrones(playerId).FirstOrDefault(drone => drone.Id == id);
        public Drone GetDrone(int droneId) => Drones.FirstOrDefault(drone => drone.Id == droneId);
        public Fish GetFish(int fishId) => Fishes.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetLostedFish(int fishId) => LostedFishes.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetMonster(int fishId) => Monsters.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetSwimmingFish(int fishId) => SwimmingFishes.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetAnyFish(int fishId) => Fishes.Union(LostedFishes).Union(Monsters).FirstOrDefault(fish => fish.Id == fishId);
        public int GetSymmetricFishId(int fishId) => fishId + (fishId % 2 == 0 ? 1 : -1);
        public Fish GetSymmetricFish(Fish fish) => GetSwimmingFish(GetSymmetricFishId(fish.Id));
        public int GetSymmetricDroneId(int droneId) => 2 * (droneId % 2 + 1) - droneId;
        public Drone GetSymmetricDroneId(Drone drone) => GetDrone(GetSymmetricDroneId(drone.Id));
        public HashSet<int> GetScannedFish(int playerId) => _scannedFish[playerId] 
                                                         ?? (_scannedFish[playerId] = GetDrones(playerId)
                                                            .SelectMany(drone => drone.Scans)
                                                            .Union(GetScans(playerId))
                                                            .Distinct().ToHashSet());
        public HashSet<int> GetUnscannedFish(int playerId) => _unscannedFish[playerId]
                                                             ?? (_unscannedFish[playerId] = SwimmingFishes
                                                            .Select(fish => fish.Id)
                                                            .Except(GetScannedFish(playerId)).ToHashSet());

        public Drone GetNewDrone(int playerId, int droneId)
        {
            var drone = new Drone(droneId, playerId);
            GetDrones(playerId).Add(drone);
            return drone;
        }

        public void FishLosted(Fish fish)
        {
            LostedFishes.Add(fish);
            Fishes.Remove(fish);
        }
    }
}

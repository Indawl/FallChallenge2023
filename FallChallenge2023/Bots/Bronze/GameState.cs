using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
    {
        public const int MAP_SIZE = 10000;
        public const int LAST_TURN = 200;
        public const int FIRST_SAVE_KOEF = 2;
        public const int MONSTER_MIN_START_Y = 5000;

        public static Vector CENTER = new Vector((MAP_SIZE - 1) / 2.0, (MAP_SIZE - 1) / 2.0);

        public static Dictionary<FishType, int[]> HABITAT = new Dictionary<FishType, int[]>()
        {
            { FishType.ANGLER, new int[] { 2500, 9999 } },
            { FishType.JELLY, new int[] { 2500, 5000 } },
            { FishType.FISH, new int[] { 5000, 7500 } },
            { FishType.CRAB, new int[] { 7500, 9999 } }
        };

        public static Dictionary<FishType, int> REWARDS = new Dictionary<FishType, int>()
        {
            { FishType.JELLY, 1 },
            { FishType.FISH, 2 },
            { FishType.CRAB, 3 },
            { FishType.ONE_COLOR, 3 },
            { FishType.ONE_TYPE, 4 }
        };

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
            return state;
        }

        public List<int> GetScans(int playerId) => playerId == 0 ? MyScans : EnemyScans;
        public int GetScore(int playerId) => playerId == 0 ? MyScore : EnemyScore;
        public int SetScore(int playerId, int score) => playerId == 0 ? MyScore = score : EnemyScore = score;
        public Fish GetFish(int id) => Fishes.FirstOrDefault(_ => _.Id == id);
        public IEnumerable<Fish> GetFishes() => Fishes.Where(_ => _.Status != FishStatus.LOSTED);
        public IEnumerable<Drone> GetDrones(int playerId) => Drones.Where(_ => _.PlayerId == playerId);
        public Drone GetDrone(int id) => Drones.FirstOrDefault(_ => _.Id == id);

        public Drone GetNewDrone(int droneId, int playerId)
        {
            var drone = new Drone(droneId, playerId);
            Drones.Add(drone);
            return drone;
        }

        public IEnumerable<int> GetScannedFishes(int playerId) => GetDrones(playerId).SelectMany(_ => _.Scans).Distinct().Union(GetScans(playerId));

        public Fish GetSymmetricFish(Fish fish)
        {
            var offset = fish.Color == FishColor.UGLY ? 16 : 4;
            var fishId = fish.Id + ((fish.Id - offset) % 2 == 0 ? 1 : -1);
            return GetFish(fishId);
        }
    }
}

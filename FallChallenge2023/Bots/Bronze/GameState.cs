using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
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

        public object Clone()
        {
            var state = (GameState)MemberwiseClone();
            state.MyScans = new List<int>(MyScans);
            state.EnemyScans = new List<int>(EnemyScans);
            state.Drones = new Dictionary<int, Drone>();
            foreach (var drone in Drones)
                state.Drones.Add(drone.Key, (Drone)drone.Value.Clone());
            state.Fishes = new Dictionary<int, Fish>();
            foreach (var fish in Fishes)
                state.Fishes.Add(fish.Key, (Fish)fish.Value.Clone());
            return state;
        }
    }
}

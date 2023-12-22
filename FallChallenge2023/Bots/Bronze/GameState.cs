using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
    {
        public const int MAP_SIZE = 10000;
        public const int LAST_TURN = 200;
        public const int FIRST_SAVE_KOEF = 2;
        public const int MONSTER_MIN_START_Y = 5000;

        public static Dictionary<FishType, int[]> HABITAT = new Dictionary<FishType, int[]>()
        {
            { FishType.ANGLER, new int[] { 2500, 10000 } },
            { FishType.JELLY, new int[] { 2500, 5000 } },
            { FishType.FISH, new int[] { 5000, 7500 } },
            { FishType.CRAB, new int[] { 7500, 10000 } }
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
    }
}

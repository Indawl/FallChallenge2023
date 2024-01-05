﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
    {
        public GameState Parent { get; private set; }
        public int Score => MyScore - EnemyScore;
        public IEnumerable<Drone> Drones => MyDrones.Union(EnemyDrones);
        public IEnumerable<Fish> SwimmingFishes => Fishes.Union(Monsters);

        public object Clone()
        {
            var state = (GameState)MemberwiseClone();
            state.Parent = this;
            state.MyScans = new HashSet<int>(MyScans);
            state.EnemyScans = new HashSet<int>(EnemyScans);
            state.MyDrones = CloneDrones(MyDrones);
            state.EnemyDrones = CloneDrones(EnemyDrones);
            state.Fishes = CloneFishes(Fishes);
            state.Monsters = CloneFishes(Monsters);
            state.LostedFishes = CloneFishes(LostedFishes);
            state.VisibleFishes = new HashSet<int>(VisibleFishes);
            return state;
        }
        public List<Drone> CloneDrones(List<Drone> drones)
        {
            var newDrones = new List<Drone>();
            foreach (var drone in drones)
                newDrones.Add((Drone)drone.Clone());
            return newDrones;
        }
        public List<Fish> CloneFishes(List<Fish> fishes)
        {
            var newFishes = new List<Fish>();
            foreach (var fish in fishes)
                newFishes.Add((Fish)fish.Clone());
            return newFishes;
        }

        public int GetScore(int playerId) => playerId == 0 ? MyScore : EnemyScore;
        public int SetScore(int playerId, int score) => playerId == 0 ? MyScore = score : EnemyScore = score;
        public int AddScore(int playerId, int score) => playerId == 0 ? MyScore += score : EnemyScore += score;
        public HashSet<int> GetScans(int playerId) => playerId == 0 ? MyScans : EnemyScans;
        public List<Drone> GetDrones(int playerId) => playerId == 0 ? MyDrones : EnemyDrones;
        public Drone GetDrone(int playerId, int id) => GetDrones(playerId).FirstOrDefault(drone => drone.Id == id);
        public Drone GetDrone(int droneId) => Drones.FirstOrDefault(drone => drone.Id == droneId);
        public Fish GetFish(int fishId) => Fishes.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetLostedFish(int fishId) => LostedFishes.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetMonster(int fishId) => Monsters.FirstOrDefault(fish => fish.Id == fishId);
        public Fish GetSwimmingFish(int fishId) => SwimmingFishes.FirstOrDefault(fish => fish.Id == fishId);
        public int GetSymmetricFishId(int fishId) => fishId + (fishId % 2 == 0 ? 1 : -1);
        public Fish GetSymmetricFish(Fish fish) => GetSwimmingFish(GetSymmetricFishId(fish.Id));

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

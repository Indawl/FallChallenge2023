using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameState : GameStateBase, ICloneable
    {
        public List<Fish> Monsters { get; private set; } = new List<Fish>();
        public List<Fish>[] ScannedFishes { get; private set; } = new List<Fish>[2];
        public List<Fish>[] UnscannedFishes { get; private set; } = new List<Fish>[2];

        public int Score => MyScore - EnemyScore;
        public IEnumerable<Fish> SwimmingFishes => Fishes.Where(_ => _.Status != FishStatus.LOSTED && _.Color != FishColor.UGLY);

        public void Initialize()
        {
            Monsters = Fishes.Where(_ => _.Color == FishColor.UGLY).ToList();

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
            state.Drones = CloneDrones(Drones);
            state.Fishes = CloneFishes(Fishes);
            state.Initialize();
            return state;
        }

        public List<Drone> CloneDrones(IEnumerable<Drone> drones)
        {
            var newDrones = new List<Drone>();
            foreach (var drone in drones)
                newDrones.Add((Drone)drone.Clone());
            return newDrones;
        }

        public List<Fish> CloneFishes(IEnumerable<Fish> fishes)
        {
            var newFishes = new List<Fish>();
            foreach (var fish in fishes)
                newFishes.Add((Fish)fish.Clone());
            return newFishes;
        }

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

        public Fish GetSymmetricFish(Fish fish)
        {
            var offset = fish.Color == FishColor.UGLY ? 16 : 4;
            var fishId = fish.Id + ((fish.Id - offset) % 2 == 0 ? 1 : -1);
            return GetFish(fishId);
        }
        

        public Vector GetAroundMonsterTo(Vector from, Vector to, Drone drone = null, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANLE)
        {
            var speed = to - from;
            if (speed.Length() > GameProperties.DRONE_MAX_SPEED) speed = (speed.Normalize() * GameProperties.DRONE_MAX_SPEED).Round();
            return GetAroundMonster(from, speed, drone, epsilon);
        }

        public Vector GetAroundMonster(Vector from, Vector speed, Drone drone = null, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANLE)
        {
            if (CheckCollisionWithMonsters(Fishes.Where(_ => _.Color == FishColor.UGLY && _.Speed != null), Drones, from, ref speed, drone, GameProperties.MONSTER_TRAVERSAL_TURNS, epsilon)) return from;
            else return from + speed;
        }

        private bool CheckCollisionWithMonsters(IEnumerable<Fish> fishes, IEnumerable<Drone> drones, Vector from, ref Vector speed, Drone drone = null, int forMoves = GameProperties.MONSTER_TRAVERSAL_TURNS, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANLE)
        {
            epsilon *= Math.PI / 180;

            var newTo = from + speed;
            var newSpeed = speed.IsZero() ? new Vector(GameProperties.DRONE_MAX_SPEED, 0) : speed;

            var alpha = 0.0;
            var wise = true;
            var collision = true;

            while (collision)
            {
                if (alpha > Math.PI) return true;

                while (collision)
                {
                    collision = false;

                    foreach (var fish in fishes)
                        while (CheckCollision(fish.Position, fish.Speed, from, newTo))
                        {
                            alpha = (wise ? epsilon : 0.0) - alpha;
                            wise = !wise;
                            if (alpha > Math.PI) return true;

                            var rSpeed = (newSpeed.Rotate(alpha).Round().Normalize() * GameProperties.DRONE_MAX_SPEED).Round();
                            newTo = SnapToDroneZone(from + rSpeed);
                            collision = true;
                        }
                }
                
                // Check next turn
                if (forMoves > 0)
                {
                    var nextFishes = CloneFishes(fishes);
                    var nextDrones = CloneDrones(drones);
                    if (drone != null) nextDrones.First(_ => _.Id == drone.Id).Position = newTo;
                    UpdateFishPositions(nextFishes, nextDrones);

                    var nextSpeed = newTo - from;
                    if (CheckCollisionWithMonsters(nextFishes, nextDrones, newTo, ref nextSpeed, drone, forMoves - 1, GameProperties.MONSTER_TRAVERSAL_ANLE_FAST))
                    {
                        alpha = (wise ? epsilon : 0.0) - alpha;
                        wise = !wise;
                        newTo = SnapToDroneZone(from + newSpeed.Rotate(alpha).Round());
                        collision = true;
                    }
                }
            }

            speed = newTo - from;
            return false;
        }

        

        

        private void DoScans()
        {
            foreach (var drone in Drones)
            {
                drone.NewScans.Clear();
                foreach (var fish in Fishes.Where(_ => _.Status != FishStatus.LOSTED && !GetScans(drone.PlayerId).Contains(_.Id) && !drone.Scans.Contains(_.Id)))
                    if (fish.Position.InRange(drone.Position, drone.LightRadius))
                    {
                        drone.Scans.Add(fish.Id);
                        drone.NewScans.Add(fish.Id);
                    }
            }
        }

        private void CheckDrones()
        {
            var monsters = Fishes.Where(_ => _.Speed != null && _.Color == FishColor.UGLY).ToList();

            foreach (var drone in Drones)
                foreach (var monster in monsters)
                    if (CheckCollision(monster.Position, monster.Speed, drone.Position - drone.Speed, drone.Position))
                    {
                        drone.Emergency = true;
                        drone.Lighting = false;
                        drone.Scans.Clear();
                        break;
                    }
        }

        public bool IsGameOver()
        {
            if (Turn > 200) return true;

            var fishes = Fishes.Where(_ => _.Status != FishStatus.LOSTED).ToList();

            for (int i = 0; i < 2; i++)
            {
                var savedFishes = GetScans(i);
                if (!fishes.All(_ => savedFishes.Contains(_.Id))) return false;
            }

            return true;
        }

        private void DoSave()
        {
            var scannedFishes = new List<int>[2];
            var bonusesFishes = new List<int>[2];
            var lastOneType = new List<FishType>[2];
            var lastOneColor = new List<FishColor>[2];
            var oneType = new List<FishType>[2];
            var oneColor = new List<FishColor>[2];

            for (int i = 0; i < 2; i++)
            {
                // Get drones for save
                var drones = GetDrones(i).Where(_ => _.Position.Y <= GameProperties.SURFACE).ToList();

                // Get score for fishes
                scannedFishes[i] = drones.SelectMany(_ => _.Scans).Distinct().ToList();
                bonusesFishes[i] = scannedFishes[i].Except(GetScannedFishes(1 - i)).ToList();

                // Set score
                foreach (var fishId in scannedFishes[i])
                    SetScore(i, GetScore(i) + GameProperties.REWARDS[GetFish(fishId).Type]);

                foreach (var fishId in bonusesFishes[i])
                    SetScore(i, GetScore(i) + GameProperties.REWARDS[GetFish(fishId).Type]);

                // Get last one type/color we had
                GetComboBonuses(i, out lastOneColor[i], out lastOneType[i]);

                // Scan fishes
                GetScans(i).AddRange(scannedFishes[i]);
                drones.ForEach(_ => _.Scans.Clear());
            }

            // Get bonuses for type/color
            for (int i = 0; i < 2; i++)
                GetComboBonuses(i, out oneColor[i], out oneType[i]);

            for (int i = 0; i < 2; i++)
            {
                // Get score for combo
                var comboColor = oneColor[i].Except(lastOneColor[i]).ToList();
                var comboBonusesColor = comboColor.Except(oneColor[1 - i]).ToList();

                var comboType = oneType[i].Except(lastOneType[i]).ToList();
                var comboTypeColor = comboType.Except(oneType[1 - i]).ToList();

                // Set score for combo
                SetScore(i, GetScore(i) + GameProperties.REWARDS[FishType.ONE_COLOR] * (comboColor.Count + comboBonusesColor.Count));
                SetScore(i, GetScore(i) + GameProperties.REWARDS[FishType.ONE_TYPE] * (comboType.Count + comboTypeColor.Count));
            }
        }

        private void GetComboBonuses(int playerId, out List<FishColor> oneColor, out List<FishType> oneType)
        {
            oneColor = new List<FishColor>();
            oneType = new List<FishType>();

            var scanFishes = GetScans(playerId).Select(_ => GetFish(_)).ToList();

            foreach (var color in GameProperties.COLORS)
            {
                var fishes = scanFishes.Where(c => c.Color == color).ToList();
                if (GameProperties.TYPES.All(_ => fishes.Exists(f => f.Type == _)))
                    oneColor.Add(color);
            }

            foreach (var type in GameProperties.TYPES)
            {
                var fishes = scanFishes.Where(c => c.Type == type).ToList();
                if (GameProperties.COLORS.All(_ => fishes.Exists(f => f.Color == _)))
                    oneType.Add(type);
            }
        }
    }
}

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
            { FishType.JELLY, new int[] { 2500, 4999 } },
            { FishType.FISH, new int[] { 5000, 7499 } },
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

        public List<Fish> PotencialFishes { get; set; } = new List<Fish>();

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
            state.PotencialFishes = new List<Fish>();
            foreach (var fish in PotencialFishes)
                state.PotencialFishes.Add((Fish)fish.Clone());
            return state;
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

        public List<int> GetScannedFishes(int playerId) => GetDrones(playerId).SelectMany(_ => _.Scans).Distinct().Union(GetScans(playerId)).ToList();

        public Fish GetSymmetricFish(Fish fish)
        {
            var offset = fish.Color == FishColor.UGLY ? 16 : 4;
            var fishId = fish.Id + ((fish.Id - offset) % 2 == 0 ? 1 : -1);
            return GetFish(fishId);
        }

        private Vector SnapToDroneZone(Vector position)
        {
            var newPosition = position;

            if (position.X < 0) newPosition = new Vector(0, position.Y);
            if (position.X > MAP_SIZE - 1) newPosition = new Vector(MAP_SIZE - 1, position.Y);
            if (position.Y < 0) newPosition = new Vector(position.X, 0);
            if (position.Y > MAP_SIZE - 1) newPosition = new Vector(position.X, MAP_SIZE - 1);

            return newPosition;
        }

        private Vector SnapToFishZone(FishType type, Vector position)
        {
            var newPosition = position;

            var habitat = HABITAT[type];
            if (position.X < 0) newPosition = new Vector(0, position.Y);
            if (position.X > MAP_SIZE - 1) newPosition = new Vector(MAP_SIZE - 1, position.Y);
            if (position.Y < habitat[0]) newPosition = new Vector(position.X, habitat[0]);
            if (position.Y > habitat[1]) newPosition = new Vector(position.X, habitat[1]);

            return newPosition;
        }

        public void UpdateFishPositions(IEnumerable<Fish> fishes)
        {
            // New Position
            foreach (var fish in fishes.Where(_ => _.Position != null && _.Speed != null))
                fish.Position = GetNextPosition(fish.Type, fish.Position, fish.Speed);

            // New Speed
            foreach (var fish in fishes.Where(_ => _.Position != null && _.Speed != null))
                fish.Speed = UpdateFishSpeed(fish);
        }

        public void UpdateFishPositions(IEnumerable<Fish> fishes, Func<Fish, bool> predicate)
        {
            UpdateFishPositions(fishes.Where(predicate));
        }

        private Vector GetNextPosition(FishType type, Vector position, Vector speed) => SnapToFishZone(type, position + speed);

        private Vector UpdateFishSpeed(Fish fish) => fish.Color == FishColor.UGLY ?
            UpdateUglySpeed(fish.Id, fish.Position, fish.Speed) :
            UpdateFishSpeed(fish.Id, fish.Type, fish.Position, fish.Speed);

        private Vector UpdateFishSpeed(int fishId, FishType type, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(Drones
                .Where(_ => !_.Emergency && _.Position.InRange(position, Drone.MOTOR_RANGE))
                .Select(_ => _.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((position - pos).Normalize() * Fish.FRIGHTENED_SPEED).Round();
            }
            else
            {
                // Near fish
                var fishesPositions = position.GetClosest(Fishes
                    .Where(_ => _.Position != null && _.Speed != null && _.Id != fishId && _.Color != FishColor.UGLY && _.Position.InRange(position, Fish.MIN_DISTANCE_BT_FISH))
                    .Select(_ => _.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = (position - pos).Normalize() * Fish.SPEED;
                }
                else speed = speed.Normalize() * Fish.SPEED;

                // Border
                var nextPosition = position + speed;

                var habitat = HABITAT[type];
                if (nextPosition.X < 0 || nextPosition.X > MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();

                speed = speed.EpsilonRound().Round();
            }

            return speed;
        }

        private Vector UpdateUglySpeed(int fishId, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(Drones
                .Where(_ => !_.Emergency && _.Position.InRange(position, _.LightRadius))
                .Select(_ => _.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((pos - position).Normalize() * Fish.MONSTER_ATTACK_SPEED).Round();
            }
            else 
            {
                speed = speed.Normalize() * Fish.MONSTER_SPEED;

                // Near other ugly
                var fishesPositions = position.GetClosest(Fishes
                    .Where(_ => _.Position != null && _.Speed != null && _.Id != fishId && _.Color == FishColor.UGLY && _.Position.InRange(position, Fish.MIN_DISTANCE_BT_MONSTER))
                    .Select(_ => _.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = (position - pos).Normalize() * Fish.MONSTER_SPEED;
                }

                speed = speed.Round();

                // Border
                var nextPosition = position + speed;

                var habitat = HABITAT[FishType.ANGLER];
                if (nextPosition.X < 0 || nextPosition.X > MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();
            }

            return speed;
        }

        public Vector GetAroundMonsterTo(Vector from, Vector to, double epsilon = 0.1)
        {
            var speed = to - from;
            if (speed.LengthSqr() > Drone.MAX_SPEED_SQR) speed = (speed.Normalize() * Drone.MAX_SPEED).Round();
            return GetAroundMonster(from, speed, epsilon);
        }

        public Vector GetAroundMonster(Vector from, Vector speed, double epsilon = 0.1)
        {
            var newTo = from + speed;

            speed = speed.Normalize() * Drone.MAX_SPEED;
            epsilon *= Math.PI / 180;

            var alpha = 0.0;
            var collision = true;
            var wise = true;

            while (collision)
            {
                collision = false;
                foreach (var fish in Fishes.Where(_ => _.Color == FishColor.UGLY && _.Speed != null))
                    while (CheckCollision(fish.Position, fish.Speed, from, newTo))
                    {
                        alpha = (wise ? epsilon : 0.0) - alpha;
                        wise = !wise;
                        newTo = SnapToDroneZone(from + speed.Rotate(alpha)).Round();
                        collision = true;
                        if (alpha > Math.PI) return newTo;
                    }
            }

            return newTo;
        }

        public static bool CheckCollision(Vector fishPostion, Vector fishSpeed, Vector droneFrom, Vector droneTo)
        {
            if (fishSpeed.IsZero() && droneTo.Equals(droneFrom)) return false;

            var pos = fishPostion - droneFrom;
            var vd = droneTo - droneFrom;
            if (vd.Equals(fishSpeed)) return false;

            var vf = fishSpeed - vd;
            var a = vf.LengthSqr();
            var b = 2.0 * Vector.Dot(pos, vf);
            var c = pos.LengthSqr() - Fish.MONSTER_ATTACK_RADIUS_SQR;
            var delta = b * b - 4.0 * a * c;
            if (delta < 0.0) return false;

            double t = (-b - Math.Sqrt(delta)) / (2.0 * a);

            if (t <= 0.0 || t > 1.0) return false;

            return true;
        }

        public void FindPotencialFishes()
        {
            PotencialFishes = new List<Fish>();
            foreach (var fish in Fishes)
                PotencialFishes.Add((Fish)fish.Clone());

            UpdateFishPositions(PotencialFishes);
        }
    }
}

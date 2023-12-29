using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameReferee
    {
        public GameState State { get; }

        public GameReferee(GameState state)
        {
            State = state;
        }

        public void UpdateFishPositions()
        {
            UpdateFishPositions(State.Fishes);
        }

        public void UpdateFishPositions(Func<Fish, bool> predicate)
        {
            UpdateFishPositions(State.Fishes.Where(predicate));
        }

        public void SetNextState(List<DroneAgent> agents = null)
        {
            State.Turn++;
            if (agents != null) 
                agents.ForEach(_ => UpdateDronePosition(State.Drones.First(d => d.Id == _.Drone.Id), _.Action));
            UpdateFishPositions();
            DoScans();
            DoSave();
        }

        public void Finish()
        {
            foreach (var drone in State.Drones)
                drone.Position = new Vector();

            DoSave();
        }

        public bool IsGameOver()
        {
            if (State.Turn > 200) return true;

            for (int i = 0; i < 2; i++)
            {
                var savedFishes = State.GetScans(i);
                if (!State.SwimmingFishes.All(_ => savedFishes.Contains(_.Id))) return false;
            }

            return true;
        }

        private void UpdateDronePosition(Drone drone, GameAction action)
        {
            var oldPosition = drone.Position;

            // Do action
            if (drone.Emergency)
                drone.Position -= new Vector(0, GameProperties.DRONE_EMERGENCY_SPEED);
            else if (action is GameActionMove)
            {
                var a = action as GameActionMove;
                drone.Position = a.Position;
                drone.Lighting = a.Light;
            }
            else if (action is GameActionWait)
            {
                var a = action as GameActionWait;
                drone.Position += new Vector(0, GameProperties.DRONE_SINK_SPEED);
                drone.Lighting = a.Light;
            }

            // Snap zone
            drone.Position = GameUtils.SnapToDroneZone(drone.Position);

            // Set speed
            drone.Speed = drone.Position - oldPosition;

            // Check collision
            foreach (var fish in State.Fishes.Where(_ => _.Color == FishColor.UGLY && _.Speed != null))
                if (GameUtils.CheckCollision(fish.Position, fish.Speed, drone.Position - drone.Speed, drone.Position))
                {
                    drone.Emergency = true;
                    drone.Lighting = false;
                    drone.Scans.Clear();
                    drone.NewScans.Clear();
                    break;
                }

            // Check battery
            if (drone.Lighting)
                if (drone.Battery < GameProperties.BATTERY_DRAIN) drone.Lighting = false;
                else drone.Battery -= GameProperties.BATTERY_DRAIN;
            else if (!drone.Emergency && drone.Battery <= GameProperties.MAX_BATTERY - GameProperties.BATTERY_RECHARGE)
                drone.Battery += GameProperties.BATTERY_RECHARGE;
                     
            // Check repair
            if (drone.Position.Y == 0) drone.Emergency = false;
        }

        private void UpdateFishPositions(IEnumerable<Fish> fishes)
        {
            // New Position
            foreach (var fish in fishes.Where(_ => _.Position != null && _.Speed != null))
                fish.Position = GetNextPosition(fish.Type, fish.Position, fish.Speed);

            // New Speed
            foreach (var fish in fishes.Where(_ => _.Position != null && _.Speed != null))
                fish.Speed = GetFishSpeed(fish);
        }

        private Vector GetNextPosition(FishType type, Vector position, Vector speed) => GameUtils.SnapToFishZone(type, position + speed);

        private Vector GetFishSpeed(Fish fish) => fish.Color == FishColor.UGLY ?
            GetUglySpeed(fish.Id, fish.Position, fish.Speed) :
            GetFishSpeed(fish.Id, fish.Type, fish.Position, fish.Speed);

        private Vector GetFishSpeed(int fishId, FishType type, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(State.Drones
                .Where(_ => !_.Emergency && _.Position.InRange(position, GameProperties.MOTOR_RANGE))
                .Select(_ => _.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((position - pos).Normalize() * GameProperties.FISH_FRIGHTENED_SPEED).Round();
            }
            else
            {
                // Near fish
                var fishesPositions = position.GetClosest(State.Fishes
                    .Where(_ => _.Position != null && _.Speed != null && _.Id != fishId && _.Color != FishColor.UGLY && _.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_FISH))
                    .Select(_ => _.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = (position - pos).Normalize() * GameProperties.FISH_SPEED;
                }
                else speed = speed.Normalize() * GameProperties.FISH_SPEED;

                // Border
                var nextPosition = position + speed;

                var habitat = GameProperties.HABITAT[type];
                if (nextPosition.X < 0 || nextPosition.X > GameProperties.MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();

                speed = speed.EpsilonRound().Round();
            }

            return speed;
        }

        private Vector GetUglySpeed(int fishId, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(State.Drones
                .Where(_ => !_.Emergency && _.Position.InRange(position, _.LightRadius))
                .Select(_ => _.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((pos - position).Normalize() * GameProperties.MONSTER_ATTACK_SPEED).Round();
            }
            else
            {
                if (speed.Length() > GameProperties.MONSTER_SPEED)
                    speed = (speed.Normalize() * GameProperties.MONSTER_SPEED).Round();

                // Near other ugly
                var fishesPositions = position.GetClosest(State.Fishes
                    .Where(_ => _.Position != null && _.Speed != null && _.Id != fishId && _.Color == FishColor.UGLY && _.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_MONSTER))
                    .Select(_ => _.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = ((position - pos).Normalize() * GameProperties.FISH_SPEED).Round(); // Fish.MONSTER_SPEED; // repeat error from referee
                }

                // Border
                var nextPosition = position + speed;

                var habitat = GameProperties.HABITAT[FishType.ANGLER];
                if (nextPosition.X < 0 || nextPosition.X > GameProperties.MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();
            }

            return speed;
        }
            
        private void DoScans()
        {
            foreach (var drone in State.Drones)
            {
                drone.NewScans.Clear();
                foreach (var fish in State.SwimmingFishes.Where(_ => !State.GetScans(drone.PlayerId).Contains(_.Id) && !drone.Scans.Contains(_.Id)))
                    if (fish.Position.InRange(drone.Position, drone.LightRadius))
                    {
                        drone.Scans.Add(fish.Id);
                        drone.NewScans.Add(fish.Id);
                    }
            }
        }

        private void DoSave()
        {
            var scannedFishes = new List<Fish>[2];
            var bonusesFishes = new List<Fish>[2];
            var lastOneType = new List<FishType>[2];
            var lastOneColor = new List<FishColor>[2];
            var oneType = new List<FishType>[2];
            var oneColor = new List<FishColor>[2];

            for (int i = 0; i < 2; i++)
            {
                // Get drones for save
                var drones = State.GetDrones(i).Where(_ => _.Position.Y <= GameProperties.SURFACE).ToList();

                // Get score for fishes
                scannedFishes[i] = drones.SelectMany(_ => _.Scans).Distinct().Select(_ => State.GetFish(_)).ToList();
                bonusesFishes[i] = scannedFishes[i].Except(State.ScannedFishes[1 - i]).ToList();//error only enemy scans+dronsurface scan

                // Set score
                foreach (var fish in scannedFishes[i])
                    State.AddScore(i, GameProperties.REWARDS[fish.Type]);

                foreach (var fish in bonusesFishes[i])
                    State.AddScore(i, GameProperties.REWARDS[fish.Type]);

                // Get last one type/color we had
                GetComboBonuses(i, out lastOneColor[i], out lastOneType[i]);

                // Scan fishes
                State.GetScans(i).AddRange(scannedFishes[i].Select(_ => _.Id));
                drones.ForEach(_ => { _.Scans.Clear(); _.NewScans.Clear(); });
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
                State.AddScore(i, GameProperties.REWARDS[FishType.ONE_COLOR] * (comboColor.Count + comboBonusesColor.Count));
                State.AddScore(i, GameProperties.REWARDS[FishType.ONE_TYPE] * (comboType.Count + comboTypeColor.Count));
            }
        }

        private void GetComboBonuses(int playerId, out List<FishColor> oneColor, out List<FishType> oneType)
        {
            oneColor = new List<FishColor>();
            oneType = new List<FishType>();

            var scanFishes = State.GetScans(playerId).Select(_ => State.GetFish(_)).ToList();

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

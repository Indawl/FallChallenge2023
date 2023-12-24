using DevLib.Game;
using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class Bot : IGameBot
    {
        public Stopwatch StopWatch => new Stopwatch();

        protected List<DroneAgent> Agents { get; } = new List<DroneAgent>();

        #region Read from console
        private GameState State { get; set; }

        public void ReadInitialize()
        {
            State = new GameState();

            var creatureCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < creatureCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                State.Fishes.Add(new Fish(int.Parse(inputs[0]), (FishColor)int.Parse(inputs[1]), (FishType)int.Parse(inputs[2])));
            }
        }

        public IGameState ReadState()
        {
            // Read new state
            State.Turn++;

            for (int k = 0; k < 2; k++)
                State.SetScore(k, int.Parse(Console.ReadLine()));

            for (int k = 0; k < 2; k++)
            {
                State.GetScans(k).Clear();
                var scanCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < scanCount; i++)
                    State.GetScans(k).Add(int.Parse(Console.ReadLine()));
            }

            var dronesRadarBlips = new Dictionary<int, List<RadarBlip>>();
            for (int k = 0; k < 2; k++)
            {
                var droneCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < droneCount; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');

                    var droneId = int.Parse(inputs[0]);
                    var drone = State.GetDrone(droneId) ?? State.GetNewDrone(droneId, k);

                    var battery = drone.Battery;
                    dronesRadarBlips[drone.Id] = drone.RadarBlips;

                    drone.Speed = drone.Position;
                    drone.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                    drone.Speed = drone.Speed == null ? new Vector() : (drone.Position - drone.Speed);
                    drone.Emergency = int.Parse(inputs[3]) == 1;
                    drone.Battery = int.Parse(inputs[4]);
                    drone.Lighting = drone.Battery < battery;
                    drone.NewScans = drone.Scans;

                    drone.Scans = new List<int>();
                    drone.RadarBlips = new List<RadarBlip>();
                }
            }

            var droneScanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < droneScanCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                State.GetDrone(int.Parse(inputs[0])).Scans.Add(int.Parse(inputs[1]));
            }

            State.Drones.ForEach(_ => _.NewScans = _.Scans.Except(_.NewScans).ToList());
            State.Fishes.Where(_ => _.Status == FishStatus.VISIBLE).ToList().ForEach(_ => _.Status = FishStatus.UNVISIBLE);

            var lostedFishes = State.Fishes.Select(_ => _.Id).ToList();
            var fishesCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < fishesCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fish = State.GetFish(int.Parse(inputs[0]));

                fish.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                fish.Speed = new Vector(int.Parse(inputs[3]), int.Parse(inputs[4]));
                fish.Status = FishStatus.VISIBLE;
            }

            var radarBlipCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < radarBlipCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var drone = State.GetDrone(int.Parse(inputs[0]));
                var radarBlip = new RadarBlip(int.Parse(inputs[1]), (BlipType)Enum.Parse(typeof(BlipType), inputs[2]));
                if (dronesRadarBlips[drone.Id].Any()) radarBlip.LastType = dronesRadarBlips[drone.Id].First(_ => _.FishId == radarBlip.FishId).Type;
                else radarBlip.LastType = radarBlip.Type;
                drone.RadarBlips.Add(radarBlip);
                lostedFishes.Remove(radarBlip.FishId);
            }
            lostedFishes.ForEach(_ => State.GetFish(_).Status = FishStatus.LOSTED);
#if TEST_MODE
            Console.Error.WriteLine(JsonSerializer.Serialize<GameStateBase>(State));
#endif            
            return State;
        }
        #endregion

        private void UpdateFishPositions(GameState state)
        {
            // New Position
            foreach (var fish in state.Fishes.Where(_ => _.Position != null && _.Status == FishStatus.UNVISIBLE))
                SetNextPosition(fish);

            // New Speed
            foreach (var fish in state.Fishes.Where(_ => _.Position != null && _.Status == FishStatus.UNVISIBLE))
                if (fish.Color == FishColor.UGLY)
                    UpdateUglySpeed(state, fish);
                else
                    UpdateFishSpeed(state, fish);
        }

        private void SetNextPosition(Fish fish)
        {
            fish.Position += fish.Speed;

            var habitat = GameState.HABITAT[fish.Type];
            if (fish.Position.X < 0) fish.Position = new Vector(0, fish.Position.Y);
            if (fish.Position.X > GameState.MAP_SIZE - 1) fish.Position = new Vector(GameState.MAP_SIZE - 1, fish.Position.Y);
            if (fish.Position.Y < habitat[0]) fish.Position = new Vector(fish.Position.X, habitat[0]);
            if (fish.Position.Y > habitat[1]) fish.Position = new Vector(fish.Position.X, habitat[1]);
        }

        private void UpdateFishSpeed(GameState state, Fish fish)
        {
            var dronePositions = state.Drones
                .Where(_ => !_.Emergency && _.Position.InRange(fish.Position, Drone.MOTOR_RANGE))
                .Select(_ => _.Position)
                .ToList();
            if (dronePositions.Any())
            {
                var position = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                fish.Speed = ((fish.Position - position).Normalize() * Fish.FRIGHTENED_SPEED).EpsilonRound().Round();
            }
            else
            {
                var fishesPositions = state.Fishes
                    .Where(_ => _.Position != null && _ != fish && _.Color != FishColor.UGLY && _.Position.InRange(fish.Position, Fish.MIN_DISTANCE_BT_FISH))
                    .Select(_ => _.Position)
                    .ToList();
                if (fishesPositions.Any())
                {
                    var position = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    fish.Speed = ((fish.Position - position).Normalize() * Fish.SPEED).EpsilonRound().Round();
                }
                else fish.Speed = (fish.Speed.Normalize() * Fish.SPEED).EpsilonRound().Round();

                var habitat = GameState.HABITAT[fish.Type];
                var nextPosition = fish.Position + fish.Speed;

                if (nextPosition.X < 0 || nextPosition.X > GameState.MAP_SIZE - 1) fish.Speed = fish.Speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) fish.Speed = fish.Speed.VSymmetric();
            }
        }

        private void UpdateUglySpeed(GameState state, Fish fish)
        {
            var dronePositions = state.Drones
                .Where(_ => !_.Emergency && _.Position.InRange(fish.Position, _.LightRadius))
                .Select(_ => _.Position)
                .ToList();
            if (dronePositions.Any())
            {
                var position = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                fish.Speed = ((position - fish.Position).Normalize() * Fish.MONSTER_ATTACK_SPEED).Round();
            }
            else if (fish.Speed != null && !fish.Speed.IsZero())
            {
                fish.Speed = (fish.Speed.Normalize() * Fish.MONSTER_SPEED).Round();

                var fishesPositions = state.Fishes
                    .Where(_ => _.Position != null && _ != fish && _.Color == FishColor.UGLY && _.Position.InRange(fish.Position, Fish.MIN_DISTANCE_BT_MONSTER))
                    .Select(_ => _.Position)
                    .ToList();
                if (fishesPositions.Any())
                {
                    var position = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    fish.Speed = ((fish.Position - position).Normalize() * Fish.MONSTER_SPEED).Round();
                }

                var habitat = GameState.HABITAT[fish.Type];
                var nextPosition = fish.Position + fish.Speed;

                if (nextPosition.X < 0 || nextPosition.X > GameState.MAP_SIZE - 1) fish.Speed = fish.Speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) fish.Speed = fish.Speed.VSymmetric();
            }
        }

        private void FindFishPositions(GameState state)
        {
            // From Enemy Scans
            foreach (var drone in state.GetDrones(1))
                foreach (var fishId in drone.NewScans)
                {
                    var fish = state.GetFish(fishId);
                    if (fish.Status == FishStatus.UNVISIBLE) // We dont see fish
                        if (fish.Position == null)  // Never assume
                        {
                            fish.Position = new Vector(drone.Position);
                            fish.Speed = new Vector();
                        }
                        else if (!fish.Position.InRange(drone.Position, drone.LightRadius)) // Wrong assume => correct it
                        {
                            fish.Position = drone.Position + (fish.Position - drone.Position).Normalize() * drone.LightRadius;
                            if (fish.Position.InRange(drone.Position, Drone.MOTOR_RANGE))
                                fish.Speed = (fish.Position - drone.Position).Normalize() * Fish.FRIGHTENED_SPEED;
                            else fish.Speed = new Vector();
                        }
                }

            // Symmetric Fish
            foreach (var fish in state.Fishes.Where(_ => _.Status == FishStatus.VISIBLE))
            {
                var sFish = state.GetSymmetricFish(fish);
                if (sFish.Position == null)
                {
                    sFish.Position = fish.Position.HSymmetric(GameState.CENTER.X);
                    if (fish.Color == FishColor.UGLY) sFish.Speed = new Vector();
                    else if (state.GetDrones(0).Any(_ => _.Position.InRange(fish.Position, Drone.MOTOR_RANGE))) sFish.Speed = -fish.Speed.HSymmetric().Normalize() * Fish.SPEED;
                    else sFish.Speed = fish.Speed.HSymmetric();
                }
            }

            // Correct position from radar
            foreach (var fish in state.Fishes.Where(_ => _.Status == FishStatus.UNVISIBLE))
            {
                var habitat = GameState.HABITAT[fish.Type];
                var range = new RectangleRange(0, habitat[0], GameState.MAP_SIZE, habitat[1]);

                foreach (var drone in state.GetDrones(0))
                    range = range.Intersect(drone.RadarBlips.First(_ => _.FishId == fish.Id).GetRange(drone.Position));

                if (fish.Position == null)
                {
                    fish.Position = range.Center;
                    fish.Speed = new Vector();
                }
                else if (!fish.Position.InRange(range))
                {
                    fish.Position = range.Intersect(fish.Position, range.Center);
                    fish.Speed = new Vector();
                }

                foreach (var drone in state.GetDrones(0))
                    if (fish.Position.InRange(drone.Position, drone.LightRadius))
                    {
                        fish.Position = range.Center; // set to centre of range
                        if (fish.Position.InRange(drone.Position, drone.LightRadius)) // still in light, go to far of ligth
                            fish.Position = drone.Position + (fish.Position - drone.Position).Normalize() * drone.LightRadius;
                        fish.Speed = new Vector();
                    }
            }
        }

        public IGameAction GetAction(IGameState gameState)
        {
            var state = gameState as GameState;

            // Calculate new positions
            UpdateFishPositions(state);

            // Find fish's positions
            FindFishPositions(state);

            // Create agents
            CreateAgents(state);

            // Determinate actions for agents
            foreach (var agent in Agents)
                agent.FindAction(state);

            return new GameActionList(Agents.Select(_ => (IGameAction)_.Action).ToList());
        }

        private void CreateAgents(GameState state)
        {
            if (Agents.Any()) return;

            foreach (var drone in state.GetDrones(0))
                Agents.Add(new DroneAgent(drone));
        }
    }
}

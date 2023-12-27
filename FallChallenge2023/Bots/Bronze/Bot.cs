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
        public const int SERIAL_FROM_TURN = 0;
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

            // Drones
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

            // Drone's scans
            var droneScanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < droneScanCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                State.GetDrone(int.Parse(inputs[0])).Scans.Add(int.Parse(inputs[1]));
            }

            State.Drones.ForEach(_ => _.NewScans = _.Scans.Except(_.NewScans).ToList());
            State.Fishes.Where(_ => _.Status == FishStatus.VISIBLE).ToList().ForEach(_ => _.Status = FishStatus.UNVISIBLE);

            // Visible fishs
            var lostedFishes = State.Fishes.Where(_ => _.Status != FishStatus.LOSTED).Select(_ => _.Id).ToList();
            var fishesCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < fishesCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fish = State.GetFish(int.Parse(inputs[0]));

                fish.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                fish.Speed = new Vector(int.Parse(inputs[3]), int.Parse(inputs[4]));
                fish.Status = FishStatus.VISIBLE;
            }

            // Radar blips
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
            lostedFishes.ForEach(_ =>
            {
                var fish = State.GetFish(_);
                fish.Status = FishStatus.LOSTED;
                fish.Position = null;
                fish.Speed = null;
            });

#if TEST_MODE
            if (State.Turn > SERIAL_FROM_TURN)
                Console.Error.WriteLine(JsonSerializer.Serialize<GameStateBase>(State));
#endif            
            return State;
        }
        #endregion

        private void FindFishPositions(GameState state)
        {
            // From Enemy Scans
            foreach (var drone in state.GetDrones(1))
                foreach (var fishId in drone.NewScans)
                {
                    var fish = state.GetFish(fishId);
                    if (fish.Position == null)  // Never assume
                        fish.Position = new Vector(drone.Position);
                    else if (!fish.Position.InRange(drone.Position, drone.LightRadius)) // Wrong assume => correct it
                    {
                        fish.Position = drone.Position + (fish.Position - drone.Position).Normalize() * drone.LightRadius;
                        fish.Speed = null;
                    }
                    if (fish.Status == FishStatus.UNKNOWED) fish.Status = FishStatus.UNVISIBLE;
                }

            // Symmetric Fish
            var scannedFish = state.Drones.SelectMany(_ => _.NewScans).ToList();
            foreach (var fish in state.Fishes.Where(_ => _.Position != null && scannedFish.Contains(_.Id)))
            {
                var sFish = state.GetSymmetricFish(fish);
                if (sFish.Position == null || sFish.Status == FishStatus.UNKNOWED)
                {
                    sFish.Position = fish.Position.HSymmetric(GameState.CENTER.X);
                    if (fish.Speed != null)
                    {
                        if (state.GetDrones(0).Any(_ => _.Position.InRange(fish.Position, Drone.MOTOR_RANGE))) sFish.Speed = -fish.Speed.HSymmetric().Normalize() * Fish.SPEED;
                        else sFish.Speed = fish.Speed.HSymmetric();
                    }
                    if (sFish.Status == FishStatus.UNKNOWED) sFish.Status = FishStatus.UNVISIBLE;
                }
            }

            // Correct position from radar
            foreach (var fish in state.Fishes.Where(_ => _.Status == FishStatus.UNVISIBLE || _.Status == FishStatus.UNKNOWED))
            {
                var habitat = GameState.HABITAT[fish.Type];
                var range = new RectangleRange(0, habitat[0], GameState.MAP_SIZE - 1, habitat[1]);

                foreach (var drone in state.GetDrones(0))
                    range = range.Intersect(drone.RadarBlips.First(_ => _.FishId == fish.Id).GetRange(drone.Position));

                if (fish.Position == null)
                    fish.Position = range.Center;
                else if (!fish.Position.InRange(range))
                {
                    fish.Position = range.Intersect(fish.Position, range.Center);
                    fish.Speed = null;
                }

                // If my light, but i dont see it
                foreach (var drone in state.GetDrones(0))
                    if (fish.Position.InRange(drone.Position, drone.LightRadius))
                    {
                        fish.Position = range.Center; // set to centre of range
                        if (fish.Position.InRange(drone.Position, drone.LightRadius)) // still in light, go to far of ligth
                            fish.Position = drone.Position + (fish.Position - drone.Position).Normalize() * drone.LightRadius;
                        fish.Speed = null;
                    }
            }
        }

        public IGameAction GetAction(IGameState gameState)
        {
            var state = gameState as GameState;

            // Calculate new positions
            state.UpdateFishPositions(state.Fishes, _ => _.Status == FishStatus.UNVISIBLE);

            // Find fish's positions
            FindFishPositions(state);

            // Find potencial fish positions
            state.FindPotencialFishes();

            // Create agents
            CreateAgents(state);

            // Determinate actions for agents
            foreach (var agent in Agents)
                agent.FindAction(state);

            return new GameActionList(Agents.Select(_ => (IGameAction)_.Action).ToList());
        }

        private void CreateAgents(GameState state)
        {
            if (!Agents.Any())
                foreach (var drone in state.GetDrones(0))
                    Agents.Add(new DroneAgent(drone));

            // Distribute tasks
            var minAgent = Agents[0].Drone.Position.X <= Agents[1].Drone.Position.X ? 0 : 1;
            var maxAgent = 1 - minAgent;
            var center = (Agents[0].Drone.Position.X + Agents[1].Drone.Position.X) / 2;

            Agents[0].Fishes = state.Fishes.Where(_ => _.Status != FishStatus.LOSTED && _.Color != FishColor.UGLY && _.Position.X <= center).ToList();
            Agents[1].Fishes = state.Fishes.Where(_ => _.Status != FishStatus.LOSTED && _.Color != FishColor.UGLY && _.Position.X > center).ToList();

            Agents[0].Uglys = state.Fishes.Where(_ => _.Color == FishColor.UGLY && _.Position.X <= center).ToList();
            Agents[1].Uglys = state.Fishes.Where(_ => _.Color == FishColor.UGLY && _.Position.X > center).ToList();

            var scannedFish = state.GetScannedFishes(0);

            for (int i = 0; i < 2; i++)
                Agents[i].UnscannedFishes = Agents[i].Fishes.Where(_ => !scannedFish.Contains(_.Id)).ToList();
        }
    }
}

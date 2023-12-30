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

        public List<DroneAgent> Agents { get; protected set; } = new List<DroneAgent>();

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
            State.Fishes.ForEach(_ => { if (_.Status == FishStatus.VISIBLE) _.Status = FishStatus.UNVISIBLE; });

            // Visible fishs
            var lostedFishes = State.SwimmingFishes.Select(_ => _.Id).ToList();
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

            // Set losted status
            lostedFishes.ForEach(_ =>
            {
                var fish = State.GetFish(_);
                fish.Status = FishStatus.LOSTED;
                fish.Position = null;
                fish.Speed = null;
            });

#if TEST_MODE
            if (State.Turn > GameProperties.SERIAL_FROM_TURN)
                Console.Error.WriteLine(JsonSerializer.Serialize<GameStateBase>(State));
#endif            
            return State;
        }
        #endregion

        public IGameAction GetAction(IGameState gameState)
        {
            var state = gameState as GameState;

            // Init state
            state.Initialize();

            // Calculate new positions
            var referee = new GameReferee(state);
            referee.UpdateFishPositions(_ => _.Status == FishStatus.UNVISIBLE);

            // Find fish's positions
            FindFishPositions(state);

            // Create agents
            CreateAgents(state);
            DistributeAgents(state, 0);

            // Calculate potencial score
            //CalculateScore(state);

            // Determinate actions for agents
            foreach (var agent in Agents)
                agent.FindAction();

            return new GameActionList(Agents.Select(_ => (IGameAction)_.Action).ToList());
        }

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
                        fish.Position = drone.Position + ((fish.Position - drone.Position).Normalize() * drone.LightRadius).Round();
                        fish.Speed = null;
                    }
                    if (fish.Status == FishStatus.UNKNOWED) fish.Status = FishStatus.UNVISIBLE;
                }

            // Symmetric Fish
            var scannedFish = state.Drones
                .SelectMany(_ => _.NewScans)
                .Distinct()
                .Union(state.Fishes.Where(_ => _.Status == FishStatus.VISIBLE && _.Color == FishColor.UGLY).Select(_ => _.Id))
                .Select(_ => state.GetFish(_))
                .Where(_ => _.Position != null)
                .ToList();
            foreach (var fish in scannedFish)
            {
                var sFish = state.GetSymmetricFish(fish);
                if (sFish.Position == null || sFish.Status == FishStatus.UNKNOWED)
                {
                    sFish.Position = fish.Position.HSymmetric(GameProperties.CENTER.X);
                    if (fish.Speed != null && fish.Speed.LengthSqr() < GameProperties.FISH_SPEED_SQR_DELTA) sFish.Speed = fish.Speed.HSymmetric();
                    if (sFish.Status == FishStatus.UNKNOWED) sFish.Status = FishStatus.UNVISIBLE;
                }
            }

            // Correct position from radar
            foreach (var fish in state.Fishes.Where(_ => _.Status == FishStatus.UNVISIBLE || _.Status == FishStatus.UNKNOWED))
            {
                var habitat = GameProperties.HABITAT[fish.Type];
                var range = new RectangleRange(0, habitat[0], GameProperties.MAP_SIZE - 1, habitat[1]);

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
                            fish.Position = drone.Position + ((fish.Position - drone.Position).Normalize() * drone.LightRadius).Round();
                        fish.Speed = null;
                    }
            }
        }

        protected void CreateAgents(GameState state)
        {
            if (!Agents.Any())
                foreach (var drone in state.GetDrones(0))
                    Agents.Add(new DroneAgent(drone.Id));
            Agents.ForEach(_ => _.Initialize(state));
        }

        protected void DistributeAgents(GameState state, int playerId)
        {
            // Defined agents
            var agents = Agents.Where(_ => _.Drone.PlayerId == playerId).ToList();

            var agentId = agents[0].Drone.Position.X <= agents[1].Drone.Position.X ? 0 : 1;
            agents[agentId].LessX = -1;

            if (agents[0].Drone.Emergency || agents[0].Goal != null) agents[0] = agents[1];
            else if (agents[1].Drone.Emergency || agents[1].Goal != null) agents[1] = agents[0];

            // Distribute tasks
            foreach (var fish in state.UnscannedFishes[playerId])
                agents[fish.Position.X <= GameProperties.CENTER.X ? agentId : 1 - agentId].UnscannedFishes.Add(fish);

            // Share tasks
            if (agents[0].UnscannedFishes.Any() && !agents[1].UnscannedFishes.Any()) ShareTasks(agents[0], agents[1]);
            else if (agents[1].UnscannedFishes.Any() && !agents[0].UnscannedFishes.Any()) ShareTasks(agents[1], agents[0]);
        }

        private void ShareTasks(DroneAgent fromAgent, DroneAgent toAgent)
        {
            var fish = fromAgent.UnscannedFishes.OrderBy(_ => (_.Position - toAgent.Drone.Position).LengthSqr()).First();
            if (fromAgent.UnscannedFishes.Count > 1 || (fish.Position - toAgent.Drone.Position).LengthSqr() < (fish.Position - fromAgent.Drone.Position).LengthSqr())
            {
                toAgent.UnscannedFishes.Add(fish);
                fromAgent.UnscannedFishes.Remove(fish);
            }
        }
    }
}

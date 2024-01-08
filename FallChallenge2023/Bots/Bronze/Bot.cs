using DevLib.Game;
using FallChallenge2023.Bots.Bronze.GameMath;
using FallChallenge2023.Bots.Bronze.Simulations;
using System;
using System.Diagnostics;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class Bot : IGameBot
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public Stopwatch StopWatch => _stopwatch;

        protected MinMax Simultation { get; private set; } = new MinMax()
        {            
            TimeOutTime = 49,
            Depth = 5
        };

        public Bot()
        {
            Simultation.StopWatch = StopWatch;
        }

        #region Read from console
        private GameState State { get; set; }

        public void ReadInitialize()
        {
            State = new GameState();

            var creatureCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < creatureCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fish = new Fish(int.Parse(inputs[0]), (FishColor)int.Parse(inputs[1]), (FishType)int.Parse(inputs[2]));
                if (fish.Type == FishType.ANGLER) State.Monsters.Add(fish);
                else State.Fishes.Add(fish);
            }

            // Initialize positions
            FindStartPositions(State);
        }

        public IGameState ReadState()
        {
            StopWatch.Restart();

            // Read new state
            var previousState = State;
            State = (GameState)State.Clone();
            State.Turn++;

            for (int playerId = 0; playerId < 2; playerId++)
                State.SetScore(playerId, int.Parse(Console.ReadLine()));

            for (int playerId = 0; playerId < 2; playerId++)
            {
                State.GetScans(playerId).Clear();
                var scanCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < scanCount; i++)
                    State.GetScans(playerId).Add(int.Parse(Console.ReadLine()));
            }

            // Drones
            for (int playerId = 0; playerId < 2; playerId++)
            {
                var droneCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < droneCount; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');

                    var droneId = int.Parse(inputs[0]);
                    var drone = State.GetDrone(playerId, droneId) ?? State.GetNewDrone(playerId, droneId);
                    var lastDrone = previousState.GetDrone(playerId, droneId);

                    drone.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                    drone.Speed = lastDrone == null ? new Vector() : (drone.Position - lastDrone.Position);
                    drone.MotorOn = !drone.Equals(new Vector(0, GameProperties.DRONE_SINK_SPEED));
                    drone.Emergency = int.Parse(inputs[3]) == 1;
                    drone.Battery = int.Parse(inputs[4]);
                    drone.Lighting = drone.Battery < (lastDrone == null ? GameProperties.MAX_BATTERY : lastDrone.Battery);
                    drone.LightRadius = drone.Lighting ? GameProperties.LIGHT_SCAN_RADIUS : GameProperties.DARK_SCAN_RADIUS;

                    drone.Scans.Clear();
                }
            }

            // Drone's scans
            var droneScanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < droneScanCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                State.GetDrone(int.Parse(inputs[0])).Scans.Add(int.Parse(inputs[1]));
            }

            // Drone's new scans
            foreach (var drone in State.Drones)
            {
                var lastDrone = previousState.GetDrone(drone.PlayerId, drone.Id);
                if (lastDrone != null) drone.NewScans = drone.Scans.Except(lastDrone.Scans).ToHashSet();
            }

            // Visible fishs
            State.VisibleFishes.Clear();

            var fishesCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < fishesCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fish = State.GetSwimmingFish(int.Parse(inputs[0]));

                fish.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                fish.Speed = new Vector(int.Parse(inputs[3]), int.Parse(inputs[4]));
                fish.Location = new RectangleRange(fish.Position, fish.Position);

                State.VisibleFishes.Add(fish.Id);
            }

            // Radar blips
            var lostedFishes = State.Fishes.Select(fish => fish.Id).ToHashSet();

            var radarBlipCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < radarBlipCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var drone = State.GetDrone(0, int.Parse(inputs[0]));
                var radarBlip = new RadarBlip(int.Parse(inputs[1]), (BlipType)Enum.Parse(typeof(BlipType), inputs[2]));
                drone.RadarBlips.Add(radarBlip);
                lostedFishes.Remove(radarBlip.FishId);
            }

            // Set losted
            foreach (var fishId in lostedFishes)
                State.FishLosted(State.GetFish(fishId));

#if TEST_MODE
            if (State.Turn > GameProperties.SERIAL_FROM_TURN)
                Console.Error.WriteLine(JsonSerializer.Serialize<GameStateBase>(State));
#endif            
            StopWatch.Stop();

            return State;
        }
        #endregion

        public IGameAction GetAction(IGameState gameState)
        {
            StopWatch.Start();

            // Update fish position
            var referee = new GameReferee(gameState as GameState);
            referee.UpdatePositions(fish => !referee.State.VisibleFishes.Contains(fish.Id));

            // Correct fish positions
            CorrectFishPositions(referee.State);

            // Update speed
            referee.UpdateSpeeds(fish => !referee.State.VisibleFishes.Contains(fish.Id));

            // Simulation
            Simultation.FindBestAction(referee.State);
            
            // Write working time
            StopWatch.Stop();
            Console.Error.WriteLine(string.Format("Time: {0} ms"), StopWatch.ElapsedMilliseconds);

            // Return action
            return Simultation.GetStateDetails(Simultation.Referee.State).BestVariant.Action;
        }

        private void FindStartPositions(GameState state)
        {
            foreach (var fish in state.SwimmingFishes.Where(_ => _.Id % 2 == 0)) // Exclude symmetric fish
            {
                // Get symmetric fish
                var sFish = state.GetSwimmingFish(fish.Id + 1);

                // Get start location
                if (fish.Type == FishType.ANGLER)
                    fish.Location = new RectangleRange(0, GameProperties.MAP_SIZE / 2, GameProperties.MAP_SIZE - 1, GameProperties.MAP_SIZE - 1);
                else
                    fish.Location = new RectangleRange(1000, GameProperties.HABITAT[fish.Type][0] + 1000, GameProperties.MAP_SIZE - 1001, GameProperties.HABITAT[fish.Type][1] - 1000);
                sFish.Location = fish.Location;

                // Correct location by radar
                foreach (var drone in state.MyDrones)
                {
                    fish.Location = fish.Location.Intersect(drone.RadarBlips.First(radarBlip => radarBlip.FishId == fish.Id).GetRange(drone.Position));
                    sFish.Location = sFish.Location.Intersect(drone.RadarBlips.First(radarBlip => radarBlip.FishId == sFish.Id).GetRange(drone.Position));
                }

                // Correct by symmetric
                var range = fish.Location.Intersect(sFish.Location.HSymmetric(GameProperties.CENTER.X));
                var sRange = sFish.Location.Intersect(fish.Location.HSymmetric(GameProperties.CENTER.X));

                // If undefined
                if (range.Equals(sRange))
                    if (fish.Type == FishType.ANGLER)
                    {
                        range = new RectangleRange(range.From, new Vector((range.From.X + range.To.X - 1) / 2, range.To.Y));
                        sRange = new RectangleRange(new Vector(range.To.X + 1, range.To.Y), sRange.To);
                    }
                    else
                    {
                        range = new RectangleRange(range.From, range.From);
                        sRange = new RectangleRange(sRange.To, sRange.To);
                    }

                // Set location
                fish.Location = range;
                sFish.Location = sRange;


                // Set position
                fish.Position = fish.Location.Center;
                sFish.Position = sFish.Location.Center;
            }
        }

        private void CorrectFishPositions(GameState state)
        {
            var referee = new GameReferee(state);

            // Symmetric Fish
            foreach (var fish in state.VisibleFishes.Select(_ => state.GetSwimmingFish(_)))
            {
                var sFish = state.GetSymmetricFish(fish);
                if (sFish != null && sFish.Speed == null)
                {
                    sFish.Location = fish.Location.HSymmetric(GameProperties.CENTER.X);
                    sFish.Position = fish.Position.HSymmetric(GameProperties.CENTER.X);
                    // Check if we have fluence
                    if (State.Drones.Where(drone => !drone.Emergency && fish.Position.InRange(drone.Position,
                                                        fish.Type == FishType.ANGLER ? drone.LightRadius : GameProperties.MOTOR_RANGE))
                                    .Select(drone => drone.Position).Any())
                    {
                        sFish.Speed = new Vector();
                        sFish.Speed = referee.GetFishSpeed(sFish);
                        if (sFish.Speed.IsZero()) sFish = null;
                    }
                    else sFish.Speed = fish.Speed.HSymmetric();
                }
            }

            // From Enemy Scans
            foreach (var drone in state.EnemyDrones)
                foreach (var fish in drone.NewScans .Where(_ => !state.VisibleFishes.Contains(_))
                                                    .Select(fishId => state.GetFish(fishId)))
                {
                    // Undefined fish or position not for scan
                    if (fish.Speed == null || !fish.Position.InRange(drone.Position, drone.LightRadius))
                    {
                        fish.Location = fish.Location.Intersect(drone.Position, drone.LightRadius);
                        fish.Position = fish.Location.Center;
                        fish.Speed = null;

                        // Undefined symmetric fish
                        var sFish = state.GetSymmetricFish(fish);
                        if (sFish != null && sFish.Speed == null)
                        {
                            sFish.Location = fish.Location.HSymmetric(GameProperties.CENTER.X);
                            sFish.Position = fish.Position.HSymmetric(GameProperties.CENTER.X);
                        }
                    }
                }

            // Correct position from radar
            foreach (var fish in state.SwimmingFishes.Where(_ => !state.VisibleFishes.Contains(_.Id)))
            {
                var habitat = GameProperties.HABITAT[fish.Type];
                var range = new RectangleRange(0, habitat[0], GameProperties.MAP_SIZE - 1, habitat[1]);

                foreach (var drone in state.MyDrones)
                    range = range.Intersect(drone.RadarBlips.First(_ => _.FishId == fish.Id).GetRange(drone.Position));

                // Decrease location
                fish.Location = fish.Location.Scale(fish.Type == FishType.ANGLER ? GameProperties.MONSTER_SPEED : GameProperties.FISH_SPEED).Intersect(range);

                // If fish position in wrong range
                if (!fish.Location.InRange(fish.Position))
                {   
                    fish.Position = fish.Location.Intersect(fish.Position, fish.Location.Center);
                    fish.Speed = null;
                }

                // If my light, but i dont see it
                var drones = State.MyDrones
                    .Where(drone => !drone.Emergency && fish.Position.InRange(drone.Position, drone.LightRadius))
                    .ToList();
                if (drones.Any())
                {
                    Vector direction = null;

                    // If both drones didnt see fish
                    if (drones.Count > 1 && !drones[1].Position.Equals(drones[0].Position))
                    {
                        var offsetPosition = drones[1].Position - drones[0].Position;
                        var offsetLengthSqr = offsetPosition.LengthSqr();
                        var radiusSqr0 = drones[0].LightRadius * drones[0].LightRadius / offsetLengthSqr;
                        var radiusSqr1 = drones[1].LightRadius * drones[1].LightRadius / offsetLengthSqr;

                        var project = (radiusSqr0 - radiusSqr1) / 2.0 + 0.5;
                        var cross = Math.Sqrt(radiusSqr0 - project * project) + GameProperties.DELTA_RADIUS;

                        fish.Position = offsetPosition * project;
                        direction = cross * offsetPosition.Cross();
                        if (!fish.Location.InRange(direction + fish.Position)) direction = cross * offsetPosition.Cross(-1);
                    }
                    else
                    {
                        fish.Position = drones[0].Position;
                        direction = (fish.Location.Center - drones[0].Position).Normalize() * (drones[0].LightRadius + GameProperties.DELTA_RADIUS);
                    }

                    fish.Position = (fish.Position + direction).Round();
                    if (!fish.Location.InRange(fish.Position))
                        fish.Position = fish.Location.Intersect(fish.Position, fish.Location.Center);
                    fish.Speed = null;
                }
            }
        }
    }
}

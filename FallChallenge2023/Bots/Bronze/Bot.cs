using DevLib.Game;
using DevLib.GameMath;
using FallChallenge2023.Bots.Bronze.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FallChallenge2023.Bots.Bronze
{
    public class Bot : IGameBot
    {
        public Stopwatch StopWatch => new Stopwatch();

        private GameState State { get; set; }

        #region Read from console
        public void ReadInitialize()
        {
            State = new GameState();

            var creatureCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < creatureCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fishId = int.Parse(inputs[0]);

                State.Fishes.Add(fishId, new Fish(fishId, (FishColor)int.Parse(inputs[1]), (FishType)int.Parse(inputs[2])));
            }
        }

        public IGameState ReadState()
        {
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

            for (int k = 0; k < 2; k++)
            {
                var droneCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < droneCount; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');
                    var droneId = int.Parse(inputs[0]);

                    if (!State.Drones.TryGetValue(droneId, out var drone))
                        State.Drones.Add(droneId, drone = new Drone(droneId, k));

                    var battery = drone.Battery;

                    drone.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                    drone.Emergency = int.Parse(inputs[3]) == 1;                    
                    drone.Battery = int.Parse(inputs[4]);
                    drone.Lighting = (battery - drone.Battery) == Drone.BATTERY_DRAIN;
                    drone.LastScanCount = drone.Scans.Count;

                    drone.Scans = new List<int>();
                    drone.RadarBlips = new List<RadarBlip>();
    }
            }

            var droneScanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < droneScanCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                State.Drones[int.Parse(inputs[0])].Scans.Add(int.Parse(inputs[1]));
            }

            foreach (var fish in State.Fishes.Values)
                fish.Lost = true;

            var fishesCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < fishesCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fish = State.Fishes[int.Parse(inputs[0])];

                fish.Position = new Vector(int.Parse(inputs[1]), int.Parse(inputs[2]));
                fish.Speed = new Vector(int.Parse(inputs[3]), int.Parse(inputs[4]));
            }

            var radarBlipCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < radarBlipCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fishId = int.Parse(inputs[1]);
                State.Fishes[fishId].Lost = false;
                State.Drones[int.Parse(inputs[0])].RadarBlips.Add(
                    new RadarBlip(fishId, (RadarType)Enum.Parse(typeof(RadarType), inputs[2])
                ));
            }

#if TEST_MODE
            Console.Error.WriteLine(JsonSerializer.Serialize(State));
#endif

            return State;
        }
#endregion

        public IGameAction GetAction(IGameState gameState)
        {
            var actions = new GameActionList();
            actions.Actions.Add(new GameActionWait());
            actions.Actions.Add(new GameActionWait());
            return actions;
        }
    }
}

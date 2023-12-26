using DevLib.Game;
using DevLib.GameMath;
using FallChallenge2023.Bots.Wood.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FallChallenge2023.Bots.Wood
{
    public class Bot : IGameBot
    {
        public Stopwatch StopWatch => new Stopwatch();

        private Dictionary<int, FishPropery> FishProperies = new Dictionary<int, FishPropery>();

        #region Read from console
        public void ReadInitialize()
        {
            var creatureCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < creatureCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                FishProperies[int.Parse(inputs[0])] = new FishPropery((FishColor)int.Parse(inputs[1]), (FishType)int.Parse(inputs[2]));
            }
        }

        public IGameState ReadState()
        {
            var state = new GameState();

            for (int k = 0; k < 2; k++)
                state.Score[k] = int.Parse(Console.ReadLine());

            for (int k = 0; k < 2; k++)
            {
                var scanCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < scanCount; i++)
                    state.Scans[k].Add(int.Parse(Console.ReadLine()));
            }

            for (int k = 0; k < 2; k++)
            {
                var droneCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < droneCount; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');
                    var droneId = int.Parse(inputs[0]);
                    state.Drones[droneId] = new Drone(
                        droneId, k,                 // Id, PlayerId
                        int.Parse(inputs[1]),       // X
                        int.Parse(inputs[2]),       // Y
                        int.Parse(inputs[3]) == 1,  // Emergency
                        int.Parse(inputs[4])        // Battery
                    );
                }
            }

            var droneScanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < droneScanCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                state.Drones[int.Parse(inputs[0])].Scans.Add(int.Parse(inputs[1]));
            }

            var fishesCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < fishesCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fishId = int.Parse(inputs[0]);
                var property = FishProperies[fishId];
                state.Fishes[fishId] = new Fish(
                    fishId,                 // Id
                    property.Color,         // Color
                    property.Type,          // Type
                    int.Parse(inputs[1]),   // X
                    int.Parse(inputs[2]),   // Y
                    int.Parse(inputs[3]),   // Vx
                    int.Parse(inputs[4])    // Vy
                );
            }

            var radarBlipCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < radarBlipCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                state.Drones[int.Parse(inputs[0])].RadarBlips.Add(
                    new RadarBlip(int.Parse(inputs[1]), (RadarType)Enum.Parse(typeof(RadarType), inputs[2])
                ));
            }

            return state;
        }
        #endregion

        private int turn = 0;
        public IGameAction GetAction(IGameState gameState)
        {
            GameState state = gameState as GameState;

            turn++;
            var light = turn % 2 == 0;

            var scaned = state.Scans[0].Union(state.Scans[1]);

            var drone = state.Drones.Values.First(_ => _.PlayerId == 0);
            var foeDrone = state.Drones.Values.First(_ => _.PlayerId == 1);
            var position = new VectorD(drone.X, drone.Y);
            var foePosition = new VectorD(foeDrone.X, foeDrone.Y);

            if (foePosition.Y - position.Y < 601 && position.Y <= foePosition.Y)
                foreach (var fishId in drone.Scans)
                    if (!scaned.Any(_ => _ == fishId) && foeDrone.Scans.Any(_ => _ == fishId))
                    {
                        Console.Error.WriteLine(string.Format("{0} UP", fishId));
                        return new GameActionMove(new VectorD(position.X, 500), false);
                    }

            var fishes = state.Fishes.Values.ToList();
            fishes.Sort((a, b) => a.Position.Distance(position).CompareTo(b.Position.Distance(position)));

            for (int t = 0; t < 2; t++)
            {
                foreach (var fish in state.Fishes.Values)
                    if (((t == 1 && !state.Scans[0].Any(_ => _ == fish.Id)) || !scaned.Any(_ => _ == fish.Id)) && !drone.Scans.Any(_ => _ == fish.Id))
                    {
                        Console.Error.WriteLine(string.Format("{0} Find", fish.Id));
                        return new GameActionMove(fish.Position, light);
                    }

                foreach (var radar in drone.RadarBlips)
                    if (((t == 1 && !state.Scans[0].Any(_ => _ == radar.Id)) || !scaned.Any(_ => _ == radar.Id)) && !drone.Scans.Any(_ => _ == radar.Id))
                    {
                        Console.Error.WriteLine(string.Format("{0} {1} {2}", position.ToIntString(), radar.Type, radar.Id));
                        switch (radar.Type)
                        {
                            case RadarType.TL:
                                return new GameActionMove(position + new VectorD(-1000, -1000), light);
                            case RadarType.TR:
                                return new GameActionMove(position + new VectorD(1000, -1000), light);
                            case RadarType.BL:
                                return new GameActionMove(position + new VectorD(-1000, 1000), light);
                            case RadarType.BR:
                                return new GameActionMove(position + new VectorD(1000, 1000), light);
                        }
                    }
                if (drone.Scans.Count > 0) break;
            }

            Console.Error.WriteLine(string.Format("NONE UP"));
            return new GameActionMove(new VectorD(position.X, 500), false);
        }
    }
}

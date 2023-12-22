using DevLib.Game;
using FallChallenge2023.Bots.Bronze.Actions;
using System;
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
                State.Fishes.Add(new Fish(int.Parse(inputs[0]), (FishColor)int.Parse(inputs[1]), (FishType)int.Parse(inputs[2])));
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
                    var drone = State.GetDrone(droneId) ?? State.GetNewDrone(droneId, k);

                    var battery = drone.Battery;
                    var scansCount = drone.Scans.Count;

                    drone.X = int.Parse(inputs[1]);
                    drone.Y = int.Parse(inputs[2]);
                    drone.Emergency = int.Parse(inputs[3]) == 1;                    
                    drone.Battery = int.Parse(inputs[4]);
                    drone.Lighting = drone.Battery < battery;
                    drone.Scanning = drone.Scans.Count > scansCount;

                    drone.Scans.Clear();
                    drone.RadarBlips.Clear();
    }
            }

            var droneScanCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < droneScanCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                State.GetDrone(int.Parse(inputs[0])).Scans.Add(int.Parse(inputs[1]));
            }

            State.Fishes.ForEach(_ => _.Status = FishStatus.LOSTED);

            var fishesCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < fishesCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var fish = State.GetFish(int.Parse(inputs[0]));

                fish.X = int.Parse(inputs[1]);
                fish.Y = int.Parse(inputs[2]);
                fish.Vx = int.Parse(inputs[3]);
                fish.Vy = int.Parse(inputs[4]);
            }

            var radarBlipCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < radarBlipCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var radarBlip = new RadarBlip(int.Parse(inputs[1]), (BlipType)Enum.Parse(typeof(BlipType), inputs[2]));
                State.GetDrone(int.Parse(inputs[0])).RadarBlips.Add(radarBlip);
                var fish = State.GetFish(radarBlip.FishId);
                fish.Status = (fish.X == 0 && fish.Y == 0) ? FishStatus.UNKNOWED : FishStatus.SWIMMING;
            }

#if TEST_MODE
            Console.Error.WriteLine(JsonSerializer.Serialize<GameStateBase>(State));
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

using DebugUtils;
using DebugUtils.Objects;
using DebugUtils.Objects.Maps;
using DevLib.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class GameManager : DebugManager
    {
        public GameManager(DebugState state) : base(state)
        {
        }

        public static void Generate()
        {
            using (var openFileDialog = new OpenFileDialog() { RestoreDirectory = true })
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DebugState rootState = null;
                    DebugState parentState = null;

                    using (StreamReader reader = new StreamReader(openFileDialog.OpenFile()))
                    {
                        var line = reader.ReadLine();
                        while (line != null)
                        {
                            if (line[0] == '{')
                            {
                                var state = JsonConvert.DeserializeObject<GameState>(line);
                                var debugState = new DebugState(string.Format("Turn {0}", state.Turn), state, parentState);

                                if (parentState == null) rootState = debugState;
                                else parentState.NextMoves.Add(debugState);

                                parentState = debugState;
                            }

                            line = reader.ReadLine();
                        }
                    }

                    var manager = new GameManager(rootState);
                    manager.Show();
                }

            Environment.Exit(0);
        }

        public override DebugObject GetObjectFromState(IGameState state)
        {
            var gameState = state as GameState;

            var rootObj = new DebugOcean();

            // Score
            for (int k = 0; k < 2; k++)
                rootObj.Childs.Add(new DebugTextObject(k == 0 ? "My Score" : "Enemy Score", new Rectangle(k == 0 ? 20 : 1590, 160, 300, 40),
                    string.Format("{0} Score: {1}", k == 0 ? "My" : "Enemy", gameState.GetScore(k)), rootObj));

            // Ocean
            var oceanFloor = new DebugOceanFloor(rootObj);
            rootObj.Childs.Add(oceanFloor);

            // Fishes
            var fishes = new List<DebugFish>();
            foreach (var fish in gameState.Fishes.Values.Where(_ => _.Position != null))
                fishes.Add(new DebugFish(fish.ToString(), fish, oceanFloor));
            foreach (var fish in fishes)
                oceanFloor.Childs.Add(fish);

            // Drones
            var drones = new List<DebugDrone>();
            foreach (var drone in gameState.Drones.Values)
                drones.Add(new DebugDrone(drone.ToString(), drone, oceanFloor));
            foreach (var drone in drones)
                oceanFloor.Childs.Add(drone);

            // Scans
            var scans = new DebugMap[] {
                new DebugMap("My Scans", rootObj) { Visible = true, Position = new Rectangle(5, 425, 335, 405) },
                new DebugMap("Enemy Scans", rootObj) { Visible = true, Position = new Rectangle(1575, 425, 335, 405) }
            };

            // Saved scans
            for (int k = 0; k < 2; k++)
            {
                rootObj.Childs.Add(scans[k]);

                foreach (var fishId in gameState.GetScans(k))
                {
                    var fish = gameState.Fishes[fishId];
                    scans[k].Childs.Add(new DebugScanFish(string.Format("SAVED {0}", fish), k, fish, true, false, scans[k]));
                }
            }

            // Dron scans
            foreach (var drone in drones)
                foreach (var fishId in drone.Drone.Scans.Where(_ => !gameState.GetScans(drone.Drone.PlayerId).Any(__ => __ == _)))
                {
                    var fish = gameState.Fishes[fishId];
                    scans[drone.Drone.PlayerId].Childs.Add(new DebugScanFish(fish.ToString(), drone.Drone.PlayerId, fish, false, false, scans[drone.Drone.PlayerId]));
                }

            // Losted fish
            for (int k = 0; k < 2; k++)
                foreach (var fish in gameState.Fishes.Values.Where(_ => _.Type != FishType.ANGLER && _.Lost))
                    if (!scans[k].Childs.Any(_ => (_ as DebugScanFish).Fish.Id == fish.Id))
                        scans[k].Childs.Add(new DebugScanFish(string.Format("LOST {0}", fish), k, fish, true, true, scans[k]));

            return rootObj;
        }
    }
}

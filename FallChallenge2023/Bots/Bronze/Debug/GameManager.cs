using DebugUtils;
using DebugUtils.Buttons;
using DebugUtils.Objects;
using DebugUtils.Objects.Maps;
using DevLib.Game;
using Newtonsoft.Json;
using System;
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
            Bots.Add(new DebugBot(this));
            Buttons.Add(new DebugButtonGetAction(this));
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
                            if (line.Length > 0 && line[0] == '{')
                                try
                            {
                                var state = JsonConvert.DeserializeObject<GameState>(line);
                                var debugState = new DebugState(string.Format("Turn {0}", state.Turn), state, parentState);

                                if (parentState == null) rootState = debugState;
                                else parentState.NextMoves.Add(debugState);

                                parentState = debugState;
                            } catch (JsonReaderException) { break; }

                            line = reader.ReadLine();
                        }
                    }

                    var manager = new GameManager(rootState);
                    manager.Show();
                }

            Environment.Exit(0);
        }

        public override DebugObject GetObjectFromState(IGameState state, DebugObject parent = null)
        {
            var gameState = state as GameState;

            var rootObj = new DebugOcean(parent);

            // Score
            for (int k = 0; k < 2; k++)
                rootObj.Childs.Add(new DebugTextObject(k == 0 ? "My Score" : "Enemy Score", new Rectangle(k == 0 ? 20 : 1590, 160, 300, 40),
                    string.Format("{0} Score: {1}", k == 0 ? "My" : "Enemy", gameState.GetScore(k)), rootObj));

            // Ocean
            var oceanFloor = new DebugOceanFloor(rootObj);
            rootObj.Childs.Add(oceanFloor);

            // Fishes
            foreach (var fish in gameState.Fishes.Where(_ => _.Position != null))
                oceanFloor.Childs.Add(new DebugFish(fish, oceanFloor));

            // Drones
            foreach (var drone in gameState.Drones)
                oceanFloor.Childs.Add(new DebugDrone(drone, oceanFloor));

            // Scans
            var scans = new DebugMap[] {
                new DebugMap("My Scans", rootObj) { Position = new Rectangle(5, 425, 335, 405), Visible = true },
                new DebugMap("Enemy Scans", rootObj) { Position = new Rectangle(1575, 425, 335, 405), Visible = true }
            };

            // Saved scans
            for (int k = 0; k < 2; k++)
            {
                rootObj.Childs.Add(scans[k]);
                foreach (var fishId in gameState.GetScans(k))
                    scans[k].Childs.Add(new DebugScanFish(k, gameState.GetFish(fishId), true, false, scans[k]));
            }

            // Dron scans
            foreach (var drone in gameState.Drones)
                foreach (var fishId in drone.Scans.Where(_ => !gameState.GetScans(drone.PlayerId).Any(__ => __ == _)))
                    scans[drone.PlayerId].Childs.Add(new DebugScanFish(drone.PlayerId, gameState.GetFish(fishId), false, false, scans[drone.PlayerId]));

            // Losted fish
            for (int k = 0; k < 2; k++)
                foreach (var fish in gameState.Fishes.Where(_ => _.Status == FishStatus.LOSTED))
                    if (!scans[k].Childs.Any(_ => (_ as DebugScanFish).Fish.Id == fish.Id))
                        scans[k].Childs.Add(new DebugScanFish(k, fish, true, true, scans[k]));

            return rootObj;
        }
    }
}

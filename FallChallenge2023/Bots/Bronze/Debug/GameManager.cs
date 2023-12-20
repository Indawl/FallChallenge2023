using DebugUtils;
using DebugUtils.Objects;
using DevLib.Game;
using Newtonsoft.Json;
using System;
using System.IO;
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
            return base.GetObjectFromState(state);
        }
    }
}

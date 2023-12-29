using DebugUtils.Objects;
using DebugUtils.Controls.Properties;
using DevLib.Game;
using System.Collections.Generic;
using FallChallenge2023.Bots.Bronze.Agents;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugSimulationState : DebugState, IPropertiesDetailsAction
    {
        public List<DroneAgent> Agents { get; set; }

        public DebugSimulationState(string name, IGameState state, DebugState parent = null) : base(name, state, parent) { }

        public void GetDetails()
        {
            (new GameManager(this)).Show();
        }

        public override string ToString() => string.Join("; ", Agents);
    }
}

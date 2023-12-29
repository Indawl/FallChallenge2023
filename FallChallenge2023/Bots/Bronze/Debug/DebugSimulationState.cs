using DebugUtils.Objects;
using DebugUtils.Controls.Properties;
using DevLib.Game;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugSimulationState : DebugState, IPropertiesDetailsAction
    {
        public DebugSimulationState(string name, IGameState state, DebugState parent = null) : base(name, state, parent) { }

        public void GetDetails()
        {
            (new GameManager(this)).Show();
        }
    }
}

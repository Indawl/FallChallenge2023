using DebugUtils;
using DebugUtils.Objects;
using DevLib.Game;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugBot : Bot, IDebugBot
    {
        public GameManager Manager { get; }

        public DebugBot(GameManager manager)
        {
            Manager = manager;
        }

        public void GetDebugAction(IGameState state, ref DebugObject debugObject)
        {
            var gameState = (GameState)(state as GameState).Clone();

            GetAction(gameState);

            // Init
            debugObject = Manager.GetObjectFromState(gameState);

            // Ocean
            var oceanFloor = debugObject.Childs.First(_ => _.Name == "Ocean Floor");

            // Actions
            foreach (var agent in Agents)
            {
                var debugDrone = oceanFloor.Childs.Where(_ => _ is DebugDrone).First(_ => (_ as DebugDrone).Drone.Id == agent.Drone.Id);
                debugDrone.Childs.Add(new DebugAction(agent.Action, debugDrone));
            }
        }
    }
}

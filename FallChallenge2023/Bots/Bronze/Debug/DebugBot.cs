using DebugUtils;
using DebugUtils.Objects;
using DevLib.Game;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugBot : Bot, IDebugBot
    {
        public GameManager Manager { get; }
        private DebugObject SimulationObject { get; set; }
        private int SimulationCount { get; set; }

        public DebugBot(GameManager manager)
        {
            Manager = manager;
        }

        public void GetDebugAction(IGameState state, ref DebugObject debugObject)
        {
            var gameState = (GameState)(state as GameState).Clone();

            // Init
            Agents.Clear();

            // Simulation init
            SimulationCount = 0;
            SimulationObject = new DebugObject("Simulations", debugObject);

            // Get actions
            GetAction(gameState);

            // State
            debugObject = Manager.GetObjectFromState(gameState);

            // Ocean
            var oceanFloor = debugObject.Childs.First(_ => _.Name == "Ocean Floor");

            // Actions
            foreach (var agent in Agents)
            {
                var debugDrone = oceanFloor.Childs.Where(_ => _ is DebugDrone).First(_ => (_ as DebugDrone).Drone.Id == agent.Drone.Id);
                debugDrone.Childs.Add(new DebugAction(agent.Action, debugDrone));
            }

            // Simulation
            debugObject.Childs.Add(SimulationObject);
        }

        protected override int GetSimulationScore(GameState state, List<Agents.DroneAgent> agents)
        {
            var bot = new DebugSimulationBot();
            var score = bot.DebugSimulation(out var obj, state, agents);
            SimulationObject.Properties.Add((++SimulationCount).ToString(), obj);
            return score;
        }
    }
}

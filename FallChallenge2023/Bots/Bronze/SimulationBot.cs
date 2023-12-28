using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class SimulationBot : Bot
    {
        public SimulationBot(List<DroneAgent> agents)
        {
            foreach (var agent in agents)
                Agents.Add(agent);
        }

        public void UpdateDronePosition(GameState state)
        {
            // Initialize agents
            DistributeAgents(state, 0);
            DistributeAgents(state, 1);

            // Determinate actions for agents
            foreach (var agent in Agents)
                agent.FindAction();

            // Accept actions
            foreach (var agent in Agents.Where(_ => !_.Drone.Emergency))
            {
                var oldPosition = agent.Drone.Position;

                // Do action
                if (agent.Action is GameActionMove)
                {
                    var action = agent.Action as GameActionMove;
                    agent.Drone.Position = action.Position;
                    agent.Drone.Lighting = action.Light;
                }
                else if (agent.Action is GameActionWait)
                {
                    var action = agent.Action as GameActionWait;
                    agent.Drone.Position += new Vector(0, GameProperties.DRONE_SINK_SPEED);
                    agent.Drone.Lighting = action.Light;
                }

                // Set speed
                agent.Drone.Speed = agent.Drone.Position - oldPosition;
            }
        }
    }
}

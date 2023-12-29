using FallChallenge2023.Bots.Bronze.Agents;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class SimulationBot : Bot
    {
        public int Simulation(GameState state, List<DroneAgent> agents)
        {
            Agents = agents;

            var referee = new GameReferee((GameState)state.Clone());
            
            while (!referee.IsGameOver())
            {
                FindActions(referee.State);                
                referee.SetNextState(Agents);                
            }
            referee.Finish();

            return referee.State.Score;
        }

        protected void FindActions(GameState state)
        {
            // Initialize agents
            CreateAgents(state);
            DistributeAgents(state, 0);
            DistributeAgents(state, 1);

            // Determinate actions for agents
            foreach (var agent in Agents)
                agent.FindAction();
        }
    }
}

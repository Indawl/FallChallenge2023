using DebugUtils.Objects;
using FallChallenge2023.Bots.Bronze.Agents;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugSimulationBot : SimulationBot
    {
        public int DebugSimulation(out DebugSimulationState debugState, GameState state, List<DroneAgent> agents)
        {
            debugState = new DebugSimulationState("Turn " + state.Turn, state) { Agents = agents };
            var stepState = debugState as DebugState;

            Agents = agents;

            var referee = new GameReferee((GameState)state.Clone());

            while (!referee.IsGameOver())
            {                
                FindActions(referee.State);
                referee.SetNextState(Agents);

                var newDebugState = new DebugState("Turn " + referee.State.Turn, (GameState)referee.State.Clone(), debugState);
                stepState.NextMoves.Add(newDebugState);
                stepState = newDebugState;
            }
            referee.Finish();

            stepState.NextMoves.Add(new DebugState("Finish", referee.State, debugState));

            return referee.State.Score;
        }
    }
}

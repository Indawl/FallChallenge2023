using FallChallenge2023.Bots.Bronze.Actions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public SaveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override GameAction GetDecision() => new GameActionMove(Agent.Drone.X, 0);
    }
}

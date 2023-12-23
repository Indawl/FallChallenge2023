using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public SaveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override GameAction GetDecision() => new GameActionMove(new Vector(Agent.Drone.Position.X, 0));
    }
}

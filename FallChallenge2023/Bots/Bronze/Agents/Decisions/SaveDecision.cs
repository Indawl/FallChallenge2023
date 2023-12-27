using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public SaveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override GameAction GetDecision()
        {
            var newPosition = State.GetAroundMonster(Agent.Drone.Position, new Vector(0, -Drone.MAX_SPEED));
            return new GameActionMove(newPosition, State.IsFishInRange(Agent.Drone.PlayerId, newPosition));
        }
    }
}

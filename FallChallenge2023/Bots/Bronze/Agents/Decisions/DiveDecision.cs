using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class DiveDecision : Decision
    {
        public DiveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
            Conditions.Add(new DiveCondition(agent, state));
        }

        public override GameAction GetDecision()
        {
            var newPosition = Agent.Drone.Position + new Vector(0, Drone.MAX_SPEED);
            var light = (new FishDetectedCondition(Agent, State, newPosition)).Check();
            return new GameActionMove(newPosition, light);
        }
    }
}

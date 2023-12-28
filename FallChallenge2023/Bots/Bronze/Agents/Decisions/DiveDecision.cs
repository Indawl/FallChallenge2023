using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class DiveDecision : Decision
    {
        public DiveDecision(DroneAgent agent) : base(agent)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new DiveCondition(Agent));
        }

        public override GameAction GetDecision()
        {
            var newPosition = Agent.Drone.Position + new Vector(0, GameProperties.DRONE_MAX_SPEED);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Gurgle Gurgle" };
        }
    }
}

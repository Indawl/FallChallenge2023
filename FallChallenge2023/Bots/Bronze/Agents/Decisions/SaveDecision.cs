using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public SaveDecision(DroneAgent agent) : base(agent) { }

        public override GameAction GetDecision()
        {
            var newPosition = Agent.State.GetAroundMonster(Agent.Drone.Position, new Vector(0, -GameProperties.DRONE_MAX_SPEED), Agent.Drone);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Well Done" };
        }
    }
}

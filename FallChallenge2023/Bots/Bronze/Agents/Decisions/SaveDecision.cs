using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public override DecisionType Type => DecisionType.SaveDecision;
        public SaveDecision(DroneAgent agent) : base(agent) { }

        public override GameAction GetAction()
        {
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, new Vector(Agent.Drone.Position.X, GameProperties.SURFACE), Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Well done" };
        }
    }
}

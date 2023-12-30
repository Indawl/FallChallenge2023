using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public override int Id => GameProperties.SaveDecision;
        public SaveDecision(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.Drone.Scans.Any();

        public override bool CheckGoal(DroneAgentGoal goal) => !Agent.Drone.Scans.Any();

        public override GameAction GetAction()
        {
            Agent.Goal = new DroneAgentGoal(Id);
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, new Vector(Agent.Drone.Position.X, GameProperties.SURFACE), Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Do it" };
        }
    }
}

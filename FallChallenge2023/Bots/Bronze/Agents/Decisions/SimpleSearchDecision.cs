using FallChallenge2023.Bots.Bronze.Actions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SimpleSearchDecision : Decision
    {
        public SimpleSearchDecision(DroneAgent agent) : base(agent) { }

        public override GameAction GetDecision()
        {
            var fish = Agent.UnscannedFishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, fish.Position, Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Searching..." };
        }
    }
}

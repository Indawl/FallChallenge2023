using FallChallenge2023.Bots.Bronze.Actions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public override DecisionType Type => DecisionType.SearchDecision;

        public SearchDecision(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.UnscannedFishes.Any();

        public override GameAction GetAction()
        {
            var fish = Agent.UnscannedFishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, fish.Position, Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Searching..." };
        }
    }
}

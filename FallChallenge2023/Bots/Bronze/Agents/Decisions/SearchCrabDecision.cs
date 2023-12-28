using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchCrabDecision : Decision
    {
        public SearchCrabDecision(DroneAgent agent) : base(agent) { }

        public override void SetConditions()
        {
            Conditions.Add(new SearchCrabCondition(Agent));
        }

        public override GameAction GetDecision()
        {
            var fish = Agent.UnscannedFishes.Where(_ => _.Type == FishType.CRAB).OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = Agent.State.GetAroundMonsterTo(Agent.Drone.Position, fish.Position, Agent.Drone);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Hunting..." };
        }
    }
}

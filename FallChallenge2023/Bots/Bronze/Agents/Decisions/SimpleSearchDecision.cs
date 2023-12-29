using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SimpleSearchDecision : Decision
    {
        public SimpleSearchDecision(DroneAgent agent) : base(agent) { }

        public override void SetConditions()
        {
            Conditions.Add(new SearchCondition(Agent));
        }

        public override GameAction GetDecision()
        {
            var fish = Agent.UnscannedFishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, fish.Position, Agent.Drone.Id, GameProperties.MONSTER_TRAVERSAL_ANLE_FAST);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition));
        }
    }
}

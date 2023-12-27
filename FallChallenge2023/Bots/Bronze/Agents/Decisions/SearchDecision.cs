using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public SearchDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new SearchCondition(Agent, State));
        }

        public override GameAction GetDecision()
        {
            var fish = Agent.UnscannedFishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = State.GetAroundMonsterTo(Agent.Drone.Position, fish.Position, Agent.Drone);
            var light = State.IsFishInRange(Agent.Drone.PlayerId, newPosition);
            return new GameActionMove(newPosition, light);
        }
    }
}

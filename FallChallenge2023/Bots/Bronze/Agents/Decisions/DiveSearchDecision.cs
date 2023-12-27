using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class DiveSearchDecision : Decision
    {
        public DiveSearchDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new DiveSearchCondition(Agent, State));
        }

        public override GameAction GetDecision()
        {
            var fish = Agent.UnscannedFishes.Where(_ => _.Type == FishType.CRAB).OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = State.GetAroundMonsterTo(Agent.Drone.Position, fish.Position);
            return new GameActionMove(newPosition, State.IsFishInRange(Agent.Drone.PlayerId, newPosition));
        }
    }
}

using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class DiveSearchDecision : Decision
    {
        public DiveSearchDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
            Conditions.Add(new DiveSearchCondition(agent, state));
        }

        public override GameAction GetDecision()
        {
            var fish = Agent.Fishes.Where(_ => _.Type == FishType.CRAB).OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();
            var newPosition = State.GetAroundMonsterTo(Agent.Drone.Position, fish.Position);
            var light = (new FishDetectedCondition(Agent, State, newPosition)).Check();
            return new GameActionMove(newPosition, light);
        }
    }
}

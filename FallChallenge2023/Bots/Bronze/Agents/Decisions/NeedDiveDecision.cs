using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class NeedDiveDecision : Decision
    {
        public NeedDiveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new DiveSearchCondition(Agent, State));
        }

        public override void SetDecisionts()
        {
            DecisionsOk.Add(new DiveDecision(Agent, State));        // Dive
            DecisionsOk.Add(new DiveSearchDecision(Agent, State));  // Search for dive scan

            DecisionsFail.Add(new SearchDecision(Agent, State));    // Search for scan
        }

        public override GameAction GetDecision()
        {
            var newPosition = Agent.Drone.Position + new Vector(0, Drone.MAX_SPEED);
            return new GameActionMove(newPosition, State.IsFishInRange(Agent.Drone.PlayerId, newPosition));
        }
    }
}

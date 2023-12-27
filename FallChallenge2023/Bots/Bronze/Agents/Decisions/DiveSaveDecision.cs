using FallChallenge2023.Bots.Bronze.Agents.Conditions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class DiveSaveDecision : SaveDecision
    {
        public DiveSaveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new DiveSaveCondition(Agent, State));
        }
    }
}

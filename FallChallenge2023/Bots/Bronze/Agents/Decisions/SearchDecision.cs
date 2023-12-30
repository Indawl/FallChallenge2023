using FallChallenge2023.Bots.Bronze.Agents.Conditions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public SearchDecision(DroneAgent agent) : base(agent) { }

        public override void SetConditions()
        {
            Conditions.Add(new SearchCondition(Agent));
        }

        public override void SetDecisionts()
        {
            DecisionsOk.Add(new SimpleSearchDecision(Agent));
        }
    }
}

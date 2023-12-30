using FallChallenge2023.Bots.Bronze.Agents.Conditions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class NeedDiveDecision : Decision
    {
        public NeedDiveDecision(DroneAgent agent) : base(agent) { }

        public override void SetConditions()
        {
            Conditions.Add(new NeedDiveCondition(Agent));
        }

        public override void SetDecisionts()
        {
            DecisionsOk.Add(new DiveDecision(Agent));        // Dive
            DecisionsOk.Add(new SearchCrabDecision(Agent));  // Search for dive scan

            DecisionsFail.Add(new SearchDecision(Agent));    // Search for scan
        }
    }
}

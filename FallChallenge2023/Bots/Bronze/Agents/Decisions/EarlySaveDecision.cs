using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class EarlySaveDecision : SaveDecision
    {
        public EarlySaveDecision(DroneAgent agent) : base(agent) { }

        public override void SetConditions()
        {
            Conditions.Add(new EarlySaveCondition(Agent));
        }

        public override GameAction GetDecision()
        {
            var action = base.GetDecision();
            action.Text = "Faster Faster";
            return action;
        }
    }
}

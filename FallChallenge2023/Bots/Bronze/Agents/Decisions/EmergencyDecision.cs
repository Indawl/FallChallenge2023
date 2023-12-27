using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class EmergencyDecision : Decision
    {
        public EmergencyDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new EmergencyCondition(Agent, State));
        }

        public override GameAction GetDecision() => new GameActionWait() { Text = "!!!SOS!!!" };
    }
}

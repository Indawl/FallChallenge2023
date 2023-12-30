using FallChallenge2023.Bots.Bronze.Actions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class Decision
    {
        public virtual DecisionType Type => DecisionType.Decision;
        protected DroneAgent Agent { get; }

        public Decision(DroneAgent agent)
        {
            Agent = agent;
        }
        public virtual bool Check() => true;
        public virtual bool CheckGoal(DroneAgentGoal goal) => true;
        public virtual GameAction GetAction() => null;
    }
}

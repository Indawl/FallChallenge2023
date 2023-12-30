using FallChallenge2023.Bots.Bronze.Agents.Decisions;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneAgentGoal
    {
        public DecisionType Type { get; }

        public DroneAgentGoal(DecisionType type)
        {
            Type = type;
        }
    }
}

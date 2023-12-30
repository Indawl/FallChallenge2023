using FallChallenge2023.Bots.Bronze.Actions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class EmergencyDecision : Decision
    {
        public override int Id => GameProperties.EmergencyDecision;

        public EmergencyDecision(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.Drone.Emergency;

        public override GameAction GetAction() => new GameActionWait() { Text = "SOS" };
    }
}

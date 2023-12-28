namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class EmergencyCondition : Condition
    {
        public override int Id => GameProperties.EmergencyCondition;

        public EmergencyCondition(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.Drone.Emergency;
    }
}

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class EmergencyCondition : Condition
    {
        public EmergencyCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 1;

        public override bool Check() => Agent.Drone.Emergency;
    }
}

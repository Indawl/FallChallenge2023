namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class DiveCondition : Condition
    {
        public DiveCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 2;

        public override bool Check() => State.Turn <= 1200;
    }
}

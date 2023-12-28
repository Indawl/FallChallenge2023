namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class EarlySaveCondition : Condition
    {
        public override int Id => GameProperties.EarlySaveCondition;

        public EarlySaveCondition(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.EarlySave;
    }
}

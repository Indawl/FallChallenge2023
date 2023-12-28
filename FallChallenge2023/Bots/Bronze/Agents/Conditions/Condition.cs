namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class Condition
    {
        public virtual int Id { get; } = 0;

        protected DroneAgent Agent { get; }

        public Condition(DroneAgent agent)
        {
            Agent = agent;
        }

        public virtual bool Check() => false;
    }
}

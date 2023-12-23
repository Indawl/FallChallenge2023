namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class Condition
    {
        public virtual int Id { get; } = 0;

        protected DroneAgent Agent { get; }
        protected GameState State { get; }

        public Condition(DroneAgent agent, GameState state)
        {
            Agent = agent;
            State = state;
        }

        public virtual bool Check() => false;
    }
}

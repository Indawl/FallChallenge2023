using FallChallenge2023.Bots.Bronze.Actions;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public abstract class Decision
    {
        public int DroneId { get; }
        public GameAction Action { get; private set; }

        public Decision(int droneId)
        {
            DroneId = droneId;
        }
        public GameAction GetAction(GameState state) => Action == null ? Action = CalculateAction(state) : CalculateAction(state);
        public abstract bool Finished(GameState state);

        protected abstract GameAction CalculateAction(GameState state);

        protected bool NeedLight(GameState state) => false;
    }
}

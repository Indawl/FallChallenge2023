using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public SaveDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override void SetConditions()
        {
            Conditions.Add(new SaveCondition(Agent, State));
        }

        public override GameAction GetDecision()
        {
            var newPosition = State.GetAroundMonster(Agent.Drone.Position, new Vector(0, -Drone.MAX_SPEED));
            var light = (new FishDetectedCondition(Agent, State, newPosition)).Check();
            return new GameActionMove(newPosition, light);
        }
    }
}

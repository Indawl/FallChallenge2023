using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SaveDecision : Decision
    {
        public SaveDecision(int droneId) : base(droneId) { }

        public override bool Finished(GameState state) => !state.GetDrone(DroneId).Scans.Any();

        protected override GameAction CalculateAction(GameState state)
        {
            var drone = state.GetDrone(DroneId);
            var newPosition = GameUtils.GetAroundMonster(state, drone.Position, new Vector(0, -GameProperties.DRONE_MAX_SPEED));
            return new GameActionMove(newPosition, NeedLight(state));
        }
    }
}

using FallChallenge2023.Bots.Bronze.Actions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class KickAwayDecision : Decision
    {
        public int FishId { get; }

        public KickAwayDecision(int droneId, int fishId) : base(droneId)
        {
            FishId = fishId;
        }

        public override bool Finished(GameState state) => !state.GetUnscannedFish(1 - state.GetDrone(DroneId).PlayerId).Contains(FishId);

        protected override GameAction CalculateAction(GameState state)
        {
            var drone = state.GetDrone(DroneId);


            var newPosition = GameUtils.GetAroundMonsterTo(state, drone.Position, state.GetFish(Fishs.First()).Position);
            return new GameActionMove(newPosition, NeedLight(state));
        }
    }
}

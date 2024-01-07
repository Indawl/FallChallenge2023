using FallChallenge2023.Bots.Bronze.Actions;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public int FishId { get; }
        public List<int> Fishs { get; }

        public SearchDecision(int droneId, int fishId, List<int> fishs) : base(droneId)
        {
            FishId = fishId;
            Fishs = fishs;
        }

        public override bool Finished(GameState state)
        {
            var drone = state.GetDrone(DroneId);
            if (!state.GetUnscannedFish(drone.PlayerId).Contains(FishId)) return true;

            Fishs.RemoveAll(fishId => !state.GetUnscannedFish(drone.PlayerId).Contains(fishId));
            return false;
        }

        protected override GameAction CalculateAction(GameState state)
        {
            var drone = state.GetDrone(DroneId);

            var fish = state.GetFish(FishId);
            var fishPosition = fish.Position;

            var turbo = fish.Speed == null || fish.Speed.IsZero();
            if (!turbo)
            {
                fishPosition += fish.Speed;
                if (fishPosition.InRange(drone.Position))
            }

            var newPosition = GameUtils.GetAroundMonsterTo(state, drone.Position, fishPosition, turbo);
            return new GameActionMove(newPosition, NeedLight(state));
        }
    }
}

using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public int FishId { get; }
        public int? NextFishId { get; set; }

        public SearchDecision(int droneId, int fishId, int? nextFishId = null) : base(droneId)
        {
            FishId = fishId;
            NextFishId = nextFishId;
        }

        public override bool Finished(GameState state) => !state.GetUnscannedFish(state.GetDrone(DroneId).PlayerId).Contains(FishId);

        protected override GameAction CalculateAction(GameState state)
        {
            var drone = state.GetDrone(DroneId);

            var fish = state.GetFish(FishId);
            var fishPosition = fish.Position;

            var turbo = fish.Speed.IsZero();
            if (!turbo)
            {
                fishPosition += fish.Speed;

                if (NextFishId.HasValue)
                {
                    var distSqr = fishPosition.DistanceSqr(drone.Position);
                    if (distSqr < (GameProperties.LIGHT_SCAN_RADIUS + GameProperties.DRONE_MAX_SPEED) * (GameProperties.LIGHT_SCAN_RADIUS + GameProperties.DRONE_MAX_SPEED))
                    {
                        var dist = drone.Position - fishPosition;
                        var radf = GameProperties.LIGHT_SCAN_RADIUS * GameProperties.LIGHT_SCAN_RADIUS / distSqr;
                        var cent = radf - GameProperties.DRONE_MAX_SPEED * GameProperties.DRONE_MAX_SPEED / distSqr + 0.5;
                        var toCent = dist * Math.Sqrt(cent);
                        var toCross = toCent.Cross() * Math.Sqrt(radf - cent);

                        var nextDist = state.GetFish(NextFishId.Value).Position - fishPosition;
                        if (Vector.Skew(toCross, nextDist - toCent) < 0)
                        {
                            var nextPos = nextDist.Normalize() * GameProperties.LIGHT_SCAN_RADIUS;
                            if (!nextPos.InRange(drone.Position, GameProperties.DRONE_MAX_SPEED))
                                fishPosition += toCent + toCross * Math.Sign(Vector.Skew(dist, nextDist));
                            else fishPosition += nextPos;
                        }
                        else
                        {
                            nextDist = state.GetFish(NextFishId.Value).Position - drone.Position;
                            var nextPos = nextDist.Normalize() * GameProperties.DRONE_MAX_SPEED;
                            if (!nextPos.InRange(fishPosition, GameProperties.LIGHT_SCAN_RADIUS))
                                fishPosition += toCent + toCross * Math.Sign(Vector.Skew(dist, nextDist));
                            else fishPosition += nextPos;
                        }
                    }
                }
            }

            var newPosition = GameUtils.GetAroundMonsterTo(state, drone.Position, fishPosition, turbo);
            return new GameActionMove(newPosition, NeedLight(state, drone, newPosition));
        }
    }
}

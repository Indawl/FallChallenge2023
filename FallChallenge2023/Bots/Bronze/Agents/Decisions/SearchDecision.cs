using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public SearchDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        protected Vector GetEscapeMonserMove(Vector move)
        {
            foreach (var monster in State.Fishes.Where(_ => _.Status == FishStatus.SWIMMING && (_.Position - Agent.Drone.Position).LengthSqr <= Drone.SCAN_RADIUS * Drone.SCAN_RADIUS))
                return Agent.Drone.Position + monster.Speed * Drone.MAX_SPEED;
            return move;
        }

        public override GameAction GetDecision()
        {
            foreach (var radarBlip in Agent.Drone.RadarBlips)
                if (!State.GetScannedFishes(Agent.Drone.PlayerId).Any(_ => _ == radarBlip.FishId))
                    return new GameActionMove(GetEscapeMonserMove(Agent.Drone.Position + Drone.MAX_SPEED * RadarBlip.GetDirection(radarBlip.Type)), State.Turn % 3 == 0);

            return null;
        }
    }
}

using FallChallenge2023.Bots.Bronze.Actions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public SearchDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override GameAction GetDecision()
        {
            //foreach (var radarBlip in Agent.Drone.RadarBlips)
            //    if (!State.GetScannedFishes(Agent.Drone.PlayerId).Any(_ => _ == radarBlip.FishId))
            //        return new GameActionMove(
            //            Agent.Drone.X + Drone.MAX_SPEED * RadarBlip.GetDirectionX(radarBlip.Type), 
            //            Agent.Drone.Y + Drone.MAX_SPEED * RadarBlip.GetDirectionY(radarBlip.Type), 
            //            State.Turn % 3 == 0);

            return null;
        }
    }
}

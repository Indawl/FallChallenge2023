using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class DiveDecision : Decision
    {
        public override int Id => GameProperties.DiveDecision;

        public DiveDecision(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.State.Turn < 13 && !Agent.State.Fishes.Any(_ =>
             _.Color == FishColor.UGLY &&
             _.Position.InRange(Agent.Drone.Position, Agent.Drone.LightRadius + GameProperties.MONSTER_DETECTED_RADIUS_ADD));

        public override GameAction GetAction()
        {
            var newPosition = Agent.Drone.Position + new Vector(0, GameProperties.DRONE_MAX_SPEED);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Gurgle Gurgle" };
        }
    }
}

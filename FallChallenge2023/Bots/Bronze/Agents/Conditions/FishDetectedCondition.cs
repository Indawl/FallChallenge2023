using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class FishDetectedCondition : Condition
    {
        private Vector Position { get; }

        public FishDetectedCondition(DroneAgent agent, GameState state, Vector position) : base(agent, state)
        {
            Position = position;
        }

        public override int Id => 2;

        public override bool Check() => State.Fishes.Any(_ => _.Status == FishStatus.UNKNOWED && _.Position.InRange(Position, Drone.LIGHT_SCAN_RADIUS));
    }
}

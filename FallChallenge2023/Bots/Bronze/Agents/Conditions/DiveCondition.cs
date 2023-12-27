using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class DiveCondition : Condition
    {
        public DiveCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 100;

        public override bool Check() => State.Turn < 13 && !State.Fishes.Where(_ => 
            _.Color == FishColor.UGLY &&
            _.Position.InRange(Agent.Drone.Position, Agent.Drone.LightRadius + Drone.MONSTER_DETECTED_RADIUS_ADD)).Any();
    }
}

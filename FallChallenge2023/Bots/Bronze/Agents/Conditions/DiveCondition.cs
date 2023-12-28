using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class DiveCondition : Condition
    {
        public override int Id => GameProperties.DiveCondition;

        public DiveCondition(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.State.Turn < 13 && !Agent.State.Fishes.Where(_ => 
            _.Color == FishColor.UGLY &&
            _.Position.InRange(Agent.Drone.Position, Agent.Drone.LightRadius + GameProperties.MONSTER_DETECTED_RADIUS_ADD)).Any();
    }
}

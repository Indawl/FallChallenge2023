using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class NeedDiveCondition : Condition
    {
        public override int Id => GameProperties.NeedDiveCondition;

        public NeedDiveCondition(DroneAgent agent) : base(agent) { }

        public override bool Check() =>
            Agent.UnscannedFishes.Any(_ => _.Type == FishType.CRAB) ||
            Agent.Drone.Scans.Any(_ => Agent.State.GetFish(_).Type == FishType.CRAB);
    }
}

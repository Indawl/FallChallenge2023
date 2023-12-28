using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class SearchCrabCondition : Condition
    {
        public override int Id => GameProperties.SearchCrabCondition;

        public SearchCrabCondition(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.UnscannedFishes.Where(_ => _.Type == FishType.CRAB).Any();
    }
}

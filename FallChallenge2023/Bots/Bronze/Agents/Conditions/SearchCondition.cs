using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class SearchCondition : Condition
    {
        public override int Id => GameProperties.SearchCondition;

        public SearchCondition(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.UnscannedFishes.Any();
    }
}

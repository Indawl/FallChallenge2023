using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class DiveSearchCondition : Condition
    {
        public DiveSearchCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 101;

        public override bool Check() => Agent.UnscannedFishes.Where(_ => _.Type == FishType.CRAB).Any();
    }
}

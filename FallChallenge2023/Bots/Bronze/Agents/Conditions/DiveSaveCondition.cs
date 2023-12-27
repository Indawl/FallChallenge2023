using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class DiveSaveCondition : Condition
    {
        public DiveSaveCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 102;

        public override bool Check() => Agent.Drone.Scans.Where(_ => State.GetFish(_).Type == FishType.CRAB).Any();
    }
}

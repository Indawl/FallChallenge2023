using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class DiveSearchCondition : Condition
    {
        public DiveSearchCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 101;

        public override bool Check()
        {
            var scannedFish = State.GetScannedFishes(Agent.Drone.PlayerId).Where(_ => State.GetFish(_).Type == FishType.CRAB).ToList();
            return Agent.Fishes.Any(_ => !scannedFish.Contains(_.Id));
        }
    }
}

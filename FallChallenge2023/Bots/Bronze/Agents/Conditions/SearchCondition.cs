using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class SearchCondition : Condition
    {
        public SearchCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 200;

        public override bool Check()
        {
            var scannedFish = State.GetScannedFishes(Agent.Drone.PlayerId);
            return Agent.Fishes.Any(_ => !scannedFish.Contains(_.Id));
        }
    }
}

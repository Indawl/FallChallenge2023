using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Conditions
{
    public class SaveCondition : Condition
    {
        public SaveCondition(DroneAgent agent, GameState state) : base(agent, state)
        {
        }

        public override int Id => 3;

        public override bool Check() => Agent.Drone.Scans.Any();
    }
}

using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneSearchAgent : DroneAgent
    {
        public DroneSearchAgent(GameState state, Drone drone) : base(state, drone) { }

        protected override void SetDecisions()
        {
            Decisions = new List<Decision>()
            {
                new EmergencyDecision(this),
                new SimpleSearchDecision(this),
                new SaveDecision(this)
            };
        }
    }
}

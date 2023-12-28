using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneSaveAgent : DroneAgent
    {
        public DroneSaveAgent(GameState state, Drone drone) : base(state, drone) { }

        protected override void SetDecisions()
        {
            Decisions = new List<Decision>()
            {      
                new EmergencyDecision(this),
                new SaveDecision(this)
            };
        }
    }
}

using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneSaveAgent : DroneAgent
    {
        public DroneSaveAgent(int droneId) : base(droneId)
        {
            NeedSave = true;
        }

        public override string ToString() => "DroneSaveAgent";

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

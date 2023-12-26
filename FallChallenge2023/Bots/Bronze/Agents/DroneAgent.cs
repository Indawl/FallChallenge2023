using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneAgent
    {
        public Drone Drone { get; }
        public List<Fish> Fishes { get; set; }
        public List<Fish> Uglys { get; set; }
        public GameAction Action { get; private set; }

        private Dictionary<int, bool> CheckedConditions { get; } = new Dictionary<int, bool>();

        public DroneAgent(Drone drone)
        {
            Drone = drone;
        }

        public void FindAction(GameState state)
        {
            Action = new GameActionWait() { Text = "WhatsssAPP???" };
            CheckedConditions.Clear();

            // Decided action
            Action = GetActionFromDecision(new List<Decision>()
            {
                new EmergencyDecision(this, state), // Need repair
                //new MotorDecision(this, state), // Need motor or not
                //new PreSaveDecision(this, state), // Need save, when not all scanning
                new DiveDecision(this, state), // Dive from start
                new SearchDecision(this, state), // Search for scan
                new SaveDecision(this, state) // Save if all scans
                //new ScareDecision(this, state), // Try to lost scaning fish
            });
        }

        private GameAction GetActionFromDecision(List<Decision> decisions)
        {
            GameAction action = null;

            foreach (var decision in decisions)
                if (CheckConditions(decision))
                {
                    if (decision.Decisions == null) action = decision.GetDecision();
                    else action = GetActionFromDecision(decision.Decisions);
                    if (action != null) break;
                }

            return action;
        }

        private bool CheckConditions(Decision decision)
        {
            foreach (var condition in decision.Conditions)
                if (!CheckCondition(condition))
                    return false;

            return true;
        }

        public bool CheckCondition(Condition condition)
        {
            if (!CheckedConditions.TryGetValue(condition.Id, out var result))
                CheckedConditions.Add(condition.Id, result = condition.Check());

            return result;
        }
    }
}

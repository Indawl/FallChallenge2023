using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneAgent
    {
        public Drone Drone { get; }
        public List<Fish> UnscannedFishes { get; set; } = new List<Fish>();
        public GameAction Action { get; private set; }

        private Dictionary<int, bool> CheckedConditions { get; } = new Dictionary<int, bool>();

        public DroneAgent(Drone drone)
        {
            Drone = drone;
        }

        public void Clear()
        {
            UnscannedFishes.Clear();
            CheckedConditions.Clear();
        }

        public void FindAction(GameState state)
        {
            // Decided action
            Action = GetActionFromDecision(new List<Decision>()
            {
                new EmergencyDecision(this, state),     // Need repair
                new NeedDiveDecision(this, state),      // Dive from start                
                new SaveDecision(this, state)           // All done
            });
        }

        private GameAction GetActionFromDecision(List<Decision> decisions)
        {
            GameAction action = null;

            foreach (var decision in decisions)
            {
                if (CheckConditions(decision))
                    if (decision.DecisionsOk.Any()) action = GetActionFromDecision(decision.DecisionsOk);
                    else action = decision.GetDecision();
                else
                    if (decision.DecisionsFail.Any()) action = GetActionFromDecision(decision.DecisionsFail);

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

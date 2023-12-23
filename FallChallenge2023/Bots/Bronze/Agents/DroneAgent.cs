using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneAgent
    {
        public Drone Drone { get; }
        public GameAction Action { get; private set; } = new GameActionWait();

        private Dictionary<int, bool> CheckedConditions { get; }

        public DroneAgent(Drone drone)
        {
            Drone = drone;
        }

        public void FindAction(GameState state)
        {
            var decisions = new List<Decision>()
            {
                new SearchDecision(this, state),
                new SaveDecision(this, state)
            };

            Action = GetActionFromDecision(decisions);
        }

        private GameAction GetActionFromDecision(List<Decision> decisions)
        {
            GameAction action = null;

            foreach (var decision in decisions)
                if (decision.CheckConditions())
                {
                    if (decision.Decisions == null) action = decision.GetDecision();
                    else action = GetActionFromDecision(decision.Decisions);
                    if (action != null) break;
                }

            return action;
        }

        public bool CheckCondition(Condition condition)
        {
            if (!CheckedConditions.TryGetValue(condition.Id, out var result))
                CheckedConditions.Add(condition.Id, result = condition.Check());

            return result;
        }
    }
}

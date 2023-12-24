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

        public bool Lighting { get; set; } = true;

        private Dictionary<int, bool> CheckedConditions { get; }

        public DroneAgent(Drone drone)
        {
            Drone = drone;
        }

        public void FindAction(GameState state)
        {
            // Decided action
            Action = GetActionFromDecision(new List<Decision>()
            {
                //new LightDecision(this, state),
                new SearchDecision(this, state),
                new SaveDecision(this, state)
                //new ScareDecision(this, state),
            });
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

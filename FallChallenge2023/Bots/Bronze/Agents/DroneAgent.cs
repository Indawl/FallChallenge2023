using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneAgent
    {
        public int DroneId { get; }

        public GameState State { get; private set; }
        public Drone Drone { get; private set; }
        public GameAction Action { get; protected set; }

        public List<Fish> UnscannedFishes { get; } = new List<Fish>();
        public bool EarlySave { get; set; } = false;

        protected List<Decision> Decisions { get; set; }

        private Dictionary<int, bool> CheckedConditions { get; } = new Dictionary<int, bool>();

        public DroneAgent(int droneId)
        {
            DroneId = droneId;

            SetDecisions();
        }

        public void Initialize(GameState state)
        {
            State = state;
            Drone = state.Drones.First(_ => _.Id == DroneId);

            EarlySave = false;

            UnscannedFishes.Clear();
            CheckedConditions.Clear();
        }

        protected virtual void SetDecisions()
        {
            Decisions = new List<Decision>()
            {
                new EmergencyDecision(this),     // Need repair
                new EarlySaveDecision(this),     // Need early save
                new NeedDiveDecision(this),      // Dive from start                
                new SaveDecision(this)           // All done, go to save
            };
        }

        public void FindAction()
        {
            Action = GetActionFromDecision(Decisions);
        }

        protected GameAction GetActionFromDecision(List<Decision> decisions)
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

        public bool NeedLighting(Vector position) => State.UnscannedFishes[Drone.PlayerId]
            .Any(_ => _.Position.InRange(position, GameProperties.DARK_SCAN_RADIUS, GameProperties.LIGHT_SCAN_RADIUS));
    }
}

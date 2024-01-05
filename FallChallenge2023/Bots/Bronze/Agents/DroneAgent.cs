using FallChallenge2023.Bots.Bronze.Actions;
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

        public DroneAgentGoal Goal { get; set; }

        private List<Decision> Decisions { get; set; }        

        public DroneAgent(int droneId)
        {
            DroneId = droneId;

            SetDecisions();
        }

        public void Initialize(GameState state)
        {
            State = state;
            Drone = state.Drones.First(_ => _.Id == DroneId);
        }

        protected virtual void SetDecisions()
        {
            Decisions = new List<Decision>()
            {
                new EmergencyDecision(this),
                new EarlySaveDecision(this),
                new KickAwayDecision(this),
                new SearchDecision(this),
                new SaveDecision(this)
            };
        }

        public void FindAction()
        {
            Action = GetActionFromDecision(Decisions);
        }

        private GameAction GetActionFromDecision(List<Decision> decisions)
        {
            GameAction action = null;

            // Check goal
            if (Goal != null)
            {
                var decision = decisions.First(_ => _.Type == Goal.Type);
                if (decision.CheckGoal(Goal)) Goal = null;
                else action = decision.GetAction();
            }

            // Do decision
            foreach (var decision in decisions)
            {
                if (decision.Check()) action = decision.GetAction();
                if (action != null) break;
            }

            return action;
        }

        public bool NeedLighting(Vector position)
        {
            var light = false;

            // Unscanned fishes close
            if (State.GetUnscannedFish(Drone.PlayerId).Any(_ => State.GetFish(_).Position.InRange(position, GameProperties.DARK_SCAN_RADIUS, GameProperties.LIGHT_SCAN_RADIUS)))
                return true;

            var enemyDrones = State.GetDrones(1 - Drone.PlayerId);

            // Enemy with light close and have new scans
            if (!Drone.Lighting && enemyDrones
                .Where(_ => _.Lighting && _.Position.InRange(Drone.Position, GameProperties.DARK_SCAN_RADIUS)
                                       && _.NewScans.Any(s => State.UnscannedFishes[Drone.PlayerId].Any(f => f.Id == s)))
                .Any())
                return true;

            // Maybe he go away from monster
            if (!Drone.Lighting && enemyDrones
                .Where(_ => !_.Position.InRange(Drone.Position, GameProperties.DARK_SCAN_RADIUS) &&
                            _.Position.InRange(position, GameProperties.DARK_SCAN_RADIUS))
                .Any())
                return true;

            // Incite monster to enemy
            foreach (var fish in State.Fishes.Where(_ => _.Speed != null && _.Color == FishColor.UGLY && _.Position.InRange(position, GameProperties.DARK_SCAN_RADIUS, GameProperties.LIGHT_SCAN_RADIUS)))
                if (enemyDrones.Any(_ => GameUtils.CheckCollision(fish.Position, fish.Speed, _.Position, _.Position + _.Speed, true)))
                    return true;
             
            return light;
        }
    }
}

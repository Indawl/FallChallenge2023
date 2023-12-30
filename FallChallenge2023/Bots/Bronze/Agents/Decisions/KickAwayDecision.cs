using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class KickAwayDecision : Decision
    {
        public override int Id => GameProperties.KickAwayDecision;

        private List<Fish> Fishes { get; set; }

        public KickAwayDecision(DroneAgent agent) : base(agent) { }
        
        public override bool Check()
        {
            Fishes = Agent.State.Fishes
                .Where(_ => _.Position.X * Agent.LessX > Agent.Drone.Position.X * Agent.LessX
                         && Agent.State.UnscannedFishes[1 - Agent.Drone.PlayerId].Contains(_)
                         && _.Position.InRange(Agent.Drone.Position, GameProperties.LIGHT_SCAN_RADIUS + GameProperties.DRONE_MAX_SPEED)).ToList();

            return Fishes.Any();
        }

        public override GameAction GetAction()
        {
            var fish = Fishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();

            var x = Agent.LessX > 0
                ? Math.Min(Agent.Drone.Position.X, GameProperties.MAP_SIZE - GameProperties.MOTOR_RANGE - fish.Speed.X + 2)
                : Math.Max(Agent.Drone.Position.X, GameProperties.MOTOR_RANGE + fish.Speed.X - 1);

            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, new Vector(x, fish.Position.Y), Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Kick away" };
        }
    }
}
